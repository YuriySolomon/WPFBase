using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
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
using System.Text.Json;

namespace WPFBase
{
    /// <summary>
    /// Логика взаимодействия для DndWindow.xaml
    /// </summary>
    public partial class DndWindow : Window
    {
        private bool LeftHold;      // признак удержания левой кнопки мыши (ЛКМ)
        private Rectangle Phantom;  // временная копия объекта для перетаскивания
        private Rectangle Sourse;   // исходеый элемент, который "копируется"
        private Point touch;        // точка курсора мыши в момент захвата объекта

        public DndWindow()
        {
            InitializeComponent();
            Phantom = null!;        // null-forgiving (!) - мы уверены. что допускае null
            Sourse = null!;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LeftHold = false;
            Phantom = null!;
            Sourse = null!;
        }

        private void Brick_MouseDown(object sender, MouseButtonEventArgs e)
        {
            /* Суть перетаскивания - в создании копии исходного элемента (фантома)
             * и перемещение его до момента отпускания. Однако. фантом обычно создается 
             * не в момент нажатия. а в момент перевого движения с нажатой кнопкой.
             */
            Sourse = (sender as Rectangle)!;
            if (e.ChangedButton == MouseButton.Left)    // событие MouseDown - для всех кнопок
            {                
                LeftHold = Sourse != null;
                touch = e.GetPosition(Sourse);
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                // на ПЛЬ поставим удаление блока (кроме исходных)
                if (Sourse != Brick1 && Sourse != Brick2)
                {
                    Field.Children.Remove(Sourse);
                }
            }
            // Задание: исходные блоки копируются. а новые блоки перемещаются сами
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (LeftHold)
            {
                Point point = e.GetPosition(Field);
                Title = point.X + " " + point.Y;
                if (Phantom == null) // движение есть. а фантома нет - это первое движение
                {
                    if (Sourse == Brick1 || Sourse == Brick2)
                    {
                        Phantom = new Rectangle
                        {
                            Width = Sourse.Width,
                            Height = Sourse.Height,
                            Fill = Sourse.Fill,
                            Stroke = Sourse.Stroke,
                            StrokeThickness = Sourse.StrokeThickness,
                            Opacity = .5
                        };
                        Field.Children.Add(Phantom);
                    }
                    else
                    {
                        Phantom = Sourse;
                        Phantom.Opacity = .5;
                    }
                }
                Canvas.SetLeft(Phantom, point.X - touch.X);
                Canvas.SetTop(Phantom, point.Y - touch.Y);
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            
            if (Phantom != null)
            {
                Phantom.Opacity = 1;
                Phantom.MouseDown += Brick_MouseDown;

                LeftHold = false;
                Phantom = null!;
            }
            
        }
        #region Menu
        private void MenuSave_Click(object sender, RoutedEventArgs e)
        {
            /* Сериализация (от англ. serial - последовательный) - представление объекта
             * в виде последовательности данных (бинарных или текстовых) обычно для
             * сохранения или передачи по обменному каналу
             * В C# популярны три сериализатора: Binary(старый. не рекомендуется)
             * XML (встроенный), JSON (System.Text, Newtonsoft)
             */
            List<BrickData> bricks = new();
            foreach (var child in Field.Children)
            {
                if (child is Rectangle brick)
                {
                    if (brick != Brick1 && brick != Brick2)
                    {
                        bricks.Add(new()
                        {
                            Height = brick.Height,
                            Width = brick.Width,
                            Left = Canvas.GetLeft(brick),
                            Top = Canvas.GetTop(brick),
                            Type = (brick.Fill.Equals(Brushes.Salmon)) ? 1 : 2
                        });
                    }
                }
            }
            String json = JsonSerializer.Serialize(bricks);
            MessageBox.Show(json);
            // "[{\"Width\":70,\"Height\":20,\"Left\":337,\"Top\":101.99999999999997,\"Type\":2},{\"Width\":70,\"Height\":20,\"Left\":337,\"Top\":136.99999999999997,\"Type\":1},{\"Width\":70,\"Height\":20,\"Left\":336,\"Top\":175.99999999999997,\"Type\":2}]"
        }
         private void MenuLoad_Click(object sender, RoutedEventArgs e)
        {
            String json = "[{\"Width\":70,\"Height\":20,\"Left\":337,\"Top\":101.99999999999997,\"Type\":2},{\"Width\":70,\"Height\":20,\"Left\":337,\"Top\":136.99999999999997,\"Type\":1},{\"Width\":70,\"Height\":20,\"Left\":336,\"Top\":175.99999999999997,\"Type\":2}]";
            BrickData[] bricks = JsonSerializer.Deserialize<BrickData[]>(json)!;
            if (bricks == null)
            {
                MessageBox.Show("Load error");
            }
            else
            {
                // очистить поле (кроме исходных)
                List<UIElement> toRemove = new();
                foreach (var child in Field.Children)
                {
                    if (child is Rectangle brick)
                    {
                        if (brick != Brick1 && brick != Brick2)
                        {
                            toRemove.Add(brick);
                        }
                    }
                }
                foreach (var child in toRemove)
                {
                    Field.Children.Remove(child);
                }
                // вставить новые элементы
                foreach (var brick in bricks)
                {
                    Rectangle r = new()
                    {
                        Width = brick.Width,
                        Height = brick.Height,
                        Fill = brick.Type == 1 ? Brushes.Salmon : Brushes.Lime,
                        Stroke = brick.Type == 1 ? Brushes.Maroon : Brushes.Orange,
                        StrokeThickness = 2
                    };
                    Canvas.SetLeft(r, brick.Left);
                    Canvas.SetTop(r, brick.Top);
                    r.MouseDown += Brick_MouseDown;
                    Field.Children.Add(r);
                }
            }
        }
        #endregion
        /* Д.З. Реализовать сохранение данніх в файл (сериализация) и вігрузку из файлв
         * (десериализация). * Организовать выбор имени файла при помощи диалога
         * Экзамен: завершить все задания. приложит архив проекта 9ссулку на репозиторий)         
         */       
    }
    class BrickData
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }
        public int Type { get; set; }
    }
}

/* Drag n'Drop (DnD)
 * Визуальный прием перетаскивания элементов мышью
 * Реализация приема состоит в следующем:
 * - элемент. который поддерживает DnD, обрабатывает нажатие мыши
 * - пространство в котором возможно перетаскивание, поддерживает 
 *   = перемещение мыши
 *   = отпускание мыши
 * 
 */
