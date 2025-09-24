using SQLite;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;

namespace CERS.Models
{
    public class SavePreferenceDatabase
    {
        private SQLiteConnection conn;

        public SavePreferenceDatabase()
        {
            conn = DependencyService.Get<ISQLite>().GetConnection();
            conn.CreateTable<SavePreferences>();
        }
        public IEnumerable<SavePreferences> GetSavePreference(string Querryhere)
        {
            var list = conn.Query<SavePreferences>(Querryhere);
            return list.ToList();
        }
        public string AddSavePreferences(SavePreferences service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteSavePreferences()
        {
            conn.Query<SavePreferences>("delete from SavePreferences");
            return "success";
        }

        public string UpdateSavePreferences(int languagecode)
        {
            conn.Query<SavePreferences>($"update SavePreferences set languagepref={languagecode}");
            return "success";
        }



    }
}
