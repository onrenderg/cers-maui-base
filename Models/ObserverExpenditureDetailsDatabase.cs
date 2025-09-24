using SQLite;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;

namespace CERS.Models
{
    public class ObserverExpenditureDetailsDatabase
    {

        private SQLiteConnection conn;
        public ObserverExpenditureDetailsDatabase()
        {
            conn = DependencyService.Get<ISQLite>().GetConnection();
            conn.CreateTable<ObserverExpenditureDetails>();
        }

        public IEnumerable<ObserverExpenditureDetails> GetObserverExpenditureDetails(string Querryhere)
        {
            var list = conn.Query<ObserverExpenditureDetails>(Querryhere);
            return list.ToList();
        }
        public string AddObserverExpenditureDetails(ObserverExpenditureDetails service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteObserverExpenditureDetails()
        {
            var del = conn.Query<ObserverExpenditureDetails>("delete from ObserverExpenditureDetails");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<ObserverExpenditureDetails>(query);
            return "success";
        }
    }
}
