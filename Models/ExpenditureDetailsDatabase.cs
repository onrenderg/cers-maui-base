using SQLite;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;

namespace CERS.Models
{
    public class ExpenditureDetailsDatabase
    {
        private SQLiteConnection conn;
        public ExpenditureDetailsDatabase()
        {
            conn = DependencyService.Get<ISQLite>().GetConnection();
            conn.CreateTable<ExpenditureDetails>();
        }

        public IEnumerable<ExpenditureDetails> GetExpenditureDetails(string Querryhere)
        {
            var list = conn.Query<ExpenditureDetails>(Querryhere);
            return list.ToList();
        }
        public string AddExpenditureDetails(ExpenditureDetails service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteExpenditureDetails()
        {
            var del = conn.Query<ExpenditureDetails>("delete from ExpenditureDetails");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<ExpenditureDetails>(query);
            return "success";
        }
    }
}
