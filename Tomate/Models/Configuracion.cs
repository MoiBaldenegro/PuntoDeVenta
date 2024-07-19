using System;
using System.Data.SQLite;
using Tomate.DBHelper;
using Tomate.HttpRequest;

namespace Tomate.Models
{
    public class Configuracion
    {
        public static class Key
        {
            public static string DIRECCION_SERVIDOR = "direccion_servidor";
            public static string TOKEN_ACCESO = "token_acceso";
            public static string ULTIMA_SINCRONIZACION = "ultima_sincronizacion";
            public static string INICIAR_WINDOWS = "inicio_windows";
            public static string CONFIGURADO = "configurado";
            //terminal
            public static string TERMINAL_NOMBRE = "terminal_nombre";
            public static string TERMINAL_SUCURSAL_ID = "terminal_sucursal_id";
            public static string TERMINAL_NUMERO = "terminal_numero";
        }

        public static void removerValor(string key)
        {
            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                string sqlQuery = "DELETE FROM configuraciones WHERE key = @KeyModal";
                var fechaActual = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var command = dbConnection.CreateCommand();
                command.CommandText = sqlQuery;
                command.Parameters.Add(new SQLiteParameter("@KeyModal", key));
                command.ExecuteNonQuery();
                dbConnection.Close();
            }
        }

        public static void setValor(string key, string valor)
        {
            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                string sqlQuery = "INSERT OR REPLACE INTO configuraciones (key, valor, updated_at) " +
                    "VALUES (@KeyModal, @Valor, @Updated_at)";
                var fechaActual = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var command = dbConnection.CreateCommand();
                command.CommandText = sqlQuery;
                command.Parameters.Add(new SQLiteParameter("@KeyModal", key));
                command.Parameters.Add(new SQLiteParameter("@Valor", valor));
                command.Parameters.Add(new SQLiteParameter("@Updated_at", fechaActual));
                command.ExecuteNonQuery();
                dbConnection.Close();
            }
        }

        public static string getValor(string key)
        {
            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();

                string sqlQuery = "SELECT valor FROM configuraciones " +
                    "WHERE key = @KeyModal";

                var command = dbConnection.CreateCommand();
                command.CommandText = sqlQuery;
                command.Parameters.Add(new SQLiteParameter("@KeyModal", key));
                var result = command.ExecuteScalar();

                dbConnection.Close();

                return result != null ? result.ToString() : null;
            }

        }

        public static void eliminarTodo()
        {
            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                string sqlQuery = "DELETE FROM configuraciones";
                var command = dbConnection.CreateCommand();
                command.CommandText = sqlQuery;
                command.ExecuteNonQuery();
                dbConnection.Close();
            }

        }

        public static void setConfigurado(string configurado)
        {
            setValor(Key.CONFIGURADO, configurado);
            ApiClient.RemoverConfiguracion();
        }

        public static bool isConfigurado()
        {
            return getValor(Key.CONFIGURADO) == "1";
        }

    }
}
