using System.Data.SQLite;
using Tomate.DBHelper.Migrations;

namespace Tomate.DBHelper
{
    public class DbMigrate
    {
        public static void Run(SQLiteConnection dbConnection, int oldVersion, int newVersion)
        {
            dbConnection.Open();
            if (oldVersion == 0)
            {
                MigrationV1.Execute(dbConnection);
            }
            dbConnection.Close();
        }

        public static void Back(SQLiteConnection dbConnection)
        {
            MigrationV1.Rollback(dbConnection);
        }
    }
}
