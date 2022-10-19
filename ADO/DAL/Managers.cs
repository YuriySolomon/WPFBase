using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFBase.ADO.DAL
{
    public class Managers
    {
        private readonly SqlConnection _connection;

        public Managers(SqlConnection connection)
        {
            _connection = connection;
        }

        public List<Entities.Manager> GetList()
        {
            List<Entities.Manager> managers = new();
            using (SqlCommand cmd = _connection.CreateCommand())
            {
                //cmd.CommandText = "SELECT Id, Surname, Name, Secname, Id_main_dep, Id_sec_dep, Id_chief FROM Managers";
                cmd.CommandText = "SELECT Id, Surname, Name, Secname, Id_main_dep, Id_sec_dep, Id_chief FROM Managers";
                using SqlDataReader res = cmd.ExecuteReader();
                while (res.Read())
                {
                    managers.Add(new()
                    {
                        Id = res.GetGuid(0),
                        Surname = res.GetString(1),
                        Name = res.GetString(2),
                        Secname = res.GetString(3),
                        Id_main_dep = res.GetGuid(4),
                        Id_sec_dep = null,//res.GetGuid(5),
                        Id_chief = null //res.GetGuid(6)
                    });
                }
            }
            return managers;
        }
    }
}
/*Д.З. В созданной базе данных реализовать ORM-DAL-BLL для формирования
 * таблицы "Отдел - Количество сотрудников" ( в классе Departments добавить метод.
 * возвращающий список отдельных "моделей" { Name - Count } ) * 
 */
