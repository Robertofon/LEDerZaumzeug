using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using LEDerZaumGUI.Views;

namespace LEDerZaumGUI.Util
{
    public class CommonErrorHandling
    {
        public static CommonErrorHandling Current { get; } = new CommonErrorHandling();

        private CommonErrorHandling()
        {
            this.MainWindow = ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow;

        }

        public Window MainWindow { get; set; }

        public async Task<string> OpenFileDialog()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Datei Öffnen";
            openFileDialog.Filters.Add(new FileDialogFilter() { Extensions = { "ledp" }, Name = "LEDerZaumZeug-Programme" });
            var strs = await openFileDialog.ShowAsync(MainWindow);
            return strs.FirstOrDefault();
            //Avalonia.Dialogs.ManagedFileDialogExtensions.ShowManagedAsync()

        }

        public async Task<string> SaveFileDialog(string? aktiveDatei)
        {
            var dialog = new SaveFileDialog()
            {
                Directory = Path.GetDirectoryName(aktiveDatei),
                DefaultExtension = "ledp",
                InitialFileName = Path.GetFileName(aktiveDatei)
            };
            dialog.Title = "Datei Speichern";
            dialog.Filters.Add(new FileDialogFilter() { Extensions = { "ledp" }, Name = "LEDerZaumZeug-Programme" });
            string strs = await dialog.ShowAsync(MainWindow);
            return strs;
            //Avalonia.Dialogs.ManagedFileDialogExtensions.UseManagedSystemDialogs()
        }

        /// <summary>
        /// Shows a common error dialog for the given exception.
        /// </summary>
        public void ShowErrorDialog(Exception ex)
        {
            if (Dispatcher.UIThread.CheckAccess())
            {
                var errorDlg = new Window();
                errorDlg.Content = new TextBlock() {Text = ex.Message};
                //errorDlg.DataContext = ex.Message;
                errorDlg.ShowDialog(MainWindow);
            }
            else
            {
                Dispatcher.UIThread.Post(
                    () => this.ShowErrorDialog(ex));
            }
        }

        /// <summary>
        /// Handles the given exception which causes the application to break.
        /// </summary>
        public void HandleFatalException(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("##### Fata Exception");
            Console.WriteLine(ex.ToString());
        }
    }
}
