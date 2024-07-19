
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using Tomate.DBHelper;

namespace Tomate.Models.Catalogo
{
    public class Tablero : Base
    {
        protected override string getTabla()
        {
            return ModelColumnDbHelper.Tablero.DB_TABLA;
        }

        protected override bool getSoftDelete()
        {
            return true;
        }

        protected override Dictionary<string, object?> getColumnas()
        {
            var coumnas = new Dictionary<string, object?>();
            coumnas[ModelColumnDbHelper.Tablero.ID] = Id;
            coumnas[ModelColumnDbHelper.Tablero.NOMBRE] = Nombre;
            coumnas[ModelColumnDbHelper.Tablero.POS_X] = PosX;
            coumnas[ModelColumnDbHelper.Tablero.POS_Y] = PosY;
            coumnas[ModelColumnDbHelper.Tablero.CREATED_AT] = CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.Tablero.UPDATED_AT] = UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.Tablero.DELETED_AT] = DeletedAt?.ToString("yyyy-MM-dd HH:mm:ss");

            return coumnas;
        }

        protected override string llavePrimaria()
        {
            return ModelColumnDbHelper.Tablero.ID;
        }

        public string? Id { get; set; }
        public string? Nombre { get; set; }
        public int? PosX { get; set; }
        public int? PosY { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }


        public Tablero() { }

        public Tablero(JObject data)
        {
            Id = (string?)data["id"];
            Nombre = (string?)data["nombre"];
            PosX = (int?)data["pos_x"];
            PosY = (int?)data["pos_y"];

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

        public Tablero(SQLiteDataReader reader)
        {
            Id = Convert.ToString(reader[ModelColumnDbHelper.Tablero.ID]);
            Nombre = Convert.ToString(reader[ModelColumnDbHelper.Tablero.NOMBRE]);
            PosX = Convert.ToInt32(reader[ModelColumnDbHelper.Tablero.POS_X]);
            PosY = Convert.ToInt32(reader[ModelColumnDbHelper.Tablero.POS_Y]);

            CreatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.Tablero.CREATED_AT]);
            if (reader[ModelColumnDbHelper.Tablero.UPDATED_AT] != DBNull.Value)
            {
                UpdatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.Tablero.UPDATED_AT]);
            }
            if (reader[ModelColumnDbHelper.Tablero.DELETED_AT] != DBNull.Value)
            {
                DeletedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.Tablero.DELETED_AT]);
            }
        }

        public static ObservableCollection<Tablero> todos()
        {
            var resultados = new ObservableCollection<Tablero>();
            string sql = "SELECT * FROM tableros " +
                        "WHERE deleted_at IS NULL " +
                        "ORDER BY pos_x ASC, pos_y ASC";
            using (SQLiteConnection dbConnection = DbHelper.GetDbConexion())
            {
                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = sql;
                var result = command.ExecuteReader();
                while (result.Read())
                {
                    resultados.Add(new Tablero(result));
                }
                result.Close();
                dbConnection.Close();
            }
            return resultados;
        }
    }
}
