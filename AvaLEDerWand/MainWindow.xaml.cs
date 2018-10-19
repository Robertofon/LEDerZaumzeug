using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace AvaLEDerWand
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            const double spacing_about = 50;
            InitializeComponent();
            LedControlViewModel ledVm = new LedControlViewModel(12, 8);
            this.DataContext = ledVm;
            this.Width = spacing_about * ledVm.Cols;
            this.Height = spacing_about * ledVm.Rows;
            this.AttachDevTools();
            //Renderer.DrawFps = true;
            //Renderer.DrawDirtyRects = Renderer.DrawFps = true;
        }


        private void InitializeComponent()
        {
            //Brushes.DarkGray
            AvaloniaXamlLoader.Load(this);
        }
    }
}