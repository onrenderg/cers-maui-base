using SQLite;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;

namespace CERS.Models
{
    public class ObservorLoginDetailsDatabase
    {
        private SQLiteConnection conn;
        public ObservorLoginDetailsDatabase()
        {
            conn = DependencyService.Get<ISQLite>().GetConnection();
            conn.CreateTable<ObservorLoginDetails>();
        }

        public IEnumerable<ObservorLoginDetails> GetObservorLoginDetails(string Querryhere)
        {
            var list = conn.Query<ObservorLoginDetails>(Querryhere);
            return list.ToList();
        }
        public string AddObservorLoginDetails(ObservorLoginDetails service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteObservorLoginDetails()
        {
            var del = conn.Query<ObservorLoginDetails>("delete from ObservorLoginDetails");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<ObservorLoginDetails>(query);
            return "success";
        }
    }
}
