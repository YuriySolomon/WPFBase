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
    /// Логика взаимодействия для MinesWindow.xaml
    /// </summary>
    public partial class MinesWindow : Window
    {
        private System.Windows.Threading.DispatcherTimer Clock;
        private int TimeGames; //сколько секкунд прошло с момента начали новой игры

        private const String MINE_SUMBOL = "\x2622";
        private const String FREE_SUMBOL = "\x0DF4";
        private const String FLAG_SUMBOL = "\x2691";
        private int allMines;   // Количество мин на карте
        private int openCell;   // Количество открытых ячеек без мин
        private bool startTime; // Для фиксации времени при старте/выиграше/проиграше

        private Random random = new();
        public MinesWindow()
        {
            InitializeComponent();
            allMines = 0;
            openCell = 0;
            Clock = new() { Interval = new TimeSpan(0, 0, 0, 1) };
            Clock.Tick += this.ClockTick!;

            for (int y = 0; y < App.FIELD_SIZE_Y; y++)
            {
                for (int x = 0; x < App.FIELD_SIZE_X; x++)
                {
                    FieldLabel label = new()
                    {
                        X = x,
                        Y = y,
                        IsMine = random.Next(5) == 0,    // random.Next(10).ToString();
                        Open = false
                    };
                    if (label.IsMine)
                    {
                        allMines++;
                    }
                    label.Content = FREE_SUMBOL; // label.IsMine ? "\x2622" : "\x0DF4";
                    label.FontSize = 20;

                    label.HorizontalContentAlignment = HorizontalAlignment.Center;
                    label.VerticalContentAlignment = VerticalAlignment.Center;

                    label.Background = Brushes.Beige;
                    label.Margin = new Thickness(1);

                    // Подключаем обработчик события
                    label.MouseLeftButtonUp += LabelClick;
                    label.MouseRightButtonDown += LabelRightClick;

                    // Регистрируем иия для элемента, по этому имени его 
                    // можно будет найти (в другом коде)
                    this.RegisterName($"label_{x}_{y}", label);

                    Field.Children.Add(label);
                }
            }
            openCell = App.FIELD_SIZE_X * App.FIELD_SIZE_Y - allMines;            
            Mines.Content = "Количество мин: " + allMines;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TimeGames = 0;
            startTime = false;            
        }

        private void ClockTick(object sender, EventArgs e)
        {
            if (! startTime)
            {
                Clock.Stop();
            }
            TimeGames++;
            int h = TimeGames / 3600;        // часы
            int m = (TimeGames % 3600) / 60; // минуты
            int s = TimeGames % 60;          // секунды
            String t = h.ToString("00") + ":" + m.ToString("00") + ":" + s.ToString("00");
            ClockGames.Content = t;
        }
        // Задание: по нажатию правой кнопки мыши устанавливается "флажок"
        // при этом ячейка не нажимается левой. Повторное нажатие снимает
        // "флажок"
        // Задание: выигрыш - проверять условия. что все свободные ячейки
        // открыты.

        private bool IsWin()
        {
            int countCell = 0;
            foreach (var child in Field.Children)
            {
                if (child is FieldLabel label)
                {                    
                    if (! label.IsMine && // если не мина и свободный или флажок, то не выиграл
                          label.Open)
                    {
                        countCell++;
                    }
                }
            }
            Mines.Content = "Количество мин: " + allMines;
            return ((openCell - countCell) == 0 ? true : false);
        }

        // обработчик события нажатия ПКМ
        private void LabelRightClick(object sender, RoutedEventArgs e)
        {
            if (! startTime)
            {
                Clock.Start();
                startTime = true;
            }
            if (sender is FieldLabel label)
            {
                // Если контент - не закрытая ячейка. то не обрабатываем нажатие
                if (!label.Content.Equals(FREE_SUMBOL) && !label.Content.Equals(FLAG_SUMBOL)) return;
                //label.Content =
                //    label.Content.Equals(FLAG_SUMBOL)
                //    ? FREE_SUMBOL
                //    : FLAG_SUMBOL;
                if (label.Content.Equals(FLAG_SUMBOL))
                {
                    label.Content = FREE_SUMBOL;
                    allMines++;
                }
                else
                {
                    label.Content = FLAG_SUMBOL;
                    allMines--;
                }                
                Mines.Content = "Количество мин: " + allMines;
            }
        }

        // обработчик события нажатия ЛКМ
        private void LabelClick(object sender, RoutedEventArgs e)
        {
            if (! startTime)
            {
                Clock.Start();
                startTime = true;
            }
            if (sender is FieldLabel label)
            {
                // Если контент - флажок. то не обрабатываем нажатие
                if (label.Content.Equals(FLAG_SUMBOL)) return;

                // Если мина - сообщение Game Over, иначе количество мин 
                // отображаем на самой яческе
                if (label.IsMine)
                {
                    startTime = false;
                    // закрашиваем мину на которой подорвались и показываем ее
                    label.Content = MINE_SUMBOL;
                    label.Background = Brushes.Red;

                    // открываем все мины на поле
                    foreach (var child in Field.Children)
                    {
                        if (child is FieldLabel cell)
                        {
                            if (cell.IsMine)
                            {
                                cell.Content = MINE_SUMBOL;
                            }
                        }
                    }

                    //MessageBox.Show("Game Over"); //переделат на "еще раз? (да/нет)"
                    if (MessageBoxResult.No == MessageBox.Show("Play again?", "Game Over", MessageBoxButton.YesNo))
                    {
                        this.Close();
                    }
                    else
                    {
                        allMines = 0;
                        foreach (var child in Field.Children)
                        {
                            if (child is FieldLabel cell)
                            {
                                cell.Content = FREE_SUMBOL;
                                cell.IsMine = random.Next(5) == 0;
                                cell.Open = false;
                                cell.Background = Brushes.Beige;
                                if (cell.IsMine)
                                {
                                    allMines++;
                                }
                            }
                        }
                        openCell = App.FIELD_SIZE_X * App.FIELD_SIZE_Y - allMines;                        
                        Mines.Content = "Количество мин: " + allMines;                        
                        TimeGames = 0;
                        ClockGames.Content = "00:00:00";                         
                    }
                    return;
                }
                label.Open = true;

                //// Задание: на момент старта у всех ячеек контент одинаковый,
                //// а в сообщение добавить информацию о мине
                //if (label.IsMine)
                //{
                //    MessageBox.Show($"X:{label.X}, Y:{label.Y}, Mine");
                //}

                // Задание: определить правого соседа, вывести данные о нем
                //String name = $"label_{label.X + 1}_{label.Y}"; // имя соседа (х + 1)
                // Задание: определить имена всех возможных соседей
                
                String[] names = //массив имен соседей
                {
                    $"label_{label.X - 1}_{label.Y - 1}",
                    $"label_{label.X    }_{label.Y - 1}",
                    $"label_{label.X + 1}_{label.Y - 1}",
                    $"label_{label.X - 1}_{label.Y    }",
                    $"label_{label.X + 1}_{label.Y    }",
                    $"label_{label.X - 1}_{label.Y + 1}",
                    $"label_{label.X    }_{label.Y + 1}",
                    $"label_{label.X + 1}_{label.Y + 1}",
                };
                int mines = 0;
                foreach (String name in names)
                {
                    // Поиск элемента по имени, преобразование типа
                    // var neighbour = this.FindName(name) as FieldLabel;
                    // if (neighbour != null)
                    if (this.FindName(name) is FieldLabel neighbour)
                    {
                        if (neighbour.IsMine) mines += 1;
                    }
                }

                switch (mines)
                {
                    case 1: label.Background = Brushes.LightBlue; break;
                    case 2: label.Background = Brushes.LightGreen; break;
                    case 3: label.Background = Brushes.LightGray; break;
                    case 4: label.Background = Brushes.LightPink; break;
                    case 5: label.Background = Brushes.LightCoral; break;
                    case 6: label.Background = Brushes.LightSalmon; break;
                    case 7: label.Background = Brushes.Green; break;
                    default: break;
                }
                label.Content = mines != 0 ? mines.ToString() : "";
                
                #region Win
                // Состояние поля изменилось - проверяем условие победы
                if (IsWin())
                {
                    startTime = false;

                    // помечаем все мины флажками
                    foreach (var child in Field.Children)
                    {
                        if (child is FieldLabel cell)
                        {
                            if (cell.IsMine)
                            {
                                cell.Content = FLAG_SUMBOL;
                            }
                        }
                    }
                    allMines = 0;
                    Mines.Content = "Количество мин: " + allMines;
                    // вывести сообщение и предложить повторную игру
                    if (MessageBoxResult.No == MessageBox.Show("Play again?", "Game Won", MessageBoxButton.YesNo))
                    {
                        this.Close();
                    }
                    else
                    {   
                        foreach (var child in Field.Children)
                        {
                            if (child is FieldLabel cell)
                            {
                                cell.Content = FREE_SUMBOL;
                                cell.IsMine = random.Next(5) == 0;
                                cell.Open = false;
                                cell.Background = Brushes.Beige;
                                if (cell.IsMine)
                                {
                                    allMines++;
                                }
                            }
                        }
                        openCell = App.FIELD_SIZE_X * App.FIELD_SIZE_Y - allMines;
                        Mines.Content = "Количество мин: " + allMines;
                        TimeGames = 0;
                        ClockGames.Content = "00:00:00";
                    }
                    return;
                }
                #endregion
            }
        }
       
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Clock.Stop();
        }
    }
    /* Д.З. Обеспечить отображение ячеек с разным количеством мин
     *  ** если в ячейке получается ноль, то открывать все соседние ячейки
     *  Добавить элемент, отображающий количество мин на поле, уменьшать его 
     *  после установеи "флажков"
     *   * Добавить таймер игры (время от начала игры)
     *   Не забыть - новая игра отменяет цвета и сбрасывает время.
     */

    // Класс, расширающий Label дополнительными полями
    class FieldLabel : Label
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsMine { get; set; }
        public bool Open { get; set; }        
    }
}

/*Взаимодействие элементов.
 * На примере тгры "Сапер"
 * Главная идея - по щелчку на ячейке поля отображается количество
 * "мин". находящихся в соседних ячейках
 * 
 * 1. объявляем константы, ктоторые будут жоступны ао всем приложении
 *  (и в коде. и в разметке): в файле приложения App.xaml.cs
 *   public const int FIELD_SIZE_X = 8
 *   
 * 2. Разметка. 
 *      <UniformGrid
        x:Name="Field" - имя, по которому UniformGrid будет доступен в коде
 *      Columns="{x:Static local:App.FIELD_SIZE_X}" - "обратная" связь - 
 *                                                  используем данные из кода
 *
 * 3. Код.
 *      - организовываем циклы по введенным в App константам
 *      - создаевм элементы Label и добавляем их в коллекцию
 *          Field.Children - дочерние элементы контейнера Field (смотри UniformGrid)
 * ---------------
 * Задание 1: сохранить в Label дополнительную информацию (признак мины,
 * координаты на поле)
 * Задание 2: иметь возможность определить соседей - ячейки с заданными 
 * координатами
 * 
 * Решение 1: а) у каждого элемента есть "свободное" поле Tag, в котором можно
 *              сохранять произвольные данные;
 *            б) есть возможность создавать наследников
 *              (от label) и добавлять данные
 * Решение 2: а) перебирать коллекцию     Field.Children и искать элементы с 
 *              заданными координатами;
 *            б) зарегистрировать имена для ячеек и искать их по известным именам.
 * */
