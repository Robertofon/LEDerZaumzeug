using System;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace LEDerZaumzeug.Outputs
{
    /// <summary>
    /// Output implementierung für TPM2.Net protokoll.
    /// Siehe: http://www.tpm2.de/
    /// </summary>
    public class Tpm2NetOutput : OutputBase
    {
        private Socket _socket;

        public string IP_Adresse { get; set; }

        public int UDP_Port { get; set; }

        public override async Task Initialize(object paramset)
        {
            await base.Initialize(paramset);


            // UDP socket aufmachen
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            await _socket.ConnectAsync(IPAddress.Parse(IP_Adresse), UDP_Port);
            if( _socket.Connected )
            {
                Console.WriteLine("Happy: conn" + IP_Adresse + ":" + UDP_Port);
            }
            else
            {
                Console.WriteLine("Nix conn" + IP_Adresse + ":" + UDP_Port);
            }

        }

        public override async Task Play(RGBPixel[,] pixels)
        {
            byte[] byteData = MappedOutput(this.PxMap, pixels);
            bool gut = await _socket.SendAsync(byteData, 0, byteData.Length, 0);
        }

        public override void SetSize(int rechenDimX, int rechenDimY)
        {
            // tue nichts, da die Dimension nicht geändert werden kann.
        }
    }
}
