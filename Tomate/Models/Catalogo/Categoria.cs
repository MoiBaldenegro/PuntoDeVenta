
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using Tomate.DBHelper;

namespace Tomate.Models.Catalogo
{
    public class Categoria : Base
    {
        protected override string getTabla()
        {
            return ModelColumnDbHelper.Categoria.DB_TABLA;
        }

        protected override bool getSoftDelete()
        {
            return true;
        }

        protected override Dictionary<string, object?> getColumnas()
        {
            var coumnas = new Dictionary<string, object?>();
            coumnas[ModelColumnDbHelper.Categoria.ID] = Id;
            coumnas[ModelColumnDbHelper.Categoria.CATEGORIA_PADRE_ID] = CategoriaPadreId;
            coumnas[ModelColumnDbHelper.Categoria.NIVEL] = Nivel;
            coumnas[ModelColumnDbHelper.Categoria.NOMBRE] = Nombre;
            coumnas[ModelColumnDbHelper.Categoria.ORDEN] = Orden;
            coumnas[ModelColumnDbHelper.Categoria.ACTIVO] = Activo;
            coumnas[ModelColumnDbHelper.Categoria.CREATED_AT] = CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.Categoria.UPDATED_AT] = UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.Categoria.DELETED_AT] = DeletedAt?.ToString("yyyy-MM-dd HH:mm:ss");

            return coumnas;
        }

        protected override string llavePrimaria()
        {
            return ModelColumnDbHelper.Categoria.ID;
        }

        public string? Id { get; set; }
        public string? CategoriaPadreId { get; set; }
        public int? Nivel { get; set; }
        public string? Nombre { get; set; }
        public int? Orden { get; set; }
        public bool? Activo { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }


        public Categoria() { }

        public Categoria(JObject data)
        {
            Id = (string?)data["id"];
            CategoriaPadreId = (string?)data["categoria_padre_id"];
            Nivel = (int?)data["nivel"];
            Nombre = (string?)data["nombre"];
            Orden = (int?)data["orden"];
            Activo = (int?)data["activo"] == 1;

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

        public Categoria(SQLiteDataReader reader)
        {
            Id = Convert.ToString(reader[ModelColumnDbHelper.Categoria.ID]);
            CategoriaPadreId = Convert.ToString(reader[ModelColumnDbHelper.Categoria.CATEGORIA_PADRE_ID]);
            Nivel = Convert.ToInt32(reader[ModelColumnDbHelper.Categoria.NIVEL]);
            Nombre = Convert.ToString(reader[ModelColumnDbHelper.Categoria.NOMBRE]);
            Orden = Convert.ToInt32(reader[ModelColumnDbHelper.Categoria.ORDEN]);
            Activo = Convert.ToInt32(reader[ModelColumnDbHelper.Categoria.ACTIVO]) == 1;

            CreatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.Categoria.CREATED_AT]);
            if (reader[ModelColumnDbHelper.Categoria.UPDATED_AT] != DBNull.Value)
            {
                UpdatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.Categoria.UPDATED_AT]);
            }
            if (reader[ModelColumnDbHelper.Categoria.DELETED_AT] != DBNull.Value)
            {
                DeletedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.Categoria.DELETED_AT]);
            }
        }

        public static ObservableCollection<Categoria> todas()
        {
            var resultados = new ObservableCollection<Categoria>();
            string sql = "SELECT * FROM categorias " +
                        "WHERE deleted_at IS NULL AND activo = 1 AND nivel = 3 " +
                        "ORDER BY orden ASC";
            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = sql;
                var result = command.ExecuteReader();
                while (result.Read())
                {
                    resultados.Add(new Categoria(result));
                }
                result.Close();
                dbConnection.Close();
            }
            return resultados;
        }
    }
}
