using System;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;

namespace Tomate.DBHelper
{

    public class DbHelper
    {
        private static int DBVERSION = 1;
        private static String DBNAME = "tomate.db";
        private static string DBPATH = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Tomate PV\\Database";
        private static string DBFILEPATH;


        public static void EliminarBaseDatos()
        {
            #if DEBUG
                DBPATH = Environment.CurrentDirectory + "\\Database";
            #endif

            if (!string.IsNullOrEmpty(DBPATH) && !Directory.Exists(DBPATH))
                Directory.CreateDirectory(DBPATH);
            DBFILEPATH = DBPATH + "\\" + DBNAME;
            if (File.Exists(DBFILEPATH))
            {
                File.Delete(DBFILEPATH);
            }
        }
        public static void CrearArchivoDb()
        {
            #if DEBUG
                DBPATH = Environment.CurrentDirectory + "\\Database";
            #endif

            if (!string.IsNullOrEmpty(DBPATH) && !Directory.Exists(DBPATH))
                Directory.CreateDirectory(DBPATH);
            DBFILEPATH = DBPATH + "\\" + DBNAME;
            if (!File.Exists(DBFILEPATH))
            {
                SQLiteConnection.CreateFile(DBFILEPATH);
            }
        }

        public static void SetDbVersion(int version)
        {
            using (SQLiteConnection dbConnection = GetDbConexion())
            {
                dbConnection.Open();
                string sqlQuery = "INSERT INTO db_versions (version, created_at) " +
                    "VALUES (@Version, @Created_at)";
                var currentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var command = dbConnection.CreateCommand();
                command.CommandText = sqlQuery;
                command.Parameters.Add(new SQLiteParameter("@Version", version));
                command.Parameters.Add(new SQLiteParameter("@Created_at", currentDate));
                command.ExecuteNonQuery();
                dbConnection.Close();
            }

        }

        public static SQLiteConnection GetDbConexion()
        {
            var configuracion = new SQLiteConnectionStringBuilder { DataSource = DBFILEPATH, Version = 3 };
            SQLiteConnection dbConnection = new SQLiteConnection(configuracion.ConnectionString);
            return dbConnection;
        }

        public static void CorrerMigraciones()
        {
            CrearArchivoDb();
            if (!ExisteTabla("db_versions"))
            {
                CrearTablaVersiones();
            }

            var oldVersion = GetVersionActualBaseDatos();
            if (oldVersion < DBVERSION)
            {
                using (SQLiteConnection dbConnection = GetDbConexion())
                {
                    DbMigrate.Run(dbConnection, oldVersion, DBVERSION);
                    SetDbVersion(DBVERSION);
                }

            }
        }

        public static void CrearTablaVersiones()
        {
            using (SQLiteConnection dbConnection = GetDbConexion())
            {
                dbConnection.Open();

                string versionTable = "CREATE TABLE db_versions ("
                    + "version INTEGER DEFAULT 0,"
                    + "created_at TEXT)";

                var command = dbConnection.CreateCommand();
                command.CommandText = versionTable;
                command.ExecuteNonQuery();
                dbConnection.Close();
            }
        }

        public static void RestablecerTablas()
        {
            using (SQLiteConnection dbConnection = GetDbConexion())
            {
                dbConnection.Open();

                DbMigrate.Back(dbConnection);

                dbConnection.Close();
            }
        }

        public static bool ExisteTabla(string tableName)
        {
            using (SQLiteConnection dbConnection = GetDbConexion())
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = "SELECT name FROM sqlite_master WHERE name='" + tableName + "'";
                var result = command.ExecuteScalar();
                dbConnection.Close();
                return result != null && result.ToString() == tableName;
            }

        }

        public static int GetVersionActualBaseDatos()
        {
            using (SQLiteConnection dbConnection = GetDbConexion())
            {
                dbConnection.Open();

                string sqlQuery = "SELECT version FROM db_versions " +
                    "ORDER BY created_at desc LIMIT 1";

                var command = dbConnection.CreateCommand();
                command.CommandText = sqlQuery;
                var result = command.ExecuteScalar();
                dbConnection.Close();

                return Convert.ToInt32(result);
            }

        }
    }
}
