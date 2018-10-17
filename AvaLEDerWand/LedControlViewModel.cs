using Avalonia.Diagnostics.ViewModels;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AvaLEDerWand
{
    public class LedControlViewModel : ViewModelBase
    {
        private string _name;
        private int t = 0;

        public LedControlViewModel()
        {
            Leds.Add(new LedVm() { LedBrush = Brushes.AliceBlue });
        }

        public void DoKlick(int num)
        {
            Random r = new Random(Environment.TickCount);
            this.Name = "Ohoho" + num;
            this.Leds.Add(new LedVm() {
                LedBrush = new SolidColorBrush((uint)r.Next(int.MaxValue)),
                Row = ((t % 9) /3)*35,
                Col = (t % 3) *35
            });
            t++;
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _name, value);
            }
        }

        public IList<LedVm> Leds { get; } = new ObservableCollection<LedVm>();
    }

    public class LedVm : ViewModelBase
    {
        private ISolidColorBrush _ledBrush;

        public ISolidColorBrush LedBrush
        {
            get
            {
                return _ledBrush;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref _ledBrush, value);
            }
        }

        public double Col { get; set; }
        public double Row { get; set; }
    }
}
