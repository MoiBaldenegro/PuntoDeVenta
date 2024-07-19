
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using Tomate.DBHelper;

namespace Tomate.Models.Ventas
{
    public class VentaTipo : Base
    {
        protected override string getTabla()
        {
            return ModelColumnDbHelper.VentaTipo.DB_TABLA;
        }

        protected override bool getSoftDelete()
        {
            return true;
        }

        protected override Dictionary<string, object?> getColumnas()
        {
            var coumnas = new Dictionary<string, object?>();
            coumnas[ModelColumnDbHelper.VentaTipo.ID] = Id;
            coumnas[ModelColumnDbHelper.VentaTipo.NOMBRE] = Nombre;
            coumnas[ModelColumnDbHelper.VentaTipo.PREDETERMINADO] = Predeterminado;
            coumnas[ModelColumnDbHelper.VentaTipo.ORDEN] = Orden;
            coumnas[ModelColumnDbHelper.VentaTipo.ACTIVO] = Activo;
            coumnas[ModelColumnDbHelper.VentaTipo.CREATED_AT] = CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.VentaTipo.UPDATED_AT] = UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.VentaTipo.DELETED_AT] = DeletedAt?.ToString("yyyy-MM-dd HH:mm:ss");

            return coumnas;
        }

        protected override string llavePrimaria()
        {
            return ModelColumnDbHelper.VentaTipo.ID;
        }

        public string? Id { get; set; }
        public string? Nombre { get; set; }
        public bool? Predeterminado { get; set; }
        public int? Orden { get; set; }
        public bool? Activo { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }


        public VentaTipo() { }

        public VentaTipo(JObject data)
        {
            Id = (string?)data["id"];
            Nombre = (string?)data["nombre"];
            Predeterminado = (int?)data["predeterminado"] == 1;
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

        public VentaTipo(SQLiteDataReader reader)
        {
            Id = Convert.ToString(reader[ModelColumnDbHelper.VentaTipo.ID]);
            Nombre = Convert.ToString(reader[ModelColumnDbHelper.VentaTipo.NOMBRE]);
            Predeterminado = Convert.ToInt32(reader[ModelColumnDbHelper.VentaTipo.PREDETERMINADO]) == 1;
            Orden = Convert.ToInt32(reader[ModelColumnDbHelper.VentaTipo.ORDEN]);
            Activo = Convert.ToInt32(reader[ModelColumnDbHelper.VentaTipo.ACTIVO]) == 1;

            CreatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.VentaTipo.CREATED_AT]);
            if (reader[ModelColumnDbHelper.VentaTipo.UPDATED_AT] != DBNull.Value)
            {
                UpdatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.VentaTipo.UPDATED_AT]);
            }
            if (reader[ModelColumnDbHelper.VentaTipo.DELETED_AT] != DBNull.Value)
            {
                DeletedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.VentaTipo.DELETED_AT]);
            }
        }

        public static ObservableCollection<VentaTipo> todas(bool mostrarPredeterminado = true)
        {
            var resultados = new ObservableCollection<VentaTipo>();
            string sql = "SELECT * FROM ventas_tipos " +
                        "WHERE deleted_at IS NULL AND activo = 1 ";
            if (!mostrarPredeterminado)
            {
                sql += "AND predeterminado != 1 ";
            }

            sql += "ORDER BY orden ASC";

            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = sql;
                var result = command.ExecuteReader();
                while (result.Read())
                {
                    resultados.Add(new VentaTipo(result));
                }
                result.Close();
                dbConnection.Close();
            }
            return resultados;
        }

        public static VentaTipo? buscarId(string id)
        {
            VentaTipo? ventaTipo = null;
            string sql = "SELECT * FROM ventas_tipos " +
                        "WHERE id LIKE @Id";

            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = sql;

                command.Parameters.Add(new SQLiteParameter("@Id", id));
                var result = command.ExecuteReader();
                while (result.Read())
                {
                    ventaTipo = new VentaTipo(result);
                    break;
                }
                result.Close();
                dbConnection.Close();
            }
            return ventaTipo;
        }
    }
}
