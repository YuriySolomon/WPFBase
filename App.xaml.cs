using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WPFBase
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const int FIELD_SIZE_X = 6; // Размер поля по горизонтали
        public const int FIELD_SIZE_Y = 7; // Размер поля по вертикали
        public const String ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=E:\Studing\C#\Programms\! WPF\WPFBase\ADO\ADO121.mdf;Integrated Security=True";
        public const String ConnectionStringEF = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=ADO121_EF.mdf;Integrated Security=True";
    }
}
