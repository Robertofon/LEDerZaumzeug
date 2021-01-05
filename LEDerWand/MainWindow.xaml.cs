using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Newtonsoft.Json;
using ReactiveUI;

namespace LEDerWand
{
    public class MainWindow : Window
    {
        private Menu menu;

        public MainWindow()
        {
            const double spacing_about = 50;
            InitializeComponent();

            this.menu = this.FindControl<Menu>("menu");

            KeyBindings.Add(new KeyBinding()
            {
                Command = ReactiveCommand.Create(ToggleFullScreen),
                Gesture = new KeyGesture(Key.F11),
            });
            WandConfig cfg;
            try
            {
                using (var file = File.OpenText(WandConfig.CFG_NAME))
                {
                    cfg = JsonConvert.DeserializeObject<WandConfig>(file.ReadToEnd());
                }
            }
            catch (Exception e)
            {
                cfg = new WandConfig()
                {
                    LedShape = LedShape.Eck,
                    LedCols = 8,
                    LedRows = 12,
                    Protocol = LedProto.TPM2,
                    ListenInterface = "localhost",
                    ListenPort = 65506,
                    StartFullScreen = false,
                };
                var win = new Window();
                win.Content = e.Message;
            }
            MainWindowViewModel viewModel = new MainWindowViewModel(this, cfg);
            this.DataContext = viewModel;
            this.Width = spacing_about * viewModel.LedVm.Cols;
            this.Height = spacing_about * viewModel.LedVm.Rows;
            //this.AttachDevTools();
            Renderer.DrawFps = true;
            //Renderer.DrawDirtyRects = Renderer.DrawFps = true;
        }

        public void ToggleFullScreen()
        {
            if (this.WindowState != WindowState.FullScreen)
            {
                SetFullScreen(true);
            }
            else
            {
                SetFullScreen(false);
            }
        }


        public void SetFullScreen(in bool fs)
        {
            if (fs)
            {
                this.WindowState = WindowState.FullScreen;
                this.menu.SetValue(Menu.MaxHeightProperty, 0);
            }
            else
            {
                this.WindowState = WindowState.Normal;
                this.menu.ClearValue(Menu.MaxHeightProperty);
            }
        }

        private void InitializeComponent()
        {
            //Brushes.DarkGray
            AvaloniaXamlLoader.Load(this);
        }
    }
}