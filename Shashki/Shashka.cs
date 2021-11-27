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
        RadialGradientBrush _black = new RadialGradientBrush()
        {
            GradientStops = new GradientStopCollection()
            {
                new GradientStop(System.Windows.Media.Color.FromRgb(147, 147, 147), 0.317),
                new GradientStop(System.Windows.Media.Color.FromRgb(147, 147, 147), 0.689),
                new GradientStop(System.Windows.Media.Color.FromRgb(0, 0, 0), 0.234),
                new GradientStop(System.Windows.Media.Color.FromRgb(0, 0, 0), 0.59),
                new GradientStop(System.Windows.Media.Color.FromRgb(0, 0, 0), 0.78),
                new GradientStop(System.Windows.Media.Color.FromRgb(147, 147, 147), 0.983),
                new GradientStop(System.Windows.Media.Color.FromRgb(0, 0, 0), 0.43),
                new GradientStop(System.Windows.Media.Color.FromRgb(0, 0, 0), 0.915),
            }
        };
        RadialGradientBrush _white = new RadialGradientBrush()
        {
            GradientStops = new GradientStopCollection()
            {
                new GradientStop(System.Windows.Media.Color.FromRgb(147, 147, 147), 0.317),
                new GradientStop(System.Windows.Media.Color.FromRgb(147, 147, 147), 0.689),
                new GradientStop(System.Windows.Media.Color.FromRgb(255, 255, 255), 0.234),
                new GradientStop(System.Windows.Media.Color.FromRgb(255, 255, 255), 0.59),
                new GradientStop(System.Windows.Media.Color.FromRgb(255, 255, 255), 0.78),
                new GradientStop(System.Windows.Media.Color.FromRgb(147, 147, 147), 0.983),
                new GradientStop(System.Windows.Media.Color.FromRgb(255, 255, 255), 0.43),
                new GradientStop(System.Windows.Media.Color.FromRgb(255, 255, 255), 0.915),
            }
        };

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
            get 
            { 
                return _isSelected; 
            } 
            set 
            { 
                _isSelected = value;
                if (value) _figure.BorderBrush = Brushes.Gold;
                else _figure.BorderBrush = Brushes.Transparent;
            }
        }
        public bool IsDamka { get; set; }
        public Border Figure { get => _figure; }
    }
    public enum Team
    {
        White,
        Black
    }
}
