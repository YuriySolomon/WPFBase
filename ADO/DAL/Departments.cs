using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFBase.ADO.DAL
{
    // DAL - Data Access Layer - набор "инструментов" для доступа к данным
    // из таблицы Departments
    public class Departments
    {
        private readonly SqlConnection _connection;
        public Departments(SqlConnection connection)
        {
            _connection = connection;
        }

         public List<Entities.Department> GetList()
        {
            using(SqlCommand cmd = _connection.CreateCommand())
            {
                List<Entities.Department> departments = new();
                cmd.CommandText = "SELECT Id, Name FROM Departments";
                SqlDataReader res = cmd.ExecuteReader();
                while (res.Read())
                {
                    departments.Add(new Entities.Department
                    {
                        Id = res.GetGuid(0),
                        Name = res.GetString(1)
                    });
                }
                res.Close();
                return departments;
            }
        }
    }
}
