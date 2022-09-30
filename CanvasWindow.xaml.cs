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
    /// Логика взаимодействия для CanvasWindow.xaml
    /// </summary>
    public partial class CanvasWindow : Window
    {
        // Таймер - инструмент создания хроно-событий или
        // периолического запуска функии/метода
        private System.Windows.Threading.DispatcherTimer Timer;
        private bool LeftKeyHold; // признак удержания кнопки "влево"
        private bool RightKeyHold; // признак удержания кнопки "вправо"
        private double ShipVelocity; // скорость кораблика)

        private List<Rectangle> Bricks; // коллекция блоков
        private List<Rectangle> Bonuses; // бонусы

        private int CountBonuses; // количество пойманых бонусов
        
        public CanvasWindow()
        {
            InitializeComponent();
            Timer = new() { Interval = new TimeSpan(0, 0, 0, 0, 20) };
            Timer.Tick += this.TimerTick; 
            Bricks = new();
            Bonuses = new();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // собитие "готовности" окна. Работу с элементами UI (User Interfacce
            // желательно реализовывать в этом событии
            Timer.Start();
            // создаем объект с данными (BallData), ссылку на него помещаем
            // в поле Tag объекта-шарик Ball ( <Ellipse x:Name ="Ball" )
            // Tag - специальное "резервное№ поле для добавления своих данных
            Ball.Tag = new BallData { Vx = -2, Vy = -2 };
            ShipVelocity = 5;
            CountBonuses = 0;          
                
            foreach (var child in Field.Children) // обходим элементы Canvas
            {
                if (child is Rectangle rect) // отбираем прямоугольники
                {
                    if ( rect != Ship) // Ship - прямоугольник, не включаем в блоки
                    {
                        Bricks.Add(rect); // добавляем блок в коллекцию
                    }
                }
            }
        }

        // метод. который периодически запускаетсф таймером
        private void TimerTick(object? sender, EventArgs e)
        {
            #region Движение шарика
            if (Ball.Tag is BallData ballData)
            {
                double ballX = Canvas.GetLeft(Ball); // Определяем координаты 
                double ballY = Canvas.GetTop(Ball);  // шарика на холсту (Canvas)

                ballX += ballData.Vx; // Движение - изменение координат
                ballY += ballData.Vy; //
                // ограничения движения
                if (ballX <= 0) //выход за левую грань
                {
                    ballData.Vx = -ballData.Vx; // меняем скорость по Х
                    ballX = 0; // "подравниваем" если вышел за холст
                }
                if (ballY <= 0)
                {
                    ballData.Vy = -ballData.Vy;
                    ballY = 0;
                }
                if (ballX >= Field.ActualWidth - Ball.ActualWidth)
                {
                    ballData.Vx = -ballData.Vx;
                    ballX = Field.ActualWidth - Ball.ActualWidth;
                }
                // Окончательное падение
                if (ballY >= Field.ActualHeight - Ball.ActualHeight / 2)
                {
                    MessageBox.Show("Game Over");
                    this.Close();
                }
                // "Зона" ракетки, откуда возможно отбитие
                if (ballY >= Canvas.GetTop(Ship) - Ball.ActualHeight)
                {
                    double shipX = Canvas.GetLeft(Ship);
                    // находиться ли шарик над ракеткой
                    if (ballX + Ball.ActualWidth / 2 >= shipX
                        && ballX + Ball.ActualWidth / 2 <= shipX + Ship.ActualWidth)
                    {
                        ballData.Vy = -ballData.Vy;
                        ballY = Canvas.GetTop(Ship) - Ball.ActualHeight;
                    }
                }


                // проблема удаления - см. комментаии ниже
                Rectangle? removed = null; // ссылка на удаляемый эл-т
                foreach (var brick in Bricks)
                {
                    #region Шарик - Brick1 соударени
                    // Шарик - Brick1 соударени              
                    double brickX = Canvas.GetLeft(brick);
                    double brickY = Canvas.GetTop(brick);
                    if (ballX + Ball.ActualWidth / 2 >= brickX &&
                        ballX + Ball.ActualWidth / 2 <= brickX + brick.ActualWidth)
                    {
                        if (ballY + Ball.ActualHeight >= brickY &&
                            ballY + Ball.ActualHeight <= brickY + 2 * Math.Abs(ballData.Vy))
                        {
                            // сверху
                            ballData.Vy = -ballData.Vy;
                            removed = brick;
                        }
                        if (ballY <= brickY + brick.ActualHeight &&
                            ballY >= brickY + brick.ActualHeight - 2 * Math.Abs(ballData.Vy))
                        {
                            // снизу
                            ballData.Vy = -ballData.Vy;
                            removed = brick;
                        }
                    }
                    else if (ballY + Ball.ActualHeight / 2 >= brickY &&
                             ballY + Ball.ActualHeight / 2 <= brickY + brick.ActualHeight)
                    {
                        if (ballX + Ball.ActualWidth >= brickX &&
                            ballX + Ball.ActualWidth <= brickX + 2 * Math.Abs(ballData.Vx))
                        {
                            // слева
                            ballData.Vx = -ballData.Vx;
                            removed = brick;
                        }
                        if (ballX <= brickX + brick.ActualWidth &&
                            ballX >= brickX + brick.ActualWidth - 2 * Math.Abs(ballData.Vx))
                        {
                            // справа
                            ballData.Vx = -ballData.Vx;
                            removed = brick;
                        }
                    }
                    #endregion
                }
                if (removed != null)
                {
                    // блок сбит - создаем "бонус"
                    var bonus = new Rectangle
                    {
                        Fill = removed.Fill,
                        Width = removed.Width,
                        Height = removed.Height
                    };                    
                    Bonuses.Add(bonus);
                    Field.Children.Add(bonus);
                    
                    Canvas.SetLeft(bonus, Canvas.GetLeft(removed));
                    Canvas.SetTop(bonus, Canvas.GetTop(removed));

                    // удаляем блок из коллекции и с поля
                    Bricks.Remove(removed);
                    Field.Children.Remove(removed);
                }                

                Canvas.SetLeft(Ball, ballX); // применение новых
                Canvas.SetTop(Ball, ballY);  // координат

                #region Движение бонусов
                if (Bonuses.Count > 0)
                {
                    foreach (var bonus in Bonuses)
                    {
                        double by = Canvas.GetTop(bonus);
                        by += 3;
                        if (by > Field.ActualHeight - bonus.ActualHeight / 2)  
                        {
                            removed = bonus;
                        }
                        // находиться ли бонус над ракеткой
                        if (by >=  Canvas.GetTop(Ship))                            
                        {
                            if (Canvas.GetLeft(bonus) + bonus.ActualWidth > Canvas.GetLeft(Ship)
                             && Canvas.GetLeft(bonus) < Canvas.GetLeft(Ship) + Ship.ActualWidth)                            
                            {
                                CountBonuses++;
                                Text.Text = "Количество пойманых блоков: " + CountBonuses.ToString();
                                removed = bonus;
                            }
                        }
                        Canvas.SetTop(bonus, by);
                    }
                    if (removed != null)
                    {
                        Bonuses.Remove(removed);
                        Field.Children.Remove(removed);
                    }
                    /* Д.З. АрканоидЖ движение бонусов
                     *  Обеспечить изчезновение бонусов
                     *  а) при пересечении с ракеткой
                     *  б) при выходе за пределы поля
                     *  ** Отобразить кол-во пойманых бонусов и время игры
                     *  CountBonuses++;
                     *  Text.Text = "Количество пойманых блоков: " + CountBonuses.ToString();
                     */

                }
                #endregion

                // Если все блоки сбиты и все бонусы пойманы/удалены - игра выиграна
                if (Bonuses.Count == 0 && Bricks.Count == 0)
                {
                    MessageBox.Show("Congratulations! You Won!");
                    this.Close();
                }
            }
            #endregion


            #region Движение каретки - обработка клавиатуры
            if (LeftKeyHold) // Если удерживается стрелка "влево"
            {
                double x = Canvas.GetLeft(Ship);
                if (x > ShipVelocity) x -= ShipVelocity;
                else x = 0;
                Canvas.SetLeft(Ship, x);         
            }
            if (RightKeyHold) // Если удерживается стрелка "влево"
            {
                double x = Canvas.GetLeft(Ship);
                if (x < Field.ActualWidth - Ship.ActualWidth - ShipVelocity) x += ShipVelocity;
                else x = Field.ActualWidth - Ship.ActualWidth;
                Canvas.SetLeft(Ship, x);
            }
            #endregion
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // собитие начала закрытия окна - здесь останавливаем таймер
            Timer.Stop(); // таймер, как системный ресурс, желательно останавливать            
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) this.Close();
            else if (e.Key == Key.Left)
            {
                // double x = Canvas.GetLeft(Ship); // обработка движения
                // x -= 3;                          // в событиях клавиатуры
                // Canvas.SetLeft(Ship, x);         // не рекмендуется
                LeftKeyHold = true; // обработка - в таймере
            }
            else if (e.Key == Key.Right)
            {
                
                RightKeyHold = true; // обработка - в таймере
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left) LeftKeyHold = false;
            if (e.Key == Key.Right) RightKeyHold = false;
        }
    }

    // Слабее, чем наследование, связь агрегация: один объект ссылается на
    // другой объект.
    class BallData // данные для шарика
    {
        public double Vx { get; set; } // скорость по горизонтали
        public double Vy { get; set; } // скорость по вертикали
    }
}

/* Задача: сбивать блок при попадании шарика
 * Проблема: информация о стлкновении получается в цикле по коллекции
 *  (Bricks), а "сбивать" блок - значит удалять его из коллекции.
 *  !! Менять коллекцию в цикле по коллекции - запрещено
 *  
 */
