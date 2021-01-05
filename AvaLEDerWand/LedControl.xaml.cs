using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LEDerWand
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
