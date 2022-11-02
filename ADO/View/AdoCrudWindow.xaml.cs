using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
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
using System.Windows.Threading;

namespace WPFBase.ADO.View
{
    /// <summary>
    /// Логика взаимодействия для AdoCrudWindow.xaml
    /// </summary>
    public partial class AdoCrudWindow : Window
    {
        // ObservableCollection - коллекция, уведомляющая контейнер о своих изменениях
        public ObservableCollection<Entities.Department> Departments { get; set; }
        public ObservableCollection<Entities.Product> Products { get; set; }
        private readonly SqlConnection _connection;
        private readonly DAL.Departments _departments;
        private readonly DAL.Products _products; 
        private DispatcherTimer timer = new DispatcherTimer();
        int n = 0;

        public AdoCrudWindow()
        {
            InitializeComponent();
            _connection = new SqlConnection(App.ConnectionString);
            ConnectDb();
            _departments = new DAL.Departments(_connection);
            Departments = new(_departments.GetList())
            {
                new Entities.Department
                {
                    Id = Guid.Empty,
                    Name = "Добавить новый отдел"
                }
            };

            _products = new DAL.Products(_connection); 
            Products = new(_products.GetList())
            {
                new Entities.Product
                {
                    Id = Guid.Empty,
                    Name = "Добавить новый товар",
                    Price = .0
                }
            }; 

            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Click;
            // Связываение данных (1) указывваем контекст - откуда беруться
            // имена ресурсов
            this.DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            //timer.Start();            
        }
        private void ConnectDb()
        {
            try
            {
                _connection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Conection error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }
        private void Timer_Click(object? sender, EventArgs e)
        {
            Departments.Add(_departments.GetList()[n]);
            n++;
            if (n == _departments.GetList().Count)
            {
                timer.Stop();
            }
        }
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            _connection?.Close();
        }
        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // двойной щелчок мыши на элементе (строке) списка
            if (sender is ListViewItem item)
            {
                // В тело заходим если sender - это ListViewItem
                // Извлекаем ссылку на объект данных (Department)
                if (item.Content is Entities.Department department)
                {
                    // department - ссылка на элемент коллекции Department,
                    // на котором сработало событие
                    // MessageBox.Show(department.ToString());
                    var editWindow = new View.Models.DepartmentWindow()
                    {
                        Department = department
                    };
                    if (editWindow.ShowDialog() == true)
                    {
                        if (department.Id == Guid.Empty) // Добавление
                        {
                            Guid id = _departments.Create(department);
                            Departments.Remove(department);
                            department.Id = id;
                            Departments.Add(department);
                            Departments.Add(new Entities.Department
                            {
                                Id = Guid.Empty,
                                Name = "Добавить новый отдел"
                            });
                        }
                        else // Изменение, Удаление
                        {
                            if (department.Name == String.Empty) // Удаление
                            {
                                Departments.Remove(department);
                                _departments.Delete(department);
                            }
                            else // Изменение
                            {
                                // Коллекция не отслеживает изменение внутри элементов
                                // поэтому создаем эфект изменения состава коллекции
                                int index = Departments.IndexOf(department);
                                Departments.Remove(department);
                                Departments.Insert(index, department);
                                // Пока обновлен только список. вносим изменения в БД
                                _departments.Update(department);
                            }                            
                        }
                    }
                }
            }
        }

        private void ListViewProducts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Entities.Product product)
                {
                    var editWindow = new View.Models.ProductWindow()
                    {
                        Product = product
                    };
                    if (editWindow.ShowDialog() == true)
                    {
                        if (product.Id == Guid.Empty)
                        {
                            Guid id = _products.Create(product);
                            Products.Remove(product);
                            product.Id = id;
                            Products.Add(product);
                            Products.Add(new Entities.Product
                            {
                                Id = Guid.Empty,
                                Name = "Добавить новый товар",
                                Price = .0
                            });
                        }
                        else
                        {
                            if (product.Name == String.Empty)
                            {
                                Products.Remove(product);
                                _products.Delete(product);
                            }
                            else
                            {
                                int index = Products.IndexOf(product);
                                Products.Remove(product);
                                Products.Insert(index, product);
                                _products.Update(product);
                            }
                        }
                    }
                }
            }
        }
    }
}
/* CRUD - (Create Read Update Delete) - концепция, согласно которой
 * информационная система должна обеспечить эти 4 операции по отношению
 * ко всем своим данны.
 * Create - добавление данных (Add, Insert) - создание новых инфо-единиц
 * Read   - отображение, извлечение данных из БД
 * Update - внесение изменений в уже существующие данные
 * Delete - удаление данных из БД. особенность БД еще и в том, что удаление
 *          нельзя откатить 9отменить). Поэтому одной из традиций является
 *          замена насттоящего удаления введением дополнительного поля
 *          "deleted" (либо признак, либо дата удаления).
 *          Как вариант, ведется отдельная таблица удалений, в которой кроме
 *          даты отмечается кто удалил. причина удаления и т.п.
 * 
 * ЗаданиеЖ ограничить возможность введения пустого названия для нового раздела
 * 
 * Д.З. реализовать концепцию CRUD для работы с таблицей товаров (Products)
 * По аналогии с рассмотренными задачами с таблицей отделов (Depatrments)
 */
