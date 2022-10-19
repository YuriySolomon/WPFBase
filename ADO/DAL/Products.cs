using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFBase.ADO.DAL
{
    public class Products
    {
        private readonly SqlConnection _connection;

        public Products(SqlConnection connection)
        {
            _connection = connection;
        }

        public List<Entities.Product> GetList()
        {
            List<Entities.Product> products = new();
            using (SqlCommand cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT Id, Name, Price FROM Products";
                using SqlDataReader res = cmd.ExecuteReader();
                while (res.Read())
                {
                    products.Add(new()
                    {
                        Id    = res.GetGuid(0),
                        Name  = res.GetString(1),
                        Price = res.GetDouble(2)
                    });
                }
            }
            return products;
        }
    }
}
