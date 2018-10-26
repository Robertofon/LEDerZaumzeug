using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using NLog;
using System.Threading;
using LEDerZaumzeug.Extensions;

namespace LEDerZaumzeug
{
    /// <summary>
    /// Hauptklasse, instanzieren um zu arbeiten.
    /// </summary>
    public class LEDerZaumZeug : IDisposable
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private readonly LEDerConfig config;
        private PixelProgram sequenz;
        private MusterPipeline activePipeline;
        private readonly List<IOutput> outputs = new List<IOutput>();
        private CancellationTokenSource cts;
        private Thread runthread;

        // Rechengrößen für max dimensionen.
        private uint masterSizeX, masterSizeY;

        // für den Szenenwechsel siehe config.
        private TimeSpan szeneStart;
        private int szenenindex;

        public LEDerZaumZeug(LEDerConfig config, PixelProgram sequenz)
        {
            this.config = config;
            this.sequenz = sequenz;
            this.activePipeline = null;
        }

        public void Dispose()
        {
            Stop();
        }

        public async Task StartAsync()
        {
            Console.Write("Start");

            // Outputs initialisieren
            this.MesseOutputsEin();

            // Gette erstes Muster aus der Mustersequenz des LED-Programms.
            this.szenenindex = 0;
            SeqItemNode prg1 = this.sequenz.Seq[this.szenenindex];
            // Daraus eine pipeline bauen:
            var engine = new MusterPipeline(prg1.Start);

            try
            {
                engine.Initialisiere(new MatrixParams() { SizeX = masterSizeX, SizeY = masterSizeY });
            }
            catch(Exception ex)
            {
                log.Error(ex, "Fehler beim erstellen der Generator und Mixer-Objekte");
                return;
            }

            this.activePipeline = engine;

            this.cts = new CancellationTokenSource();
            this.runthread = new Thread(Run);
            this.runthread.Start();
        }

        private async void Run()
        {
            // Zielvorgaben für Bilder pro sekunde und daher millisekunden pro Bild
            double zielfps = 25;
            TimeSpan spanPf = TimeSpan.FromMilliseconds(Convert.ToInt64(1 / zielfps * 1000));
            CancellationToken token = this.cts.Token;
            ulong frame = 0;

            var sw = new Stopwatch();
            sw.Start();

            // Programmwechsel
            this.szeneStart = sw.Elapsed;

            // Bilder in der Schleife generieren
            // Ende durch Cancellationtoken
            while (!token.IsCancellationRequested)
            {
                TimeSpan startticks = sw.Elapsed;
                this.PeriodischerSzenenwechselCheck(startticks);
                RGBPixel[,] bild = await this.activePipeline.ExecuteAsync(frame++);
                var outputtasks = this.outputs.Select( output => output.Play(bild));
                await Task.WhenAll(outputtasks.ToArray());

                // Zeit aufsyncen - warten so lange, bis FPS rand kommt.
                TimeSpan dauer = sw.Elapsed - startticks;
                TimeSpan wartespan = spanPf - dauer;
                wartespan = wartespan.LimitTo(TimeSpan.Zero, TimeSpan.MaxValue);
                log.Info("Aktuell dauer: " + dauer + " Warte: " + wartespan.ToString() + " Msek: " + sw.ElapsedMilliseconds);
                await Task.Delay(wartespan);
            }

            sw.Stop();
            log.Info("Cancellation getriggert - räume outputs auf");
            this.outputs.ForEach(o => o.Dispose());
            log.Info("Ende jetzt");
        }

        private void PeriodischerSzenenwechselCheck(TimeSpan aktuelleTicks)
        {
            if( (aktuelleTicks - this.szeneStart) >= this.config.SeqShowTime )
            {
                this.szeneStart = aktuelleTicks;
                log.Info("Szenenwechsel!"); 
                // TODO: Noch implementieren
            }
        }

        /// <summary>
        /// Liest die Konfig und fügt alle Outputs hinzu und ruft
        /// Initialize, womit sie auch die Konfig kennen. <see cref="IOutputs"/>
        /// sind erstellt und parametriert. Am Ende ist die interne Liste outputs
        /// populiert.
        /// </summary>
        /// <returns></returns>
        public async Task AddOutputsFromCfg()
        {
            foreach (var outpn in this.config.Outputs)
            {
                // Zeugt die Instanz beim Zugriff
                IOutput o = outpn.Inst;
                bool erfolg = await o.Initialize(this.config);
                if (erfolg == true)
                {
                    this.outputs.Add(o);
                }
                else
                {
                    Console.WriteLine("lasse Output " + o.GetType().Name + " weg.");
                }
            }
        }

        /// <summary>
        /// Fügt erstellte und parametrierte Outputs zur internen Liste hinzu.
        /// Ruft Initialize auf.
        /// 
        /// Das ist eine möglichkeit, um selbst erstellte <see cref="IOutputs"/> zuzufügen.
        /// </summary>
        /// <param name="outputs"></param>
        public async Task AddOutputsDirect(params IOutput[] outputs)
        {
            foreach(var o in outputs)
            {
                if(await o.Initialize(this.config))
                {
                    this.outputs.Add(o);
                }
            }
        }

        /// <summary>
        /// Schaut alle Outputs an und ermittelt die korrekte Auflösung, in der gearbeitet werden soll.
        /// </summary>
        private void MesseOutputsEin()
        {
            // Erkenne Outputs als die Dimension
            var masterout = this.outputs.FirstOrDefault(ou => ou.SizeMode == SizeModes.Static);
            if (masterout != null)
            {
                log.Info("Fixes Output gefunden: " + masterout.SizeX + "," + masterout.SizeY);
                this.masterSizeX = (uint)masterout.SizeX;
                this.masterSizeY = (uint)masterout.SizeY;
                foreach (var outpn in this.outputs)
                {
                    outpn.SetSize(masterout.SizeX, masterout.SizeY);
                }
            }
        }

        public void Stop()
        {
            this.cts.Cancel();
            this.runthread.Join();
        }




    }
}
