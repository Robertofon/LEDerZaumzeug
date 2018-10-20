using System;
using Avalonia;
using Avalonia.Logging.Serilog;
using AvaloniaApplication1.ViewModels;
using AvaloniaApplication1.Views;

namespace AvaloniaApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            BuildAvaloniaApp().Start<MainWindow>(() => new MainWindowViewModel());
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .LogToDebug();
    }
}
