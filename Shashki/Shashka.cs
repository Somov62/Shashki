using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Shashki
{
    internal class Shashka
    {
        Border _figure;
        private bool _isSelected;
        private bool _isDamka;
        #region Colors
        private static readonly Color blackBorder = new() { A = 255, R = 40,  G = 40,  B = 40  };
        private static readonly Color blackBack   = new() { A = 255, R = 160, G = 160, B = 160 };
        private static readonly Color blackDamka  = new() { A = 255, R = 80,  G = 80,  B = 80 };
        private static readonly Color whiteBorder = new() { A = 255, R = 147, G = 147, B = 147 };
        private static readonly Color whiteBack   = new() { A = 255, R = 255, G = 255, B = 255 };
        private static readonly Color whiteDamka  = new() { A = 255, R = 200, G = 200, B = 200 };
        private readonly RadialGradientBrush _black = new()
        {
            GradientStops = new GradientStopCollection()
            {
                new GradientStop(blackBorder, 0.317),
                new GradientStop(blackBorder, 0.689),
                new GradientStop(blackBack, 0.234),
                new GradientStop(blackBack, 0.59),
                new GradientStop(blackBack, 0.78),
                new GradientStop(blackBorder, 0.983),
                new GradientStop(blackBack, 0.43),
                new GradientStop(blackBack, 0.915),
            }
        };
        private readonly RadialGradientBrush _white = new()
        {
            GradientStops = new GradientStopCollection()
            {
                new GradientStop(whiteBorder, 0.317),
                new GradientStop(whiteBorder, 0.689),
                new GradientStop(whiteBack, 0.234),
                new GradientStop(whiteBack, 0.59),
                new GradientStop(whiteBack, 0.78),
                new GradientStop(whiteBorder, 0.983),
                new GradientStop(whiteBack, 0.43),
                new GradientStop(whiteBack, 0.915),
            }
        };
        private readonly RadialGradientBrush _blackDamka = new()
        {
            GradientStops = new GradientStopCollection()
            {
                //new GradientStop(blackBorder, 0.317),
                new GradientStop(blackDamka, 0.43),
                new GradientStop(blackDamka, 0.59),
                new GradientStop(blackBack, 0.78),
                new GradientStop(blackDamka, 0.234),
                new GradientStop(blackBorder, 0.689),
                new GradientStop(blackBack, 0.915),
                new GradientStop(blackBorder, 0.983),
            }
        };
        private readonly RadialGradientBrush _whiteDamka = new()
        {
            GradientStops = new GradientStopCollection()
            {
                //new GradientStop(whiteBorder, 0.317),
                new GradientStop(whiteDamka, 0.43),
                new GradientStop(whiteDamka, 0.59),
                new GradientStop(whiteBack, 0.78),
                new GradientStop(whiteDamka, 0.234),
                new GradientStop(whiteBorder, 0.689),
                new GradientStop(whiteBack, 0.915),
                new GradientStop(whiteBorder, 0.983),
            }
        };
        #endregion
        public Shashka(int rowCoord, int columnCoord, Team team)
        {
            Color = team;
            RowCoord = rowCoord;
            ColumnCoord = columnCoord;
            _figure = new Border() { Margin = new Thickness(10), CornerRadius = new CornerRadius(40), Background = team == Team.Black ? _black : _white, IsHitTestVisible = false,  BorderBrush = Brushes.Transparent, BorderThickness =new Thickness(3) };
        }
        public int RowCoord { get; set; }
        public int ColumnCoord { get; set; }
        public Team Color { get; set; }
        public bool IsSelected 
        {
            get => _isSelected;
            set
            { 
                _isSelected = value;
                if (value) _figure.BorderBrush = Brushes.Gold;
                else _figure.BorderBrush = Brushes.Transparent;
            }
        }
        public bool IsDamka { get => _isDamka; set { 
                _isDamka = value;
                if (value && Color == Team.White) _figure.Background = _whiteDamka;
                if (value && Color == Team.Black) _figure.Background = _blackDamka;
                if (!value && Color == Team.White) _figure.Background = _white;
                if (!value && Color == Team.Black) _figure.Background = _black;
            } 
        }
        public Border Figure { get => _figure; }
    }
    public enum Team
    {
        White,
        Black
    }
}
