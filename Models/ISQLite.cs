
using SQLite;

namespace CERS
{
    public interface ISQLite
    {
        SQLiteConnection GetConnection();
    }
}
