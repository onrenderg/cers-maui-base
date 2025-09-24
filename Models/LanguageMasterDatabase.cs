using SQLite;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;

namespace CERS.Models
{
    public class LanguageMasterDatabase
    {
        private SQLiteConnection conn;
        public LanguageMasterDatabase()
        {
            conn = DependencyService.Get<ISQLite>().GetConnection();
            conn.CreateTable<LanguageMaster>();
        }
        public IEnumerable<LanguageMaster> GetLanguageMaster(string Querryhere)
        {
            var list = conn.Query<LanguageMaster>(Querryhere);
            return list.ToList();
        }
        public string AddLanguageMaster(LanguageMaster service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteLanguageMaster()
        {
            var del = conn.Query<LanguageMaster>("delete from LanguageMaster");
            return "success";
        }
    }
}
