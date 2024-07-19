using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using Tomate.DBHelper;
using Tomate.Statics;
using Tomate.Utils;

namespace Tomate.Models.Usuarios
{
    public class Usuario : Base
    {
        protected override string getTabla()
        {
            return ModelColumnDbHelper.Usuario.DB_TABLA;
        }

        protected override bool getSoftDelete()
        {
            return true;
        }

        protected override Dictionary<string, object?> getColumnas()
        {
            var coumnas = new Dictionary<string, object?>();
            coumnas[ModelColumnDbHelper.Usuario.ID] = Id;
            coumnas[ModelColumnDbHelper.Usuario.PERFIL_ID] = PerfilId;
            coumnas[ModelColumnDbHelper.Usuario.CODIGO] = Codigo;
            coumnas[ModelColumnDbHelper.Usuario.PASSWORD] = Password;
            coumnas[ModelColumnDbHelper.Usuario.NOMBRE] = Nombre;
            coumnas[ModelColumnDbHelper.Usuario.ALIAS] = Alias;
            coumnas[ModelColumnDbHelper.Usuario.AUSENTE] = Ausente;
            coumnas[ModelColumnDbHelper.Usuario.PV_ACCESO] = PvAcceso;
            coumnas[ModelColumnDbHelper.Usuario.PV_ATIENDE_MESAS] = PvAtiendeMesas;
            coumnas[ModelColumnDbHelper.Usuario.ACTIVO] = Activo;
            coumnas[ModelColumnDbHelper.Usuario.CREATED_AT] = CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.Usuario.UPDATED_AT] = UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.Usuario.DELETED_AT] = DeletedAt?.ToString("yyyy-MM-dd HH:mm:ss");

            return coumnas;
        }

        protected override string llavePrimaria()
        {
            return ModelColumnDbHelper.Usuario.ID;
        }

        public string? Id { get; set; }
        public string? PerfilId { get; set; }
        public string? PerfilNombre { get; set; }
        public string? Codigo { get; set; }
        public string? Password { get; set; }
        public string? Nombre { get; set; }
        public string? Alias { get; set; }

        public string? CodigoAlias
        {
            get
            {
                return $"{Codigo} {Alias}";
            }
        }

        public bool Ausente { get; set; } = false;

        public bool PvAcceso { get; set; } = false;

        public bool? PvAtiendeMesas { get; set; } = false;

        public bool? Activo { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public Usuario() { }


        public Usuario(JObject data)
        {
            Id = (string?)data["id"];
            PerfilId = (string?)data["perfil_id"];
            Codigo = (string?)data["codigo"];
            Password = (string?)data["pv_password"];
            Nombre = (string?)data["nombre"];
            Alias = (string?)data["alias"];
            Ausente = (int?)data["ausente"] == 1 || (int?)data["ausente"] == null;
            PvAcceso = (int?)data["pv_acceso"] == 1 || (int?)data["pv_acceso"] == null;
            PvAtiendeMesas = (int?)data["pv_atiende_mesas"] == 1 || (int?)data["pv_atiende_mesas"] == null;
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

        public Usuario(SQLiteDataReader reader)
        {
            Id = Convert.ToString(reader[ModelColumnDbHelper.Usuario.ID]);
            PerfilId = Convert.ToString(reader[ModelColumnDbHelper.Usuario.PERFIL_ID]);
            if (reader["perfil_nombre"] != DBNull.Value)
            {
                PerfilNombre = Convert.ToString(reader["perfil_nombre"]);
            }
            Codigo = Convert.ToString(reader[ModelColumnDbHelper.Usuario.CODIGO]);
            Password = Convert.ToString(reader[ModelColumnDbHelper.Usuario.PASSWORD]);
            Nombre = Convert.ToString(reader[ModelColumnDbHelper.Usuario.NOMBRE]);
            Alias = Convert.ToString(reader[ModelColumnDbHelper.Usuario.ALIAS]);
            Ausente = Convert.ToInt32(reader[ModelColumnDbHelper.Usuario.AUSENTE]) == 1;
            PvAcceso = Convert.ToInt32(reader[ModelColumnDbHelper.Usuario.PV_ACCESO]) == 1;
            PvAtiendeMesas = Convert.ToInt32(reader[ModelColumnDbHelper.Usuario.PV_ATIENDE_MESAS]) == 1;
            Activo = Convert.ToInt32(reader[ModelColumnDbHelper.Usuario.ACTIVO]) == 1;

            CreatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.Usuario.CREATED_AT]);
            if (reader[ModelColumnDbHelper.Usuario.UPDATED_AT] != DBNull.Value)
            {
                UpdatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.Usuario.UPDATED_AT]);
            }
            if (reader[ModelColumnDbHelper.Usuario.DELETED_AT] != DBNull.Value)
            {
                DeletedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.Usuario.DELETED_AT]);
            }
        }

        public static Usuario? buscarCodigo(string codigo, bool? pvAcceso = null)
        {
            Usuario? usuario = null;
            string sql = "SELECT u.*, p.nombre AS perfil_nombre FROM usuarios AS u " +
                        "JOIN perfiles AS p ON p.id = u.perfil_id " +
                        "JOIN usuarios_sucursales AS us ON us.usuario_id = u.id " +
                        "AND us.sucursal_id = @SucursalId AND us.deleted_at IS NULL " +
                        "WHERE u.deleted_at IS NULL AND u.activo = 1 " +
                        "AND u.codigo LIKE @Codigo";

            if (pvAcceso != null)
            {
                sql += " AND u.pv_acceso = @PvAcceso";
            }

            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = sql;

                command.Parameters.Add(new SQLiteParameter("@Codigo", codigo));
                command.Parameters.Add(new SQLiteParameter("@SucursalId", TerminalStatic.SUCURSAL_ID));
                if (pvAcceso != null)
                {
                    command.Parameters.Add(new SQLiteParameter("@PvAcceso", (bool)pvAcceso ? "1" : "0"));
                }
                var result = command.ExecuteReader();
                while (result.Read())
                {
                    usuario = new Usuario(result);
                    break;
                }
                result.Close();
                dbConnection.Close();
            }
            return usuario;
        }

        public static Usuario? buscarId(string id)
        {
            Usuario? usuario = null;
            string sql = "SELECT u.*, p.nombre AS perfil_nombre FROM usuarios AS u " +
                        "JOIN perfiles AS p ON p.id = u.perfil_id " +
                        "JOIN usuarios_sucursales AS us ON us.usuario_id = u.id " +
                        "AND us.sucursal_id = @SucursalId AND us.deleted_at IS NULL " +
                        "WHERE u.deleted_at IS NULL AND u.activo = 1 " +
                        "AND u.id LIKE @Id";

            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = sql;
                command.Parameters.Add(new SQLiteParameter("@SucursalId", TerminalStatic.SUCURSAL_ID));
                command.Parameters.Add(new SQLiteParameter("@Id", id));
                var result = command.ExecuteReader();
                while (result.Read())
                {
                    usuario = new Usuario(result);
                    break;
                }
                result.Close();
                dbConnection.Close();
            }
            return usuario;
        }

        public static ObservableCollection<Usuario> todos(bool mostrarUsuariosSeguros = true, string? perfilId = null, string? buscar = null)
        {
            var resultados = new ObservableCollection<Usuario>();
            string sql = "SELECT u.*, p.nombre AS perfil_nombre FROM usuarios AS u " +
                        "JOIN perfiles AS p ON p.id = u.perfil_id " +
                        "JOIN usuarios_sucursales AS us ON us.usuario_id = u.id " +
                        "AND us.sucursal_id = @SucursalId AND us.deleted_at IS NULL " +
                        "WHERE u.deleted_at IS NULL AND u.activo = 1 ";

            if (!mostrarUsuariosSeguros)
            {
                sql += "AND cast(u.codigo AS INTEGER) > 999 ";
            }
            if (perfilId != null)
            {
                sql += "AND u.perfil_id LIKE @PerfilId ";
            }
            if (buscar != null)
            {
                sql += "AND (u.nombre LIKE @Buscar OR u.alias LIKE @Buscar OR u.codigo LIKE @Buscar ) ";
            }
            sql += "ORDER BY u.codigo ASC";
            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = sql;
                command.Parameters.Add(new SQLiteParameter("@SucursalId", TerminalStatic.SUCURSAL_ID));
                if (perfilId != null)
                {
                    command.Parameters.Add(new SQLiteParameter("@PerfilId", perfilId));
                }
                if (buscar != null)
                {
                    command.Parameters.Add(new SQLiteParameter("@Buscar", buscar));
                }
                var result = command.ExecuteReader();
                while (result.Read())
                {
                    resultados.Add(new Usuario(result));
                }
                result.Close();
                dbConnection.Close();
            }
            return resultados;
        }

        public static ObservableCollection<Usuario> todosAtiendeMesas(string? perfilId = null, string? buscar = null)
        {
            var resultados = new ObservableCollection<Usuario>();
            string sql = "SELECT u.*, p.nombre AS perfil_nombre FROM usuarios AS u " +
                        "JOIN perfiles AS p ON p.id = u.perfil_id " +
                        "JOIN usuarios_sucursales AS us ON us.usuario_id = u.id " +
                        "AND us.sucursal_id = @SucursalId AND us.deleted_at IS NULL " +
                        "WHERE u.deleted_at IS NULL AND u.activo = 1 AND u.pv_atiende_mesas = 1 ";

            if (perfilId != null)
            {
                sql += "AND u.perfil_id LIKE @PerfilId ";
            }
            if (buscar != null)
            {
                sql += "AND (u.nombre LIKE @Buscar OR u.alias LIKE @Buscar OR u.codigo LIKE @Buscar ) ";
            }
            sql += "ORDER BY u.codigo ASC";
            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = sql;
                command.Parameters.Add(new SQLiteParameter("@SucursalId", TerminalStatic.SUCURSAL_ID));
                if (perfilId != null)
                {
                    command.Parameters.Add(new SQLiteParameter("@PerfilId", perfilId));
                }
                if (buscar != null)
                {
                    command.Parameters.Add(new SQLiteParameter("@Buscar", buscar));
                }
                var result = command.ExecuteReader();
                while (result.Read())
                {
                    resultados.Add(new Usuario(result));
                }
                result.Close();
                dbConnection.Close();
            }
            return resultados;
        }

        public bool RevisarPermiso(string permiso)
        {
            return PermisosStatic.RevisarPermiso($"{PerfilId}", permiso);
        }

        public bool BuscarMesa(string numeroMesa)
        {
            return MesasStatic.RevisarMesaPermiso(numeroMesa, $"{Id}");
        }

    }
}
