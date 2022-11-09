using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFBase.SystemProgramming
{
    /// <summary>
    /// Логика взаимодействия для Sp1Window.xaml
    /// </summary>
    public partial class Sp1Window : Window
    {
        public Sp1Window()
        {
            InitializeComponent();
            Messages.Text = "";  
        }

        #region Basics
        private void StartThread_Click(object sender, RoutedEventArgs e)
        {
            new Thread(ThreadMethod).Start();
        }
        private void ShowMessage_Click(object sender, RoutedEventArgs e)
        {
            // MessageBox.Show("Сообщение");
            Messages.Text += "Сообщение\n";
        }
        private void ThreadMethod()
        {
            Dispatcher.Invoke(AddMessage, new object[] { "Start" });
            Thread.Sleep(2000);
            Dispatcher.Invoke(AddMessage, new object[] { "Stop" });
        }
        private void AddMessage(String message)
        {
            Messages.Text += message + "\n";
        }
        #endregion

        #region Inflation
        // Синхронно решение
        private void StartSync_Click(object sender, RoutedEventArgs e)
        {
            double sum = 100;
            Inflation.Text = "На начао года: " + sum;
            for (int i = 0; i < 12; i++)
            {
                sum *= 1.1;  // +10%
                Thread.Sleep(100 + rnd.Next(100));
                Inflation.Text += String.Format(
                    "\nМесяц {0}, Итог {1}",
                    i + 1, sum);
            }
        }

        // ------------- Многопоточное решение -------------

        Random rnd = new Random();

        double Sum;                        // "Общая" переменная. изменяемая разными потоками
        readonly object locker = new();    // объект для синхронизации операций с Sum

        int activeThreads;                 // счетчикактивных потоков
        readonly object locker2 = new();   // объект для синхронизации операций с activeThreads

        // Метод, который будет работать в потоке
        private void AddMonth(object? data)
        {
            if (data is ThreadData threadData) // конвертируем object в ThreadData
            {
                // имитируем затрату времени на запрос коэфициента инфляции                    
                double coef;                         // єта част не требует блокировки,
                Thread.Sleep(100 + rnd.Next(100));   // т.к. не использует общую переменную
                coef = 1 + rnd.NextDouble() / 5;     // 
                double sum;
                lock (locker) // синхро-блок, создающий транзакцию от чтения до записи
                {
                    // метод должен изменяфт общую сумму, значит переменная должна быть "общей"
                    sum = Sum;        // получаем данные о текущем состоянии                                      
                    sum *= coef;      // расчитываем результат
                    Sum = sum;        // сохраняем результат в "общем" хранилище                                        
                }

                // напрямую обратиться к єлементам окна нельзя, т.е. єто другой поток
                // Inflation.Text += String.Format("\n Итог {1}", sum);
                Dispatcher.Invoke( // альтернатива смотри строка 45-48
                    () => Inflation.Text += String.Format("\nМесяц {1} Итог {0}", sum, threadData.Month)
                );

                lock (locker2)
                {
                    activeThreads--;
                    // за это время другой поток может еще уменьшить activeThreads
                    if (activeThreads == 0)
                    {
                        Dispatcher.Invoke(InflationComputed);
                    }
                }                
            }            
        }

        private void InflationComputed() // завершение - расчет закончен
        {
            Inflation.Text += String.Format("\n--- Итог {0}", Sum);
            StartAsyncButton.IsEnabled = true;  // разблокируем кнопку - работа завершена
        }

        private void StartAsync_Click(object sender, RoutedEventArgs e)
        {            
            Sum = 100;  // начальная сумма
            Inflation.Text = "На начао года: " + Sum;
            int monthes = 12;
            activeThreads = monthes;
            for (int i = 0; i < monthes; i++)
            {
                // activeThreads++; - если потоки быстро отрабатывают, до повтора цикла
                //  возможно уменьшение activeThreads из потока
                // new Thread(AddMonth).Start(); -- без параметров
                new Thread(AddMonth).Start(      // параметры для метода передаются в .Start()
                    new ThreadData { Month = i + 1 }
                );
            }
            StartAsyncButton.IsEnabled = false; // блокируем кнопку до завершения всех потоков
        }

        #endregion
    }

    class ThreadData // для передачи данных в потоковый метод
    {
        public int Month { get; set; }
    }
}

