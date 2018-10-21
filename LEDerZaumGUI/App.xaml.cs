using Avalonia;
using Avalonia.Markup.Xaml;

namespace LEDerZaumGUI
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
