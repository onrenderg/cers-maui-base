using SQLite;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;


namespace CERS.Models
{
    public class ObserverCandidatesDatabase
    {
        private SQLiteConnection conn;
        public ObserverCandidatesDatabase()
        {
            conn = DependencyService.Get<ISQLite>().GetConnection();
            conn.CreateTable<ObserverCandidates>();
        }

        public IEnumerable<ObserverCandidates> GetObserverCandidates(string Querryhere)
        {
            var list = conn.Query<ObserverCandidates>(Querryhere);
            return list.ToList();
        }
        public string AddObserverCandidates(ObserverCandidates service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteObserverCandidates()
        {
            var del = conn.Query<ObserverCandidates>("delete from ObserverCandidates");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<ObserverCandidates>(query);
            return "success";
        }
    }
}
