using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Tomate.DBHelper;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Globalization;

namespace Tomate.Models
{
    public class PagoPredefinido : Base
    {
        protected override string getTabla()
        {
            return ModelColumnDbHelper.PagoPredefinido.DB_TABLA;
        }

        protected override bool getSoftDelete()
        {
            return true;
        }

        protected override Dictionary<string, object?> getColumnas()
        {
            var coumnas = new Dictionary<string, object?>();
            coumnas[ModelColumnDbHelper.PagoPredefinido.ID] = Id;
            coumnas[ModelColumnDbHelper.PagoPredefinido.FORMA_PAGO_ID] = FormaPagoId;
            coumnas[ModelColumnDbHelper.PagoPredefinido.IMPORTE] = Importe;
            coumnas[ModelColumnDbHelper.PagoPredefinido.ORDEN] = Orden;
            coumnas[ModelColumnDbHelper.PagoPredefinido.CREATED_AT] = CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.PagoPredefinido.UPDATED_AT] = UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.PagoPredefinido.DELETED_AT] = DeletedAt?.ToString("yyyy-MM-dd HH:mm:ss");

            return coumnas;
        }

        protected override string llavePrimaria()
        {
            return ModelColumnDbHelper.PagoPredefinido.ID;
        }

        public string? Id { get; set; }
        public string? FormaPago { get; set; }
        public string? FormaPagoId { get; set; }
        public float? Importe { get; set; }
        public int? Orden { get; set; }
        public bool Propina { get; set; } = false;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public string CantidadFormato
        {
            get
            {
                return $"{(Importe ?? 0).ToString("C", CultureInfo.CurrentCulture)}";
            }
        }


        public PagoPredefinido() { }

        public PagoPredefinido(string formaPago, float cantidad) 
        {
            FormaPago = formaPago;
            Importe = cantidad;
        }

        public PagoPredefinido(string formaPago, float cantidad, bool propina)
        {
            FormaPago = formaPago;
            Importe = cantidad;
            Propina = propina;
        }


        public PagoPredefinido(JObject data)
        {
            Id = (string?)data["id"];
            FormaPagoId = (string?)data["forma_pago_id"];
            Importe = (float?)data["importe"];
            Orden = (int?)data["orden"];
            CreatedAt = (DateTime?)data["created_at"];
            UpdatedAt = (DateTime?)data["updated_at"];
            DeletedAt = (DateTime?)data["deleted_at"];
        }

        public PagoPredefinido(SQLiteDataReader reader)
        {
            Id = Convert.ToString(reader[ModelColumnDbHelper.PagoPredefinido.ID]);
            FormaPago = Convert.ToString(reader[ModelColumnDbHelper.FormaPago.NOMBRE]);
            FormaPagoId = Convert.ToString(reader[ModelColumnDbHelper.PagoPredefinido.FORMA_PAGO_ID]);
            Propina = Convert.ToInt32(reader[ModelColumnDbHelper.FormaPago.PROPINA]) == 1;
            Importe = (float)Convert.ToDouble(reader[ModelColumnDbHelper.PagoPredefinido.IMPORTE]);
            Orden = Convert.ToInt32(reader[ModelColumnDbHelper.PagoPredefinido.ORDEN]);
            
            CreatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.PagoPredefinido.CREATED_AT]);
            if (reader[ModelColumnDbHelper.PagoPredefinido.UPDATED_AT] != DBNull.Value)
            {
                UpdatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.PagoPredefinido.UPDATED_AT]);
            }
            if (reader[ModelColumnDbHelper.PagoPredefinido.DELETED_AT] != DBNull.Value)
            {
                DeletedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.PagoPredefinido.DELETED_AT]);
            }
        }

        public static ObservableCollection<PagoPredefinido> todos()
        {
            var resultados = new ObservableCollection<PagoPredefinido>();
            string sql = "SELECT pp.*, fp.nombre, fp.propina FROM pagos_predefinidos AS pp " +
                         "JOIN formas_pagos AS fp ON fp.id = pp.forma_pago_id " +
                         "WHERE pp.deleted_at IS NULL " +
                         "ORDER BY pp.orden ASC";
            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = sql;
                var result = command.ExecuteReader();
                while (result.Read())
                {
                    resultados.Add(new PagoPredefinido(result));
                }
                result.Close();
                dbConnection.Close();
            }
            return resultados;
        }

    }
}
