
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Tomate.DBHelper;

namespace Tomate.Models.Catalogo
{
    public class CategoriaExtra : Base
    {
        protected override string getTabla()
        {
            return ModelColumnDbHelper.CategoriaExtra.DB_TABLA;
        }

        protected override bool getSoftDelete()
        {
            return true;
        }

        protected override Dictionary<string, object?> getColumnas()
        {
            var coumnas = new Dictionary<string, object?>();
            coumnas[ModelColumnDbHelper.CategoriaExtra.ID] = Id;
            coumnas[ModelColumnDbHelper.CategoriaExtra.CATEGORIA_ID] = CategoriaId;
            coumnas[ModelColumnDbHelper.CategoriaExtra.PRODUCTO_ID] = ProductoId;
            coumnas[ModelColumnDbHelper.CategoriaExtra.CREATED_AT] = CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.CategoriaExtra.UPDATED_AT] = UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.CategoriaExtra.DELETED_AT] = DeletedAt?.ToString("yyyy-MM-dd HH:mm:ss");

            return coumnas;
        }

        protected override string llavePrimaria()
        {
            return ModelColumnDbHelper.CategoriaExtra.ID;
        }

        public string? Id { get; set; }
        public string? CategoriaId { get; set; }
        public string? ProductoId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }


        public CategoriaExtra() { }

        public CategoriaExtra(JObject data)
        {
            Id = (string?)data["id"];
            CategoriaId = (string?)data["categoria_id"];
            ProductoId = (string?)data["producto_id"];

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

        public CategoriaExtra(SQLiteDataReader reader)
        {
            Id = Convert.ToString(reader[ModelColumnDbHelper.CategoriaExtra.ID]);
            CategoriaId = Convert.ToString(reader[ModelColumnDbHelper.CategoriaExtra.CATEGORIA_ID]);
            ProductoId = Convert.ToString(reader[ModelColumnDbHelper.CategoriaExtra.PRODUCTO_ID]);


            CreatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.CategoriaExtra.CREATED_AT]);
            if (reader[ModelColumnDbHelper.CategoriaExtra.UPDATED_AT] != DBNull.Value)
            {
                UpdatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.CategoriaExtra.UPDATED_AT]);
            }
            if (reader[ModelColumnDbHelper.CategoriaExtra.DELETED_AT] != DBNull.Value)
            {
                DeletedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.CategoriaExtra.DELETED_AT]);
            }
        }

    }
}
