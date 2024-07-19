
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using Tomate.DBHelper;

namespace Tomate.Models.Ventas
{
    public class MotivoCancelacion : Base
    {
        protected override string getTabla()
        {
            return ModelColumnDbHelper.MotivoCancelacion.DB_TABLA;
        }

        protected override bool getSoftDelete()
        {
            return true;
        }

        protected override Dictionary<string, object?> getColumnas()
        {
            var coumnas = new Dictionary<string, object?>();
            coumnas[ModelColumnDbHelper.MotivoCancelacion.ID] = Id;
            coumnas[ModelColumnDbHelper.MotivoCancelacion.NOMBRE] = Nombre;
            coumnas[ModelColumnDbHelper.MotivoCancelacion.ORDEN] = Orden;
            coumnas[ModelColumnDbHelper.MotivoCancelacion.CREATED_AT] = CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.MotivoCancelacion.UPDATED_AT] = UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.MotivoCancelacion.DELETED_AT] = DeletedAt?.ToString("yyyy-MM-dd HH:mm:ss");

            return coumnas;
        }

        protected override string llavePrimaria()
        {
            return ModelColumnDbHelper.MotivoCancelacion.ID;
        }

        public string? Id { get; set; }
        public string? Nombre { get; set; }
        public int? Orden { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }


        public MotivoCancelacion() { }

        public MotivoCancelacion(JObject data)
        {
            Id = (string?)data["id"];
            Nombre = (string?)data["nombre"];
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

        public MotivoCancelacion(SQLiteDataReader reader)
        {
            Id = Convert.ToString(reader[ModelColumnDbHelper.MotivoCancelacion.ID]);
            Nombre = Convert.ToString(reader[ModelColumnDbHelper.MotivoCancelacion.NOMBRE]);
            Orden = Convert.ToInt32(reader[ModelColumnDbHelper.MotivoCancelacion.ORDEN]);

            CreatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.MotivoCancelacion.CREATED_AT]);
            if (reader[ModelColumnDbHelper.MotivoCancelacion.UPDATED_AT] != DBNull.Value)
            {
                UpdatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.MotivoCancelacion.UPDATED_AT]);
            }
            if (reader[ModelColumnDbHelper.MotivoCancelacion.DELETED_AT] != DBNull.Value)
            {
                DeletedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.MotivoCancelacion.DELETED_AT]);
            }
        }

        public static ObservableCollection<MotivoCancelacion> todos()
        {
            var resultados = new ObservableCollection<MotivoCancelacion>();
            string sql = "SELECT * FROM motivos_cancelacion " +
                        "WHERE deleted_at IS NULL " +
                        "ORDER BY orden ASC";
            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = sql;
                var result = command.ExecuteReader();
                while (result.Read())
                {
                    resultados.Add(new MotivoCancelacion(result));
                }
                result.Close();
                dbConnection.Close();
            }
            return resultados;
        }
    }
}
