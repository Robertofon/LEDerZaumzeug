using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LEDerZaumGUI.Views
{
    public class SzeneEditorControl : UserControl
    {
        public SzeneEditorControl()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
