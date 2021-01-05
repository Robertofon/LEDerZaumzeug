using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using ReactiveUI;
using LEDerZaumzeug;
using System.Threading.Tasks;
using System.IO;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using LEDerWand;
using LEDerZaumGUI.Models;
using LEDerZaumGUI.Util;

namespace LEDerZaumGUI.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private InternalAvaOutput _avaOutput;
        private LEDerZaumZeug _lz;
        private LEDerConfig _lcfg = new LEDerConfig() { Outputs = { }, SeqShowTime = TimeSpan.FromSeconds(14) };

        private string _status = "Nix";
        private string _quelltext = null;
        private string? _aktiveDatei = null;

        public SzeneEditorViewModel PrgVM { get; } = new SzeneEditorViewModel();
 
        public LedControlViewModel LedViewModel { get; } = new LedControlViewModel(Config.LedDimensionRows, Config.LedDimensionRows);
        
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

        /// <summary>
        /// Aktuell geladenen Datei.
        /// </summary>
        public string? AktiveDatei
        {
            get => _aktiveDatei;
            set
            {
                this.RaiseAndSetIfChanged(ref _aktiveDatei, value);
            }
        }

        public async Task DoProgrammLaden()
        {
            string str = await CommonErrorHandling.Current.OpenFileDialog();
            await LadeVonDatei(str);
        }

        public async Task DoProgrammLadenStd()
        {
            await LadeVonDatei("Programm.ledp");
        }

        IDisposable _quelltextUpdater;
        private async Task LadeVonDatei(string dateiname)
        {
            dateiname = Path.GetFullPath(dateiname);
            using (var fs = File.OpenText(dateiname))
            {
                Quelltext = await fs.ReadToEndAsync();
            }

            this.AktiveDatei = dateiname;
            Status = $"Programm geladen : '{dateiname}'";
            PrgVM.LadeVonString(this.Quelltext);
            _quelltextUpdater = PrgVM.WhenQuelltextChanged.Subscribe(qtext => this.Quelltext = qtext);
            //PrgVM.Seq.CollectionChanged -= UpdateQuelltext;
            //PrgVM.Seq.CollectionChanged += UpdateQuelltext;
        }

        public void DoProgrammEntladen()
        {
            this.AktiveDatei = null;
            this.Quelltext = "";
            _quelltextUpdater?.Dispose();
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
            string path = await CommonErrorHandling.Current.SaveFileDialog(this.AktiveDatei);
            using (var fs = File.CreateText(path))
            {
                await fs.WriteAsync(this.Quelltext);
            }

            Status = "Programm gespeichert";
            this.AktiveDatei = path;
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
