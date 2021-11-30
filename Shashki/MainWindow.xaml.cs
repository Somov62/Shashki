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
            new Shashka(0, 1, Team.White){ IsDamka=true},
            //new Shashka(0, 3, Team.White),
            //new Shashka(0, 5, Team.White),
            //new Shashka(0, 7, Team.White),
            //new Shashka(1, 0, Team.White),
            //new Shashka(1, 2, Team.White),
            //new Shashka(1, 4, Team.White),
            //new Shashka(1, 6, Team.White),
            //new Shashka(2, 1, Team.White),
            //new Shashka(2, 3, Team.White),
            //new Shashka(2, 5, Team.White),
            //new Shashka(2, 7, Team.White),
        };
        private List<Shashka> _blackShashks = new List<Shashka>()
        {
            new Shashka(6, 1, Team.Black){ IsDamka=true},
            //new Shashka(6, 3, Team.Black),
            //new Shashka(6, 5, Team.Black),
            //new Shashka(6, 7, Team.Black),
            //new Shashka(7, 0, Team.Black),
            //new Shashka(7, 2, Team.Black),
            //new Shashka(7, 4, Team.Black),
            //new Shashka(7, 6, Team.Black),
            //new Shashka(5, 0, Team.Black),
            //new Shashka(5, 2, Team.Black),
            //new Shashka(5, 4, Team.Black),
            //new Shashka(5, 6, Team.Black),
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
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((i + j) % 2 == 1)
                    {
                        Border br = new Border();
                        Button btn = new Button() { DataContext = br };
                        btn.Click += Button_Click;
                        br.Child = btn;
                        playGround.Children.Add(br);
                        Grid.SetRow(br, i);
                        Grid.SetColumn(br, j);
                    }
                    else
                    {
                        Border br = new Border() { Background = Brushes.White };
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
            if (SelectedShahka == null) return;
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
                    if (SelectedShahka.Color == Team.Black && SelectedShahka.RowCoord == 0)
                    {
                        SelectedShahka.IsDamka = true;
                    }
                    if (SelectedShahka.Color == Team.White && SelectedShahka.RowCoord == 7)
                    {
                        SelectedShahka.IsDamka = true;
                    }
                    Output();
                    SelectedShahka.IsSelected = false;
                    IsWhiteGo = !IsWhiteGo;
                    whitePart.IsEnabled = IsWhiteGo;
                    blackPart.IsEnabled = !IsWhiteGo;
                    _eatEvent = false;
                }
            }
        }
        private bool Step(object sender)
        {
            (int row, int column) = GetCoord(sender);
            if (!SelectedShahka.IsDamka)
            {
                //если шашка не является дамкой...
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
            }
            else
            {
                //если шашка является дамкой...
                //проверка что клик произошол на диагонали дамки
                if (Math.Abs(SelectedShahka.RowCoord - row) == Math.Abs(SelectedShahka.ColumnCoord - column))
                {
                    if (!_eatEvent && CheckDamkaCanGo(SelectedShahka, (SelectedShahka.RowCoord - row), (SelectedShahka.ColumnCoord - column)))
                    {
                        SelectedShahka.RowCoord = row;
                        SelectedShahka.ColumnCoord = column;
                        _eatEvent = false;
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

            if (shashka.IsDamka == true)
            {
                if (CheckEatDamkaOneDiagonal(shashka, -1, -1)) return true;
                if (CheckEatDamkaOneDiagonal(shashka, -1, +1)) return true;
                if (CheckEatDamkaOneDiagonal(shashka, +1, -1)) return true;
                if (CheckEatDamkaOneDiagonal(shashka, +1, +1)) return true;
            }

            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="damka">проверяемая дамка</param>
        /// <param name="x"> если меньше нуля то право иначе лево</param>
        /// <param name="y"> если меньше нуля то верх иначе </param>
        /// <returns></returns>
        private bool CheckEatDamkaOneDiagonal(Shashka damka, int x, int y, bool wasEnemy = false)
        {
            if (x < 0 && y < 0)
            {
                //проверка на наличие дружественной шашки на -1 -1 по отношениию к прошлой проверки
                if (FindShashka(damka.RowCoord - 1, damka.ColumnCoord - 1, damka.Color))
                {
                    return false;
                }

                //вычисляем цвет врага
                Team colorEnemy;
                if (damka.Color == Team.White)
                    colorEnemy = Team.Black;
                else
                    colorEnemy = Team.White;

                //проверка на наличие вражеской шашки на -1 -1 по отношениию к прошлой проверки
                if (FindShashka(damka.RowCoord - 1, damka.ColumnCoord - 1, colorEnemy))
                {
                    if (wasEnemy)
                    {
                        return false;
                    }
                    else
                    {
                        return CheckEatDamkaOneDiagonal(new Shashka(damka.RowCoord - 1, damka.ColumnCoord - 1, damka.Color), x, y, true);
                    }
                }

                //при отсутствии шашек на -1 -1 по отношениию к прошлой проверки
                if (wasEnemy)
                {
                    return true;
                }
                else
                {
                    return CheckEatDamkaOneDiagonal(new Shashka(damka.RowCoord - 1, damka.ColumnCoord - 1, damka.Color), x, y, false);
                }
            }
            if (x >= 0 && y < 0)
            {
                //проверка на наличие дружественной шашки на +1 -1 по отношениию к прошлой проверки
                if (FindShashka(damka.RowCoord + 1, damka.ColumnCoord - 1, damka.Color))
                {
                    return false;
                }

                //вычисляем цвет врага
                Team colorEnemy;
                if (damka.Color == Team.White)
                    colorEnemy = Team.Black;
                else
                    colorEnemy = Team.White;

                //проверка на наличие вражеской шашки на +1 -1 по отношениию к прошлой проверки
                if (FindShashka(damka.RowCoord + 1, damka.ColumnCoord - 1, colorEnemy))
                {
                    if (wasEnemy)
                    {
                        return false;
                    }
                    else
                    {
                        return CheckEatDamkaOneDiagonal(new Shashka(damka.RowCoord + 1, damka.ColumnCoord - 1, damka.Color), x, y, true);
                    }
                }

                //при отсутствии шашек на +1 -1 по отношениию к прошлой проверки
                if (wasEnemy)
                {
                    return true;
                }
                else
                {
                    return CheckEatDamkaOneDiagonal(new Shashka(damka.RowCoord + 1, damka.ColumnCoord - 1, damka.Color), x, y, false);
                }
            }
            if (x < 0 && y >= 0)
            {
                //проверка на наличие дружественной шашки на -1 +1 по отношениию к прошлой проверки
                if (FindShashka(damka.RowCoord - 1, damka.ColumnCoord + 1, damka.Color))
                {
                    return false;
                }

                //вычисляем цвет врага
                Team colorEnemy;
                if (damka.Color == Team.White)
                    colorEnemy = Team.Black;
                else
                    colorEnemy = Team.White;

                //проверка на наличие вражеской шашки на -1 +1 по отношениию к прошлой проверки
                if (FindShashka(damka.RowCoord - 1, damka.ColumnCoord + 1, colorEnemy))
                {
                    if (wasEnemy)
                    {
                        return false;
                    }
                    else
                    {
                        return CheckEatDamkaOneDiagonal(new Shashka(damka.RowCoord - 1, damka.ColumnCoord + 1, damka.Color), x, y, true);
                    }
                }

                //при отсутствии шашек на -1 +1 по отношениию к прошлой проверки
                if (wasEnemy)
                {
                    return true;
                }
                else
                {
                    return CheckEatDamkaOneDiagonal(new Shashka(damka.RowCoord - 1, damka.ColumnCoord + 1, damka.Color), x, y, false);
                }
            }
            if (x >= 0 && y >= 0)
            {
                //проверка на наличие дружественной шашки на +1 +1 по отношениию к прошлой проверки
                if (FindShashka(damka.RowCoord + 1, damka.ColumnCoord + 1, damka.Color))
                {
                    return false;
                }

                //вычисляем цвет врага
                Team colorEnemy;
                if (damka.Color == Team.White)
                    colorEnemy = Team.Black;
                else
                    colorEnemy = Team.White;

                //проверка на наличие вражеской шашки на +1 +1 по отношениию к прошлой проверки
                if (FindShashka(damka.RowCoord + 1, damka.ColumnCoord + 1, colorEnemy))
                {
                    if (wasEnemy)
                    {
                        return false;
                    }
                    else
                    {
                        return CheckEatDamkaOneDiagonal(new Shashka(damka.RowCoord + 1, damka.ColumnCoord + 1, damka.Color), x, y, true);
                    }
                }

                //при отсутствии шашек на +1 +1 по отношениию к прошлой проверки
                if (wasEnemy)
                {
                    return true;
                }
                else
                {
                    return CheckEatDamkaOneDiagonal(new Shashka(damka.RowCoord + 1, damka.ColumnCoord + 1, damka.Color), x, y, false);
                }
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="damka"></param>
        /// <param name="x">равны по модулю(обязательно)-----меньше нуля то низ   если больше то вверх</param>
        /// <param name="y">равны по модулю(обязательно)-----меньше нуля то лево  если больше то право</param>
        /// <param name="wasEnemy"></param>
        /// <returns></returns>
        private bool CheckDamkaCanGo(Shashka damka, int x, int y)
        {
            if (x < 0 && y < 0)
            {
                if (FindShashka(damka.RowCoord + 1, damka.ColumnCoord + 1))
                {
                    return false;
                }
                else
                {
                    x++;
                    y++;
                    return CheckDamkaCanGo(new Shashka(damka.RowCoord + 1, damka.ColumnCoord + 1, damka.Color), x, y);
                }
            }
            if (x > 0 && y < 0)
            {
                if (FindShashka(damka.RowCoord - 1, damka.ColumnCoord + 1))
                {
                    return false;
                }
                else
                {
                    x--;
                    y++;
                    return CheckDamkaCanGo(new Shashka(damka.RowCoord - 1, damka.ColumnCoord + 1, damka.Color), x, y);
                }
            }
            if (x < 0 && y > 0)
            {
                if (FindShashka(damka.RowCoord + 1, damka.ColumnCoord - 1))
                {
                    return false;
                }
                else
                {
                    x++;
                    y--;
                    return CheckDamkaCanGo(new Shashka(damka.RowCoord + 1, damka.ColumnCoord - 1, damka.Color), x, y);
                }
            }
            if (x > 0 && y > 0)
            {
                if (FindShashka(damka.RowCoord - 1, damka.ColumnCoord - 1))
                {
                    return false;
                }
                else
                {
                    x--;
                    y--;
                    return CheckDamkaCanGo(new Shashka(damka.RowCoord - 1, damka.ColumnCoord - 1, damka.Color), x, y);
                }
            }
            if (x == 0)
            {
                return true;
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
