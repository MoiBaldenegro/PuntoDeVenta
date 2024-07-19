using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Tomate.DBHelper;

namespace Tomate.Models
{
    public class Base
    {
        protected virtual string llavePrimaria()
        {
            return "";
        }

        protected virtual string getTabla()
        {
            return "";
        }

        protected virtual bool getSoftDelete()
        {
            return true;
        }

        protected virtual Dictionary<string, object?> getColumnas()
        {
            return new Dictionary<string, object?>();
        }

        public void save()
        {
            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();

                string columnas = "";
                string datos = "";

                foreach (KeyValuePair<string, object?> valor in getColumnas())
                {
                    columnas += $"{valor.Key}, ";
                    datos += $"@{valor.Key}, ";
                    command.Parameters.Add(new SQLiteParameter($"@{valor.Key}", valor.Value));
                }
                columnas = columnas.Substring(0, columnas.Length - 2);
                datos = datos.Substring(0, datos.Length - 2);
                string sqlQuery = $"INSERT OR REPLACE INTO {getTabla()} ({columnas}) VALUES ({datos})";
                command.CommandText = sqlQuery;
                command.ExecuteNonQuery();
                dbConnection.Close();
            }
        }

        public void delete()
        {
            if (getSoftDelete())
            {
                deleteSoft();
                return;
            }

            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                string sqlQuery = $"DELETE FROM {getTabla()} WHERE {llavePrimaria()} = @{llavePrimaria()}";
                var command = dbConnection.CreateCommand();
                command.CommandText = sqlQuery;
                command.Parameters.Add(new SQLiteParameter($"@{llavePrimaria()}", getColumnas()[llavePrimaria()]));
                command.ExecuteNonQuery();
                dbConnection.Close();
            }

        }

        private void deleteSoft()
        {
            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                string sqlQuery = $"UPDATE {getTabla()} SET deleted_at = @DeletedAt";
                var command = dbConnection.CreateCommand();
                command.CommandText = sqlQuery;
                command.Parameters.Add(new SQLiteParameter("@DeletedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                command.ExecuteNonQuery();
                dbConnection.Close();
            }
        }

        /*public static SQLiteDataReader? buscar(string sql)
        {
            return buscar(sql, null);
        }

        public static SQLiteDataReader? buscar(string sql, Dictionary<string, object?>? parametros)
        {
            DbHelper dbHelper = new DbHelper();
            using (SQLiteConnection dbConnection = dbHelper.GetDbConexion())
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = sql;

                if (parametros != null)
                {
                    foreach (KeyValuePair<string, object?> valor in parametros)
                    {
                        command.Parameters.Add(new SQLiteParameter(valor.KeyModal, valor.Value));
                    }
                }

                var result = command.ExecuteReader();
                while (result.Read())
                {
                    dbConnection.Close();
                    return result;
                }
                result.Close();
            }
            return null;
        }

        public static List<SQLiteDataReader> select(string sql)
        {
            return select(sql, null);
        }

        public static List<SQLiteDataReader> select(string sql, Dictionary<string, object?>? parametros)
        {
            var resultadosSql = new List<SQLiteDataReader>();
            DbHelper dbHelper = new DbHelper();
            using (SQLiteConnection dbConnection = dbHelper.GetDbConexion())
            {
                dbConnection.Open();
                
                var command = dbConnection.CreateCommand();
                command.CommandText = sql;

                if (parametros != null)
                {
                    foreach (KeyValuePair<string, object?> valor in parametros)
                    {
                        command.Parameters.Add(new SQLiteParameter(valor.KeyModal, valor.Value));
                    }
                }

                var result = command.ExecuteReader();
                while (result.Read())
                {
                    resultadosSql.Add(result);
                }
                result.Close();
                dbConnection.Close();
            }

            return resultadosSql;
        }*/

    }
}
