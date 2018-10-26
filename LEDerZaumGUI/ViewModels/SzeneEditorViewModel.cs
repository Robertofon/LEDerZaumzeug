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
        private string _info;
        private SeqItemNode _selnode;

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
        public List<SeqItemNode> Seq { get; private set; }

        public SeqItemNode SelNode
        {
            get
            {
                return _selnode;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref _selnode, value);
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
            catch(Exception task)
            {

            }
        }



    }
}
