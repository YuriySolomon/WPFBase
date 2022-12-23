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
            // MessageBox.Show("Повідомлення");
            Messages.Text += "Повідомлення\n";
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
            Inflation.Text = "На початок року: " + sum;
            for (int i = 0; i < 12; i++)
            {
                sum *= 1.1;  // +10%
                Thread.Sleep(100 + rnd.Next(100));
                Inflation.Text += String.Format(
                    "\nМісяць {0}, Разом {1}",
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
                    () => Inflation.Text += String.Format("\nМісяць {1} Разом {0}", sum, threadData.Month)
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
            Inflation.Text += String.Format("\n--- Разом {0}", Sum);
            StartAsyncButton.IsEnabled = true;  // разблокируем кнопку - работа завершена
        }

        private void StartAsync_Click(object sender, RoutedEventArgs e)
        {            
            Sum = 100;  // начальная сумма
            Inflation.Text = "На початок року: " + Sum;
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

        #region DZ 1
        
        private int CheckEnterNumber()
        {
            int num;     // введенное число пользователем            
            Summa.Text = String.Empty;            
            try
            {
                num = Convert.ToInt32(TextBoxNum.Text);
            }
            catch
            {
                MessageBox.Show("Введені дані не э числом.", "Помилка!",
                MessageBoxButton.OK, MessageBoxImage.Error);
                TextBoxNum.Text = "";
                return 0;
            }
            if (num <= 0 )
            {
                MessageBox.Show("Ви ввели невірне число.", "Помилка!",
                MessageBoxButton.OK, MessageBoxImage.Error);
                TextBoxNum.Text = "";
                return 0;
            }            
            return num;
        }
        private void StartSum_Click(object sender, RoutedEventArgs e)
        {
            int num;     // введенное число пользователем
            int sum = 0; // сумма чисел от 1 до num            
            num = CheckEnterNumber();            
            for (int i = 1; i <= num; i++)
            {
                sum += i;
                Thread.Sleep(30 + rnd.Next(30));
                Summa.Text += String.Format(
                    "\n Step {0}  add {1}   total {2} ",
                    i, i, sum);
            }
        }
        
        int SumNum;
        readonly object lockerSumNum = new();        // объект для синхронизации операций с SumNum

        int activeThreadsSum;                        // счетчикактивных потоков с SumNum
        readonly object lockerThreadsSum = new();    // объект для синхронизации операций с activeThreadsSum
        
        int threadsNum;

        private void AddNumber(object? data) // Метод, который будет работать в потоке
        {
            if (data is ThreadData threadData) 
            {   
                Thread.Sleep(30 + rnd.Next(30));
                int sum;
                lock (lockerSumNum) // синхро-блок, создающий транзакцию от чтения до записи
                {
                    sum = SumNum;                                       
                    sum += threadData.Month;      
                    SumNum = sum;                   
                }
               
                Dispatcher.Invoke(
                    () => Summa.Text += String.Format("\n Step {0}  add {1}   total {2} ", threadsNum++, threadData.Month, sum)
                );

                lock (lockerThreadsSum)
                {
                    activeThreadsSum--;                   
                    if (activeThreadsSum == 0)
                    {
                        Dispatcher.Invoke(SumNumComputed);
                    }
                }
            }
        }
        private void SumNumComputed() // завершение - расчет закончен
        {
            Summa.Text += String.Format("\n--- Разом {0}", SumNum);
            StartAsyncSum.IsEnabled = true; 
        }
        private void StartAsyncSum_Click(object sender, RoutedEventArgs e)
        {
            int num;     // введенное число пользователем
            SumNum = 0;  // начальная сумма                        
            threadsNum = 1;
            num = CheckEnterNumber();
            if (num == 0) return;
            activeThreadsSum = num;
            for (int i = 1; i <= num; i++)
            {
                new Thread(AddNumber).Start(new ThreadData { Month = i });               
            }
            StartAsyncSum.IsEnabled = false;
        }
        #endregion

        #region Start/Stop potok
        Thread worker;
        CancellationTokenSource tokenSource;

        private void Start1_Click(object sender, RoutedEventArgs e)
        {
            Progress1.Value = 0;
            worker = new Thread(Worker);
            tokenSource = new();
            CancellationToken token = tokenSource.Token;
            worker.Start(token);
        }

        private void Stop1_Click(object sender, RoutedEventArgs e)
        {
            // worker.Abort(); - deprecated, не работает
            tokenSource.Cancel();
        }

        private void Worker(object? pars)
        {
            if (pars is CancellationToken token)
            {
                int i = 0;
                try
                {
                    for (i = 0; i < 100; i++)
                    {
                        if (token.IsCancellationRequested)
                        {
                            // break; - просто остановка работы
                            token.ThrowIfCancellationRequested(); // исключение
                        }
                        Thread.Sleep(50);
                        Dispatcher.Invoke(() => Progress1.Value++);
                    }
                }
                catch (OperationCanceledException)
                {
                    // поток остановлен - нужно завершающие действия
                    while (i > 0)
                    {
                        Thread.Sleep(10);
                        Dispatcher.Invoke(() => Progress1.Value--);
                        i--;
                    }                                      
                }
            }                       
        }
        #endregion

        #region DZ 2

        Thread threadOne;
        Thread threadTwo;
        Thread threadThree;
        CancellationTokenSource tokenOne;
        CancellationTokenSource tokenTwo;
        CancellationTokenSource tokenThree;

        private void ThreadtOne(object? pars)
        {
            if (pars is CancellationToken token)
            {
                int i = 0;
                try
                {
                    for (i = 0; i < 100; i++)
                    {
                        if (token.IsCancellationRequested)
                        {
                            token.ThrowIfCancellationRequested(); // исключение
                        }
                        Thread.Sleep(50);
                        Dispatcher.Invoke(() => ProgressOne.Value++);                        
                    }
                }
                catch (OperationCanceledException)
                {
                    while (i > 0)
                    {
                        Thread.Sleep(10);
                        Dispatcher.Invoke(() => ProgressOne.Value--);
                        i--;
                    }
                }
            }
        }
        private void ThreadtTwo(object? pars)
        {
            if (pars is CancellationToken token)
            {
                int i = 0;
                try
                {
                    for (i = 0; i < 100; i++)
                    {
                        if (token.IsCancellationRequested)
                        {
                            token.ThrowIfCancellationRequested(); // исключение
                        }
                        Thread.Sleep(50);                        
                        Dispatcher.Invoke(() => ProgressTwo.Value++);                        
                    }
                }
                catch (OperationCanceledException)
                {
                    while (i > 0)
                    {
                        Thread.Sleep(10);
                        Dispatcher.Invoke(() => ProgressTwo.Value--);
                        i--;
                    }
                }
            }
        }
        private void ThreadtThree(object? pars)
        {
            if (pars is CancellationToken token)
            {
                int i = 0;
                try
                {
                    for (i = 0; i < 100; i++)
                    {
                        if (token.IsCancellationRequested)
                        {
                            token.ThrowIfCancellationRequested(); // исключение
                        }
                        Thread.Sleep(50);
                        Dispatcher.Invoke(() => ProgressThree.Value++);
                    }
                }
                catch (OperationCanceledException)
                {
                    while (i > 0)
                    {
                        Thread.Sleep(10);
                        Dispatcher.Invoke(() => ProgressThree.Value--);
                        i--;
                    }
                }
            }
        }
        private void StartOne_Click(object sender, RoutedEventArgs e)
        {
            ProgressOne.Value = 0;
            ProgressTwo.Value = 0;
            ProgressThree.Value = 0;
            threadOne = new Thread(ThreadtOne);
            threadTwo = new Thread(ThreadtTwo);
            threadThree = new Thread(ThreadtThree);
            tokenOne = new();
            tokenTwo = new();
            tokenThree = new();
            CancellationToken token1 = tokenOne.Token;
            CancellationToken token2 = tokenTwo.Token;
            CancellationToken token3 = tokenThree.Token;
            threadOne.Start(token1);
            

            //threadTwo.Start(token2);
            //threadThree.Start(token3);           
        }        
        private void StopOne_Click(object sender, RoutedEventArgs e)
        {
            //tokenOne.Cancel();
        }
        private void StartTwo_Click(object sender, RoutedEventArgs e)
        {
            ProgressOne.Value = 0;
            ProgressTwo.Value = 0;
            ProgressThree.Value = 0;
            threadOne = new Thread(ThreadtOne);
            threadTwo = new Thread(ThreadtTwo);
            threadThree = new Thread(ThreadtThree);
            tokenOne = new();
            tokenTwo = new();
            tokenThree = new();
            CancellationToken token1 = tokenOne.Token;
            CancellationToken token2 = tokenTwo.Token;
            CancellationToken token3 = tokenThree.Token;
            threadOne.Start(token1);
            threadTwo.Start(token2);
            threadThree.Start(token3);
        }
        private void StopTwo_Click(object sender, RoutedEventArgs e)
        {
            tokenOne.Cancel();
            tokenTwo.Cancel();
            tokenThree.Cancel();
        }
        private void StartThree_Click(object sender, RoutedEventArgs e)
        {            
            ProgressThree.Value = 0;
            threadThree = new Thread(ThreadtThree);
            tokenThree = new();
            CancellationToken token = tokenThree.Token;
            threadThree.Start(token);
        }
        private void StopThree_Click(object sender, RoutedEventArgs e)
        {
            tokenThree.Cancel();
        }
        #endregion
    }

    class ThreadData // для передачи данных в потоковый метод
    {
        public int Month { get; set; }
    }
}

