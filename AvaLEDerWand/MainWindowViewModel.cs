using Avalonia.Controls;
using ReactiveUI;

namespace AvaLEDerWand
{
    public class MainWindowViewModel : ReactiveObject
    {
        private readonly MainWindow _w;

        public LedControlViewModel LedVm { get; }

        public MainWindowViewModel(MainWindow w)
        {
            _w = w;
            LedVm = new LedControlViewModel(12, 8);
            
        }

        public void Exit()
        {
            _w.Close();
        }

        public void OpenConfig()
        {
            //_w.Close();
        }
    }
}