using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Tomate.DBHelper;

namespace Tomate.Models.Catalogo
{
    public class ListaPrecios : Base
    {
        protected override string getTabla()
        {
            return ModelColumnDbHelper.ListaPrecios.DB_TABLA;
        }

        protected override bool getSoftDelete()
        {
            return true;
        }

        protected override Dictionary<string, object?> getColumnas()
        {
            var coumnas = new Dictionary<string, object?>();
            coumnas[ModelColumnDbHelper.ListaPrecios.ID] = Id;
            coumnas[ModelColumnDbHelper.ListaPrecios.NOMBRE] = Nombre;
            coumnas[ModelColumnDbHelper.ListaPrecios.CREATED_AT] = CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.ListaPrecios.UPDATED_AT] = UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.ListaPrecios.DELETED_AT] = DeletedAt?.ToString("yyyy-MM-dd HH:mm:ss");

            return coumnas;
        }

        protected override string llavePrimaria()
        {
            return ModelColumnDbHelper.ListaPrecios.ID;
        }

        public string? Id { get; set; }
        public string? Nombre { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ListaPrecios() { }

        public ListaPrecios(JObject data)
        {
            Id = (string?)data["id"];
            Nombre = (string?)data["nombre"];
          
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

        public ListaPrecios(SQLiteDataReader reader)
        {
            Id = Convert.ToString(reader[ModelColumnDbHelper.ListaPrecios.ID]);
            Nombre = Convert.ToString(reader[ModelColumnDbHelper.ListaPrecios.NOMBRE]);
            
            CreatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.ListaPrecios.CREATED_AT]);
            if (reader[ModelColumnDbHelper.ListaPrecios.UPDATED_AT] != DBNull.Value)
            {
                UpdatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.ListaPrecios.UPDATED_AT]);
            }
            if (reader[ModelColumnDbHelper.ListaPrecios.DELETED_AT] != DBNull.Value)
            {
                DeletedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.ListaPrecios.DELETED_AT]);
            }
        }
    }
}
