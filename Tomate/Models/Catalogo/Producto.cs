using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Globalization;
using Tomate.DBHelper;

namespace Tomate.Models.Catalogo
{
    public class Producto : Base
    {
        protected override string getTabla()
        {
            return ModelColumnDbHelper.Producto.DB_TABLA;
        }

        protected override bool getSoftDelete()
        {
            return true;
        }

        protected override Dictionary<string, object?> getColumnas()
        {
            var coumnas = new Dictionary<string, object?>();
            coumnas[ModelColumnDbHelper.Producto.ID] = Id;
            coumnas[ModelColumnDbHelper.Producto.CATEGORIA_ID] = CategoriaId;
            //coumnas[ModelColumnDbHelper.Producto.PRODUCTO_TIPO_ID] = ProductoTipoId;
            coumnas[ModelColumnDbHelper.Producto.NOMBRE] = Nombre;
            coumnas[ModelColumnDbHelper.Producto.PRECIO] = Precio;
            coumnas[ModelColumnDbHelper.Producto.IVA] = Iva;
            coumnas[ModelColumnDbHelper.Producto.ORDEN] = Orden;
            coumnas[ModelColumnDbHelper.Producto.ACTIVO] = Activo;
            coumnas[ModelColumnDbHelper.Producto.EXTRAS] = Extras;
            coumnas[ModelColumnDbHelper.Producto.CREATED_AT] = CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.Producto.UPDATED_AT] = UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.Producto.DELETED_AT] = DeletedAt?.ToString("yyyy-MM-dd HH:mm:ss");

            return coumnas;
        }

        protected override string llavePrimaria()
        {
            return ModelColumnDbHelper.Producto.ID;
        }

        public string? Id { get; set; }
        public string? CategoriaId { get; set; }
        //public string? ProductoTipoId { get; set; }
        public string? Nombre { get; set; }
        public string? NombrePos { 
            get
            {
                return $"{Nombre} {PosX} {PosY}";
            }
        }
        public float? Precio { get; set; }
        public float? Iva { get; set; }
        public int? Orden { get; set; }
        public bool? Activo { get; set; }
        public string? Extras { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        //opcionales si se trae de un tablero
        public string? TableroProductoId { get; set; }
        public int? PosX { get; set; }
        public int? PosY { get; set; }
        public bool TableroActivo { get; set; } = true;

        public string? Prohibido {
            get
            {
                return TableroActivo ? null : "Si";
            }
        }
        

        public string Opacity 
        { 
            get
            {
                return !TableroActivo ? "0.4" : "1";
            }
        }


        public string PrecioFormato
        {
            get
            {
                return $"{Precio?.ToString("C", CultureInfo.CurrentCulture)}";
            }
        }

        public Producto() { }

        public Producto(JObject data)
        {
            Id = (string?)data["id"];
            CategoriaId = (string?)data["categoria_id"];
            /*if ((string?)data["producto_tipo_id"] != null)
            {
                ProductoTipoId = (string?)data["producto_tipo_id"];
            }*/
            Nombre = (string?)data["nombre"];
            Precio = (float?)data["precio"];
            Iva = (float?)data["iva"];
            Orden = (int?)data["orden"];
            Activo = (int?)data["activo"] == 1;
            Extras = (string?)data["extras"];

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

        public Producto(SQLiteDataReader reader)
        {
            Id = Convert.ToString(reader[ModelColumnDbHelper.Producto.ID]);
            if (reader[ModelColumnDbHelper.Producto.CATEGORIA_ID] != DBNull.Value)
            {
                CategoriaId = Convert.ToString(reader[ModelColumnDbHelper.Producto.CATEGORIA_ID]);
            }
            /*if (reader[ModelColumnDbHelper.Producto.PRODUCTO_TIPO_ID] != DBNull.Value)
            {
                ProductoTipoId = Convert.ToString(reader[ModelColumnDbHelper.Producto.PRODUCTO_TIPO_ID]);
            }*/
            Nombre = Convert.ToString(reader[ModelColumnDbHelper.Producto.NOMBRE]);
            
            Iva = (float)Convert.ToDouble(reader[ModelColumnDbHelper.Producto.IVA]);
            try
            {
                if (reader[ModelColumnDbHelper.Producto.PRECIO] != DBNull.Value)
                {
                    Precio = (float)Convert.ToDouble(reader[ModelColumnDbHelper.Producto.PRECIO]);
                }
            }
            catch (Exception)
            {

            }

            if (reader[ModelColumnDbHelper.Producto.ORDEN] != DBNull.Value)
            {
                Orden = Convert.ToInt32(reader[ModelColumnDbHelper.Producto.ORDEN]);
            }
            try
            {
                Activo = Convert.ToInt32(reader[ModelColumnDbHelper.Producto.ACTIVO]) == 1;
            }
            catch (Exception)
            {

            }
            Extras = Convert.ToString(reader[ModelColumnDbHelper.Producto.EXTRAS]);
            try
            {
                if (reader["tablero_producto_id"] != DBNull.Value)
                {
                    TableroProductoId = Convert.ToString(reader["tablero_producto_id"]);
                }
                if (reader[ModelColumnDbHelper.TableroProducto.POS_X] != DBNull.Value)
                {
                    PosX = Convert.ToInt32(reader[ModelColumnDbHelper.TableroProducto.POS_X]);
                }
                if (reader[ModelColumnDbHelper.TableroProducto.POS_Y] != DBNull.Value)
                {
                    PosY = Convert.ToInt32(reader[ModelColumnDbHelper.TableroProducto.POS_Y]);
                }
                if (reader[ModelColumnDbHelper.TableroProducto.ACTIVO] != DBNull.Value)
                {
                    TableroActivo = Convert.ToInt32(reader[ModelColumnDbHelper.TableroProducto.ACTIVO]) == 1;
                }
            }
            catch (Exception)
            {

            }
           
            CreatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.Producto.CREATED_AT]);
            if (reader[ModelColumnDbHelper.Producto.UPDATED_AT] != DBNull.Value)
            {
                UpdatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.Producto.UPDATED_AT]);
            }
            if (reader[ModelColumnDbHelper.Producto.DELETED_AT] != DBNull.Value)
            {
                DeletedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.Producto.DELETED_AT]);
            }
        }

        public static ObservableCollection<Producto> todosExtras(string categoriaId, string? listaPreciosId)
        {
            var resultados = new ObservableCollection<Producto>();
            string sql = "SELECT p.id, p.nombre, p.iva, p.orden, p.activo, p.extras, "+
                        "p.created_at, p.updated_at, " +
                        "p.deleted_at, IFNULL(lpp.precio, p.precio) as precio, ce.categoria_id FROM productos as p " +
                        "JOIN categorias_extras AS ce ON ce.producto_id = p.id " +
                        "AND ce.categoria_id = @CategoriaId AND ce.deleted_at IS NULL " +
                        "LEFT JOIN listas_precios_productos AS lpp ON lpp.producto_id = p.id " +
                        "AND lpp.deleted_at IS NULL AND lpp.lista_precios_id = @ListaPreciosId " +
                        "WHERE p.deleted_at IS NULL AND p.activo = 1 " +
                        "ORDER BY p.orden ASC, p.nombre ASC";
            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = sql;
                command.Parameters.Add(new SQLiteParameter("@CategoriaId", categoriaId));
                command.Parameters.Add(new SQLiteParameter("@ListaPreciosId", listaPreciosId));
                var result = command.ExecuteReader();
                while (result.Read())
                {
                    resultados.Add(new Producto(result));
                }
                result.Close();
                dbConnection.Close();
            }
            return resultados;
        }

        public static ObservableCollection<Producto> todos(string categoriaId)
        {
            var resultados = new ObservableCollection<Producto>();
            string sql = "SELECT * FROM productos " +
                        "WHERE deleted_at IS NULL AND activo = 1 " +
                        "AND categoria_id = @CategoriaId " +
                        "ORDER BY orden ASC, nombre ASC";
            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = sql;
                command.Parameters.Add(new SQLiteParameter("@CategoriaId", categoriaId));
                var result = command.ExecuteReader();
                while (result.Read())
                {
                    resultados.Add(new Producto(result));
                }
                result.Close();
                dbConnection.Close();
            }
            return resultados;
        }

        public static Producto? buscarId(string id, bool deletedAt = false)
        {
            Producto? producto = null;
            string sql = "SELECT p.* FROM productos AS p " +
                        "WHERE p.id LIKE @Id";

            if (deletedAt)
            {
                sql += " AND p.deleted_at IS NULL";
            }

            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = sql;

                command.Parameters.Add(new SQLiteParameter("@Id", id));
                var result = command.ExecuteReader();
                while (result.Read())
                {
                    producto = new Producto(result);
                    break;
                }
                result.Close();
                dbConnection.Close();
            }
            return producto;
        }
    }
}
