using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using Tomate.DBHelper;

namespace Tomate.Models.Catalogo
{
    public class ListaPreciosProducto : Base
    {
        protected override string getTabla()
        {
            return ModelColumnDbHelper.ListaPreciosProducto.DB_TABLA;
        }

        protected override bool getSoftDelete()
        {
            return true;
        }

        protected override Dictionary<string, object?> getColumnas()
        {
            var coumnas = new Dictionary<string, object?>();
            coumnas[ModelColumnDbHelper.ListaPreciosProducto.ID] = Id;
            coumnas[ModelColumnDbHelper.ListaPreciosProducto.LISTA_PRECIOS_ID] = ListaPreciosId;
            coumnas[ModelColumnDbHelper.ListaPreciosProducto.PRODUCTO_ID] = ProductoId;
            coumnas[ModelColumnDbHelper.ListaPreciosProducto.PRECIO] = Precio;
            coumnas[ModelColumnDbHelper.ListaPreciosProducto.CREATED_AT] = CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.ListaPreciosProducto.UPDATED_AT] = UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.ListaPreciosProducto.DELETED_AT] = DeletedAt?.ToString("yyyy-MM-dd HH:mm:ss");

            return coumnas;
        }

        protected override string llavePrimaria()
        {
            return ModelColumnDbHelper.ListaPreciosProducto.ID;
        }

        public string? Id { get; set; }
        public string? ListaPreciosId { get; set; }
        public string? ProductoId { get; set; }
        public float? Precio { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        public string PrecioFormato
        {
            get
            {
                return $"{Precio?.ToString("C", CultureInfo.CurrentCulture)}";
            }
        }

        public ListaPreciosProducto() { }

        public ListaPreciosProducto(JObject data)
        {
            Id = (string?)data["id"];
            ListaPreciosId = (string?)data["lista_precios_id"];
            ProductoId = (string?)data["producto_id"];
            Precio = (float?)data["precio"];
            
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

        public ListaPreciosProducto(SQLiteDataReader reader)
        {
            Id = Convert.ToString(reader[ModelColumnDbHelper.ListaPreciosProducto.ID]);
            ListaPreciosId = Convert.ToString(reader[ModelColumnDbHelper.ListaPreciosProducto.LISTA_PRECIOS_ID]);
            ProductoId = Convert.ToString(reader[ModelColumnDbHelper.ListaPreciosProducto.PRODUCTO_ID]);
            Precio = (float)Convert.ToDouble(reader[ModelColumnDbHelper.ListaPreciosProducto.PRECIO]);
            
            CreatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.ListaPreciosProducto.CREATED_AT]);
            if (reader[ModelColumnDbHelper.ListaPreciosProducto.UPDATED_AT] != DBNull.Value)
            {
                UpdatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.ListaPreciosProducto.UPDATED_AT]);
            }
            if (reader[ModelColumnDbHelper.ListaPreciosProducto.DELETED_AT] != DBNull.Value)
            {
                DeletedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.ListaPreciosProducto.DELETED_AT]);
            }
        }
    }
}
