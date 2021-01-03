using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using ReactiveUI;
using LEDerZaumzeug;
using System.Threading.Tasks;
using System.IO;
using System.Reactive.Linq;
using AvaLEDerWand;
using Avalonia.Diagnostics.ViewModels;
using LEDerZaumGUI.Util;

namespace LEDerZaumGUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private InternalAvaOutput _avaOutput;
        private LEDerZaumZeug _lz;
        private LEDerConfig _lcfg = new LEDerConfig() { Outputs = { }, SeqShowTime = TimeSpan.FromSeconds(14) };

        private string _status = "Nix";
        private string _quelltext = "Nix";

        public SzeneEditorViewModel PrgVM { get; } = new SzeneEditorViewModel();
 
        public LedControlViewModel LedViewModel { get; } = new LedControlViewModel(24,16);

        public string Status
        {
            get => _status;
            set
            {
                this.RaiseAndSetIfChanged(ref _status, value);
            }
        }

        public string Quelltext
        {
            get => _quelltext;
            set
            {
                this.RaiseAndSetIfChanged(ref _quelltext, value);
            }
        }

        public async Task DoProgrammLaden()
        {
            using (var fs = File.OpenText("Programm.ledp"))
            {
                Quelltext = await fs.ReadToEndAsync();
            }

            Status = "Programm geladen";
            PrgVM.LadeVonString(this.Quelltext);
            PrgVM.Seq.CollectionChanged -= UpdateQuelltext;
            PrgVM.Seq.CollectionChanged += UpdateQuelltext;
        }

        private void UpdateQuelltext(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            string programmjson = PrgVM.GetAlsString();
            if (programmjson != null)
            {
                this.Quelltext = programmjson;
            }
        }

        public async Task DoProgrammSpeichern()
        {
            using (var fs = File.CreateText("Programm1.ledp"))
            {
                await fs.WriteAsync(this.Quelltext);
            }

            Status = "Programm gespeichert";
        }

        public async Task DoStartPixelei(object o)
        {
            try
            {
                var prog = SerialisierungsFabrik.ReadProgramFromString(Quelltext);

                _avaOutput = new InternalAvaOutput(this.LedViewModel);

                _lz = new LEDerZaumZeug(_lcfg, prog);
                Status = "Outputs zufügen";

                await _lz.AddOutputsDirect(_avaOutput);

                Status = "Startend";
                await _lz.StartAsync();
                Status = "Läuft";
            }
            catch(Exception ex)
            {
                Status = ex.Message;
            }
        }

        public void DoStopPixelei(object o)
        {
            try
            {
                _lz.Stop();
            }
            catch(Exception task)
            {

            }
            Status = "geStoppt";
        }

        public void DoPausePixelei(object o)
        {

        }

    }
}
