using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Shashki
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields
        private bool _eatEvent = false;
        private bool IsWhiteGo = true;
        private bool _fook = false;

        private int _damkEatRow;
        private int _damkEatColumn;

        private Shashka SelectedShahka;

        private List<Shashka> _whiteShashks = new List<Shashka>(12)
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
        private List<Shashka> _blackShashks = new List<Shashka>(12)
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
        public bool EatEvent { get => _eatEvent; set { _eatEvent = value; finishStep.IsEnabled = value; } }

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
            whiteCount.Content = (_whiteShashks.Capacity - _whiteShashks.Count).ToString();
            blackCount.Content = (_blackShashks.Capacity - _blackShashks.Count).ToString();
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            (int row, int column) = GetCoord(sender);
            if (!EatEvent && (FindShashka(row, column, _blackShashks) || FindShashka(row, column, _whiteShashks)))
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
                    if (EatEvent)
                    {
                        _fook = false;
                        if (CheckEat(SelectedShahka))
                        {
                            Output();
                            return;
                        }
                    }
                    FinishStep_Click(this, null);
                }
            }
        }

        private bool Step(object sender)
        {
            (int row, int column) = GetCoord(sender);
            if (!SelectedShahka.IsDamka)
            {
                //если шашка не является дамкой...
                if (!EatEvent)
                {
                    if (SelectedShahka.Color == Team.Black)
                    {
                        if (SelectedShahka.RowCoord - row == 1 && Math.Abs(SelectedShahka.ColumnCoord - column) == 1)
                        {
                            if (!FindShashka(row, column, _blackShashks) && !FindShashka(row, column, _whiteShashks))
                            {
                                SelectedShahka.RowCoord = row;
                                SelectedShahka.ColumnCoord = column;
                                EatEvent = false;
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
                                EatEvent = false;
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
                            EatEvent = true;
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
                            EatEvent = true;
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
                            EatEvent = true;
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
                            EatEvent = true;
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
                    if (!EatEvent && CheckDamkaCanGo(SelectedShahka, (SelectedShahka.RowCoord - row), (SelectedShahka.ColumnCoord - column)))
                    {
                        SelectedShahka.RowCoord = row;
                        SelectedShahka.ColumnCoord = column;
                        EatEvent = false;
                        return true;
                    }
                    if (CheckDamkaCanEat(SelectedShahka, (SelectedShahka.RowCoord - row), (SelectedShahka.ColumnCoord - column)))
                    {

                        List<Shashka> enemyList = new List<Shashka>();
                        if (SelectedShahka.Color == Team.Black) enemyList = _whiteShashks;
                        else enemyList = _blackShashks;
                        RemoveShashka(_damkEatRow, _damkEatColumn, enemyList);
                        SelectedShahka.RowCoord = row;
                        SelectedShahka.ColumnCoord = column;
                        EatEvent = true;
                        return true;
                    }
                }
            }
            return false;
        }
        public void FinishStep_Click(object sender, RoutedEventArgs e)
        {
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
            if (!CheckMove())
            {
                Win();
                return;
            }
            stepBorder.IsEnabled = IsWhiteGo;
            whitePart.IsEnabled = IsWhiteGo;
            blackPart.IsEnabled = !IsWhiteGo;
            EatEvent = false;
        }
        private void Win()
        {
            DoubleAnimation opacityAnimation = new()
            {
                To = 0.7,
                Duration = new TimeSpan(0, 0, 2),
            };
            backWin.BeginAnimation(Border.OpacityProperty, opacityAnimation);
            DoubleAnimation opacityAnimation2 = new()
            {
                To = 1,
                Duration = new TimeSpan(0, 0, 3),
                BeginTime = new TimeSpan(0, 0, 1)
            };
            winLabel.BeginAnimation(Border.OpacityProperty, opacityAnimation2);
            backWin.IsHitTestVisible = true;
            switch (IsWhiteGo)
            {
                case false:
                    winLabel.Content = "победа за белыми";
                    break;
                case true:
                    winLabel.Content = "победа за чёрными";
                    break;
            }
            finishStep.IsEnabled = false;
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
        public void Restart_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = new();
            this.Close();
            window.Show();
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            if (back.Opacity == 1)
            {
                back.BeginAnimation(Border.OpacityProperty, null);
                titry.BeginAnimation(Label.MarginProperty, null);
                back.Height = 0;
                back.Opacity = 0;
                titry.Margin = new Thickness(0, 800, 0, 0);
                Keyboard.ClearFocus();
                return;
            }
            SolidColorBrush animatedBrush = new SolidColorBrush();
            try
            {
                this.RegisterName("AnimatedBrush", animatedBrush);
            }
            catch { }

            DoubleAnimation heightAnimation = new()
            {
                To = 800,
                Duration = new TimeSpan(0, 0, 0, 0, 400),
            };
            back.BeginAnimation(Border.HeightProperty, heightAnimation);

            DoubleAnimation opacityAnimation = new()
            {
                To = 1,
                Duration = new TimeSpan(0, 0, 2),
            };
            BounceEase func = new();
            func.EasingMode = EasingMode.EaseOut;
            opacityAnimation.EasingFunction = func;
            back.BeginAnimation(Border.OpacityProperty, opacityAnimation);

            #region ColorAnim на будущее 
            //ColorAnimation colorAnimation = new()
            //{
            //    To = Color.FromArgb(255, 0, 0, 0),
            //    Duration = TimeSpan.FromSeconds(2.5)
            //};
            //Storyboard.SetTargetName(colorAnimation, "AnimatedBrush");
            //Storyboard.SetTargetProperty(colorAnimation, new PropertyPath(SolidColorBrush.ColorProperty));
            //back.Background = animatedBrush;
            //ElasticEase func = new();
            //func.EasingMode = EasingMode.EaseOut;
            //colorAnimation.EasingFunction = func;
            //// Create a storyboard to apply the animation.
            //Storyboard myStoryboard = new Storyboard();
            //myStoryboard.Children.Add(colorAnimation);
            //myStoryboard.Begin(this);
            #endregion

            ThicknessAnimation thicknessAnimation = new ThicknessAnimation()
            {
                To = new Thickness(0, -1850, 0, 0),
                Duration = new TimeSpan(0, 0, 37),
                BeginTime = new TimeSpan(0, 0, 3)
            };
            titry.BeginAnimation(Label.MarginProperty, thicknessAnimation);

            btnTitry.Focus();
            Task closeTitle = new Task(new Action(CloseTitle));
            closeTitle.Start();
        }
        private void CloseTitle()
        {
            Thread.Sleep(40500);
            this.Dispatcher.Invoke(() =>
            {
                back.BeginAnimation(Border.OpacityProperty, null);
                titry.BeginAnimation(Label.MarginProperty, null);
                back.Height = 0;
                back.Opacity = 0;
                titry.Margin = new Thickness(0, 800, 0, 0);
                Keyboard.ClearFocus();
            });
        }
        private void Support_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Хочешь ли ты поддержать разработчиков добрым словом?", "", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                MessageBox.Show("спасибо :З\n\nВот тебе интересные и полезные строчки почитать:\n\nПри бесполом размножении медуза оседает на дно, становясь полипом.\nЗатем становится некоем подобием стопочки маленьких медуз, которые, в свою очередь, отцепляются и разносятся течением, становясь со временем взрослыми особями", "Спасибо)", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else MessageBox.Show("ладно -_-\n\nВот тебе интересные и полезные строчки почитать:\n\nПри бесполом размножении медуза оседает на дно, становясь полипом.\nЗатем становится некоем подобием стопочки маленьких медуз, которые, в свою очередь, отцепляются и разносятся течением, становясь со временем взрослыми особями", "", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
