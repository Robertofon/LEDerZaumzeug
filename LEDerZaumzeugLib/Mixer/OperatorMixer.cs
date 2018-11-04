using LEDerZaumzeug.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace LEDerZaumzeug.Mixer
{
    /// <summary>
    /// Arithmetische Zusammenführung.
    /// Erlaubt es, n Kanäle mit vorgegebenen Operatoren zu fusionieren.
    /// Multiplikation, Addition, Minus, Abwedeln....
    /// </summary>
    [Description("Arithmetischer Mixer mit konfigurierbaren Operatoren")]
    public class OperatorMixer : IMixer
    {
        /// <summary>
        /// Operator, der verwendet wird, um alle eingänge zu verknoten.
        /// Bei nicht kommutativen Operatoren zählt die Reihenfolte.
        /// Index 0 bis n.
        /// </summary>
        public Operatoren Operator { get; set; }

        public Task Initialize(MatrixParams matrixParameters)
        {
            return Task.CompletedTask;
        }

        public Task<RGBPixel[,]> Mix(IList<RGBPixel[,]> sources, ulong frame)
        {
            // Schneller Ausgang
            if(sources.Count == 1)
            {
                return Task.FromResult(sources[0]);
            }

            Func<RGBPixel,RGBPixel,RGBPixel> fn;
            switch (this.Operator)
            {
                case Operatoren.Max:
                    fn = (a,b) => a = new RGBPixel(Math.Max(a.R, b.R), Math.Max(a.G, b.G), Math.Max(a.B, b.B));
                    break;
                case Operatoren.Min:
                    fn = (a,b) => a = new RGBPixel(Math.Min(a.R, b.R), Math.Min(a.G, b.G), Math.Min(a.B, b.B));
                    break;
                case Operatoren.Add:
                    fn = (a,b) => a = a + b;
                    break;
                case Operatoren.Sub:
                    fn = (a,b) => a = a - b;
                    break;
                case Operatoren.Mul:
                    fn = (a,b) => a = a * b;
                    break;
                case Operatoren.Div:
                    fn = (a,b) => a = a / b;
                    break;
                default:
                    fn = (a,b) => a;
                    break;
            }

            // erstes Ding klonen.
            RGBPixel[,] res = sources[0].Clone2(o=>o);
            for( int i=1; i<sources.Count; i++)
            {
                res.MapInplace(sources[i], fn);
            }

            return Task.FromResult(res);
        }

        void IDisposable.Dispose()
        {
        }

        public enum Operatoren 
        {
            Max,
            Min,
            Mul,
            Add,
            Sub,
            Div,
        }
    }
}
