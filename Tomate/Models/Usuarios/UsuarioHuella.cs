using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Diagnostics;
using Tomate.DBHelper;
using Tomate.Statics;
using Tomate.Utils;

namespace Tomate.Models.Usuarios
{
    public class UsuarioHuella : Base
    {
        protected override string getTabla()
        {
            return ModelColumnDbHelper.UsuarioHuella.DB_TABLA;
        }

        protected override bool getSoftDelete()
        {
            return true;
        }

        protected override Dictionary<string, object?> getColumnas()
        {
            var coumnas = new Dictionary<string, object?>();
            coumnas[ModelColumnDbHelper.UsuarioHuella.ID] = Id;
            coumnas[ModelColumnDbHelper.UsuarioHuella.USUARIO_ID] = UsuarioId;
            coumnas[ModelColumnDbHelper.UsuarioHuella.HUELLA] = Huella;
            coumnas[ModelColumnDbHelper.UsuarioHuella.POSICION] = Posicion;
            coumnas[ModelColumnDbHelper.UsuarioHuella.CREATED_AT] = CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.UsuarioHuella.UPDATED_AT] = UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.UsuarioHuella.DELETED_AT] = DeletedAt?.ToString("yyyy-MM-dd HH:mm:ss");

            return coumnas;
        }

        protected override string llavePrimaria()
        {
            return ModelColumnDbHelper.UsuarioHuella.ID;
        }

        public string? Id { get; set; }
        public string? UsuarioId { get; set; }
        public byte[]? Huella { get; set; }
        public int? Posicion { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public UsuarioHuella() { }


        public UsuarioHuella(JObject data)
        {
            Id = (string?)data["id"];
            UsuarioId = (string?)data["usuario_id"];
            if ((string?)data["huella"] != null)
            {
                try
                {
                    Huella = Convert.FromBase64String($"{(string?)data["huella"]}");
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }
            Posicion = (int?)data["posicion"];
            CreatedAt = (DateTime?)data["created_at"];
            if ((DateTime?)data["updated_at"] != null)
            {
                UpdatedAt = (DateTime?)data["updated_at"];
            }
            if ((DateTime?)data["deleted_at"] != null)
            {
                DeletedAt = (DateTime?)data["deleted_at"];
            }
        }

        public UsuarioHuella(SQLiteDataReader reader)
        {
            Id = Convert.ToString(reader[ModelColumnDbHelper.UsuarioHuella.ID]);
            UsuarioId = Convert.ToString(reader[ModelColumnDbHelper.UsuarioHuella.USUARIO_ID]);
            if (reader[ModelColumnDbHelper.UsuarioHuella.HUELLA] != DBNull.Value)
            {
                Huella = (byte[])reader[ModelColumnDbHelper.UsuarioHuella.HUELLA];
            }
            Posicion = Convert.ToInt32(reader[ModelColumnDbHelper.UsuarioHuella.POSICION]);

            CreatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.UsuarioHuella.CREATED_AT]);
            if (reader[ModelColumnDbHelper.UsuarioHuella.UPDATED_AT] != DBNull.Value)
            {
                UpdatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.UsuarioHuella.UPDATED_AT]);
            }
            if (reader[ModelColumnDbHelper.UsuarioHuella.DELETED_AT] != DBNull.Value)
            {
                DeletedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.UsuarioHuella.DELETED_AT]);
            }
        }

        public static ObservableCollection<UsuarioHuella> todas()
        {
            var resultados = new ObservableCollection<UsuarioHuella>();
            string sql = "SELECT uh.*, u.pv_acceso FROM usuarios_huellas AS uh " +
                        "JOIN usuarios AS u ON u.id = uh.usuario_id AND u.deleted_at IS NULL " +
                        "JOIN usuarios_sucursales AS us ON us.usuario_id = u.id " +
                        "AND us.sucursal_id = @SucursalId AND us.deleted_at IS NULL " +
                        "WHERE uh.deleted_at IS NULL " +
                        "AND uh.huella IS NOT NULL";
            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = sql;
                command.Parameters.Add(new SQLiteParameter("@SucursalId", TerminalStatic.SUCURSAL_ID));
                var result = command.ExecuteReader();
                while (result.Read())
                {
                    resultados.Add(new UsuarioHuella(result));
                }
                result.Close();
                dbConnection.Close();
            }
            return resultados;
        }

        public static ObservableCollection<UsuarioHuella> todas(string usuarioId)
        {
            var resultados = new ObservableCollection<UsuarioHuella>();
            string sql = "SELECT uh.* FROM usuarios_huellas AS uh " +
                        "JOIN usuarios AS u ON u.id = uh.usuario_id " +
                        "WHERE uh.deleted_at IS NULL AND u.deleted_at IS NULL " +
                        "AND uh.huella IS NOT NULL AND uh.usuario_id = @UsuarioId";
            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = sql;
                command.Parameters.Add(new SQLiteParameter("@UsuarioId", usuarioId));
                var result = command.ExecuteReader();
                while (result.Read())
                {
                    resultados.Add(new UsuarioHuella(result));
                }
                result.Close();
                dbConnection.Close();
            }
            return resultados;
        }

        public static UsuarioHuella? buscar(string usuarioId, int posicion)
        {
            UsuarioHuella? huella = null;
            string sql = "SELECT uh.* FROM usuarios_huellas AS uh " +
                        "JOIN usuarios AS u ON u.id = uh.usuario_id " +
                        "WHERE uh.deleted_at IS NULL AND u.deleted_at IS NULL " +
                        "AND uh.huella IS NOT NULL AND uh.usuario_id = @UsuarioId " +
                        "AND posicion = @Posicion";
            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = sql;
                command.Parameters.Add(new SQLiteParameter("@UsuarioId", usuarioId));
                command.Parameters.Add(new SQLiteParameter("@Posicion", $"{posicion}"));
                var result = command.ExecuteReader();
                while (result.Read())
                {
                    huella = new UsuarioHuella(result);
                    break;
                }
                result.Close();
                dbConnection.Close();
            }
            return huella;
        }

    }
}
