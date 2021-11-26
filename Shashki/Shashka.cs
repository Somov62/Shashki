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
        public Shashka(int rowCoord, int columnCoord, Team team)
        {
            Color = team;
            RowCoord = rowCoord;
            ColumnCoord = columnCoord;
            _figure = new Border() { Margin = new Thickness(20), CornerRadius = new CornerRadius(30), Background = team == Team.Black ? Brushes.DarkGray : Brushes.White, IsHitTestVisible = false,  BorderBrush = Brushes.Transparent, BorderThickness =new Thickness(3) };
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
