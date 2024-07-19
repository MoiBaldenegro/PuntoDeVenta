using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Tomate.DBHelper;

namespace Tomate.Models.Usuarios
{
    public class PerfilPermiso : Base
    {

        protected override string getTabla()
        {
            return ModelColumnDbHelper.PerfilPermiso.DB_TABLA;
        }

        protected override bool getSoftDelete()
        {
            return true;
        }

        protected override Dictionary<string, object?> getColumnas()
        {
            var coumnas = new Dictionary<string, object?>();
            coumnas[ModelColumnDbHelper.PerfilPermiso.ID] = Id;
            coumnas[ModelColumnDbHelper.PerfilPermiso.PERFIL_ID] = PerfilId;
            coumnas[ModelColumnDbHelper.PerfilPermiso.PERMISO] = Permiso;
            coumnas[ModelColumnDbHelper.PerfilPermiso.CREATED_AT] = CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.PerfilPermiso.UPDATED_AT] = UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.PerfilPermiso.DELETED_AT] = DeletedAt?.ToString("yyyy-MM-dd HH:mm:ss");

            return coumnas;
        }

        protected override string llavePrimaria()
        {
            return ModelColumnDbHelper.PerfilPermiso.ID;
        }

        public string? Id { get; set; }
        public string? PerfilId { get; set; }
        public string? Permiso { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public PerfilPermiso() { }

        public PerfilPermiso(JObject data)
        {

            Id = (string?)data["id"];
            PerfilId = (string?)data["perfil_id"];
            Permiso = (string?)data["permiso"];
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

        public PerfilPermiso(SQLiteDataReader reader)
        {
            Id = Convert.ToString(reader[ModelColumnDbHelper.PerfilPermiso.ID]);
            PerfilId = Convert.ToString(reader[ModelColumnDbHelper.PerfilPermiso.PERFIL_ID]);
            Permiso = Convert.ToString(reader[ModelColumnDbHelper.PerfilPermiso.PERMISO]);
            CreatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.PerfilPermiso.CREATED_AT]);
            if (reader[ModelColumnDbHelper.PerfilPermiso.UPDATED_AT] != DBNull.Value)
            {
                UpdatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.PerfilPermiso.UPDATED_AT]);
            }
            if (reader[ModelColumnDbHelper.PerfilPermiso.DELETED_AT] != DBNull.Value)
            {
                DeletedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.PerfilPermiso.DELETED_AT]);
            }
        }

        public static List<PerfilPermiso> todos()
        {
            var resultados = new List<PerfilPermiso>();
            string sql = "SELECT * FROM perfiles_permisos " +
                        "WHERE deleted_at IS NULL";
            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = sql;
                var result = command.ExecuteReader();
                while (result.Read())
                {
                    resultados.Add(new PerfilPermiso(result));
                }
                result.Close();
                dbConnection.Close();
            }
            return resultados;
        }

    }
}
