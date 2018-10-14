using System;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using LEDerZaumzeug.Extensions;

namespace LEDerZaumzeug.Outputs
{
    /// <summary>
    /// Output implementierung für TPM2.Net protokoll.
    /// Siehe: http://www.tpm2.de/
    /// </summary>
    public class Tpm2NetOutput : OutputBase
    {
        private static readonly NLog.ILogger log = NLog.LogManager.GetLogger(nameof(Tpm2NetOutput));

        private Socket _socket;

        public string IP_Adresse { get; set; }

        public int UDP_Port { get; set; }

        public override async Task<bool> Initialize(LEDerConfig config)
        {
            await base.Initialize(config);

            try
            {
                if (string.IsNullOrWhiteSpace(this.IP_Adresse))
                {
                    throw new ArgumentNullException("IP_Adresse fehlt!");
                }

                if (0 == this.UDP_Port)
                {
                    throw new ArgumentNullException("UDP_Port fehlt!");
                }

                // UDP socket aufmachen
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                await _socket.ConnectAsync(IPAddress.Parse(IP_Adresse), UDP_Port);
                if (_socket.Connected)
                {
                    Console.WriteLine("Happy: conn" + IP_Adresse + ":" + UDP_Port);
                }
                else
                {
                    Console.WriteLine("Nix conn" + IP_Adresse + ":" + UDP_Port);
                }

                return true;
            }
            catch( Exception ex)
            {
                log.Error(ex, "Fehler beim Initialisieren: ");
                return false;
            }

        }

        /// <summary>
        /// Maximale Größe der Nutzdaten eines Tpm2.Net-Pakets. Begrenzt durch framesize als WORD,
        /// aber mehr noch durch die standard Ethernetgröße von 1500 Byte. Abzüglich 12 byte = 2*header Tmp2.
        /// </summary>
        private const int Tpm2NetMaxFrameSize = 1500 - 12; 
        private const byte Tpm2BlockStartByte = 0x9C; // StartByte für TPM2.Net 
        private const byte Tpm2BlockEndByte = 0x36;   // Endbyte für TPM2.Net 

        private enum Tmp2BlockTyp : byte
        {
            DatenPaketBlockTyp = 0xDA,
            KommandoPaketBlockTyp = 0xC0,
            BestätigungBlockTyp = 0xAC,
            AntwortmitDatenBlockTyp = 0xAD
        }

        /// <summary>
        /// Erstellt den Header eines Tpm2.Net-Pakets
        /// </summary>
        /// <param name="blockTyp">nix besonderes meist daten.</param>
        /// <param name="frm_size">Länge der Nutzdaten exklusive Schlußbyte.</param>
        /// <param name="paketnum">0, bei <paramref name="paketzahl"/>==1, sonst 1basiert."/></param>
        /// <param name="paketzahl">Anzahl der erwarteten pakete.</param>
        /// <returns></returns>
        private byte[] Tpm2NetProtokoll(Tmp2BlockTyp blockTyp, ushort frm_size, byte paketnum, byte paketzahl)
        {
            return new byte[]
            {
                Tpm2BlockStartByte,
                (byte)blockTyp,
                (byte)((frm_size >> 8) & 0xFF),  // high byte framesize
                (byte)((frm_size) & 0xFF),      // low byte framesize
                paketnum,
                paketzahl
            };
        }

        public override async Task Play(RGBPixel[,] pixels)
        {
            // Daten umtransformieren in Bytefolge von RGB-Tupeln.
            byte[] byteData = MappedOutput(this.PxMap, pixels, SubPixelOrder.RGB);

            // Anzahl der Pakete ermitteln:  (durch 3 ergibt sich gleich ein ganzzahlig durch 3 teilbares Längengebot)
            int paketzahl = (byteData.Length/3) / (Tpm2NetMaxFrameSize/3);
            int frameSizePlan = (Tpm2NetMaxFrameSize / 3) * 3;
            int offset = 0;
            int i = (paketzahl==1) ? 0 : 1;  // Paketzähler 0 bei paketzahl==1, sonst 1 basiert.

            // Daten in byteData an 3-Byte-Grenzen aufgeteilt auf mehrere Pakete versenden in einer Schleife
            while ( offset < byteData.Length )
            {
                // daten "ausschneiden" - paketlen max so viel wie byte da sind.
                ushort frameSize = (ushort)frameSizePlan.LimitTo(byteData.Length - offset);
                var dataSeg = new ArraySegment<byte>(byteData, offset, frameSize);
                offset += frameSize;
                byte[] header = Tpm2NetProtokoll(Tmp2BlockTyp.AntwortmitDatenBlockTyp, frameSize, (byte)i, (byte)paketzahl);
                i += 1;
                var headerseg = new ArraySegment<byte>(header);
                var byteDataa = new ArraySegment<byte>(byteData);
                Task<int> sent = _socket.SendAsync(new[] { headerseg, byteData, new byte[] { Tpm2BlockEndByte } }, SocketFlags.None);
                await sent;
            }
        }

        public override void SetSize(int rechenDimX, int rechenDimY)
        {
            // tue nichts, da die Dimension nicht geändert werden kann.
        }
    }
}
