using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
    /// Логика взаимодействия для ProcessesWindow.xaml
    /// </summary>
    public partial class ProcessesWindow : Window
    {
        public ObservableCollection<Process> Processes { get; set; }
        public ProcessesWindow()
        {
            Processes = new();
            InitializeComponent();
            this.DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Process[] proc = Process.GetProcesses(); // получаем все системные процессы
            Processes.Clear();
            foreach (Process process in proc)   // переносим их в наблюдаемую коллекцию
            {
                Processes.Add(process);                
            }
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                try
                {
                    ThreadsBlock.Text = String.Empty;
                    if (item.Content is Process process)
                    {
                        foreach (ProcessThread thread in process.Threads)
                        {
                            ThreadsBlock.Text += thread.Id + " " + thread.TotalProcessorTime + "\n";
                        }
                    }
                }
                catch
                {
                    ThreadsBlock.Text = "Отказано в доступе";
                }
            }
        }

        private void NotepadButton_Click(object sender, RoutedEventArgs e)
        {
            var notepad = Process.Start("notepad.exe");
            Task.Run(() =>
            {
                Task.Delay(3000).ContinueWith(t => notepad.Kill());
            });
        }

        private void ExplorerButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer", System.AppDomain.CurrentDomain.BaseDirectory);
        }

        private void StepButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("MicrosoftEdge.exe", "http://itstep.org");
        }
    }
}
/* Добавить на окно кнопку выбора файла для открытия в блокноте:
 * пользователь нажимает кнопку выбора файла
 * в случае "ОК" программа запускает блокнот и открывает в нем выбранный файл
 * Добавить кнопку закрытия блокнота (в нашем окно), активировать ее когда
 * запущен блокнот
 * ** Добавить поле ввода URL (иеб-адреса) и кнопку открытия браузера с этим URL 
 */