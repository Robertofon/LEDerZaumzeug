using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LEDerZaumzeug.Extensions;
using System.Net;
using System.Net.Sockets;

namespace LEDerZaumzeug.Outputs
{
    /// <summary>
    /// Output implementierung für TPM2.Net protokoll.
    /// Siehe: http://www.tpm2.de/
    /// </summary>
    public class Tpm2NetOutput : IOutput
    {
        /// <summary>Pixelordermap. Natürliche Order der Liste ist die Reihe der
        /// Verkettung der LEDs, was eben als erstes am Draht hängt ist die 0.
        /// Dann steht die dazu passende Koordinate drin. Ergo: dieser Output
        /// geht über die Liste drüber und pickt sich die pixel mit den Koorinaten
        /// der <see cref="pxmap"/> heraus und nur das wid auch ausgegeben.</summary>
        private List<(int, int)> pxmap = new List<(int, int)>(200);

        public SizeModes SizeMode { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; } 
        public PixelArrangement PixelOrder { get; set; }
        public List<(int,int)> PxMap { get{ return pxmap;} } 
        private Socket _socket;

        public void Dispose()
        {
            
        }

        public Task<OutputInfos> GetInfos()
        {
            return Task.FromResult(new OutputInfos() { Dim = new System.Drawing.SizeF(12, 8), DesiredSubSamplemode = SubSample.S1x1 } );
        }

        public Task Initialize(object paramset)
        {
            // Pixelorder herausbekommen, Mapping machen
            GenerischeOrder(pxmap, this.PixelOrder);
            
            
            // UDP socket aufmachen
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            bool a = await _socket.ConnectAsync(IPAddress.Parse(address), port);

            await _socket.SendAsync(byteData, 0, byteData.Length, 0,  
     
        }

        private void GenerischeOrder(List<(int, int)> pxmap, PixelArrangement pa)
        {
            int steph = pa.IsStartRight()? -1: +1;
            int stepv = pa.IsStartTop()? +1 : -1;
            int x = pa.IsStartRight()? this.SizeX-1 : 0;
            int y = pa.IsStartTop()? 0 : this.SizeY-1;

            for(int i=0; i<this.SizeX * this.SizeY; i++)
            {
                pxmap.Add( (x,y) );

                // weiter im Text. Horiz oder vert
                if(pa.IsHoriz())
                {
                    x += steph;
                }
                else // IsVert()
                {
                    y += stepv;
                }

                // Ende erreicht? Bei X
                if( x < 0 || x >= this.SizeX)
                {
                    if( pa.IsSnakeWise() )
                    {
                        // X snake-resetten (bleibe auf deiner seite, lauf anders)
                        x = x.LimitTo(0, this.SizeX-1);
                        steph = -steph;  // invertiere richtung
                    }
                    else // zeilenweise
                    { 
                        // x resetten
                        x = pa.IsStartRight()? this.SizeX-1 : 0;
                    }
                    y += stepv;
                }

                // Ende erreicht? Bei Y
                if( y < 0 || y >= this.SizeY)
                {
                    if( pa.IsSnakeWise() )
                    {
                        // Y snake-resetten (bleibe auf deiner seite, lauf anders)
                        y = y.LimitTo(0, this.SizeY-1);
                        stepv = -stepv; // invertiere Richtung
                    }
                    else // zeilenweise
                    { 
                        // y resetten
                        y = pa.IsStartTop()? 0 : this.SizeY-1;
                    }
                    x += steph;
                }

            }
        }

        public async Task Play(RGBPixel[,] pixels)
        {
            // Den Scheiß an den Mann bringen
            await Task.Delay(3);
        }
    }
}
