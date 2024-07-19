
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using Tomate.DBHelper;

namespace Tomate.Models.Ventas
{
    public class Mesa : Base
    {
        protected override string getTabla()
        {
            return ModelColumnDbHelper.Mesa.DB_TABLA;
        }

        protected override bool getSoftDelete()
        {
            return true;
        }

        protected override Dictionary<string, object?> getColumnas()
        {
            var coumnas = new Dictionary<string, object?>();
            coumnas[ModelColumnDbHelper.Mesa.ID] = Id;
            coumnas[ModelColumnDbHelper.Mesa.USUARIO_ID] = UsuarioId;
            coumnas[ModelColumnDbHelper.Mesa.NUMERO_MESA] = NumeroMesa;
            coumnas[ModelColumnDbHelper.Mesa.ORDEN] = Orden;
            coumnas[ModelColumnDbHelper.Mesa.CREATED_AT] = CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.Mesa.UPDATED_AT] = UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.Mesa.DELETED_AT] = DeletedAt?.ToString("yyyy-MM-dd HH:mm:ss");

            return coumnas;
        }

        protected override string llavePrimaria()
        {
            return ModelColumnDbHelper.Mesa.ID;
        }

        public string? Id { get; set; }
        public string? UsuarioId { get; set; }
        public string? UsuarioNombre { get; set; }
        public bool UsuarioAusente { get; set; } = false;
        public string? NumeroMesa { get; set; }
        public int? Orden { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }


        //extras para mostrar en asignar mesas
        public Venta? Venta { get; set; }
        public string? VentaExiste { 
            get 
            {
                if (Venta != null)
                {
                    return Venta.Id;
                }
                return null;
            }
        }

        public string? VentaTiempo
        {
            get
            {
                return Venta?.Tiempo ?? null;
            }
        }

        public string? VentaColorEstatus
        {
            get
            {
                return Venta?.ColorEstatus ?? null;
            }
        }

        public string? VentaTiempoImpresionTicket
        {
            get
            {
                return Venta?.TiempoImpresionTicket ?? null;
            }
        }

        public string? VentaTotalFormato
        {
            get
            {
                return Venta?.TotalFormato ?? null;
            }
        }

        public bool Seleccionada { get; set; } = false;

        public string? ColorNombre { get; set; } = "#666666";
        public string? ColorTotal { get; set; } = "#333333";
        public string? Foreground { get; set; } = "#fc5656";
        public string? Background { get; set; } = "#FFFFFF";

        public Mesa() { }

        public Mesa(JObject data)
        {
            Id = (string?)data["id"];
            UsuarioId = (string?)data["usuario_id"];
            NumeroMesa = (string?)data["num_mesa"];
            Orden = (int?)data["orden"];

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

        public Mesa(SQLiteDataReader reader)
        {
            Id = Convert.ToString(reader[ModelColumnDbHelper.Mesa.ID]);
            if (reader[ModelColumnDbHelper.Mesa.USUARIO_ID] != DBNull.Value)
            {
                UsuarioId = Convert.ToString(reader[ModelColumnDbHelper.Mesa.USUARIO_ID]);
            }
            if (reader["usuario_alias"] != DBNull.Value)
            {
                UsuarioNombre = $"{Convert.ToString(reader["usuario_alias"])}";
            }
            if (reader["usuario_ausente"] != DBNull.Value)
            {
                UsuarioAusente = Convert.ToInt32(reader["usuario_ausente"]) == 1;
            }


            NumeroMesa = Convert.ToString(reader[ModelColumnDbHelper.Mesa.NUMERO_MESA]);
            Orden = Convert.ToInt32(reader[ModelColumnDbHelper.Mesa.ORDEN]);

            CreatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.Mesa.CREATED_AT]);
            if (reader[ModelColumnDbHelper.Mesa.UPDATED_AT] != DBNull.Value)
            {
                UpdatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.Mesa.UPDATED_AT]);
            }
            if (reader[ModelColumnDbHelper.Mesa.DELETED_AT] != DBNull.Value)
            {
                DeletedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.Mesa.DELETED_AT]);
            }
        }

        public static ObservableCollection<Mesa> todas()
        {
            var resultados = new ObservableCollection<Mesa>();
            string sql = "SELECT m.*, u.codigo as usuario_codigo, u.alias as usuario_alias, " +
                         "u.ausente as usuario_ausente " +
                         "FROM mesas AS m " +
                         "LEFT JOIN usuarios AS u ON u.id = m.usuario_id " +
                         "WHERE m.deleted_at IS NULL " +
                         "ORDER BY m.orden ASC";
            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = sql;
                var result = command.ExecuteReader();
                while (result.Read())
                {
                    resultados.Add(new Mesa(result));
                }
                result.Close();
                dbConnection.Close();
            }
            return resultados;
        }

        public static bool MesaUsuario(string numeroMesa, string usuarioId)
        {
            bool mesa = false;
            var resultados = new ObservableCollection<Mesa>();
            string sql = "SELECT count(1) as total FROM mesas " +
                         "WHERE deleted_at IS NULL " +
                         "AND usuario_id = @UsuarioId AND num_mesa = @NumeroMesa";
            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = sql;
                command.Parameters.Add(new SQLiteParameter("@UsuarioId", usuarioId));
                command.Parameters.Add(new SQLiteParameter("@NumeroMesa", numeroMesa));
                var result = command.ExecuteReader();
                while (result.Read())
                {
                    mesa = Convert.ToInt32(result["total"]) > 0;
                }
                result.Close();
                dbConnection.Close();
            }
            return mesa;
        }

        public void SetSeleccionada(bool seleccionada)
        {
            Seleccionada = seleccionada;
            if (Seleccionada)
            {
                Background = "#fc5656";
                Foreground = "#FFFFFF";
                ColorNombre = "#FFFFFF";
                ColorTotal = "#FFFFFF";
            }
            else
            {
                Background = "#FFFFFF";
                Foreground = "#fc5656";
                ColorNombre = "#666666";
                ColorTotal = "#333333";
            }
        }
    }
}
