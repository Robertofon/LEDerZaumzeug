using LEDerZaumzeug;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace LEDerZaumGUI.ViewModels
{
    public class ProgrammSequenzViewModel : ViewModelBase
    {
        private string _info;
        private MusterNode _selnode;

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
        
        public List<MusterNode> Seq { get; private set; }

        public MusterNode SelNode
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
            catch (Exception task)
            {

            }
        }

    } 
}
