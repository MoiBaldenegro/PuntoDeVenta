
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using Tomate.DBHelper;

namespace Tomate.Models.Catalogo
{
    public class TableroProducto : Base
    {
        protected override string getTabla()
        {
            return ModelColumnDbHelper.TableroProducto.DB_TABLA;
        }

        protected override bool getSoftDelete()
        {
            return true;
        }

        protected override Dictionary<string, object?> getColumnas()
        {
            var coumnas = new Dictionary<string, object?>();
            coumnas[ModelColumnDbHelper.TableroProducto.ID] = Id;
            coumnas[ModelColumnDbHelper.TableroProducto.TABLERO_ID] = TableroId;
            coumnas[ModelColumnDbHelper.TableroProducto.PRODUCTO_ID] = ProductoId;
            coumnas[ModelColumnDbHelper.TableroProducto.POS_X] = PosX;
            coumnas[ModelColumnDbHelper.TableroProducto.POS_Y] = PosY;
            coumnas[ModelColumnDbHelper.TableroProducto.ACTIVO] = Activo;
            coumnas[ModelColumnDbHelper.TableroProducto.CREATED_AT] = CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.TableroProducto.UPDATED_AT] = UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.TableroProducto.DELETED_AT] = DeletedAt?.ToString("yyyy-MM-dd HH:mm:ss");

            return coumnas;
        }

        protected override string llavePrimaria()
        {
            return ModelColumnDbHelper.Tablero.ID;
        }

        public string? Id { get; set; }

        public string? Nombre { get; set; }
        public string? TableroId { get; set; }
        public string? ProductoId { get; set; }
        public int? PosX { get; set; }
        public int? PosY { get; set; }

        public bool Activo { get; set; } = true;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }


        public TableroProducto() { }

        public TableroProducto(JObject data)
        {
            Id = (string?)data["id"];
            TableroId = (string?)data["tablero_id"];
            ProductoId = (string?)data["producto_id"];
            PosX = (int?)data["pos_x"];
            PosY = (int?)data["pos_y"];
            Activo = (int?)data["activo"] == 1 || (int?)data["activo"] == null;

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

        public TableroProducto(SQLiteDataReader reader)
        {
            Id = Convert.ToString(reader[ModelColumnDbHelper.TableroProducto.ID]);
            TableroId = Convert.ToString(reader[ModelColumnDbHelper.TableroProducto.TABLERO_ID]);
            ProductoId = Convert.ToString(reader[ModelColumnDbHelper.TableroProducto.PRODUCTO_ID]);
            PosX = Convert.ToInt32(reader[ModelColumnDbHelper.TableroProducto.POS_X]);
            PosY = Convert.ToInt32(reader[ModelColumnDbHelper.TableroProducto.POS_Y]);
            Activo = Convert.ToInt32(reader[ModelColumnDbHelper.TableroProducto.ACTIVO]) == 1;

            CreatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.TableroProducto.CREATED_AT]);

            if (reader[ModelColumnDbHelper.Producto.NOMBRE] != DBNull.Value)
            {
                Nombre = Convert.ToString(reader[ModelColumnDbHelper.Producto.NOMBRE]);
            }

            if (reader[ModelColumnDbHelper.TableroProducto.UPDATED_AT] != DBNull.Value)
            {
                UpdatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.TableroProducto.UPDATED_AT]);
            }
            if (reader[ModelColumnDbHelper.TableroProducto.DELETED_AT] != DBNull.Value)
            {
                DeletedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.TableroProducto.DELETED_AT]);
            }
        }

        public static ObservableCollection<Producto> todos(string tableroId, string? listaPreciosId)
        {
            var resultados = new ObservableCollection<Producto>();
            string sql = "SELECT p.id, p.categoria_id, p.nombre, p.iva, p.orden, p.extras, "+
                        "p.created_at, p.updated_at, " +
                        "p.deleted_at, tp.id as tablero_producto_id, " + 
                        "IFNULL(lpp.precio, p.precio) as precio, tp.pos_x, tp.pos_y, tp.activo FROM productos AS p " +
                        "JOIN tableros_productos AS tp ON tp.producto_id = p.id " +
                        "AND tp.deleted_at IS NULL " +
                        "LEFT JOIN listas_precios_productos AS lpp ON lpp.producto_id = p.id " +
                        "AND lpp.deleted_at IS NULL AND lpp.lista_precios_id = @ListaPreciosId " +
                        "WHERE p.deleted_at IS NULL AND p.activo = 1 " +
                        "AND tp.tablero_id = @TableroProductoId " +
                        "ORDER BY tp.pos_x ASC, tp.pos_y ASC";
            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = sql;
                command.Parameters.Add(new SQLiteParameter("@TableroProductoId", tableroId));
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

        public static ObservableCollection<Producto> todos(string tableroId)
        {
            var resultados = new ObservableCollection<Producto>();

            string sql = "SELECT p.id, p.categoria_id, p.nombre, p.iva, p.orden, p.extras, " +
                        "p.created_at, p.updated_at, " +
                        "p.deleted_at, tp.id as tablero_producto_id, " + 
                        "tp.pos_x, tp.pos_y, tp.activo FROM productos AS p " +
                        "JOIN tableros_productos AS tp ON tp.producto_id = p.id " +
                        "AND tp.deleted_at IS NULL " +
                        "WHERE p.deleted_at IS NULL AND p.activo = 1 " +
                        "AND tp.tablero_id = @TableroProductoId " +
                        "ORDER BY tp.pos_x ASC, tp.pos_y ASC";
            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = sql;
                command.Parameters.Add(new SQLiteParameter("@TableroProductoId", tableroId));
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

        public static ObservableCollection<Producto> todos()
        {
            var resultados = new ObservableCollection<Producto>();

            string sql = "SELECT p.id, p.categoria_id, p.nombre, p.iva, p.orden, p.extras, " +
                        "p.created_at, p.updated_at, " +
                        "p.deleted_at, tp.id as tablero_producto_id, " +
                        "tp.pos_x, tp.pos_y, tp.activo FROM productos AS p " +
                        "JOIN tableros_productos AS tp ON tp.producto_id = p.id " +
                        "AND tp.deleted_at IS NULL " +
                        "WHERE p.deleted_at IS NULL AND p.activo = 1 " +
                        "ORDER BY tp.pos_x ASC, tp.pos_y ASC";
            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = sql;
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

        public static Producto? buscarId(string tableroProductoId)
        {
            Producto? producto = null;
            string sql = "SELECT p.id, p.categoria_id, p.nombre, p.iva, p.orden, p.extras, " +
                       "p.created_at, p.updated_at, " +
                       "p.deleted_at, tp.id as tablero_producto_id, " +
                       "tp.pos_x, tp.pos_y, tp.activo FROM productos AS p " +
                       "JOIN tableros_productos AS tp ON tp.producto_id = p.id " +
                       "AND tp.deleted_at IS NULL " +
                       "WHERE p.deleted_at IS NULL AND p.activo = 1 " +
                       "AND tp.id = @TableroProductoId " +
                       "ORDER BY tp.pos_x ASC, tp.pos_y ASC";

            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = sql;

                command.Parameters.Add(new SQLiteParameter("@TableroProductoId", tableroProductoId));
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


        public static Producto? buscarProductoId(string productoId)
        {
            Producto? producto = null;
            string sql = "SELECT p.id, p.categoria_id, p.nombre, p.iva, p.orden, p.extras, " +
                       "p.created_at, p.updated_at, " +
                       "p.deleted_at, tp.id as tablero_producto_id, " +
                       "tp.pos_x, tp.pos_y, tp.activo FROM productos AS p " +
                       "JOIN tableros_productos AS tp ON tp.producto_id = p.id " +
                       "AND tp.deleted_at IS NULL " +
                       "WHERE p.deleted_at IS NULL AND p.activo = 1 " +
                       "AND p.id = @ProductoId " +
                       "ORDER BY tp.pos_x ASC, tp.pos_y ASC";

            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = sql;

                command.Parameters.Add(new SQLiteParameter("@ProductoId", productoId));
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


        public static ObservableCollection<Producto> desactivados()
        {
            var resultados = new ObservableCollection<Producto>();

            string sql = "SELECT p.id, p.categoria_id, p.nombre, p.iva, p.orden, p.extras, " +
                        "p.created_at, p.updated_at, " +
                        "p.deleted_at, tp.id as tablero_producto_id, "+
                        "tp.pos_x, tp.pos_y, tp.activo FROM productos AS p " +
                        "JOIN tableros_productos AS tp ON tp.producto_id = p.id " +
                        "AND tp.deleted_at IS NULL " +
                        "WHERE p.deleted_at IS NULL AND p.activo = 1 " +
                        "AND tp.activo = 0 " +
                        "ORDER BY tp.pos_x ASC, tp.pos_y ASC";
            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = sql;
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

        public static void actualizarActivo(string tableroProductoId, bool activo)
        {
            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                string sqlQuery = $"UPDATE tableros_productos SET activo = @Activo WHERE id = @Id";
                var command = dbConnection.CreateCommand();
                command.CommandText = sqlQuery;
                command.Parameters.Add(new SQLiteParameter("@Activo", activo));
                command.Parameters.Add(new SQLiteParameter("@id", tableroProductoId));
                command.ExecuteNonQuery();
                dbConnection.Close();
            }
        }
    }
}
