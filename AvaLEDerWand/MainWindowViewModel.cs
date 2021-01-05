using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using LEDerZaumzeug;
using Newtonsoft.Json;
using ReactiveUI;

namespace LEDerWand
{
    public class MainWindowViewModel : ReactiveObject
    {
        private readonly MainWindow _w;
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private Color[] _leds;
        private Task _task;
        private string _infoTxt;

        public WandConfig Cfg { get; set; }

        public LedControlViewModel LedVm { get; }

        public MainWindowViewModel(MainWindow w, WandConfig cfg)
        {
            Cfg = cfg;
            _w = w;
            _w.Closing += (sender, args) =>
            {
                _cts.Cancel();
            };

            LedVm = new LedControlViewModel(cfg.LedRows, cfg.LedCols);
            _leds = new Color[cfg.LedCols * (cfg.LedRows + 1)];
            this.InfoTxt = $"Dim:{cfg.LedRows}∙{cfg.LedCols}, PxO:{PixelArrangement.LNH_TL}";

            StartListening();
        }

        public string InfoTxt
        {
            get => _infoTxt;
            set => this.RaiseAndSetIfChanged(ref _infoTxt, value);
        }

        private void StartListening()
        {
            _task = Task.Run(DoNetworkListening, _cts.Token);
        }

        public void Exit()
        {
            _w.Close();
        }

        public void ToggleFullScreen()
        {
            _w.ToggleFullScreen();
        }

        private async Task DoNetworkListening()
        {
            while (true)
            {
                _cts.Token.ThrowIfCancellationRequested();
                if (Cfg.Protocol == LedProto.TPM2)
                {
                    await DoTpm2Net();
                }
            }
        }

        public async Task OpenConfig()
        {
            using (var f = File.CreateText(WandConfig.CFG_NAME))
            {
                string s = JsonConvert.SerializeObject(this.Cfg);
                await f.WriteAsync(s);
            }
            //_w.Close();
        }

        private async Task DoTpm2Net()
        {
            const byte CmdLEDCount = 0x10;
            UdpClient receivingUdpClient = new UdpClient(Cfg.ListenPort);
            //IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                int NUM_LEDS = Cfg.LedCols*Cfg.LedRows;
                while (true)
                {
                    _cts.Token.ThrowIfCancellationRequested();

                    UdpReceiveResult udpReceiveResult = await receivingUdpClient.ReceiveAsync();

                    int led_index = 0;
                    byte[] buf = udpReceiveResult.Buffer;
                    if (buf.Length >= 6 && buf[0] == 0x9C)
                    {
                        byte blocktype = buf[1]; // block type (0xDA)
                        uint framelength = ((uint) buf[2] << 8) | (uint) buf[3]; // frame length (0x0069) = 105 leds
                        byte packagenum = buf[4]; // packet number 0-255 0x00 = no frame split (0x01)
                        byte numpackages = buf[5]; // total packets 1-255 (0x01)
                        // Command modus  (um größeninfo zu liefern)
                        if (blocktype == 0xC0)
                        {
                            // Befehlsmodus
                            // Einziger Befehl: Lese Matrix-Size 0x10  Lesen der Anzahl angeschlossener  Pixel  0x)
                            // Erstes Byte der Nutzdaten: Kommandokontrollbyte ; Zweites Byte: Befehl
                            // Byte#1: MSB   (bit8) :  Richtung des Befehls (0=Lesebefehl, 1=Schreibbefehl) 
                            // Byte#1: MSB-1 (bit7) :  Befehl erwartet Antwort? (0=Nein, 1=Ja)
                            // Byte#1: MSB-2..LSB : Reserviert
                            if (framelength >= 2)
                            {
                                bool schreibbefehl = (buf[6] & 0x80) > 0;
                                bool lesebefehl = !schreibbefehl;
                                bool antworterwartet = (buf[6] & 0x40) > 0;
                                byte befehl = buf[7];
                                if (befehl == CmdLEDCount && lesebefehl && antworterwartet)
                                {
                                    byte[] antwort = new byte[3];
                                    antwort[0] = 0xAD;
                                    antwort[1] = (byte) ((NUM_LEDS >> 8) & 0xFF);
                                    antwort[2] = (byte) (NUM_LEDS & 0xFF);
                                    // send a reply, to the IP address and port that sent us the packet we received
                                    await receivingUdpClient.SendAsync(antwort, antwort.Length);
                                }

                            }
                        }
                        // Daten modus : Block hat 0xDA typ.
                        else if (blocktype == 0xDA)
                        {
                            //if (debuginfo)
                            //{
                            //    Serial.print("Pkgnum="); Serial.print(packagenum); Serial.print("/"); Serial.println(numpackages);
                            //}
                            int packetindex;

                            // teste auch auf End-Byte !
                            if (buf.Length >= framelength + 7 && buf[6 + framelength] == 0x36)
                            {
                                packetindex = 6;
                                if (packagenum == 1)
                                {
                                    //if (debuginfo)
                                    //{ Serial.println("ledindex=0 (reset)"); }
                                    led_index = 0;
                                }

                                //if (debuginfo)
                                //{ Serial.print("packetindex="); Serial.println(packetindex); }

                                while (packetindex < (framelength + 6))
                                {
                                    _leds[led_index] = new Color(byte.MaxValue, buf[packetindex],
                                        buf[packetindex + 1], buf[packetindex + 2]);
                                    led_index++;
                                    packetindex += 3;
                                    // Sicherheitsabbruch
                                    if (led_index == NUM_LEDS)
                                    {
                                        packagenum = numpackages;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    await Dispatcher.UIThread.InvokeAsync(() => LedVm.FeedData(_leds));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}