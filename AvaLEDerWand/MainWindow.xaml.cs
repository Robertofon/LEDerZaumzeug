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
            MainWindowViewModel viewModel = new MainWindowViewModel(this);
            this.DataContext = viewModel;
            this.Width = spacing_about * viewModel.LedVm.Cols;
            this.Height = spacing_about * viewModel.LedVm.Rows;
            //this.AttachDevTools();
            Renderer.DrawFps = true;
            //Renderer.DrawDirtyRects = Renderer.DrawFps = true;
        }


        private void InitializeComponent()
        {
            //Brushes.DarkGray
            AvaloniaXamlLoader.Load(this);
        }
    }
}