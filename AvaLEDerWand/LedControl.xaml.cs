using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaLEDerWand
{
    public class LedControl : UserControl
    {
        public LedControl()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
