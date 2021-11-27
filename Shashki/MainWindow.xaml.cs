using System;
using System.Collections;
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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields
        private List<Shashka> _whiteShashks = new List<Shashka>()
        {
            new Shashka(0 + 1, 1 + 1, Team.White),
            new Shashka(0 + 1, 3 + 1, Team.White),
            new Shashka(0 + 1, 5 + 1, Team.White),
            new Shashka(0 + 1, 7 + 1, Team.White),
            new Shashka(1 + 1, 0 + 1, Team.White),
            new Shashka(1 + 1, 2 + 1, Team.White),
            new Shashka(1 + 1, 4 + 1, Team.White),
            new Shashka(1 + 1, 6 + 1, Team.White),
            new Shashka(2 + 1, 1 + 1, Team.White),
            new Shashka(2 + 1, 3 + 1, Team.White),
            new Shashka(2 + 1, 5 + 1, Team.White),
            new Shashka(2 + 1, 7 + 1, Team.White),
        };
        private List<Shashka> _blackShashks = new List<Shashka>()
        {
            new Shashka(6 + 1, 1 + 1, Team.Black),
            new Shashka(6 + 1, 3 + 1, Team.Black),
            new Shashka(6 + 1, 5 + 1, Team.Black),
            new Shashka(6 + 1, 7 + 1, Team.Black),
            new Shashka(7 + 1, 0 + 1, Team.Black),
            new Shashka(7 + 1, 2 + 1, Team.Black),
            new Shashka(7 + 1, 4 + 1, Team.Black),
            new Shashka(7 + 1, 6 + 1, Team.Black),
            new Shashka(5 + 1, 0 + 1, Team.Black),
            new Shashka(5 + 1, 2 + 1, Team.Black),
            new Shashka(5 + 1, 4 + 1, Team.Black),
            new Shashka(5 + 1, 6 + 1, Team.Black),
        };
        private List<Shashka> _fookShashks = new List<Shashka>();

        private bool _eatEvent = false;
        private bool IsWhiteGo = true;
        private bool _fook = false;

        private Shashka SelectedShahka;
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            Output();
        }

        private void Output()
        {
            playGround.Children.Clear();
            for (int i = 1; i < 9; i++)
            {
                for (int j = 1; j < 9; j++)
                {
                    if ((i + j) % 2 == 1)
                    {
                        Border br = new Border() { Background = Brushes.DodgerBlue };
                        Button btn = new Button() { BorderThickness = new Thickness(0), Background = Brushes.Transparent, DataContext = br };
                        btn.Click += Button_Click;
                        br.Child = btn;
                        playGround.Children.Add(br);
                        Grid.SetRow(br, i);
                        Grid.SetColumn(br, j);
                    }
                    else
                    {
                        Border br = new Border() { Background = Brushes.Wheat };
                        playGround.Children.Add(br);
                        Grid.SetRow(br, i);
                        Grid.SetColumn(br, j);
                    }
                }
            }
            foreach (var item in _whiteShashks)
            {
                playGround.Children.Add(item.Figure);
                Grid.SetRow(item.Figure, item.RowCoord);
                Grid.SetColumn(item.Figure, item.ColumnCoord);
            }
            foreach (var item in _blackShashks)
            {
                playGround.Children.Add(item.Figure);
                Grid.SetRow(item.Figure, item.RowCoord);
                Grid.SetColumn(item.Figure, item.ColumnCoord);
            }
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            (int row, int column) = GetCoord(sender);
            if (!_eatEvent && (FindShashka(row, column, _blackShashks) || FindShashka(row, column, _whiteShashks)))
            {
                if (IsWhiteGo)
                {
                    if (TakeFook(row, column)) return; 
                    FindShashka(row, column, _whiteShashks, true);
                    return;
                }
                else
                {
                    if (TakeFook(row, column)) return;
                    FindShashka(row, column, _blackShashks, true);
                    return;
                }
            }
            if (SelectedShahka.IsSelected == true)
            {
                _fook = CheckFook();
                if (Step(sender))
                {
                    if (_eatEvent)
                    {
                        _fook = false;
                        if (CheckEat(SelectedShahka))
                        {
                            Output();
                            return;
                        }
                    }
                    Output();
                    SelectedShahka.IsSelected = false;
                    IsWhiteGo = !IsWhiteGo;
                    _eatEvent = false;
                }
            }
        }

        private bool Step(object sender)
        {
            (int row, int column) = GetCoord(sender);
            if (!_eatEvent)
            {
                if (SelectedShahka.Color == Team.Black)
                {
                    if (SelectedShahka.RowCoord - row == 1 && Math.Abs(SelectedShahka.ColumnCoord - column) == 1)
                    {
                        if (!FindShashka(row, column, _blackShashks) && !FindShashka(row, column, _whiteShashks))
                        {
                            SelectedShahka.RowCoord = row;
                            SelectedShahka.ColumnCoord = column;
                            _eatEvent = false;
                            return true;
                        }
                    }
                }
                if (SelectedShahka.Color == Team.White)
                {
                    if (SelectedShahka.RowCoord - row == -1 && Math.Abs(SelectedShahka.ColumnCoord - column) == 1)
                    {
                        if (!FindShashka(row, column, _blackShashks) && !FindShashka(row, column, _whiteShashks))
                        {
                            SelectedShahka.RowCoord = row;
                            SelectedShahka.ColumnCoord = column;
                            _eatEvent = false;
                            return true;
                        }
                    }
                }
            }
            if (Math.Abs(SelectedShahka.RowCoord - row) == 2 && Math.Abs(SelectedShahka.ColumnCoord - column) == 2)
            {
                if (FindShashka(row, column, _blackShashks)) return false;
                if (FindShashka(row, column, _whiteShashks)) return false;
                List<Shashka> enemyList = new List<Shashka>();
                if (SelectedShahka.Color == Team.Black) enemyList = _whiteShashks;
                else enemyList = _blackShashks;
                if (SelectedShahka.RowCoord > row && SelectedShahka.ColumnCoord > column)
                {
                    if (FindShashka(SelectedShahka.RowCoord - 1, SelectedShahka.ColumnCoord - 1, enemyList))
                    {
                        RemoveShashka(SelectedShahka.RowCoord - 1, SelectedShahka.ColumnCoord - 1, enemyList);
                        SelectedShahka.RowCoord = row;
                        SelectedShahka.ColumnCoord = column;
                        _eatEvent = true;
                        return true;
                    }
                }
                if (SelectedShahka.RowCoord < row && SelectedShahka.ColumnCoord < column)
                {
                    if (FindShashka(SelectedShahka.RowCoord + 1, SelectedShahka.ColumnCoord + 1, enemyList))
                    {
                        RemoveShashka(SelectedShahka.RowCoord + 1, SelectedShahka.ColumnCoord + 1, enemyList);
                        SelectedShahka.RowCoord = row;
                        SelectedShahka.ColumnCoord = column;
                        _eatEvent = true;
                        return true;
                    }
                }
                if (SelectedShahka.RowCoord < row && SelectedShahka.ColumnCoord > column)
                {
                    if (FindShashka(SelectedShahka.RowCoord + 1, SelectedShahka.ColumnCoord - 1, enemyList))
                    {
                        RemoveShashka(SelectedShahka.RowCoord + 1, SelectedShahka.ColumnCoord - 1, enemyList);
                        SelectedShahka.RowCoord = row;
                        SelectedShahka.ColumnCoord = column;
                        _eatEvent = true;
                        return true;
                    }
                }
                if (SelectedShahka.RowCoord > row && SelectedShahka.ColumnCoord < column)
                {
                    if (FindShashka(SelectedShahka.RowCoord - 1, SelectedShahka.ColumnCoord + 1, enemyList))
                    {
                        RemoveShashka(SelectedShahka.RowCoord - 1, SelectedShahka.ColumnCoord + 1, enemyList);
                        SelectedShahka.RowCoord = row;
                        SelectedShahka.ColumnCoord = column;
                        _eatEvent = true;
                        return true;
                    }
                }
            }
            return false;
        }
        
        private bool CheckEat(Shashka shashka)
        {
            List<Shashka> enemyList = new List<Shashka>();
            if (shashka.Color == Team.Black) enemyList = _whiteShashks;
            if (shashka.Color == Team.White) enemyList = _blackShashks;
            if (FindShashka(shashka.RowCoord - 1, shashka.ColumnCoord - 1, enemyList))
            {
                if (!FindShashka(shashka.RowCoord - 2, shashka.ColumnCoord - 2, _blackShashks))
                {
                    if (!FindShashka(shashka.RowCoord - 2, shashka.ColumnCoord - 2, _whiteShashks)) return true;
                }
            }
            if (FindShashka(shashka.RowCoord - 1, shashka.ColumnCoord + 1, enemyList))
            {
                if (!FindShashka(shashka.RowCoord - 2, shashka.ColumnCoord + 2, _blackShashks))
                {
                    if (!FindShashka(shashka.RowCoord - 2, shashka.ColumnCoord + 2, _whiteShashks)) return true;
                }
            }
            if (FindShashka(shashka.RowCoord + 1, shashka.ColumnCoord - 1, enemyList))
            {
                if (!FindShashka(shashka.RowCoord + 2, shashka.ColumnCoord - 2, _blackShashks))
                {
                    if (!FindShashka(shashka.RowCoord + 2, shashka.ColumnCoord - 2, _whiteShashks)) return true;
                }
            }
            if (FindShashka(shashka.RowCoord + 1, shashka.ColumnCoord + 1, enemyList))
            {
                if (!FindShashka(shashka.RowCoord + 2, shashka.ColumnCoord + 2, _blackShashks))
                {
                    if (!FindShashka(shashka.RowCoord + 2, shashka.ColumnCoord + 2, _whiteShashks)) return true;
                }
            }
            return false;
        }

        private bool CheckFook()
        {
            _fookShashks.Clear();
            List<Shashka> friendList = new List<Shashka>();
            if (SelectedShahka.Color == Team.White) friendList = _whiteShashks;
            else friendList = _blackShashks;
            foreach (var item in friendList)
            {
                if (CheckEat(item))
                {
                    _fookShashks.Add(item);
                }
            }
            if (_fookShashks.Count > 0) return true;
            return false;
        }
        private bool TakeFook(int row, int column)
        {
            if (FindShashka(row, column, _fookShashks) && _fook)
            {
                var removeItem = GetShashka(row, column, _fookShashks);
                RemoveShashka(removeItem.RowCoord, removeItem.ColumnCoord, removeItem.Color);
                _fook = false;
                _fookShashks.Clear();
                Output();
                return true;
            }
            return false;
        }
    }
}
