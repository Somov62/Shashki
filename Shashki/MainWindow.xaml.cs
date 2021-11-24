﻿using System;
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
        private List<Shashka> _whiteShashks = new List<Shashka>()
        {
            new Shashka(0, 1, Team.White),
            new Shashka(0, 3, Team.White),
            new Shashka(0, 5, Team.White),
            new Shashka(0, 7, Team.White),
            new Shashka(1, 0, Team.White),
            new Shashka(1, 2, Team.White),
            new Shashka(1, 4, Team.White),
            new Shashka(1, 6, Team.White),
            new Shashka(2, 1, Team.White),
            new Shashka(2, 3, Team.White),
            new Shashka(2, 5, Team.White),
            new Shashka(2, 7, Team.White),
        };
        private List<Shashka> _blackShashks = new List<Shashka>()
        {
            new Shashka(6, 1, Team.Black),
            new Shashka(6, 3, Team.Black),
            new Shashka(6, 5, Team.Black),
            new Shashka(6, 7, Team.Black),
            new Shashka(7, 0, Team.Black),
            new Shashka(7, 2, Team.Black),
            new Shashka(7, 4, Team.Black),
            new Shashka(7, 6, Team.Black),
            new Shashka(5, 0, Team.Black),
            new Shashka(5, 2, Team.Black),
            new Shashka(5, 4, Team.Black),
            new Shashka(5, 6, Team.Black),
        };
        private List<Shashka> _fookShashks = new List<Shashka>();
        private bool _eatEvent = false;
        private bool IsWhiteGo = true;
        private Shashka SelectedShahka;
        public MainWindow()
        {
            InitializeComponent();
            Output();
        }

        private void Output()
        {
            playGround.Children.Clear();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((i + j) % 2 == 1)
                    {
                        Border br = new Border() { Background = Brushes.Black };
                        Button btn = new Button() { BorderThickness = new Thickness(0), Background = Brushes.Transparent, DataContext = br };
                        btn.Click += Button_Click;
                        br.Child = btn;
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
                    FindShashka(row, column, _whiteShashks, true);
                    return;
                }
                else
                {
                    FindShashka(row, column, _blackShashks, true);
                    return;
                }
            }
            if (SelectedShahka.IsSelected == true)
            {
                CheckFook();
                if (Step(sender))
                {
                    if (_eatEvent)
                    {
                        if (CheckEat(SelectedShahka))
                        {
                            Output();
                            return;
                        }
                    }
                    else
                    {
                        if (_fookShashks.Count > 0)
                        {
                            var removeItem = _fookShashks[0];
                            RemoveShashka(removeItem.RowCoord, removeItem.ColumnCoord, removeItem.Color);
                        }
                    }
                    Output();
                    SelectedShahka.IsSelected = false;
                    IsWhiteGo = !IsWhiteGo;
                    _eatEvent = false;
                }
            }
        }

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
        private (int row, int column) GetCoord(object sender)
        {
            Button btn = (Button)sender;
            Border br = (Border)btn.DataContext;
            int row = Grid.GetRow(br);
            int column = Grid.GetColumn(br);
            return (row, column);
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
    }
}
