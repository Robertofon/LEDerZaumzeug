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
            InitializeComponent();
            this.DataContext = new LedControlViewModel();
        }


        private void InitializeComponent()
        {
            //Brushes.DarkGray
            AvaloniaXamlLoader.Load(this);
        }
    }
}