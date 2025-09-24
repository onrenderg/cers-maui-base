using SQLite;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;

namespace CERS.Models
{
    public class ObserverWardsDatabase
    {
        private SQLiteConnection conn;
        public ObserverWardsDatabase()
        {
            conn = DependencyService.Get<ISQLite>().GetConnection();
            conn.CreateTable<ObserverWards>();
        }

        public IEnumerable<ObserverWards> GetObserverWards(string Querryhere)
        {
            var list = conn.Query<ObserverWards>(Querryhere);
            return list.ToList();
        }
        public string AddObserverWards(ObserverWards service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteObserverWards()
        {
            var del = conn.Query<ObserverWards>("delete from ObserverWards");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<ObserverWards>(query);
            return "success";
        }
    }
}
