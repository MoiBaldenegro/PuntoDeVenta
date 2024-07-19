using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using Tomate.Models.Usuarios;

namespace Tomate.Models.Ventas
{
    public class VentaPago
    {
        public string? Id { get; set; }
        public string? VentaId { get; set; }
        public string? CajaCorteId { get; set; }
        public string? VentaNotaId { get; set; }
        public string? FormaPagoId { get; set; }
        public string? CajeroId { get; set; }
        public string? FormaPago { get; set; }
        public string? NumeroCuenta { get; set; }
        public string? FolioVenta { get; set; }
        public float? Propina { get; set; } = 0;
        public float? Importe { get; set; } = 0;
        public float? Cambio { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public Usuario? Cajero { get; set; }

        public string? CreatedAtFormato
        {
            get
            {
                if (CreatedAt != null)
                {
                    return CreatedAt?.ToString("HH:mm");
                }
                return null;
            }
        }

        public string? Nuevo
        {
            get
            {
                return Id == null ? "Nuevo" : null;
            }
        }

        public string PropinaFormato
        {
            get
            {
                return $"{(Propina ?? 0).ToString("C", CultureInfo.CurrentCulture)}";
            }
        }

        public string ImporteFormato
        {
            get
            {
                return $"{(Importe ?? 0).ToString("C", CultureInfo.CurrentCulture)}";
            }
        }

        public VentaPago() { }

        public VentaPago(string folioVenta, string formaPago, float propina, float importe) 
        {
            FolioVenta = folioVenta;
            FormaPago = formaPago;
            Propina = propina;
            Importe = importe;  
        }


        public VentaPago(JObject data)
        {
            Id = (string?)data["id"];
            CajeroId = (string?)data["usuario_id"];
            VentaId = (string?)data["venta_id"];
            FormaPagoId = (string?)data["forma_pago_id"];
            Propina = (float?)data["propina"];
            Importe = (float?)data["importe"];
            Cambio = (float?)data["cambio"];
            CreatedAt = (DateTime?)data["created_at"];
            UpdatedAt = (DateTime?)data["updated_at"];
            DeletedAt = (DateTime?)data["deleted_at"];


            var formaPagoJson = data["formaPago"];
            if (formaPagoJson != null && formaPagoJson.Type == JTokenType.Object && formaPagoJson.HasValues)
            {
                var formaPago = new Models.Cobrar.FormaPago((JObject)formaPagoJson);
                if (formaPago != null)
                {
                    FormaPago = formaPago.Nombre;
                }
            }
            else
            {
                var formaPago = Models.Cobrar.FormaPago.buscarId(FormaPagoId);
                if (formaPago != null)
                {
                    FormaPago = formaPago.Nombre;
                }
            }

            if (FormaPago == null)
            {
                FormaPago = "Pago";
            }

            if (CajeroId != null)
            {
                var cajeroJson = data["cajero"];
                if (cajeroJson != null && cajeroJson.Type == JTokenType.Object && cajeroJson.HasValues)
                {
                    Cajero = new Usuario((JObject)cajeroJson);
                }
            }

        }

    }
}
