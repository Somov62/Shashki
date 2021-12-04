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
        private bool _eatEvent = false; //показатель съела ли шашка в прошлом ходу
        private bool IsWhiteGo = true; //показатель кто ходит
        private bool _fook = false; //показатель можно ли взять зафук 

        private int _damkEatRow; 
        private int _damkEatColumn;

        private Shashka SelectedShahka; //выбранная шашка неважно какого цвета
        
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
        private List<Shashka> _fookShashks = new List<Shashka>(); //список шашек, которые можно взять зафук
        public bool EatEvent { get => _eatEvent; set { _eatEvent = value; finishStep.IsEnabled = value; } }//включаем кнопку по состоянию _eatEvent

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Output();
        }
        private void Output()
        {
            playGround.Children.Clear(); //очищаем поле с шашками
            // рисуем заново поле
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Border br = new Border();
                    if ((i + j) % 2 == 1) //в нечетные клетки добавляем кнопки для шашек
                    {
                        Button btn = new Button() { DataContext = br };
                        btn.Click += Button_Click;
                        br.Child = btn;                        
                    }
                    else
                    {
                        br.Background = Brushes.White;//перекрываем синий цвет стиля, установленный в ресурсах
                    }
                    playGround.Children.Add(br);
                    Grid.SetRow(br, i);
                    Grid.SetColumn(br, j);
                }
            }
            //Добавляем шашки на поле
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
            //выводим количество скушанных шашек
            whiteCount.Content = (_whiteShashks.Capacity - _whiteShashks.Count).ToString();
            blackCount.Content = (_blackShashks.Capacity - _blackShashks.Count).ToString();
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            (int row, int column) = GetCoord(sender);//получаем координаты нажатой кнопки
            //Выбор шашки
            if (!EatEvent && (FindShashka(row, column, _blackShashks) || FindShashka(row, column, _whiteShashks)))
            {
                if (IsWhiteGo)
                {
                    if (TakeFook(row, column)) return;//если на нажатой кнопке стоит шашка из листа _fookShashks, её берут зафук
                    FindShashka(row, column, _whiteShashks, true); //проверяем наличие белой шашки на кнопке и выбираем её четвертым пареметром true
                    //выбранная шашка записывается в SelectedShashka
                    return;
                }
                else //тоже самое с черными
                {
                    if (TakeFook(row, column)) return;
                    FindShashka(row, column, _blackShashks, true);
                    return;
                }
            }
            if (SelectedShahka == null) return;
            if (SelectedShahka.IsSelected == true)
            {
                _fook = CheckFook(); //Проверяем возможность взять зафук, формируем _fookShashks на ход
                if (Step(row, column))//происходит ход
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

        private bool Step(int row, int column)
        {
            if (!SelectedShahka.IsDamka)
            {
                //если шашка не является дамкой...
                if (!EatEvent)//если в предыдущем ходу эта шашка не кушала
                {
                    #region Обычный ход
                    if (SelectedShahka.Color == Team.Black)
                    {
                        if (SelectedShahka.RowCoord - row == 1 && Math.Abs(SelectedShahka.ColumnCoord - column) == 1)
                        {
                            if (!FindShashka(row, column)) //если место куда должна походить шашка свободно
                            {
                                //новые координаты для шашки
                                SelectedShahka.RowCoord = row;
                                SelectedShahka.ColumnCoord = column;
                                EatEvent = false; //в этом ходу съедания не было
                                return true;
                            }
                        }
                    }
                    if (SelectedShahka.Color == Team.White)
                    {
                        if (SelectedShahka.RowCoord - row == -1 && Math.Abs(SelectedShahka.ColumnCoord - column) == 1)
                        {
                            if (!FindShashka(row, column)) //если место куда должна походить шашка свободно
                            {
                                //новые координаты для шашки
                                SelectedShahka.RowCoord = row;
                                SelectedShahka.ColumnCoord = column;
                                EatEvent = false; //в этом ходу съедания не было
                                return true;
                            }
                        }
                    }
                    #endregion
                }
                //если нажали кнопку, на которую встанет шашка если съест (грубо говоря через одну клетку по диагонали)
                if (Math.Abs(SelectedShahka.RowCoord - row) == 2 && Math.Abs(SelectedShahka.ColumnCoord - column) == 2)
                {
                    //Если местечко занято, возвращаем ложь
                    if (FindShashka(row, column, _blackShashks)) return false;
                    if (FindShashka(row, column, _whiteShashks)) return false;
                    //определяем лист врагов
                    List<Shashka> enemyList = new();
                    if (SelectedShahka.Color == Team.Black) enemyList = _whiteShashks;
                    else enemyList = _blackShashks;

                    #region Ход - скушать
                    if (SelectedShahka.RowCoord > row && SelectedShahka.ColumnCoord > column)
                    {
                        if (FindShashka(SelectedShahka.RowCoord - 1, SelectedShahka.ColumnCoord - 1, enemyList)) 
                        {
                            //если между выбранной шашкой и нажатой кнопкой есть вражеская шашка
                            RemoveShashka(SelectedShahka.RowCoord - 1, SelectedShahka.ColumnCoord - 1, enemyList);
                            SelectedShahka.RowCoord = row;
                            SelectedShahka.ColumnCoord = column;
                            EatEvent = true;//скушание произошло
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
                    #endregion
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
            //делаем дамками дошедших до другой стороны доски
            if (SelectedShahka.Color == Team.Black && SelectedShahka.RowCoord == 0)
            {
                SelectedShahka.IsDamka = true;
            }
            if (SelectedShahka.Color == Team.White && SelectedShahka.RowCoord == 7)
            {
                SelectedShahka.IsDamka = true;
            }
            //обновлняем поле
            Output();
            SelectedShahka.IsSelected = false;
            IsWhiteGo = !IsWhiteGo; //меняем ход
            if (!CheckMove()) //если ходящие не могут ходить - они проигрывают
            {
                Win();
                return;
            }
            //красим рамку в другой цвет
            stepBorder.IsEnabled = IsWhiteGo;
            whitePart.IsEnabled = IsWhiteGo;
            blackPart.IsEnabled = !IsWhiteGo;
            EatEvent = false;
        }
        private void Win()
        {
            //затемняем доску и выводим кто победил
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
                var removeItem = GetShashka(row, column, _fookShashks);//получаем шашку из фук-листа
                RemoveShashka(removeItem.RowCoord, removeItem.ColumnCoord, removeItem.Color); //и удаляем её
                //ставим полям значение по умолчанию
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
            //ТИТРЫЫ
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
