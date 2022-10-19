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
using System.Data.SqlClient;

namespace WPFBase.ADO
{
    /// <summary>
    /// Логика взаимодействия для SalesWindow.xaml
    /// </summary>
    public partial class SalesWindow : Window
    {
        private readonly SqlConnection _connection;
        private readonly DAL.Departments _departments; // "инструментарий" работы с таблицей БД
        private readonly DAL.Products _products;
        private readonly DAL.Managers _managers;
        public SalesWindow()
        {
            InitializeComponent();
            _connection = new SqlConnection(App.ConnectionString);
            _departments = new DAL.Departments(_connection);
            _products = new DAL.Products(_connection);
            _managers = new DAL.Managers(_connection);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowMonitor();
            ShowDepartments();
            ShowProducts();
            ShowManagers();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            _connection.Close();
        }
        private void ShowManagers()
        {
            ManagersGrid.ItemsSource = _managers.GetList();
            ManagersGrid.Columns[0].Visibility = Visibility.Collapsed;
            ManagersGrid.Columns[4].Visibility = Visibility.Collapsed;
            ManagersGrid.Columns[5].Visibility = Visibility.Collapsed;
            ManagersGrid.Columns[6].Visibility = Visibility.Collapsed;
        }
        private void ShowProducts()
        {
            ProductsGrid.ItemsSource = _products.GetList();
            ProductsGrid.Columns[0].Visibility = Visibility.Collapsed;
        }
        private void ShowDepartments()
        {
            // PRM - Object Relation Mapping - Отображение реляционных данных на объекты
            // "Слой" программы, отвечающий за преобразование данных в объекты и их коллекции
            // Детали - см. в папке Entities
            StringBuilder sb = new(); 
            foreach (Entities.Department department in _departments.GetList())
            {
                sb.AppendLine(department.ToString());
            }
            DepartmentsInfo.Text = sb.ToString();
        }
        private void ShowMonitor()
        {
            try
            {
                _connection.Open();
                MonitorDbLabel.Content = "Подключена";
                MonitorDbLabel.Foreground = Brushes.Green;
            }
            catch
            {
                MonitorDbLabel.Content = "Отключена";
                MonitorDbLabel.Foreground = Brushes.Red;
            }
            String sql = "SELECT COUNT(*) FROM Departments";
            using (SqlCommand cmd = new(sql, _connection))
            {
                try
                {
                    MonitorDepartmensLabel.Content = cmd.ExecuteScalar();
                    MonitorDepartmensLabel.Foreground = Brushes.Green;
                }
                catch
                {
                    MonitorDepartmensLabel.Content = "---";
                    MonitorDepartmensLabel.Foreground = Brushes.Red;
                }
            }
                
            sql = "SELECT COUNT(*) FROM Managers";           
            using (SqlCommand cmd = new(sql, _connection))
            {
                try
                {
                    MonitorManagersLabel.Content = cmd.ExecuteScalar();
                    MonitorManagersLabel.Foreground = Brushes.Green;
                }
                catch
                {
                    MonitorManagersLabel.Content = "---";
                    MonitorManagersLabel.Foreground = Brushes.Red;
                }
            }

            sql = "SELECT COUNT(*) FROM Products";
            using (SqlCommand cmd = new(sql, _connection))
            {
                try
                {
                    MonitorProductsLabel.Content = cmd.ExecuteScalar();
                    MonitorProductsLabel.Foreground = Brushes.Green;
                }
                catch
                {
                    MonitorProductsLabel.Content = "---";
                    MonitorProductsLabel.Foreground = Brushes.Red;
                }
            }

            sql = "SELECT COUNT(*) FROM Sales";
            using (SqlCommand cmd = new(sql, _connection))
            {
                try
                {
                    MonitorSalesLabel.Content = cmd.ExecuteScalar();
                    MonitorSalesLabel.Foreground = Brushes.Green;
                }
                catch
                {
                    MonitorSalesLabel.Content = "---";
                    MonitorSalesLabel.Foreground = Brushes.Red;
                }
            }
        }
    }
}
/* При работе с БД разделяют два режима работы - присоединенный и отсоединенный.
 * 
 * В присоединенном режиме подключение к БД не закрывается и каждое обращение
 * к данным является запросом к БД. Режим используется если к БД есть и другие
 * обращения (от других программ). режим обеспечивает актуальность данных.
 * 
 * В отсоединенном режиме при первом запросе ВСЕ данные из БД копируются в
 * приложение, соединение закрывается. Программа работает с локальной копией
 * и при завершении работы сохранят изменения в БД. Режим оспользуется когда
 * БД использует одна программа, других участников нет.
 * Работает быстрее, т.к. нет дополнительных запросов.
 */
