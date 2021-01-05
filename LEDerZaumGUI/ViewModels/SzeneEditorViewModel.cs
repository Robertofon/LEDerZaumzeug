using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using ReactiveUI;
using LEDerZaumzeug;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using LEDerWand;
using LEDerZaumGUI.Models;
using LEDerZaumGUI.Util;
using LEDerZaumzeug.Mixer;
using Newtonsoft.Json;

namespace LEDerZaumGUI.ViewModels
{
    public class SzeneEditorViewModel : ReactiveObject
    {
        private static NLog.ILogger log = NLog.LogManager.GetCurrentClassLogger();
        private string _info;
        private SeqItemNode _selSeqItem;
        private BaseNode? _selKnoten;
        private LEDerZaumZeug? _lzSeq = null;
        private LEDerZaumZeug? _lzNode = null;
        private bool _isAutoStartSeqLed = true;
        private bool _isAutoStartKnotenLed = true;

        public SzeneEditorViewModel()
        {
            IObservable<bool> whenKnotenSelected = this.ObservableForProperty(o => o.SelKnoten).Select(sk => sk != null);
            var whenMixerSelected = this.ObservableForProperty(o => o.SelKnoten).Select(sk => sk is MixerNode);
            this.AddFilterCommand = ReactiveCommand.Create(ExecuteAddFilter, whenKnotenSelected);
            this.AddMixerCommand = ReactiveCommand.Create(ExecuteAddMixer, whenKnotenSelected);
            this.AddGenCommand = ReactiveCommand.Create(ExecuteAddGenerator, whenMixerSelected);
            this.DupKnotenCommand = ReactiveCommand.Create(ExecuteDupKnoten, whenKnotenSelected);
            this.WechsleKnotenTypCommand = ReactiveCommand.Create(ExecuteWechsleKnotenTyp, whenKnotenSelected);
            this.EigenschaftenAnwenden = ReactiveCommand.CreateFromTask(ExecuteEigenschaftenAnwenden, whenKnotenSelected);
            this.WhenQuelltextChanged = EigenschaftenAnwenden.AsObservable()
                .CombineLatest(this.Seq.ToObservable().Throttle(TimeSpan.FromMilliseconds(50)), (unit, node) => this.GetAlsString());
            this.ObservableForProperty(o => o.SelSeqItem).Subscribe(async o =>
            {
                _lzSeq?.Stop();

                if (o.Value != null && this._isAutoStartSeqLed)
                {
                    await StartSeqLedPreview(o.Value);
                }
            });

            this.ObservableForProperty(o => o.SelKnoten).Subscribe(async o =>
            {
                _lzNode?.Stop();

                BaseNode node = o.Value;
                if (node != null && this._isAutoStartKnotenLed)
                {
                    await StartKnotenLedPreview(node);
                }
            });
        }

        public ReactiveCommand<Unit, Unit> EigenschaftenAnwenden { get; set; }

        public IObservable<string> WhenQuelltextChanged { get; }

        public ReactiveCommand<Unit, Unit> WechsleKnotenTypCommand { get; set; }

        public ReactiveCommand<Unit, Unit> AddGenCommand { get; set; }

        public ReactiveCommand<Unit, Unit> AddMixerCommand { get; set; }

        public ReactiveCommand<Unit, Unit> DupKnotenCommand { get; set; }

        public ReactiveCommand<Unit, Unit> AddFilterCommand { get; set; }

        public PixelProgram? AktivesProgramm { get; set; }

        public string Info
        {
            get { return _info; }
            private set
            {
                this.RaiseAndSetIfChanged(ref _info, value);
            }
        }

        public ObservableCollection<SeqItemNode> Seq { get; private set; } = new ObservableCollection<SeqItemNode>();

        public SeqItemNode SelSeqItem
        {
            get
            {
                return _selSeqItem;
            }

            set
            {
                this.SelKnoten = null;
                this.RaiseAndSetIfChanged(ref _selSeqItem, value);
            }
        }

        public bool IsAutoStartSeqLed
        {
            get => _isAutoStartSeqLed;
            set => this.RaiseAndSetIfChanged(ref _isAutoStartSeqLed, value);
        }

        public BaseNode? SelKnoten
        {
            get => _selKnoten;
            set => this.RaiseAndSetIfChanged(ref _selKnoten, value);
        }

        public bool IsAutoStartKnotenLed
        {
            get => _isAutoStartKnotenLed;
            set => this.RaiseAndSetIfChanged(ref _isAutoStartKnotenLed, value);
        }

        /// <summary>
        /// Aktueller aktiver Knoten Preview LED.
        /// </summary>
        public LedControlViewModel AktNodeLedViewModel { get; } = new LedControlViewModel(Config.LedDimensionRows, Config.LedDimensionCols);

        /// <summary>
        /// Aktueller aktive Sequenz Preview LED.
        /// </summary>
        public LedControlViewModel AktSeqLedViewModel { get; } = new LedControlViewModel(Config.LedDimensionRows, Config.LedDimensionCols);

        private void StopLedPreview()
        {
            _lzSeq?.Stop();
            _lzNode?.Stop();
        }

        private async Task StartKnotenLedPreview(BaseNode node)
        {
            var lcfg = new LEDerConfig() {Outputs = { }, SeqShowTime = TimeSpan.MaxValue};
            // das muss so:
            var fuzzBuz = SerialisierungsFabrik.Clone(new SeqItemNode() {Start = node as MusterNode});
            var prg = new PixelProgram()
            {
                Seq = {fuzzBuz}
            };
            _lzNode = new LEDerZaumZeug(lcfg, prg);
            await _lzNode.AddOutputsDirect(new InternalAvaOutput(this.AktNodeLedViewModel));
            await _lzNode.StartAsync();
        }

        private async Task StartSeqLedPreview(SeqItemNode snode)
        {
            var lcfg = new LEDerConfig() {Outputs = { }, SeqShowTime = TimeSpan.MaxValue};
            var prg = new PixelProgram()
            {
                Seq = {SerialisierungsFabrik.Clone(snode)}
            };
            _lzSeq = new LEDerZaumZeug(lcfg, prg);
            await _lzSeq.AddOutputsDirect(new InternalAvaOutput(this.AktSeqLedViewModel));
            await _lzSeq.StartAsync();
        }

        public void LadeVonString(string quelltext)
        {
            try
            {
                PixelProgram pgm = SerialisierungsFabrik.ReadProgramFromString(quelltext);
                this.AktivesProgramm = pgm;
                this.Info = pgm.MetaInfo["Info"]?.ToString();
                this.Seq.Clear();
                foreach (SeqItemNode node in pgm.Seq)
                {
                    this.Seq.Add(node);
                }
            }
            catch (Exception task)
            {
                log.Error(task, "Fehler beim Parsen des Programms");
                MessageBus.Current.SendMessage(task.Message);
            }
        }

        public string GetAlsString()
        {
            try
            {
                if (this.AktivesProgramm == null)
                    return null;
                this.AktivesProgramm.Seq.Clear();
                this.AktivesProgramm.Seq.AddRange(this.Seq);
                return SerialisierungsFabrik.WriteProgramToString(this.AktivesProgramm);
            }
            catch (Exception e)
            {
                log.Error(e, "Fehler beim Schreiben des Programms");
                MessageBus.Current.SendMessage(e.Message);
                return null;
            }
        }

        public void SeqItemHinauf(object o)
        {
            if (this.SelSeqItem != null && this.Seq.Any())
            {
                StopLedPreview();
                var tmpItem = this.SelSeqItem;
                int oldidx = this.Seq.IndexOf(tmpItem);
                if (oldidx >= 1)
                {
                    this.Seq.Remove(tmpItem);
                    this.Seq.Insert(oldidx-1, tmpItem);
                    this.SelSeqItem = tmpItem;
                }
            }
        }


        public void SeqItemRunter(object o)
        {
            if (this.SelSeqItem != null && this.Seq.Any())
            {
                StopLedPreview();
                var tmpItem = this.SelSeqItem;
                int oldidx = this.Seq.IndexOf(tmpItem);
                if (oldidx >= 0 && oldidx+1 != this.Seq.Count)
                {
                    this.Seq.Remove(this.SelSeqItem);
                    this.Seq.Insert(oldidx+1, tmpItem);
                    this.SelSeqItem = tmpItem;
                }
            }
        }

        public void SeqItemDup(object o)
        {
            if (this.SelSeqItem != null && this.Seq.Any())
            {
                StopLedPreview();
                SeqItemNode tmpItem = this.SelSeqItem;
                int idx = this.Seq.IndexOf(tmpItem);
                SeqItemNode dupItem = SerialisierungsFabrik.Clone(tmpItem);
                dupItem.Name = dupItem.Name + "-Kopie";
                this.Seq.Insert(idx, dupItem);
            }
        }

        public void SeqItemEntf(object o)
        {
            if (this.SelSeqItem != null && this.Seq.Any())
            {
                StopLedPreview();
                var tmpItem = this.SelSeqItem;
                this.Seq.Remove(tmpItem);
            }
        }

        public void SeqItemHinzu(object o)
        {
            if (this.Seq != null)
            {
                StopLedPreview();

                var neuer = new SeqItemNode() {Name ="Seq0", Start = new GeneratorNode(){ TypeName = "LEDerZaumzeug.Generators.SolidColor" } };
                int idx = this.Seq.IndexOf(this.SelSeqItem);
                if (idx == -1)
                {
                    this.Seq.Add(neuer);
                }
                else
                {
                    this.Seq.Insert(idx, neuer);
                }

                this.SelSeqItem = neuer;
            }
        }

        private void ExecuteAddFilter()
        {
            if (this.SelKnoten != null)
            {

            }
        }

        private void ExecuteDupKnoten()
        {
            if (this.SelKnoten != null)
            {

            }

        }

        private void ExecuteWechsleKnotenTyp()
        {
        }

        private void ExecuteAddGenerator()
        {
        }

        private void ExecuteAddMixer()
        {
        }

        private Task ExecuteEigenschaftenAnwenden()
        {
            if(this.SelKnoten== null)
                return Task.CompletedTask;

            StopLedPreview();

            if (this.SelKnoten is MusterNode gn)
            {
                gn.SyncFromInst();
            }

            var alsString = this.GetAlsString();

            return Task.WhenAll( StartSeqLedPreview(this.SelSeqItem), StartKnotenLedPreview(this.SelKnoten));
        }

        private BaseNode? FindeVorfahr(BaseNode knoten, BaseNode? start = null)
        {
            start = start ?? this.SelSeqItem.Start;
            if (start == knoten)
                return null;
            else
            {
                if (IstDirektDrunter(knoten, start))
                    return start;
                if (start is FilterNode fn)
                {
                    return FindeVorfahr(knoten, fn.Quelle);
                }
                if (start is MixerNode mn)
                {
                    foreach (var node in mn.Quelle)
                    {
                        var n = FindeVorfahr(knoten, node);
                        if (n != null)
                            return n;
                    }
                }
            }

            return null;
        }

        private bool IstDirektDrunter(BaseNode knoten, BaseNode start)
        {
            if (start is FilterNode fn)
                return fn.Quelle == knoten;
            if (start is MixerNode mn)
                return mn.Quelle.Any(o => o == knoten);
            return false;
        }

    }
}
