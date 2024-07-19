using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Tomate.DBHelper;

namespace Tomate.Models.Usuarios
{
    public class UsuarioSucursal : Base
    {
        protected override string getTabla()
        {
            return ModelColumnDbHelper.UsuarioSucursal.DB_TABLA;
        }

        protected override bool getSoftDelete()
        {
            return true;
        }

        protected override Dictionary<string, object?> getColumnas()
        {
            var coumnas = new Dictionary<string, object?>();
            coumnas[ModelColumnDbHelper.UsuarioSucursal.ID] = Id;
            coumnas[ModelColumnDbHelper.UsuarioSucursal.USUARIO_ID] = UsuarioId;
            coumnas[ModelColumnDbHelper.UsuarioSucursal.SUCURSAL_ID] = SucursalId;
            coumnas[ModelColumnDbHelper.UsuarioSucursal.CREATED_AT] = CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.UsuarioSucursal.UPDATED_AT] = UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            coumnas[ModelColumnDbHelper.UsuarioSucursal.DELETED_AT] = DeletedAt?.ToString("yyyy-MM-dd HH:mm:ss");

            return coumnas;
        }

        protected override string llavePrimaria()
        {
            return ModelColumnDbHelper.UsuarioSucursal.ID;
        }

        public string? Id { get; set; }
        public string? UsuarioId { get; set; }
        public string? SucursalId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public UsuarioSucursal() { }


        public UsuarioSucursal(JObject data)
        {
            Id = (string?)data["id"];
            UsuarioId = (string?)data["usuario_id"];
            SucursalId = (string?)data["sucursal_id"];
            
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

        public UsuarioSucursal(SQLiteDataReader reader)
        {
            Id = Convert.ToString(reader[ModelColumnDbHelper.UsuarioSucursal.ID]);
            UsuarioId = Convert.ToString(reader[ModelColumnDbHelper.UsuarioSucursal.USUARIO_ID]);
            SucursalId = Convert.ToString(reader[ModelColumnDbHelper.UsuarioSucursal.SUCURSAL_ID]);

            CreatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.UsuarioSucursal.CREATED_AT]);
            if (reader[ModelColumnDbHelper.UsuarioSucursal.UPDATED_AT] != DBNull.Value)
            {
                UpdatedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.UsuarioSucursal.UPDATED_AT]);
            }
            if (reader[ModelColumnDbHelper.UsuarioSucursal.DELETED_AT] != DBNull.Value)
            {
                DeletedAt = Convert.ToDateTime(reader[ModelColumnDbHelper.UsuarioSucursal.DELETED_AT]);
            }
        }
    }
}
