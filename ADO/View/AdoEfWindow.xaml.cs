using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Microsoft.EntityFrameworkCore;

namespace WPFBase.ADO.View
{
    /// <summary>
    /// Логика взаимодействия для AdoEfWindow.xaml
    /// </summary>
    public partial class AdoEfWindow : Window
    {
        private readonly EF.FirmContext FirmContext;
        public ObservableCollection<Entities.Department> Departments { get; set; }
        public ObservableCollection<Entities.Product> Products { get; set; }
        public ObservableCollection<Entities.Manager> Managers { get; set; } 
        public AdoEfWindow()
        {
            InitializeComponent();
            FirmContext = new();
            Departments = new();
            Products = new();
            Managers = new(); 
            this.DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // синтаксис с методами
            LabelDepartments.Content =
                FirmContext.Departments.Count();
            
            // синтаксис с запросом
            var cntQuery = from p in FirmContext.Products
                           where p.Price > 0
                           select p;
            LabelProducts.Content = cntQuery.Count();

            // заполняем коллекцию
            var depQuery = from d in FirmContext.Departments
                           where d.Name != null
                           orderby d.Name descending
                           select d;
            foreach(var dep in depQuery)
            {
                Departments.Add(dep);
            }

            // другой синтаксис
            var query = FirmContext.Products
                        .Where(p => p.Price > 0)
                        .OrderBy(p => p.Name);
            foreach(var p in query)
            {
                Products.Add(p);
            }
            // Д.З. EntityFramework : вывести таблицу с сотрудниками
            // Экзамен Ж приложить архив (или ссылку на репозиторий)
            // со всеми выполненными проектами

            var manQuery = FirmContext.Managers
                            .Where(m => m.Name != null)
                            .OrderBy(m => m.Surname);
            foreach(var m in manQuery)
            {
                Managers.Add(m);
            }
        }
    }
}
/*      Entity Framework и идеология "Code first"
 *  Идеология основывается на том, что сначала описываеются классы,
 * создается контекст (окружение), а БД создается автоматически
 * из анализа и кго классов.
 *  В противоположность "Data first" нвчинается с БД. а классы и 
 * контекст создаются по результатам анализа таблиц БД.
 * 
 *  Работу с данной идеологией обеспечивает Entity Framework (Core)
 * который нужно утсановить как дополнительные пакет NuGet:
 * Microsoft.EntityFrameworkCore - основа. набор инструментов для создания и анализа контекста
 * Microsoft.EntityFrameworkCore.SqlServer - драйверы для работы с MS SQL Server
 * Microsoft.EntityFrameworkCore.Tools - инструменты командной строки для консоли пакетов
 */
