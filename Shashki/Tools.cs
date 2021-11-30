using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Shashki
{
    public partial class MainWindow : Window
    {
        private (int row, int column) GetCoord(object sender)
        {
            Button btn = (Button)sender;
            Border br = (Border)btn.DataContext;
            int row = Grid.GetRow(br);
            int column = Grid.GetColumn(br);
            return (row, column);
        }

        #region FindShashka
        private bool FindShashka(int row, int column, List<Shashka> collection, bool isSelect = false)
        {
            if (row < 0 || row > 7 || column < 0 || column > 7) return true;
            var shashk = collection.Where(shahck => shahck.RowCoord == row)
                                   .Where(shahck => shahck.ColumnCoord == column);
            List<Shashka> list = shashk.ToList();
            if (list.Count != 0)
            {
                if (isSelect)
                {
                    if (SelectedShahka != null)
                    {
                        SelectedShahka.IsSelected = false;
                    }
                    SelectedShahka = list[0];
                    SelectedShahka.IsSelected = true;
                }
                return true;
            }
            return false;
        }
        private bool FindShashka(int row, int column, Team color, bool isSelect = false)
        {
            if (color == Team.Black)
            {
                return FindShashka(row, column, _blackShashks, isSelect);
            }
            else
            {
                return FindShashka(row, column, _whiteShashks, isSelect);
            }
        }
        private bool FindShashka(int row, int column)
        {
            if (FindShashka(row, column, _blackShashks))
            {
                return true;
            }
            else
            {
                return FindShashka(row, column, _whiteShashks);
            }
        }
        #endregion

        #region RemoveShashka
        private void RemoveShashka(int row, int column, List<Shashka> collection)
        {
            var shashk = collection.Where(shahck => shahck.RowCoord == row)
                                   .Where(shahck => shahck.ColumnCoord == column);
            List<Shashka> list = shashk.ToList();
            if (list.Count > 0)
            {
                Shashka rs = list[0];
                collection.Remove(rs);
            }
        }
        private void RemoveShashka(int row, int column, Team color)
        {
            if (color == Team.Black)
            {
                RemoveShashka(row, column, _blackShashks);
            }
            else
            {
                RemoveShashka(row, column, _whiteShashks);
            }
        }
        #endregion

        private Shashka GetShashka(int row, int column, List<Shashka> collection)
        {
            if (row < 0 || row > 7 || column < 0 || column > 7) return null;
            var shashk = collection.Where(shahck => shahck.RowCoord == row)
                                   .Where(shahck => shahck.ColumnCoord == column);
            List<Shashka> list = shashk.ToList();
            if (list.Count != 0)
            {
                return list[0];
            }
            return null;
        }
    }
}
