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
        private bool CheckMove()
        {

            if (IsWhiteGo)
            {
                foreach (var item in _whiteShashks)
                {
                    //ну просто проверяем все возможные варианты куда можна наступить
                    if (item.IsDamka)
                    {
                        if (!FindShashka(item.RowCoord - 1, item.ColumnCoord - 1, _whiteShashks)) return true;
                        if (!FindShashka(item.RowCoord - 1, item.ColumnCoord + 1, _whiteShashks)) return true;
                        if (FindShashka(item.RowCoord - 1, item.ColumnCoord - 1, _blackShashks))
                        {
                            if (!FindShashka(item.RowCoord - 2, item.ColumnCoord - 2)) return true;
                        }
                        if (FindShashka(item.RowCoord - 1, item.ColumnCoord + 1, _blackShashks))
                        {
                            if (!FindShashka(item.RowCoord - 2, item.ColumnCoord + 2)) return true;
                        }
                    }
                    if (!FindShashka(item.RowCoord + 1, item.ColumnCoord - 1)) return true;
                    if (!FindShashka(item.RowCoord + 1, item.ColumnCoord + 1)) return true;
                    if (FindShashka(item.RowCoord + 1, item.ColumnCoord - 1, _blackShashks))
                    {
                        if (!FindShashka(item.RowCoord + 2, item.ColumnCoord - 2)) return true;
                    }
                    if (FindShashka(item.RowCoord + 1, item.ColumnCoord + 1, _blackShashks))
                    {
                        if (!FindShashka(item.RowCoord + 2, item.ColumnCoord + 2)) return true;
                    }
                }
                return false;
            }
            else
            {
                foreach (var item in _blackShashks)
                {
                    //ну просто проверяем все возможные варианты куда можна наступить
                    if (item.IsDamka)
                    {
                        if (!FindShashka(item.RowCoord - 1, item.ColumnCoord - 1, _blackShashks)) return true;
                        if (!FindShashka(item.RowCoord - 1, item.ColumnCoord + 1, _blackShashks)) return true;
                        if (FindShashka(item.RowCoord - 1, item.ColumnCoord - 1, _whiteShashks))
                        {
                            if (!FindShashka(item.RowCoord - 2, item.ColumnCoord - 2)) return true;
                        }
                        if (FindShashka(item.RowCoord - 1, item.ColumnCoord + 1, _whiteShashks))
                        {
                            if (!FindShashka(item.RowCoord - 2, item.ColumnCoord + 2)) return true;
                        }
                    }
                    if (!FindShashka(item.RowCoord - 1, item.ColumnCoord - 1)) return true;
                    if (!FindShashka(item.RowCoord - 1, item.ColumnCoord + 1)) return true;
                    if (FindShashka(item.RowCoord - 1, item.ColumnCoord - 1, _whiteShashks))
                    {
                        if (!FindShashka(item.RowCoord + 2, item.ColumnCoord - 2, _whiteShashks) && FindShashka(item.RowCoord - 2, item.ColumnCoord - 2, _blackShashks)) return true;
                    }
                    if (FindShashka(item.RowCoord - 1, item.ColumnCoord + 1, _whiteShashks))
                    {
                        if (!FindShashka(item.RowCoord - 2, item.ColumnCoord + 2, _whiteShashks) && FindShashka(item.RowCoord - 2, item.ColumnCoord + 2, _blackShashks)) return true;
                    }
                }
                return false;
            }

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

        #region Check Damka
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
        private bool CheckDamkaCanEat(Shashka damka, int x, int y)
        {
            if (x < 0 && y < 0)
            {
                x++;
                y++;
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

                if (FindShashka(damka.RowCoord + 1, damka.ColumnCoord + 1, colorEnemy))
                {
                    {
                        _damkEatRow = damka.RowCoord + 1;
                        _damkEatColumn = damka.ColumnCoord + 1;
                        if (CheckDamkaCanGo(new Shashka(damka.RowCoord + 1, damka.ColumnCoord + 1, damka.Color), x, y))
                            return true;
                        else
                            return false;
                    }
                }
                return CheckDamkaCanEat(new Shashka(damka.RowCoord + 1, damka.ColumnCoord + 1, damka.Color), x, y);
            }
            if (x > 0 && y < 0)
            {
                x--;
                y++;
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

                if (FindShashka(damka.RowCoord - 1, damka.ColumnCoord + 1, colorEnemy))
                {
                    {
                        _damkEatRow = damka.RowCoord - 1;
                        _damkEatColumn = damka.ColumnCoord + 1;
                        if (CheckDamkaCanGo(new Shashka(damka.RowCoord - 1, damka.ColumnCoord + 1, damka.Color), x, y))
                            return true;
                        else
                            return false;
                    }
                }
                return CheckDamkaCanEat(new Shashka(damka.RowCoord - 1, damka.ColumnCoord + 1, damka.Color), x, y);
            }
            if (x < 0 && y > 0)
            {
                x++;
                y--;
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

                if (FindShashka(damka.RowCoord + 1, damka.ColumnCoord - 1, colorEnemy))
                {
                    {
                        _damkEatRow = damka.RowCoord + 1;
                        _damkEatColumn = damka.ColumnCoord - 1;
                        if (CheckDamkaCanGo(new Shashka(damka.RowCoord + 1, damka.ColumnCoord - 1, damka.Color), x, y))
                            return true;
                        else
                            return false;
                    }
                }
                return CheckDamkaCanEat(new Shashka(damka.RowCoord + 1, damka.ColumnCoord - 1, damka.Color), x, y);
            }
            if (x > 0 && y > 0)
            {
                x--;
                y--;
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

                if (FindShashka(damka.RowCoord - 1, damka.ColumnCoord - 1, colorEnemy))
                {
                    {
                        _damkEatRow = damka.RowCoord - 1;
                        _damkEatColumn = damka.ColumnCoord - 1;
                        if (CheckDamkaCanGo(new Shashka(damka.RowCoord - 1, damka.ColumnCoord - 1, damka.Color), x, y))
                            return true;
                        else
                            return false;
                    }
                }
                return CheckDamkaCanEat(new Shashka(damka.RowCoord - 1, damka.ColumnCoord - 1, damka.Color), x, y);
            }
            return false;
        }
        #endregion
    }
}
