using Avalonia.Diagnostics.ViewModels;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AvaLEDerWand
{
    public class LedControlViewModel : ViewModelBase
    {
        private string _name;

        public LedControlViewModel(int rows, int cols)
        {
            this._rows = rows;
            this._cols = cols;
            ZeilenSpaltenAblgeich(rows, cols);
        }

        private void ZeilenSpaltenAblgeich(int rows, int cols)
        {
            if (rows * cols != Leds.Count)
            {
                Leds.Clear();
                for (int i = 0; i < rows * cols; i++)
                {
                    Leds.Add(new LedVm() { LedBrush = Brushes.Black });
                }
            }
        }

        public void FeedData(Color[] data)
        {
            int i = 0;
            foreach( var d in data)
            {
                this.Leds[i++].LedBrush = new ImmutableSolidColorBrush(d);
            }
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
                this.ZeilenSpaltenAblgeich(_rows, _cols);
            }
        }

        public int Rows
        {
            get => _rows; set
            {
                this.RaiseAndSetIfChanged(ref _rows, value);
                this.ZeilenSpaltenAblgeich(_rows, _cols);
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
