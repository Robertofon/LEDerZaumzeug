using System.Collections.Generic;
using System.Threading.Tasks;
using LEDerZaumzeug.Extensions;
using System.Net.Sockets;

namespace LEDerZaumzeug.Outputs
{
    /// <summary>
    /// Basisklasse für normale LED-Ketten Outputs. Mit dem Feature
    /// Schlangenlinien ziehen.
    /// </summary>
    public abstract class OutputBase : IOutput
    {
        /// <summary>Pixelordermap. Natürliche Order der Liste ist die Reihe der
        /// Verkettung der LEDs, was eben als erstes am Draht hängt ist die 0.
        /// Dann steht die dazu passende Koordinate drin. Ergo: dieser Output
        /// geht über die Liste drüber und pickt sich die pixel mit den Koorinaten
        /// der <see cref="pxmap"/> heraus und nur das wid auch ausgegeben.</summary>
        private readonly List<(int, int)> pxmap = new List<(int, int)>(200);

        public SizeModes SizeMode { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; } 
        public PixelArrangement PixelOrder { get; set; }
        public List<(int,int)> PxMap { get{ return pxmap;} } 

        public void Dispose()
        {
            
        }

        public Task<OutputInfos> GetInfos()
        {
            return Task.FromResult(new OutputInfos() { Dim = new System.Drawing.SizeF(12, 8), DesiredSubSamplemode = SubSample.S1x1 } );
        }

        public virtual Task Initialize(object paramset)
        {
            // Pixelorder herausbekommen, Mapping machen
            pxmap.Clear();
            GenerischeOrder(pxmap, this.SizeX, this.SizeY, this.PixelOrder);
            return Task.CompletedTask;
        }

        public abstract Task Play(RGBPixel[,] pixels);

        public abstract void SetSize(int rechenDimX, int rechenDimY);

        public static void GenerischeOrder(List<(int, int)> pxmap, int SizeX, int SizeY, PixelArrangement pa)
        {
            int steph = pa.IsStartRight()? -1: +1;
            int stepv = pa.IsStartTop()? +1 : -1;
            int x = pa.IsStartRight()? SizeX-1 : 0;
            int y = pa.IsStartTop()? 0 : SizeY-1;

            for(int i=0; i<SizeX * SizeY; i++)
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
                if( x < 0 || x >= SizeX)
                {
                    if( pa.IsSnakeWise() )
                    {
                        // X snake-resetten (bleibe auf deiner seite, lauf anders)
                        x = x.LimitTo(0, SizeX-1);
                        steph = -steph;  // invertiere richtung
                    }
                    else // zeilenweise
                    { 
                        // x resetten
                        x = pa.IsStartRight()? SizeX-1 : 0;
                    }
                    y += stepv;
                }

                // Ende erreicht? Bei Y
                if( y < 0 || y >= SizeY)
                {
                    if( pa.IsSnakeWise() )
                    {
                        // Y snake-resetten (bleibe auf deiner seite, lauf anders)
                        y = y.LimitTo(0, SizeY-1);
                        stepv = -stepv; // invertiere Richtung
                    }
                    else // zeilenweise
                    { 
                        // y resetten
                        y = pa.IsStartTop()? 0 : SizeY-1;
                    }
                    x += steph;
                }

            }
        }

        public static (byte,byte,byte) RGBfToRGBi(RGBPixel p)
        {
            return ((byte)(p.R * 255), (byte)(p.G * 255), (byte)(p.B * 255) );
        }

        /// <summary>
        /// Mappt die berechneten Pixel über die pxMap auf die Reihenfolge der 
        /// LED-installation. Dabei umrechnung von float RGB auf int(256) rgb.
        /// </summary>
        /// <param name="pxMap">Pixmap des Outputs.</param>
        /// <param name="pixels">Aktuelles Bild</param>
        /// <returns>Byte-Stream.</returns>
        public static byte[] MappedOutput( IList<(int,int)> pxMap, RGBPixel[,] pixels)
        {
            var res = new byte[pxMap.Count];
            for(int io=0, im=0; im<pxMap.Count; im++)
            {
                var koord = pxMap[im];
                (byte, byte, byte) leddata = RGBfToRGBi(pixels[koord.Item1, koord.Item2]);
                res[io++] = leddata.Item1;
                res[io++] = leddata.Item2;
                res[io++] = leddata.Item3;
            }

            return res;
        }
    }
}
