using System;
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
using System.Windows.Shapes;

namespace WPFBase
{
    /// <summary>
    /// Логика взаимодействия для ContainersWindow.xaml
    /// </summary>
    public partial class ContainersWindow : Window
    {
        public ContainersWindow()
        {
            InitializeComponent();
        }
        
    }
}

/*
 * Одно из главных отличий WPF - в позиционировании элементов на окне
 * Применяется идея контейнеров - элементов, которые по-разному 
 * организовывают свои внутрение (дочерние) элементы
 * В окне может быть только один элемент (обычно контейнер), а он уже
 * может содержать люое количество элементов, в т.ч. других контейнеров
 * 
 * Основные виды контейнеров
 * StackPanel - "одномерная" групировка, "стопка" по умолчанию ориентация вертикальная
 * Orientation="Horizontal" - |||
 * Orientation="Vertical" - =
 * "+" самы простой контейнер ( в т.ч. по сресурсам)
 * "-" обрезется если не влазит в родительський контейнер (в окно)
 * 
 * WrapPanel - тоже самое что и StackPanel, но при переполнении
 * происходит перенос элементов
 * 
 * DockPanel - контейнер с "притяжением" - элементы притягиваются
 * к одной из четырех сторон (верх-низ-лево-право), при изменении
 * размеров притяжение движет элементы вместе с гранями. Применяется 
 * для сайто-подобных окон с выделенным "футором" - нижней частью,
 * и правым меню
 * 
 * Задание: используя док-панель сделать образ страницы сайта
 * Header
 * Left Content Right
 * Footer
 * 
 * Тфбличный (сеточные) контейнеры
 * UniformGrid Rows="3" Columns="3" - простой контейнер с ячейками
 * одинакового размера. Каждый дочерный элемент автоматически
 * помещается в новую ячейку.
 * 
 * Grid - наиболее универсальный контейнер, в нем каждая ячейка собирается отдельно,
 * дочерние элементы должны быть явно указаны к какой ячейке они относятся.
 * Иначе все они попадают в одну ячейку и перекрывают друг друга.
 * 
 * Задание: повторить образ сайта (как дорк-панель) средставми Grid
 * 
 * */
