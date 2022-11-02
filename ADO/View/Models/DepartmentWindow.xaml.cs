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

namespace WPFBase.ADO.View.Models
{
    /// <summary>
    /// Окно-форма для данных из таблицы БД Departments
    /// </summary>
    public partial class DepartmentWindow : Window
    {
        public Entities.Department Department { get; set; }
        public DepartmentWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DepartmentId.Text = Department.Id.ToString();            
            // Режим добавления - если передан пустой Id
            if (Department.Id == Guid.Empty)
            {
                Save.Content = "Добавить";
                DepartmentName.Text = "";
                Delete.IsEnabled = false;
            }
            else
            {
                DepartmentName.Text = Department.Name;
            }
        }        
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (DepartmentName.Text != "")
            {
                Department.Name = DepartmentName.Text;
                DialogResult = true;
            }
            else
            {
                DialogResult = false;
            }
            //Department.Name = DepartmentName.Text;
            //DialogResult = true;
            this.Close();
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.Yes == 
                MessageBox.Show("Вы уверены?", "Удаление данных",
                MessageBoxButton.YesNo, MessageBoxImage.Question))
            {
                Department.Name = String.Empty;
                DialogResult = true;
                this.Close();
            }
        }
    }
}
