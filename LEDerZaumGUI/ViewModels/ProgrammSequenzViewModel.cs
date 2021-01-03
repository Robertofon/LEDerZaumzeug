using LEDerZaumzeug;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Diagnostics.ViewModels;
using LEDerZaumGUI.Util;

namespace LEDerZaumGUI.ViewModels
{
    public class ProgrammSequenzViewModel : ViewModelBase
    {
        private string _info;
        private SeqItemNode _selSeqItem;
        private List<SeqItemNode> _seq;
        private LEDerZaumZeug _lzSeq;


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
