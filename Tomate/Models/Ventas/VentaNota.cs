using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using Tomate.Models.Usuarios;

namespace Tomate.Models.Ventas
{
    public class VentaNota
    {
        public string? Id { get; set; }
        public string? VentaId { get; set; }
        public string? CajaCorteId { get; set; }
        public string? CajeroId { get; set; }
        public string? Folio { get; set; }
        public string? NumeroNota { get; set; }
        public string? NumeroCuenta
        {
            get
            {
                if (Venta != null)
                {
                    return $"{Venta.NumeroCuenta}";
                }
                return null;
            }
        }
        public float? Subtotal { get; set; } = 0;
        public float? Descuento { get; set; } = 0;
        public float? Total { get; set; } = 0;
        public float? Pagos { get; set; } = 0;
        public float? Saldo { get; set; } = 0;
        public float? Cambio { get; set; } = 0;
        public float? Propina { get; set; } = 0;
        public string? Estatus { get; set; }

        public bool IsPagada
        {
            get
            {
                return IsNotaPagada();
            }
        }
        public DateTime? FechaPago { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public string? HoraPagoFormato
        {
            get
            {
                if (FechaPago != null)
                {
                    return FechaPago?.ToString("HH:mm");
                }
                return null;
            }
        }

       
        public string SubtotalFormato
        {
            get
            {
                return $"{(Subtotal ?? 0).ToString("C", CultureInfo.CurrentCulture)}";
            }
        }

        public string DescuentoFormato
        {
            get
            {
                return $"{(Descuento ?? 0).ToString("C", CultureInfo.CurrentCulture)}";
            }
        }

        public string TotalFormato
        {
            get
            {
                return $"{(Total ?? 0).ToString("C", CultureInfo.CurrentCulture)}";
            }
        }

        public string PagosFormato
        {
            get
            {
                return $"{(Pagos ?? 0).ToString("C", CultureInfo.CurrentCulture)}";
            }
        }

        public string SaldoFormato
        {
            get
            {
                return $"{(Saldo ?? 0).ToString("C", CultureInfo.CurrentCulture)}";
            }
        }

        public string PropinaFormato
        {
            get
            {
                return $"{(Propina ?? 0).ToString("C", CultureInfo.CurrentCulture)}";
            }
        }


        public string Foreground
        {
            get
            {
                if (IsPagada)
                {
                    return "#999";
                }
                return "#fc5656";
            }
        }

        public Usuario? Cajero { get; set; }
        public Venta? Venta { get; set; }
        public List<VentaPago> VentaPagos = new List<VentaPago>();
        public List<VentaProducto> VentaProductos = new List<VentaProducto>();


        public VentaNota() { }

        public VentaNota(string numeroNota)
        {
            NumeroNota = numeroNota;
        }

        public VentaNota(JObject data)
        {
            Id = (string?)data["id"];
            VentaId = (string?)data["venta_id"];
            CajaCorteId = (string?)data["caja_corte_id"];
            CajeroId = (string?)data["cajero_id"];
            Folio = (string?)data["folio"];
            NumeroNota = (string?)data["num_nota"];
            Subtotal = (float?)data["subtotal"];
            Descuento = (float?)data["descuento"];
            Total = (float?)data["total"];
            Pagos = (float?) data["pagos"];
            Saldo = (float?) data["saldo"];
            Cambio = (float?)data["cambio"];
            Propina = (float?)data["propina"];
            Estatus = (string?)data["estatus"];

            FechaPago = (DateTime?)data["fecha_pago"];
            CreatedAt = (DateTime?)data["created_at"];
            UpdatedAt = (DateTime?)data["updated_at"];
            DeletedAt = (DateTime?)data["deleted_at"];

            if (CajeroId != null)
            {
                var cajeroJson = data["cajero"];
                if (cajeroJson != null && cajeroJson.Type == JTokenType.Object && cajeroJson.HasValues)
                {
                    Cajero = new Usuario((JObject)cajeroJson);
                }
            }

            if (VentaId != null)
            {
                var ventaJson = data["venta"];
                if (ventaJson != null && ventaJson.Type == JTokenType.Object && ventaJson.HasValues)
                {
                    Venta = new Venta((JObject)ventaJson);
                }
            }


            var pagosJson = data["ventaPagos"];
            if (pagosJson != null && pagosJson.Type == JTokenType.Array && pagosJson.HasValues)
            {
                foreach (JObject ventaPagoJson in pagosJson)
                {
                    var pago = new VentaPago(ventaPagoJson);
                    pago.Cajero = Cajero;
                    if (Venta != null)
                    {
                        pago.NumeroCuenta = Venta.NumeroCuenta;
                    }
                    
                    VentaPagos.Add(pago);
                }
            }

            var productosJson = data["ventaProductos"];
            if (productosJson != null && productosJson.Type == JTokenType.Array && productosJson.HasValues)
            {
                foreach (JObject productoJson in productosJson)
                {
                    var ventaProducto = new VentaProducto(productoJson);
                    if ($"{ventaProducto.NumeroCuentaOrigen}" == $"{Venta?.NumeroCuenta}")
                    {
                        ventaProducto.NumeroCuentaOrigen = null;
                    }
                    VentaProductos.Add(ventaProducto);
                }
            }


        }

        public bool IsNotaPagada()
        {
            return Estatus == "pagado";
        }

    }
}
