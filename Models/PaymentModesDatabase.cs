using SQLite;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;

namespace CERS.Models
{
    public class PaymentModesDatabase
    {
        private SQLiteConnection conn;
        public PaymentModesDatabase()
        {
            conn = DependencyService.Get<ISQLite>().GetConnection();
            conn.CreateTable<PaymentModes>();
        }

        public IEnumerable<PaymentModes> GetPaymentModes(string Querryhere)
        {
            var list = conn.Query<PaymentModes>(Querryhere);
            return list.ToList();
        }
        public string AddPaymentModes(PaymentModes service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeletePaymentModes()
        {
            var del = conn.Query<PaymentModes>("delete from PaymentModes");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<PaymentModes>(query);
            return "success";
        }
    }
}
