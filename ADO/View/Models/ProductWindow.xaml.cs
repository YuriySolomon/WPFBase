using Ninject.Activation;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace WPFBase.ADO.View.Models
{
    /// <summary>
    /// Окно-форма для данных из таблицы БД Product
    /// </summary>
    public partial class ProductWindow : Window
    {
        public Entities.Product Product { get; set; }
        
    public ProductWindow()
        {
            InitializeComponent();
        }
        // Конвертация строки суммы в double
        private double ConvertStrToDouble(string temp)
        {
            // При конвертировании string → double используем другую Culture
            // По умолчанию берет с Windows
            var culture = new CultureInfo("en-US", false);
            Thread.CurrentThread.CurrentCulture = culture;
            // Меняем запятую на точку. если такова присутствует
            temp = temp.Replace(',', '.');
            // Проверяем можно ли преобразовать строку в число с плавающей точкой
            try
            {
                double price = double.Parse(temp);
                return price;
            }
            catch
            {
                MessageBox.Show("Вы ввели не правильную сумму.", "Ошибка!",
                MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ProductId.Text = Product.Id.ToString();
            if (Product.Id == Guid.Empty)
            {
                Save.Content = "Добавить";
                ProductName.Text = "";
                ProductPrice.Text = "";
                Delete.IsEnabled = false;
            }
            else
            {
                ProductName.Text = Product.Name;
                ProductPrice.Text = Product.Price.ToString();
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {         
            if (ProductName.Text != "" && ProductPrice.Text != "")
            {                
                Product.Name = ProductName.Text;
                double result = ConvertStrToDouble(ProductPrice.Text);
                if (result != 0)
                {
                    Product.Price = result;
                    DialogResult = true;
                }
                else
                {
                    DialogResult = false;
                }                
            }
            else
            {
                DialogResult = false;
            }
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
                MessageBox.Show("Вы уверены?", "Удаление данных!",
                MessageBoxButton.YesNo, MessageBoxImage.Question))
            {
                Product.Name = String.Empty;
                Product.Price = 0;
                DialogResult = true;
                this.Close();
            }
        }
    }
}
