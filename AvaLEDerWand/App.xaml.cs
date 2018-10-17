using Avalonia;
using Avalonia.Markup.Xaml;

namespace AvaLEDerWand
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
   }
}