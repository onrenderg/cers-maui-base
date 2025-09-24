using SQLite;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;


namespace CERS.Models
{
  public  class ViewAllRemarksDatabase
    {
        private SQLiteConnection conn;
        public ViewAllRemarksDatabase()
        {
            conn = DependencyService.Get<ISQLite>().GetConnection();
            conn.CreateTable<ViewAllRemarks>();
        }

        public IEnumerable<ViewAllRemarks> GetViewAllRemarks(string Querryhere)
        {
            var list = conn.Query<ViewAllRemarks>(Querryhere);
            return list.ToList();
        }
        public string AddViewAllRemarks(ViewAllRemarks service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteViewAllRemarks()
        {
            var del = conn.Query<ViewAllRemarks>("delete from ViewAllRemarks");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<ViewAllRemarks>(query);
            return "success";
        }
    }
}
