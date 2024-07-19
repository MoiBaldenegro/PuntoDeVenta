using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Media.Media3D;
using Tomate.Models.Usuarios;
using Tomate.Models.Ventas;

namespace Tomate.Models.Caja
{
    public class CajaMovimientoEfectivo
    {
       
        public string? Id { get; set; }
        public string? CajaCorteId { get; set; }
        public string? CajaArqueoId { get; set; }
        public string? UsuarioId { get; set; }
        public string? Folio { get; set; }
        public string? Descripcion { get; set; }
        public float? Importe { get; set; }
        public bool Bloqueado { get; set; } = false;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Usuario? Usuario { get; set; }

        public string ColorImporte
        {
            get
            {
                return (Importe < 0) ? "#DE0300" : "#333333"; 
            }
        }
        public Visibility Editable
        {
            get
            {
                return !Bloqueado ? Visibility.Visible : Visibility.Collapsed;
            }
        }
       
        public string ImporteFormato
        {
            get
            {
                return $"{(Importe ?? 0).ToString("C", CultureInfo.CurrentCulture)}";
            }
        }

        public string? CreatedAtFormato
        {
            get
            {
                return CreatedAt?.ToString("HH:mm");
            }
        }

        public CajaMovimientoEfectivo() { }


        public CajaMovimientoEfectivo(JObject data)
        {
            Id = (string?)data["id"];
            CajaCorteId = (string?)data["corte_caja_id"];
            CajaArqueoId = (string?)data["caja_arqueo_id"];
            UsuarioId = (string?)data["usuario_id"];
            Folio = (string?)data["folio"];
            Descripcion = (string?)data["descripcion"];
            Importe = (float?)data["importe"];
            Bloqueado = (int?)data["bloqueado"] == 1;
 
            CreatedAt = (DateTime?)data["created_at"];
            UpdatedAt = (DateTime?)data["updated_at"];

            try
            {
                if (UsuarioId != null)
                {
                    var usuarioJson = data["usuario"];
                    if (usuarioJson != null && usuarioJson.Type == JTokenType.Object && usuarioJson.HasValues)
                    {
                        Usuario = new Usuario((JObject)usuarioJson);
                    }
                }
            }
            catch (Exception)
            {
                Usuario = null;
            }

        }

    }
}
