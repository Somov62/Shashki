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
        private bool partStep;
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
            if (!partStep)
            {
                (int row, int column) = GetCoord(sender);
                var shashk = _whiteShashks.Where(shahck => shahck.RowCoord == row)
                                          .Where(shahck => shahck.ColumnCoord == column);
                List<Shashka> list = shashk.ToList();
                if (list.Count != 0)
                {
                    SelectedShahka = list[0];
                    SelectedShahka.IsSelected = true;
                }
                else
                {
                    shashk = _blackShashks.Where(shahck => shahck.RowCoord == row)
                                         .Where(shahck => shahck.ColumnCoord == column);
                    list = shashk.ToList();
                    if (list.Count != 0)
                    {
                        SelectedShahka = list[0];
                        SelectedShahka.IsSelected = true;
                    }
                }
                partStep = true;
                return;
            }
            Step(sender);
            partStep = false;
        }
        private void Step(object sender)
        {
            (int row, int column) = GetCoord(sender);
            if (SelectedShahka.Color == Team.Black)
            {
                if (SelectedShahka.RowCoord - row == 1 && Math.Abs(SelectedShahka.ColumnCoord - column) == 1)
                {
                    int count = _blackShashks.Where(shahck => shahck.RowCoord == row)
                                         .Where(shahck => shahck.ColumnCoord == column).Count();
                    if (count == 0)
                    {
                        SelectedShahka.RowCoord = row;
                        SelectedShahka.ColumnCoord = column;
                        Output();
                    }
                    
                }

            }
        }
        private (int row, int column) GetCoord(object sender)
        {
            Button btn = (Button)sender;
            Border br = (Border)btn.DataContext;
            int row = Grid.GetRow(br);
            int column = Grid.GetColumn(br);
            return (row, column);
        }
    }
}