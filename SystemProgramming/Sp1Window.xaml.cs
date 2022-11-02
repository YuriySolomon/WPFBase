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
            Messages.Text = "";        }

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
    }
}
/*      Системное программирование. Введение. Многопоточность.
 * Развитие вычислительной техники достигло пределов физического плана
 * и последние время развивается путем увеличения кол-ва процессоров
 * в место ускорения одного процессорного модуля.
 * Программирование также смещается от написания одно-ниточных програм
 * к многониточным - с наличием методов, которые выполняются разными
 * исполнителями в разых окружениях и. возможно. абсолютно одновременно.
 * Потоки (Threads) (не путать, как "поток" переводиться еще и stream,
 * но это поток данных) - это  системные ресурсы. позволяющие обеспечить 
 * работу кода отдельно от других потоков.
 * 
 * Зачем?
 * - Разгрузка интерфейсаЖ если в обработчике события будет долгий код.
 *      то интерфейс не будет реагировать на другие события. пока не
 *      закончится данное.
 *   = В некоторых системах даже существует запрет на использование
 *   в обработчиках некоторых инструментов. например, запросов к БД,
 *   обращение к сетевым ресурсам и т.п.
 *   = НО! в большинстве случаев элементы интерфейса не разрешают
 *   менять свое состояние из других потоков. Поэтому работа в 
 *   многопоточном режиме "инг-понг" один поток стартует второй
 *   поток. второй поток делегирует первому потоку задачи вывода
 *   
 *   Main thread (Window)               New Theard (TheardMethod)
 *   ctor
 *   Loader
 *   Click ----------------------------> ctor
 *   
 *   AddMessage <---------------------- Invoke (addMessage)
 *                                      Sleep() Реаьно - 
 *    Message_Click                     Sleep() здесь
 *    Messages.Text +=                  Sleep() происходить
 *      "Сообщение\n";                  Sleep() какая-то долгая
 *                                      Sleep() работа
 *    AddMessage <--------------------- Invoke(AddMessage)
 *                                          |
 *    Message_Click                         X (return - конец потока)
 *    Messages.Text +=
 *     "Сообщение\n";
 */
