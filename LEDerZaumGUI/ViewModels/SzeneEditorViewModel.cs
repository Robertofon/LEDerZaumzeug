using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using ReactiveUI;
using LEDerZaumzeug;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using AvaLEDerWand;
using Avalonia.Diagnostics.ViewModels;
using LEDerZaumGUI.Models;
using LEDerZaumGUI.Util;
using Newtonsoft.Json;

namespace LEDerZaumGUI.ViewModels
{
    public class SzeneEditorViewModel : ViewModelBase
    {
        private static NLog.ILogger log = NLog.LogManager.GetCurrentClassLogger();
        private string _info;
        private SeqItemNode _selSeqItem;
        private ObservableCollection<SeqItemNode> _seq;
        private BaseNode _selKnoten;
        private LEDerZaumZeug? _lzSeq = null;

        public SzeneEditorViewModel()
        {
            this.ObservableForProperty(o => o.SelSeqItem).Subscribe(async o =>
            {
                _lzSeq?.Stop();

                if (o.Value != null)
                {
                    var lcfg = new LEDerConfig() {Outputs = { }, SeqShowTime = TimeSpan.MaxValue};
                    var prg = new PixelProgram()
                    {
                        Seq = {o.Value}
                    };
                    _lzSeq = new LEDerZaumZeug(lcfg, prg);
                    await _lzSeq.AddOutputsDirect(new InternalAvaOutput(this.AktSeqLedViewModel));
                    await _lzSeq.StartAsync();
                }
            });
        }

        public PixelProgram? AktivesProgramm { get; set; }

        public string Info
        {
            get { return _info; }
            private set
            {
                this.RaiseAndSetIfChanged(ref _info, value);
            }
        }

        public ObservableCollection<SeqItemNode> Seq
        {
            get
            {
                return _seq;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref _seq, value);
            }
        }

        public SeqItemNode SelSeqItem
        {
            get
            {
                return _selSeqItem;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref _selSeqItem, value);
            }
        }

        public BaseNode SelKnoten
        {
            get => _selKnoten;
            set => this.RaiseAndSetIfChanged(ref _selKnoten, value);
        }

        /// <summary>
        /// Aktueller aktiver Knoten Preview LED.
        /// </summary>
        public LedControlViewModel AktNodeLedViewModel { get; } = new LedControlViewModel(Config.LedDimensionRows, Config.LedDimensionCols);

        /// <summary>
        /// Aktueller aktive Sequenz Preview LED.
        /// </summary>
        public LedControlViewModel AktSeqLedViewModel { get; } = new LedControlViewModel(Config.LedDimensionRows, Config.LedDimensionCols);

        public void LadeVonString(string quelltext)
        {
            try
            {
                PixelProgram pgm = SerialisierungsFabrik.ReadProgramFromString(quelltext);
                this.AktivesProgramm = pgm;
                this.Info = pgm.MetaInfo["Info"]?.ToString();
                this.Seq = new ObservableCollection<SeqItemNode>(pgm.Seq);
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
                var tmpItem = this.SelSeqItem;
                this.Seq.Remove(tmpItem);
            }
        }

    }
}
