
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Windows.Controls;
using Tomate.DBHelper;

namespace Tomate.Models.Catalogo
{
    public class Modificador : Base
    {
        protected override string getTabla()
        {
            return ModelColumnDbHelper.Modificador.DB_TABLA;
        }

        protected override bool getSoftDelete()
        {
            return true;
        }

        protected override Dictionary<string, object?> getColumnas()
        {
            var coumnas = new Dictionary<string, object?>();
            coumnas[ModelColumnDbHelper.Modificador.ID] = Id;
            coumnas[ModelColumnDbHelper.Modificador.CATEGORIA_ID] = CategoriaId;
            coumnas[ModelColumnDbHelper.Modificador.NOMBRE] = Nombre;
            coumnas[ModelColumnDbHelper.Modificador.ORDEN] = Orden;
            coumnas[ModelColumnDbHelper.Modificador.CREATED_AT] = CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.Modificador.UPDATED_AT] = UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.Modificador.DELETED_AT] = DeletedAt?.ToString("yyyy-MM-dd HH:mm:ss");

            return coumnas;
        }

        protected override string llavePrimaria()
        {
            return ModelColumnDbHelper.Modificador.ID;
        }

        public string? Id { get; set; }
        public string? CategoriaId { get; set; }
        public string? Nombre { get; set; }
        public int? Orden { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public string? IsSaltoLinea
        {
            get
            {
                return Nombre == "\n" ? "Salto" : null;
            }
        }


        public Modificador() { }

        public Modificador(string nombre)
        {
            Nombre = nombre;
            if (Nombre != "\n") 
            {
                Id = Nombre;
            }
        }

        public Modificador(JObject data)
        {
            Id = (string?)data["id"];
            CategoriaId = (string?)data["categoria_id"];
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

        public Modificador(SQLiteDataReader reader)
        {
            Id = Convert.ToString(reader[ModelColumnDbHelper.Modificador.ID]);
            CategoriaId = Convert.ToString(reader[ModelColumnDbHelper.Modificador.CATEGORIA_ID]);
            Nombre = Convert.ToString(reader[ModelColumnDbHelper.Modificador.NOMBRE]);
            Orden = Convert.ToInt32(reader[ModelColumnDbHelper.Modificador.ORDEN]);

            CreatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.Modificador.CREATED_AT]);
            if (reader[ModelColumnDbHelper.Modificador.UPDATED_AT] != DBNull.Value)
            {
                UpdatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.Modificador.UPDATED_AT]);
            }
            if (reader[ModelColumnDbHelper.Modificador.DELETED_AT] != DBNull.Value)
            {
                DeletedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.Modificador.DELETED_AT]);
            }
        }

        public static ObservableCollection<Modificador> todos(string categoriaId)
        {
            var resultados = new ObservableCollection<Modificador>();
            string sql = "SELECT * FROM modificadores " +
                        "WHERE deleted_at IS NULL AND categoria_id = @CategoriaId " +
                        "ORDER BY orden, nombre ASC";
            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = sql;
                command.Parameters.Add(new SQLiteParameter("@CategoriaId", categoriaId));
                var result = command.ExecuteReader();
                while (result.Read())
                {
                    resultados.Add(new Modificador(result));
                }
                result.Close();
                dbConnection.Close();
            }
            return resultados;
        }

    }
}
