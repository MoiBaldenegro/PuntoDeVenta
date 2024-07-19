namespace Tomate.DBHelper
{
    public class ModelColumnDbHelper
    {

        public class Configuracion
        {
            public static string DB_TABLA = "configuraciones";
            public static string KEY = "key";
            public static string VALOR = "valor";
            public static string UPDATED_AT = "updated_at";
        }

        public class Perfil
        {
            public static string DB_TABLA = "perfiles";
            public static string ID = "id";
            public static string NOMBRE = "nombre";
            public static string VISIBLE = "visible";
            public static string CREATED_AT = "created_at";
            public static string UPDATED_AT = "updated_at";
            public static string DELETED_AT = "deleted_at";
        }

        public class PerfilPermiso
        {
            public static string DB_TABLA = "perfiles_permisos";
            public static string ID = "id";
            public static string PERFIL_ID = "perfil_id";
            public static string PERMISO = "permiso";
            public static string CREATED_AT = "created_at";
            public static string UPDATED_AT = "updated_at";
            public static string DELETED_AT = "deleted_at";
        }

        public class Usuario
        {
            public static string DB_TABLA = "usuarios";
            public static string ID = "id";
            public static string PERFIL_ID = "perfil_id";
            public static string PASSWORD = "password";
            public static string CODIGO = "codigo";
            public static string NOMBRE = "nombre";
            public static string ALIAS = "alias";
            public static string AUSENTE = "ausente";
            public static string PV_ACCESO = "pv_acceso";
            public static string PV_ATIENDE_MESAS = "pv_atiende_mesas";
            public static string ACTIVO = "activo";
            public static string CREATED_AT = "created_at";
            public static string UPDATED_AT = "updated_at";
            public static string DELETED_AT = "deleted_at";
        }

        public class UsuarioHuella
        {
            public static string DB_TABLA = "usuarios_huellas";
            public static string ID = "id";
            public static string USUARIO_ID = "usuario_id";
            public static string HUELLA = "huella";
            public static string POSICION = "posicion";
            public static string CREATED_AT = "created_at";
            public static string UPDATED_AT = "updated_at";
            public static string DELETED_AT = "deleted_at";
        }

        public class UsuarioSucursal
        {
            public static string DB_TABLA = "usuarios_sucursales";
            public static string ID = "id";
            public static string USUARIO_ID = "usuario_id";
            public static string SUCURSAL_ID = "sucursal_id";
            public static string CREATED_AT = "created_at";
            public static string UPDATED_AT = "updated_at";
            public static string DELETED_AT = "deleted_at";
        }

        public class Categoria
        {
            public static string DB_TABLA = "categorias";
            public static string ID = "id";
            public static string CATEGORIA_PADRE_ID = "categoria_padre_id";
            public static string NIVEL = "nivel";
            public static string NOMBRE = "nombre";
            public static string ORDEN = "orden";
            public static string ACTIVO = "activo";
            public static string CREATED_AT = "created_at";
            public static string UPDATED_AT = "updated_at";
            public static string DELETED_AT = "deleted_at";
        }

        public class Producto
        {
            public static string DB_TABLA = "productos";
            public static string ID = "id";
            public static string CATEGORIA_ID = "categoria_id";
            //public static string PRODUCTO_TIPO_ID = "producto_tipo_id";
            public static string NOMBRE = "nombre";
            public static string PRECIO = "precio";
            public static string IVA = "iva";
            public static string ORDEN = "orden";
            public static string ACTIVO = "activo";
            public static string EXTRAS = "extras";
            public static string CREATED_AT = "created_at";
            public static string UPDATED_AT = "updated_at";
            public static string DELETED_AT = "deleted_at";
        }

        public class ListaPrecios
        {
            public static string DB_TABLA = "listas_precios";
            public static string ID = "id";
            public static string NOMBRE = "nombre";
            public static string CREATED_AT = "created_at";
            public static string UPDATED_AT = "updated_at";
            public static string DELETED_AT = "deleted_at";
        }

        public class ListaPreciosProducto
        {
            public static string DB_TABLA = "listas_precios_productos";
            public static string ID = "id";
            public static string LISTA_PRECIOS_ID = "lista_precios_id";
            public static string PRODUCTO_ID = "producto_id";
            public static string PRECIO = "precio";
            public static string CREATED_AT = "created_at";
            public static string UPDATED_AT = "updated_at";
            public static string DELETED_AT = "deleted_at";
        }

        public class CategoriaExtra
        {
            public static string DB_TABLA = "categorias_extras";
            public static string ID = "id";
            public static string CATEGORIA_ID = "categoria_id";
            public static string PRODUCTO_ID = "producto_id";
            public static string CREATED_AT = "created_at";
            public static string UPDATED_AT = "updated_at";
            public static string DELETED_AT = "deleted_at";
        }

        public class Modificador
        {
            public static string DB_TABLA = "modificadores";
            public static string ID = "id";
            public static string CATEGORIA_ID = "categoria_id";
            public static string NOMBRE = "nombre";
            public static string ORDEN = "orden";
            public static string CREATED_AT = "created_at";
            public static string UPDATED_AT = "updated_at";
            public static string DELETED_AT = "deleted_at";
        }


        public class VentaTipo
        {
            public static string DB_TABLA = "ventas_tipos";
            public static string ID = "id";
            public static string NOMBRE = "nombre";
            public static string PREDETERMINADO = "predeterminado";
            public static string ORDEN = "orden";
            public static string ACTIVO = "activo";
            public static string CREATED_AT = "created_at";
            public static string UPDATED_AT = "updated_at";
            public static string DELETED_AT = "deleted_at";
        }

        public class MotivoCancelacion
        {
            public static string DB_TABLA = "motivos_cancelacion";
            public static string ID = "id";
            public static string NOMBRE = "nombre";
            public static string ORDEN = "orden";
            public static string CREATED_AT = "created_at";
            public static string UPDATED_AT = "updated_at";
            public static string DELETED_AT = "deleted_at";
        }

        public class Mesa
        {
            public static string DB_TABLA = "mesas";
            public static string ID = "id";
            public static string USUARIO_ID = "usuario_id";
            public static string NUMERO_MESA = "num_mesa";
            public static string ORDEN = "orden";
            public static string CREATED_AT = "created_at";
            public static string UPDATED_AT = "updated_at";
            public static string DELETED_AT = "deleted_at";
        }

        public class Tablero
        {
            public static string DB_TABLA = "tableros";
            public static string ID = "id";
            public static string NOMBRE = "nombre";
            public static string POS_X = "pos_x";
            public static string POS_Y = "pos_y";
            public static string CREATED_AT = "created_at";
            public static string UPDATED_AT = "updated_at";
            public static string DELETED_AT = "deleted_at";
        }

        public class TableroProducto
        {
            public static string DB_TABLA = "tableros_productos";
            public static string ID = "id";
            public static string TABLERO_ID = "tablero_id";
            public static string PRODUCTO_ID = "producto_id";
            public static string POS_X = "pos_x";
            public static string POS_Y = "pos_y";
            public static string ACTIVO = "activo";
            public static string CREATED_AT = "created_at";
            public static string UPDATED_AT = "updated_at";
            public static string DELETED_AT = "deleted_at";
        }


        public class FormaPago
        {
            public static string DB_TABLA = "formas_pagos";
            public static string ID = "id";
            public static string NOMBRE = "nombre";
            public static string PROPINA = "propina";
            public static string ORDEN = "orden";
            public static string CREATED_AT = "created_at";
            public static string UPDATED_AT = "updated_at";
            public static string DELETED_AT = "deleted_at";
        }

        public class PagoPredefinido
        {
            public static string DB_TABLA = "pagos_predefinidos";
            public static string ID = "id";
            public static string FORMA_PAGO_ID = "forma_pago_id";
            public static string IMPORTE = "importe";
            public static string ORDEN = "orden";
            public static string CREATED_AT = "created_at";
            public static string UPDATED_AT = "updated_at";
            public static string DELETED_AT = "deleted_at";
        }



    }
}
