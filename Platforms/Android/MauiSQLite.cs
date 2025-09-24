using SQLite;

namespace CERS.Platforms.Android
{
    public class MauiSQLite : ISQLite
    {
        public SQLiteConnection GetConnection()
        {
            var dbName = "CERS.db";
            var path = System.IO.Path.Combine(FileSystem.AppDataDirectory, dbName);
            var conn = new SQLiteConnection(path);
            return conn;
        }
    }
}
