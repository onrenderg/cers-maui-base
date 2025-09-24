using SQLite;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;

namespace CERS.Models
{
  public  class UserDetailsDatabase
    {
        private SQLiteConnection conn;
        public UserDetailsDatabase()
        {
            conn = DependencyService.Get<ISQLite>().GetConnection();
            conn.CreateTable<UserDetails>();
        }

        public IEnumerable<UserDetails> GetUserDetails(string Querryhere)
        {
            var list = conn.Query<UserDetails>(Querryhere);
            return list.ToList();
        }
        public string AddUserDetails(UserDetails service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteUserDetails()
        {
            var del = conn.Query<UserDetails>("delete from UserDetails");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<UserDetails>(query);
            return "success";
        }
    }
}
