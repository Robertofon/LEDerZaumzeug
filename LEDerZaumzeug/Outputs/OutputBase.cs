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

        /// <summary>
        /// Definiert die Verkabelungstopologie der Einzelnen LEDs bei Aufbau der Matrix.
        /// So kann die <see cref="PxMap"/> generisch aufgebaut werden.
        /// </summary>
        public PixelArrangement PixelOrder { get; set; }

        /// <summary>
        /// Definiert die Subpixelorder, die bei der Ausgabe als Ziel benutzt wird.
        /// </summary>
        public SubPixelOrder SubPixelOrder { get; set; } = SubPixelOrder.RGB;

        /// <summary>
        /// Liste, die für jedes Pixel in der natürlichen ausgabereihenfolge die Koordinaten
        /// im generietten Bild wiedergeben. Lookup table.
        /// </summary>
        public List<(int,int)> PxMap { get{ return pxmap;} } 

        public void Dispose()
        {
            
        }

        public Task<OutputInfos> GetInfos()
        {
            return Task.FromResult(new OutputInfos() { Dim = new System.Drawing.SizeF(12, 8), DesiredSubSamplemode = SubSample.S1x1 } );
        }

        public virtual Task<bool> Initialize(LEDerConfig config)
        {
            // Pixelorder herausbekommen, Mapping machen
            pxmap.Clear();
            GenerischeOrder(pxmap, this.SizeX, this.SizeY, this.PixelOrder);
            return Task.FromResult(true);
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
        /// Verwürfelt die Tupel eines RGB-pixels derart, wie <paramref name="neueOrder"/> vorgibt.
        /// </summary>
        /// <param name="pixel">RGB Eingabe-Tupel.</param>
        /// <param name="neueOrder">Zielorder der Subpixel.</param>
        /// <returns>Pixel-Tupel mit neuer Subpixelorder.</returns>
        public static (byte, byte, byte) ReMapSubPixel((byte, byte, byte) pixel, SubPixelOrder neueOrder)
        {
            switch (neueOrder)
            {
                case SubPixelOrder.BGR:
                    return (pixel.Item3, pixel.Item2, pixel.Item1);
                default:
                    return pixel;
            }
        }

        /// <summary>
        /// Mappt die berechneten Pixel über die pxMap auf die Reihenfolge der 
        /// LED-installation. Dabei umrechnung von float RGB auf int(256) rgb.
        /// </summary>
        /// <param name="pxMap">Pixmap des Outputs.</param>
        /// <param name="pixels">Aktuelles Bild</param>
        /// <param name="spo">Subpixel order des Ziels.</param>
        /// <returns>Byte-Stream.</returns>
        public static byte[] MappedOutput( IList<(int,int)> pxMap, RGBPixel[,] pixels, SubPixelOrder spo)
        {
            var res = new byte[pxMap.Count*3];
            for(int io=0, im=0; im<pxMap.Count; im++)
            {
                var koord = pxMap[im];
                (byte, byte, byte) leddata = RGBfToRGBi(pixels[koord.Item1, koord.Item2]);
                (res[io++], res[io++], res[io++]) = ReMapSubPixel(leddata, spo);
            }

            return res;
        }
    }
}
