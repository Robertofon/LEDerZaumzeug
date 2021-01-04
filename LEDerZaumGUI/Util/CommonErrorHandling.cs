﻿using System;
using System.Collections.Generic;
using System.Text;
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
                Window mainWindow = ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow;
                errorDlg.ShowDialog(mainWindow);
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