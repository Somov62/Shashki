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
        public Shashka(int rowCoord, int columnCoord, Team team)
        {
            Color = team;
            RowCoord = rowCoord;
            ColumnCoord = columnCoord;
            _figure = new Border() { Margin = new Thickness(20), CornerRadius = new CornerRadius(20), Background = team == Team.Black ? Brushes.Gray : Brushes.White, IsHitTestVisible = false };
        }
        public int RowCoord { get; set; }
        public int ColumnCoord { get; set; }
        public Team Color { get; set; }
        public bool IsSelected { get; set; }
        public Border Figure { get => _figure; }
    }
    public enum Team
    {
        White,
        Black
    }
}
