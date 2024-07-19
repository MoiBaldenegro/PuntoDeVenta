using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using Tomate.Models.Catalogo;
using Tomate.Models.Usuarios;

namespace Tomate.Models.Ventas
{
    public class VentaProducto
    {
        public string? Id { get; set; }
        public string? VentaId { get; set; }
        public string? VentaProductoPadreId { get; set; }
        public string? TerminalId { get; set; }
        public string? UsuarioId { get; set; }
        public string? ProductoId { get; set; }
        public string? Concepto { get; set; }
        public string? Nombre
        {
            get
            {
                string? nombre = Concepto;
                if (nombre == null && Producto != null)
                {
                    nombre = $"{Producto.Nombre}";
                }
                return IsExtra ? $"+ {nombre}" : nombre;
            }
        }
        public string? Modificadores { get; set; } = null;
        private int? _Cantidad;
        public int? Cantidad
        {
            get
            {
                return _Cantidad;
            }
            set
            {
                _Cantidad = value;
                CalcularDescuento();
            }
        }
        private float? _Precio;
        public float? Precio
        {
            get
            {
                return _Precio;
            }
            set
            {
                _Precio = value;
                CalcularDescuento();
            }
        }
        public string? DescuentoMotivo { get; set; }

        public float? DescuentoPorcentaje { get; set; }

        public float? Descuento { get; set; }
        public float? SubTotal
        {
            get
            {
                return Cantidad * Precio;
            }
        }
        public float? _Importe;
        public float? Importe
        {
            get
            {
                float? total = _Importe;
                if (TerminalId == null && Descuento != null)
                {
                    total -= Descuento;
                }
                return total;
            }
            set
            {
                _Importe = value;
            }
        }
        public int? Folio { get; set; }
        public int? NumeroOrdenCuenta { get; set; }
        public int? NumeroOrdenGeneral { get; set; }
        public int? NumeroNota { get; set; } = 1;
        public bool NoImprimir { get; set; } = false;
        public bool Seleccionado { get; set; } = false;

        //cuenta de orgen
        public string? NumeroCuentaOrigen { get; set; }
        //si es true ya no se puede editar
        public bool Pagado { get; set; } = false;

        public string NumeroNotaTexto
        {
            get
            {
                return $"Nota {NumeroNota}";
            }
        }

        public string? ImprimirTexto
        {
            get
            {
                if (NoImprimir)
                {
                    return "No";
                }
                return null;
            }
        }

       
        public string? NumeroOrdenUsuario
        {
            get
            {
                return $"{NumeroOrdenCuenta} - {Usuario?.Alias}";
            }
        }

        public string? MostrarModificadores 
        { 
            get
            {
                if (Modificadores != null && Modificadores.Length > 0)
                {
                    return Modificadores.Replace("|", " ").Replace(" \n", "\n")
                        .Replace("\n ", "\n");
                }
                return null;
            }
        }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public string? HoraUsuario
        {
            get
            {
                if (CreatedAt != null && Usuario != null)
                {
                    return $"{CreatedAt?.ToString("HH:mm")} - {Usuario?.Alias}";
                }
                return null;
            }
        }


        public string? IsDisabled
        {
            get
            {
                return Pagado || DeletedAt != null || ProductoId == null ? null : "Si";
            }
        }

        public string IsDisabledBackground
        {
            get
            {
                if (DeletedAt != null)
                {
                    return "#FFB5B9";
                }
                /*else if (Pagado)
                {
                    return "#ffffff";
                }*/
                return "#FFFFFF";
            }
        }

        public Producto? Producto { get; set; }
        public Usuario? Usuario { get; set; }



        public string? ImporteFormato
        {
            get
            {
                if (Importe != null)
                {
                    float total = (float)Importe;
                    return $"{total.ToString("C", CultureInfo.CurrentCulture)}";
                }
                return "";
            }
        }

        public string SubTotalFormato
        {
            get
            {
                if (Descuento > 0)
                {
                    float total = (float)SubTotal;
                    return $"{total.ToString("C", CultureInfo.CurrentCulture)}";
                }
                return "";
            }
        }



        public bool IsExtra
        {
            get
            {
                return VentaProductoPadreId != null;
            }
        }

        public bool IsNoExtra
        {
            get
            {
                return !IsExtra;
            }
        }


        public VentaProducto() { }

        public VentaProducto(VentaProducto ventaProducto)
        {
            Id = ventaProducto.Id;
            VentaId = ventaProducto.VentaId;
            VentaProductoPadreId = ventaProducto.VentaProductoPadreId;
            TerminalId = ventaProducto.TerminalId;
            UsuarioId = ventaProducto.UsuarioId;
            ProductoId = ventaProducto.ProductoId;
            Concepto = ventaProducto.Concepto;
            Modificadores = ventaProducto.Modificadores;
            Cantidad = ventaProducto.Cantidad;
            Precio = ventaProducto.Precio;
            DescuentoMotivo = ventaProducto.DescuentoMotivo;
            DescuentoPorcentaje = ventaProducto.DescuentoPorcentaje;
            Descuento = ventaProducto.Descuento;
            Importe = ventaProducto.Importe;
            Folio = ventaProducto.Folio;
            NumeroOrdenCuenta = ventaProducto.NumeroOrdenCuenta;
            NumeroOrdenGeneral = ventaProducto.NumeroOrdenGeneral;
            NumeroNota = ventaProducto.NumeroNota;
            CreatedAt = ventaProducto.CreatedAt;
            UpdatedAt = ventaProducto.UpdatedAt;
            DeletedAt = ventaProducto.DeletedAt;
            Producto = ventaProducto.Producto;
            Usuario = ventaProducto.Usuario;
        }


        public VentaProducto Reordenar()
        {
            var ventaProducto = new VentaProducto();
            ventaProducto.VentaId = VentaId;
            ventaProducto.UsuarioId = UsuarioId;
            ventaProducto.ProductoId = $"{ProductoId}";
            ventaProducto.Producto = Producto;
            ventaProducto.Precio = Precio;
            ventaProducto.Cantidad = Cantidad;
            ventaProducto.NumeroNota = NumeroNota;
            ventaProducto.Importe = Cantidad * Precio;
            return ventaProducto;
        }

        public VentaProducto(JObject data, string? numeroCuenta = null)
        {
            Id = (string?)data["id"];
            VentaId = (string?)data["venta_id"];
            VentaProductoPadreId = (string?)data["venta_producto_padre_id"];
            TerminalId = (string?)data["terminal_id"];
            UsuarioId = (string?)data["usuario_id"];
            ProductoId = (string?)data["producto_id"];
            Concepto = (string?)data["concepto"];
            Modificadores = (string?)data["modificadores"];
            Cantidad = (int?)data["cantidad"];
            Precio = (float?)data["precio"];
            DescuentoMotivo = (string?)data["descuento_motivo"];
            DescuentoPorcentaje = (float?)data["descuento_porcentaje"];
            Descuento = (float?)data["descuento"];
            Importe = (float?)data["importe"];
            Folio = (int?)data["folio"];
            NumeroOrdenCuenta = (int?)data["num_orden_cuenta"];
            NumeroOrdenGeneral = (int?)data["num_orden_general"];
            NumeroNota = (int?)data["num_nota"];
            NoImprimir = (string?)data["impresora_id"] == null;
            NumeroCuentaOrigen = (string?)data["num_cuenta_origen"];

            Pagado = (int?)data["pagado"] == 1;

            if (numeroCuenta != null && numeroCuenta == NumeroCuentaOrigen)
            {
                NumeroCuentaOrigen = null;
            }


            CreatedAt = (DateTime?)data["created_at"];
            UpdatedAt = (DateTime?)data["updated_at"];
            DeletedAt = (DateTime?)data["deleted_at"];

            if (ProductoId != null)
            {
                Producto = Producto.buscarId(ProductoId);
            }
            if (UsuarioId != null)
            {
                Usuario = Usuario.buscarId(UsuarioId);
            }
        }

        public void SetDescuento(string motivo, bool isPorcentaje, float? cantidad)
        {
            DescuentoMotivo = motivo;
            if (isPorcentaje)
            {
                DescuentoPorcentaje = cantidad;
                CalcularDescuento();
            }
            else
            {
                DescuentoPorcentaje = null;
                Descuento = cantidad;
            }
        }

        private void CalcularDescuento()
        {
            if (DescuentoPorcentaje != null)
            {
                Descuento = SubTotal * (DescuentoPorcentaje / 100);
            }
        }

    }
}
