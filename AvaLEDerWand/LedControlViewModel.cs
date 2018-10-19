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

        public LedControlViewModel(int rows, int cols)
        {
            this._rows = rows;
            this._cols = cols;
            for( int i=0; i<rows*cols; i++)
                Leds.Add(new LedVm() { LedBrush = Brushes.AliceBlue });
        }

        public void FeedData(int[] data)
        {
            int i = 0;
            foreach( var d in data)
            {
                this.Leds[i++].LedBrush = new SolidColorBrush((uint) d);
            }
        }

        public void DoKlick(int num)
        {
            Random r = new Random(Environment.TickCount);
            this.Name = "Ohoho" + num;
            this.Leds.Add(new LedVm() {
                LedBrush = new SolidColorBrush((uint)r.Next(int.MaxValue))
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

        private int _rows;
        private int _cols;

        public int Cols
        {
            get => _cols; set
            {
                this.RaiseAndSetIfChanged(ref _cols, value);
            }
        }

        public int Rows
        {
            get => _rows; set
            {
                this.RaiseAndSetIfChanged(ref _rows, value);
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
    }
}
