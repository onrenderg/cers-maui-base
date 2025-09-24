using SQLite;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;

namespace CERS.Models
{
    public class ExpenseSourcesDatabase
    {
        private SQLiteConnection conn;
        public ExpenseSourcesDatabase()
        {
            conn = DependencyService.Get<ISQLite>().GetConnection();
            conn.CreateTable<ExpenseSources>();
        }

        public IEnumerable<ExpenseSources> GetExpenseSources(string Querryhere)
        {
            var list = conn.Query<ExpenseSources>(Querryhere);
            return list.ToList();
        }
        public string AddExpenseSources(ExpenseSources service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteExpenseSources()
        {
            var del = conn.Query<ExpenseSources>("delete from ExpenseSources");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<ExpenseSources>(query);
            return "success";
        }
    }
}
