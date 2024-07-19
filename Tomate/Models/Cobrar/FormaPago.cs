using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Tomate.DBHelper;
using System.Collections.ObjectModel;
using System.Data.SQLite;

namespace Tomate.Models.Cobrar
{
    public class FormaPago : Base
    {
        protected override string getTabla()
        {
            return ModelColumnDbHelper.FormaPago.DB_TABLA;
        }

        protected override bool getSoftDelete()
        {
            return true;
        }

        protected override Dictionary<string, object?> getColumnas()
        {
            var coumnas = new Dictionary<string, object?>();
            coumnas[ModelColumnDbHelper.FormaPago.ID] = Id;
            coumnas[ModelColumnDbHelper.FormaPago.NOMBRE] = Nombre;
            coumnas[ModelColumnDbHelper.FormaPago.PROPINA] = Propina;
            coumnas[ModelColumnDbHelper.FormaPago.ORDEN] = Orden;
            coumnas[ModelColumnDbHelper.FormaPago.CREATED_AT] = CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.FormaPago.UPDATED_AT] = UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.FormaPago.DELETED_AT] = DeletedAt?.ToString("yyyy-MM-dd HH:mm:ss");

            return coumnas;
        }

        protected override string llavePrimaria()
        {
            return ModelColumnDbHelper.FormaPago.ID;
        }

        public string? Id { get; set; }
        public string? Nombre { get; set; }
        public int? Orden { get; set; }
        public bool Propina { get; set; } = false;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        //extras visuales
        public string Background { get; set; } = "#d0d0d0";

        public FormaPago() { }


        public FormaPago(JObject data)
        {
            Id = (string?)data["id"];
            Nombre = (string?)data["nombre"];
            Propina = (int?)data["propina"] == 1;
            Orden = (int?)data["orden"];
            CreatedAt = (DateTime?)data["created_at"];
            UpdatedAt = (DateTime?)data["updated_at"];
            DeletedAt = (DateTime?)data["deleted_at"];

        }

        public FormaPago(SQLiteDataReader reader)
        {
            Id = Convert.ToString(reader[ModelColumnDbHelper.FormaPago.ID]);
            Nombre = Convert.ToString(reader[ModelColumnDbHelper.FormaPago.NOMBRE]);
            Propina = Convert.ToInt32(reader[ModelColumnDbHelper.FormaPago.PROPINA]) == 1;
            Orden = Convert.ToInt32(reader[ModelColumnDbHelper.FormaPago.ORDEN]);
            
            CreatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.FormaPago.CREATED_AT]);
            if (reader[ModelColumnDbHelper.FormaPago.UPDATED_AT] != DBNull.Value)
            {
                UpdatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.FormaPago.UPDATED_AT]);
            }
            if (reader[ModelColumnDbHelper.FormaPago.DELETED_AT] != DBNull.Value)
            {
                DeletedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.FormaPago.DELETED_AT]);
            }
        }

        public static ObservableCollection<FormaPago> todas()
        {
            var resultados = new ObservableCollection<FormaPago>();
            string sql = "SELECT * FROM formas_pagos " +
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
                    resultados.Add(new FormaPago(result));
                }
                result.Close();
                dbConnection.Close();
            }
            return resultados;
        }

        public static FormaPago? Efectivo()
        {
            FormaPago? formaPago = null;
            string sql = "SELECT * FROM formas_pagos " +
                         "WHERE deleted_at IS NULL " +
                         "AND nombre LIKE 'Efectivo'";

            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = sql;
                var result = command.ExecuteReader();
                while (result.Read())
                {
                    formaPago = new FormaPago(result);
                    break;
                }
                result.Close();
                dbConnection.Close();
            }
            return formaPago;
        }


        public static FormaPago? buscarId(string id)
        {
            FormaPago? formaPago = null;
            string sql = "SELECT * FROM formas_pagos " +
                         "WHERE deleted_at IS NULL " +
                         "AND id LIKE @Id";

            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = sql;

                command.Parameters.Add(new SQLiteParameter("@Id", id));
                var result = command.ExecuteReader();
                while (result.Read())
                {
                    formaPago = new FormaPago(result);
                    break;
                }
                result.Close();
                dbConnection.Close();
            }
            return formaPago;
        }

       
    }
}
