using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using LEDerZaumzeug;
using System.Threading.Tasks;
using System.IO;
using AvaLEDerWand;

namespace LEDerZaumGUI.ViewModels
{
    public class SzeneEditorViewModel : ViewModelBase
    {
        private static NLog.ILogger log = NLog.LogManager.GetCurrentClassLogger();
        private string _info;
        private SeqItemNode _selSeqItem;
        private List<SeqItemNode> _seq;

        //private string _quelltext = "Nix";
        //public string Quelltext
        //{
        //    get => _quelltext;
        //    set
        //    {
        //        this.RaiseAndSetIfChanged(ref _quelltext, value);
        //    }
        //}

        public string Info
        {
            get { return _info; }
            private set
            {
                this.RaiseAndSetIfChanged(ref _info, value);
            }
        }

        public List<SeqItemNode> Seq
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

        public IList<MusterNode> Test
        {
            get
            {
                return new[] { new MixerNode() { Quelle = { new FilterNode() { Quelle = new GeneratorNode() } } },
                new MixerNode(){ Quelle={new FilterNode(){ Quelle=new GeneratorNode()}, new GeneratorNode() } } };
            }
        }

        public void LadeVonString(string quelltext)
        {
            try
            {
                PixelProgram pgm = SerialisierungsFabrik.ReadProgramFromString(quelltext);
                this.Info = pgm.MetaInfo["Info"]?.ToString();
                this.Seq = pgm.Seq;
            }
            catch (Exception task)
            {
                log.Error(task, "Fehler beim Parsen des Programms");
                MessageBus.Current.SendMessage(task.Message);
            }
        }

    }
}
