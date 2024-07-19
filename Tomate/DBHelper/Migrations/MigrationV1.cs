using System.Data.SQLite;
using Tomate.Models;

namespace Tomate.DBHelper.Migrations
{
    public class MigrationV1
    {
        public static void Execute(SQLiteConnection dbConnection)
        {
            string configuracionesTabla = "CREATE TABLE IF NOT EXISTS " + ModelColumnDbHelper.Configuracion.DB_TABLA + "("
                + ModelColumnDbHelper.Configuracion.KEY + " TEXT PRIMARY KEY,"
                + ModelColumnDbHelper.Configuracion.VALOR + " TEXT,"
                + ModelColumnDbHelper.Configuracion.UPDATED_AT + " TEXT)";
            var command = dbConnection.CreateCommand();
            command.CommandText = configuracionesTabla;
            command.ExecuteNonQuery();

            string perfilesTabla = "CREATE TABLE IF NOT EXISTS " + ModelColumnDbHelper.Perfil.DB_TABLA + "("
               + ModelColumnDbHelper.Perfil.ID + " TEXT PRIMARY KEY,"
               + ModelColumnDbHelper.Perfil.NOMBRE + " TEXT,"
               + ModelColumnDbHelper.Perfil.VISIBLE + " TINYINT DEFAULT 1,"
               + ModelColumnDbHelper.Perfil.CREATED_AT + " TEXT,"
               + ModelColumnDbHelper.Perfil.UPDATED_AT + " TEXT,"
               + ModelColumnDbHelper.Perfil.DELETED_AT + " TEXT)";
            command = dbConnection.CreateCommand();
            command.CommandText = perfilesTabla;
            command.ExecuteNonQuery();

            string perfilesPermisosTabla = "CREATE TABLE IF NOT EXISTS " + ModelColumnDbHelper.PerfilPermiso.DB_TABLA + "("
               + ModelColumnDbHelper.PerfilPermiso.ID + " TEXT PRIMARY KEY,"
               + ModelColumnDbHelper.PerfilPermiso.PERFIL_ID + " TEXT,"
               + ModelColumnDbHelper.PerfilPermiso.PERMISO + " TEXT,"
               + ModelColumnDbHelper.PerfilPermiso.CREATED_AT + " TEXT,"
               + ModelColumnDbHelper.PerfilPermiso.UPDATED_AT + " TEXT,"
               + ModelColumnDbHelper.PerfilPermiso.DELETED_AT + " TEXT)";
            command = dbConnection.CreateCommand();
            command.CommandText = perfilesPermisosTabla;
            command.ExecuteNonQuery();

            string usuariosTabla = "CREATE TABLE IF NOT EXISTS " + ModelColumnDbHelper.Usuario.DB_TABLA + "("
                + ModelColumnDbHelper.Usuario.ID + " TEXT PRIMARY KEY,"
                + ModelColumnDbHelper.Usuario.PERFIL_ID + " TEXT,"
                + ModelColumnDbHelper.Usuario.CODIGO + " TEXT,"
                + ModelColumnDbHelper.Usuario.PASSWORD + " TEXT,"
                + ModelColumnDbHelper.Usuario.NOMBRE + " TEXT,"
                + ModelColumnDbHelper.Usuario.ALIAS + " TEXT,"
                + ModelColumnDbHelper.Usuario.AUSENTE + " TINYINT DEFAULT 0,"
                + ModelColumnDbHelper.Usuario.PV_ACCESO + " TINYINT DEFAULT 0,"
                + ModelColumnDbHelper.Usuario.PV_ATIENDE_MESAS + " TINYINT DEFAULT 0,"
                + ModelColumnDbHelper.Usuario.ACTIVO + " TINYINT DEFAULT 1,"
                + ModelColumnDbHelper.Usuario.CREATED_AT + " TEXT,"
                + ModelColumnDbHelper.Usuario.UPDATED_AT + " TEXT,"
                + ModelColumnDbHelper.Usuario.DELETED_AT + " TEXT)";
            command = dbConnection.CreateCommand();
            command.CommandText = usuariosTabla;
            command.ExecuteNonQuery();

            string usuariosHuellasTabla = "CREATE TABLE IF NOT EXISTS " + ModelColumnDbHelper.UsuarioHuella.DB_TABLA + "("
                    + ModelColumnDbHelper.UsuarioHuella.ID + " TEXT PRIMARY KEY,"
                    + ModelColumnDbHelper.UsuarioHuella.USUARIO_ID + " TEXT,"
                    + ModelColumnDbHelper.UsuarioHuella.HUELLA + " BLOB,"
                    + ModelColumnDbHelper.UsuarioHuella.POSICION + " INTEGER,"
                    + ModelColumnDbHelper.UsuarioHuella.CREATED_AT + " TEXT,"
                    + ModelColumnDbHelper.UsuarioHuella.UPDATED_AT + " TEXT,"
                    + ModelColumnDbHelper.UsuarioHuella.DELETED_AT + " TEXT)";
            command = dbConnection.CreateCommand();
            command.CommandText = usuariosHuellasTabla;
            command.ExecuteNonQuery();

            string usuariosSucursalesTabla = "CREATE TABLE IF NOT EXISTS " + ModelColumnDbHelper.UsuarioSucursal.DB_TABLA + "("
                    + ModelColumnDbHelper.UsuarioSucursal.ID + " TEXT PRIMARY KEY,"
                    + ModelColumnDbHelper.UsuarioSucursal.USUARIO_ID + " TEXT,"
                    + ModelColumnDbHelper.UsuarioSucursal.SUCURSAL_ID + " TEXT,"
                    + ModelColumnDbHelper.UsuarioSucursal.CREATED_AT + " TEXT,"
                    + ModelColumnDbHelper.UsuarioSucursal.UPDATED_AT + " TEXT,"
                    + ModelColumnDbHelper.UsuarioSucursal.DELETED_AT + " TEXT)";
            command = dbConnection.CreateCommand();
            command.CommandText = usuariosSucursalesTabla;
            command.ExecuteNonQuery();

            string categoriasTabla = "CREATE TABLE IF NOT EXISTS " + ModelColumnDbHelper.Categoria.DB_TABLA + "("
                    + ModelColumnDbHelper.Categoria.ID + " TEXT PRIMARY KEY,"
                    + ModelColumnDbHelper.Categoria.CATEGORIA_PADRE_ID + ","
                    + ModelColumnDbHelper.Categoria.NIVEL + " INTEGER,"
                    + ModelColumnDbHelper.Categoria.NOMBRE + " TEXT,"
                    + ModelColumnDbHelper.Categoria.ORDEN + " INTEGER,"
                    + ModelColumnDbHelper.Categoria.ACTIVO + " TINYINT DEFAULT 1,"
                    + ModelColumnDbHelper.Categoria.CREATED_AT + " TEXT,"
                    + ModelColumnDbHelper.Categoria.UPDATED_AT + " TEXT,"
                    + ModelColumnDbHelper.Categoria.DELETED_AT + " TEXT)";
            command = dbConnection.CreateCommand();
            command.CommandText = categoriasTabla;
            command.ExecuteNonQuery();

            string productosTabla = "CREATE TABLE IF NOT EXISTS " + ModelColumnDbHelper.Producto.DB_TABLA + "("
                    + ModelColumnDbHelper.Producto.ID + " TEXT PRIMARY KEY,"
                    + ModelColumnDbHelper.Producto.CATEGORIA_ID + " TEXT,"
                    //+ ModelColumnDbHelper.Producto.PRODUCTO_TIPO_ID + " TEXT,"
                    + ModelColumnDbHelper.Producto.NOMBRE + " TEXT,"
                    + ModelColumnDbHelper.Producto.PRECIO + " REAL,"
                    + ModelColumnDbHelper.Producto.IVA + " REAL,"
                    + ModelColumnDbHelper.Producto.ORDEN + " INTEGER,"
                    + ModelColumnDbHelper.Producto.ACTIVO + " TINYINT DEFAULT 1,"
                    + ModelColumnDbHelper.Producto.EXTRAS + " TEXT,"
                    + ModelColumnDbHelper.Producto.CREATED_AT + " TEXT,"
                    + ModelColumnDbHelper.Producto.UPDATED_AT + " TEXT,"
                    + ModelColumnDbHelper.Producto.DELETED_AT + " TEXT)";
            command = dbConnection.CreateCommand();
            command.CommandText = productosTabla;
            command.ExecuteNonQuery();

            string listasPreciosTabla = "CREATE TABLE IF NOT EXISTS " + ModelColumnDbHelper.ListaPrecios.DB_TABLA + "("
                    + ModelColumnDbHelper.ListaPrecios.ID + " TEXT PRIMARY KEY,"
                    + ModelColumnDbHelper.ListaPrecios.NOMBRE + " TEXT,"
                    + ModelColumnDbHelper.ListaPreciosProducto.CREATED_AT + " TEXT,"
                    + ModelColumnDbHelper.ListaPreciosProducto.UPDATED_AT + " TEXT,"
                    + ModelColumnDbHelper.ListaPreciosProducto.DELETED_AT + " TEXT)";
            command = dbConnection.CreateCommand();
            command.CommandText = listasPreciosTabla;
            command.ExecuteNonQuery();

            string listaPreciosProductosTabla = "CREATE TABLE IF NOT EXISTS " + ModelColumnDbHelper.ListaPreciosProducto.DB_TABLA + "("
                    + ModelColumnDbHelper.ListaPreciosProducto.ID + " TEXT PRIMARY KEY,"
                    + ModelColumnDbHelper.ListaPreciosProducto.LISTA_PRECIOS_ID + " TEXT,"
                    + ModelColumnDbHelper.ListaPreciosProducto.PRODUCTO_ID + " TEXT,"
                    + ModelColumnDbHelper.ListaPreciosProducto.PRECIO + " REAL,"
                    + ModelColumnDbHelper.ListaPreciosProducto.CREATED_AT + " TEXT,"
                    + ModelColumnDbHelper.ListaPreciosProducto.UPDATED_AT + " TEXT,"
                    + ModelColumnDbHelper.ListaPreciosProducto.DELETED_AT + " TEXT)";
            command = dbConnection.CreateCommand();
            command.CommandText = listaPreciosProductosTabla;
            command.ExecuteNonQuery();

            string categoriasExtrasTabla = "CREATE TABLE IF NOT EXISTS " + ModelColumnDbHelper.CategoriaExtra.DB_TABLA + "("
                    + ModelColumnDbHelper.CategoriaExtra.ID + " TEXT PRIMARY KEY,"
                    + ModelColumnDbHelper.CategoriaExtra.CATEGORIA_ID + " TEXT,"
                    + ModelColumnDbHelper.CategoriaExtra.PRODUCTO_ID + " TEXT,"
                    + ModelColumnDbHelper.CategoriaExtra.CREATED_AT + " TEXT,"
                    + ModelColumnDbHelper.CategoriaExtra.UPDATED_AT + " TEXT,"
                    + ModelColumnDbHelper.CategoriaExtra.DELETED_AT + " TEXT)";
            command = dbConnection.CreateCommand();
            command.CommandText = categoriasExtrasTabla;
            command.ExecuteNonQuery();

            string modificadoresTabla = "CREATE TABLE IF NOT EXISTS " + ModelColumnDbHelper.Modificador.DB_TABLA + "("
                    + ModelColumnDbHelper.Modificador.ID + " TEXT PRIMARY KEY,"
                    + ModelColumnDbHelper.Modificador.CATEGORIA_ID + " TEXT,"
                    + ModelColumnDbHelper.Modificador.NOMBRE + " TEXT,"
                    + ModelColumnDbHelper.Modificador.ORDEN + " INTEGER,"
                    + ModelColumnDbHelper.Modificador.CREATED_AT + " TEXT,"
                    + ModelColumnDbHelper.Modificador.UPDATED_AT + " TEXT,"
                    + ModelColumnDbHelper.Modificador.DELETED_AT + " TEXT)";
            command = dbConnection.CreateCommand();
            command.CommandText = modificadoresTabla;
            command.ExecuteNonQuery();

            string ventasTiposTabla = "CREATE TABLE IF NOT EXISTS " + ModelColumnDbHelper.VentaTipo.DB_TABLA + "("
                    + ModelColumnDbHelper.VentaTipo.ID + " TEXT PRIMARY KEY,"
                    + ModelColumnDbHelper.VentaTipo.NOMBRE + " TEXT,"
                    + ModelColumnDbHelper.VentaTipo.PREDETERMINADO + " TINYINT DEFAULT 0,"
                    + ModelColumnDbHelper.VentaTipo.ORDEN + " INTEGER,"
                    + ModelColumnDbHelper.VentaTipo.ACTIVO + " TINYINT DEFAULT 1,"
                    + ModelColumnDbHelper.VentaTipo.CREATED_AT + " TEXT,"
                    + ModelColumnDbHelper.VentaTipo.UPDATED_AT + " TEXT,"
                    + ModelColumnDbHelper.VentaTipo.DELETED_AT + " TEXT)";
            command = dbConnection.CreateCommand();
            command.CommandText = ventasTiposTabla;
            command.ExecuteNonQuery();

            string motivosCancelacionTabla = "CREATE TABLE IF NOT EXISTS " + ModelColumnDbHelper.MotivoCancelacion.DB_TABLA + "("
                    + ModelColumnDbHelper.MotivoCancelacion.ID + " TEXT PRIMARY KEY,"
                    + ModelColumnDbHelper.MotivoCancelacion.NOMBRE + " TEXT,"
                    + ModelColumnDbHelper.MotivoCancelacion.ORDEN + " TINYINT,"
                    + ModelColumnDbHelper.MotivoCancelacion.CREATED_AT + " TEXT,"
                    + ModelColumnDbHelper.MotivoCancelacion.UPDATED_AT + " TEXT,"
                    + ModelColumnDbHelper.MotivoCancelacion.DELETED_AT + " TEXT)";
            command = dbConnection.CreateCommand();
            command.CommandText = motivosCancelacionTabla;
            command.ExecuteNonQuery();

            string mesasTabla = "CREATE TABLE IF NOT EXISTS " + ModelColumnDbHelper.Mesa.DB_TABLA + "("
                    + ModelColumnDbHelper.Mesa.ID + " TEXT PRIMARY KEY,"
                    + ModelColumnDbHelper.Mesa.USUARIO_ID + " TEXT,"
                    + ModelColumnDbHelper.Mesa.NUMERO_MESA + " TEXT,"
                    + ModelColumnDbHelper.Mesa.ORDEN + " INTEGER,"
                    + ModelColumnDbHelper.Mesa.CREATED_AT + " TEXT,"
                    + ModelColumnDbHelper.Mesa.UPDATED_AT + " TEXT,"
                    + ModelColumnDbHelper.Mesa.DELETED_AT + " TEXT)";
            command = dbConnection.CreateCommand();
            command.CommandText = mesasTabla;
            command.ExecuteNonQuery();

            string tablerosTabla = "CREATE TABLE IF NOT EXISTS " + ModelColumnDbHelper.Tablero.DB_TABLA + "("
                    + ModelColumnDbHelper.Tablero.ID + " TEXT PRIMARY KEY,"
                    + ModelColumnDbHelper.Tablero.NOMBRE + " TEXT,"
                    + ModelColumnDbHelper.Tablero.POS_X + " INTEGER,"
                    + ModelColumnDbHelper.Tablero.POS_Y + " INTEGER,"
                    + ModelColumnDbHelper.Tablero.CREATED_AT + " TEXT,"
                    + ModelColumnDbHelper.Tablero.UPDATED_AT + " TEXT,"
                    + ModelColumnDbHelper.Tablero.DELETED_AT + " TEXT)";
            command = dbConnection.CreateCommand();
            command.CommandText = tablerosTabla;
            command.ExecuteNonQuery();

            string tablerosProductosTabla = "CREATE TABLE IF NOT EXISTS " + ModelColumnDbHelper.TableroProducto.DB_TABLA + "("
                    + ModelColumnDbHelper.TableroProducto.ID + " TEXT PRIMARY KEY,"
                    + ModelColumnDbHelper.TableroProducto.TABLERO_ID + " TEXT,"
                    + ModelColumnDbHelper.TableroProducto.PRODUCTO_ID + " TEXT,"
                    + ModelColumnDbHelper.TableroProducto.POS_X + " INTEGER,"
                    + ModelColumnDbHelper.TableroProducto.POS_Y + " INTEGER,"
                    + ModelColumnDbHelper.TableroProducto.ACTIVO + " TINYINT DEFAULT 1,"
                    + ModelColumnDbHelper.TableroProducto.CREATED_AT + " TEXT,"
                    + ModelColumnDbHelper.TableroProducto.UPDATED_AT + " TEXT,"
                    + ModelColumnDbHelper.TableroProducto.DELETED_AT + " TEXT)";
            command = dbConnection.CreateCommand();
            command.CommandText = tablerosProductosTabla;
            command.ExecuteNonQuery();

            string formasPagosTabla = "CREATE TABLE IF NOT EXISTS " + ModelColumnDbHelper.FormaPago.DB_TABLA + "("
                    + ModelColumnDbHelper.FormaPago.ID + " TEXT PRIMARY KEY,"
                    + ModelColumnDbHelper.FormaPago.NOMBRE + " TEXT,"
                    + ModelColumnDbHelper.FormaPago.PROPINA + " TINYINT DEFAULT 0,"
                    + ModelColumnDbHelper.FormaPago.ORDEN + " INTEGER,"
                    + ModelColumnDbHelper.FormaPago.CREATED_AT + " TEXT,"
                    + ModelColumnDbHelper.FormaPago.UPDATED_AT + " TEXT,"
                    + ModelColumnDbHelper.FormaPago.DELETED_AT + " TEXT)";
            command = dbConnection.CreateCommand();
            command.CommandText = formasPagosTabla;
            command.ExecuteNonQuery();

            string formasPagosCantidadesTabla = "CREATE TABLE IF NOT EXISTS " + ModelColumnDbHelper.PagoPredefinido.DB_TABLA + "("
                    + ModelColumnDbHelper.PagoPredefinido.ID + " TEXT PRIMARY KEY,"
                    + ModelColumnDbHelper.PagoPredefinido.FORMA_PAGO_ID + " TEXT,"
                    + ModelColumnDbHelper.PagoPredefinido.IMPORTE + " REAL,"
                    + ModelColumnDbHelper.PagoPredefinido.ORDEN + " INTEGER,"
                    + ModelColumnDbHelper.PagoPredefinido.CREATED_AT + " TEXT,"
                    + ModelColumnDbHelper.PagoPredefinido.UPDATED_AT + " TEXT,"
                    + ModelColumnDbHelper.PagoPredefinido.DELETED_AT + " TEXT)";
            command = dbConnection.CreateCommand();
            command.CommandText = formasPagosCantidadesTabla;
            command.ExecuteNonQuery();

        }

        public static void Rollback(SQLiteConnection dbConnection)
        {
            var command = dbConnection.CreateCommand();
            command.CommandText = $"DROP TABLE IF EXISTS {ModelColumnDbHelper.Categoria.DB_TABLA}";
            command.ExecuteNonQuery();

            command = dbConnection.CreateCommand();
            command.CommandText = $"DROP TABLE IF EXISTS {ModelColumnDbHelper.CategoriaExtra.DB_TABLA}";
            command.ExecuteNonQuery();

            command = dbConnection.CreateCommand();
            command.CommandText = $"DROP TABLE IF EXISTS {ModelColumnDbHelper.ListaPrecios.DB_TABLA}";
            command.ExecuteNonQuery();

            command = dbConnection.CreateCommand();
            command.CommandText = $"DROP TABLE IF EXISTS {ModelColumnDbHelper.ListaPreciosProducto.DB_TABLA}";
            command.ExecuteNonQuery();

            command = dbConnection.CreateCommand();
            command.CommandText = $"DROP TABLE IF EXISTS {ModelColumnDbHelper.Mesa.DB_TABLA}";
            command.ExecuteNonQuery();

            command = dbConnection.CreateCommand();
            command.CommandText = $"DROP TABLE IF EXISTS {ModelColumnDbHelper.Modificador.DB_TABLA}";
            command.ExecuteNonQuery();

            command = dbConnection.CreateCommand();
            command.CommandText = $"DROP TABLE IF EXISTS {ModelColumnDbHelper.MotivoCancelacion.DB_TABLA}";
            command.ExecuteNonQuery();

            command = dbConnection.CreateCommand();
            command.CommandText = $"DROP TABLE IF EXISTS {ModelColumnDbHelper.Perfil.DB_TABLA}";
            command.ExecuteNonQuery();

            command = dbConnection.CreateCommand();
            command.CommandText = $"DROP TABLE IF EXISTS {ModelColumnDbHelper.PerfilPermiso.DB_TABLA}";
            command.ExecuteNonQuery();

            command = dbConnection.CreateCommand();
            command.CommandText = $"DROP TABLE IF EXISTS {ModelColumnDbHelper.Producto.DB_TABLA}";
            command.ExecuteNonQuery();

            command = dbConnection.CreateCommand();
            command.CommandText = $"DROP TABLE IF EXISTS {ModelColumnDbHelper.Tablero.DB_TABLA}";
            command.ExecuteNonQuery();

            command = dbConnection.CreateCommand();
            command.CommandText = $"DROP TABLE IF EXISTS {ModelColumnDbHelper.TableroProducto.DB_TABLA}";
            command.ExecuteNonQuery();

            command = dbConnection.CreateCommand();
            command.CommandText = $"DROP TABLE IF EXISTS {ModelColumnDbHelper.Usuario.DB_TABLA}";
            command.ExecuteNonQuery();

            command = dbConnection.CreateCommand();
            command.CommandText = $"DROP TABLE IF EXISTS {ModelColumnDbHelper.UsuarioHuella.DB_TABLA}";
            command.ExecuteNonQuery();

            command = dbConnection.CreateCommand();
            command.CommandText = $"DROP TABLE IF EXISTS {ModelColumnDbHelper.UsuarioSucursal.DB_TABLA}";
            command.ExecuteNonQuery();

            command = dbConnection.CreateCommand();
            command.CommandText = $"DROP TABLE IF EXISTS {ModelColumnDbHelper.VentaTipo.DB_TABLA}";
            command.ExecuteNonQuery();

            command = dbConnection.CreateCommand();
            command.CommandText = $"DROP TABLE IF EXISTS {ModelColumnDbHelper.FormaPago.DB_TABLA}";
            command.ExecuteNonQuery();

            command = dbConnection.CreateCommand();
            command.CommandText = $"DROP TABLE IF EXISTS {ModelColumnDbHelper.PagoPredefinido.DB_TABLA}";
            command.ExecuteNonQuery();

            command = dbConnection.CreateCommand();
            command.CommandText = "DROP TABLE IF EXISTS db_versions";
            command.ExecuteNonQuery();

            Configuracion.removerValor(Configuracion.Key.ULTIMA_SINCRONIZACION);
        }
    }
}
