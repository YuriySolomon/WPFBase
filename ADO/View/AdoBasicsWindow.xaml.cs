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

/* NuGet Manager (Tools - NuGet - Manage NuGet..)
 * Поиск - SqlClient
 * Выбираем - System.Data.SqlClient - Устанавливаем
 */
using System.Data.SqlClient;

namespace WPFBase.ADO
{
    /// <summary>
    /// Логика взаимодействия для AdoBasicsWindow.xaml
    /// </summary>
    public partial class AdoBasicsWindow : Window
    {
        private const String ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=E:\Studing\C#\Programms\! WPF\WPFBase\ADO\ADO121.mdf;Integrated Security=True";
        private SqlConnection connection; // объект-подключение
        public AdoBasicsWindow()
        {
            InitializeComponent();
            connection = new SqlConnection(ConnectionString);
            // !! ADO сщздает объект не открывает подключение
        }

        private void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                connection.Open();
                MessageBox.Show("Подключение открыто");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ButtonDisconnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                connection.Close();
                MessageBox.Show("Подключение закрыто");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ButtonTimestamp_Click(object sender, RoutedEventArgs e)
        {
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                MessageBox.Show("Подключение не было установлено!");
                ButtonConnect_Click(sender, e);
            }
            using (SqlCommand cmd = new SqlCommand("SELECT CURRENT_TIMESTAMP", connection))
            {
                MessageBox.Show(cmd.ExecuteScalar().ToString()); // использование команды и возврат "скаляр" - одного рез-та

            }
        }
    }   
}
    /* Обеспечить контроль открытия подключения к БД при выполнении SQL-команы.
     * Выводить предупреждение если подключение не установлено или закрыто.
     */

