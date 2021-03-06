﻿using System;
using Avalonia;
using Avalonia.ReactiveUI;
using LEDerZaumGUI.ViewModels;
using LEDerZaumGUI.Views;

namespace LEDerZaumGUI
{
    class Program
    {
        // This method is needed for IDE previewer infrastructure
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>().UsePlatformDetect().LogToDebug();

        // The entry point. Things aren't ready yet, so at this point
        // you shouldn't use any Avalonia types or anything that expects
        // a SynchronizationContext to be ready
        public static int Main(string[] args)
            => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }
}
