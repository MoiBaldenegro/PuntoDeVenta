using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Tomate.DBHelper;

namespace Tomate.Models.Usuarios
{
    public class Perfil : Base
    {

        protected override string getTabla()
        {
            return ModelColumnDbHelper.Perfil.DB_TABLA;
        }

        protected override bool getSoftDelete()
        {
            return true;
        }

        protected override Dictionary<string, object?> getColumnas()
        {
            var coumnas = new Dictionary<string, object?>();
            coumnas[ModelColumnDbHelper.Perfil.ID] = Id;
            coumnas[ModelColumnDbHelper.Perfil.NOMBRE] = Nombre;
            coumnas[ModelColumnDbHelper.Perfil.VISIBLE] = Visible;
            coumnas[ModelColumnDbHelper.Perfil.CREATED_AT] = CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.Perfil.UPDATED_AT] = UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.Perfil.DELETED_AT] = DeletedAt?.ToString("yyyy-MM-dd HH:mm:ss");

            return coumnas;
        }

        protected override string llavePrimaria()
        {
            return ModelColumnDbHelper.Perfil.ID;
        }

        public string? Id { get; set; }
        public string? Nombre { get; set; }
        public bool? Visible { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public Perfil() { }

        public Perfil(JObject data)
        {

            Id = (string?)data["id"];
            Nombre = (string?)data["nombre"];
            Visible = (int?)data["visible"] == 1;
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

        public Perfil(SQLiteDataReader reader)
        {
            Id = Convert.ToString(reader[ModelColumnDbHelper.Perfil.ID]);
            Nombre = Convert.ToString(reader[ModelColumnDbHelper.Perfil.NOMBRE]);
            Visible = Convert.ToInt32(reader[ModelColumnDbHelper.Perfil.VISIBLE]) == 1;
            CreatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.Perfil.CREATED_AT]);
            if (reader[ModelColumnDbHelper.Perfil.UPDATED_AT] != DBNull.Value)
            {
                UpdatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.Perfil.UPDATED_AT]);
            }
            if (reader[ModelColumnDbHelper.Perfil.DELETED_AT] != DBNull.Value)
            {
                DeletedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.Perfil.DELETED_AT]);
            }
        }

        public static List<Perfil> todos()
        {
            var resultados = new List<Perfil>();
            string sql = "SELECT * FROM perfiles " +
                        "WHERE deleted_at IS NULL AND visible = 1 " +
                        "ORDER BY nombre ASC";
            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = sql;
                var result = command.ExecuteReader();
                while (result.Read())
                {
                    resultados.Add(new Perfil(result));
                }
                result.Close();
                dbConnection.Close();
            }
            return resultados;
        }
    }
}
