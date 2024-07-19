using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Tomate.Models.Usuarios;

namespace Tomate.Models.Ventas
{
    public class Venta
    {
        public Usuario? Usuario { get; set; }

        public string? UsuarioAlias
        {
            get
            {
                if (Usuario != null)
                {
                    return Usuario?.Alias ?? null;
                }
                return null;
            }
        }

        public string? Id { get; set; }
        public string? UsuarioId { get; set; }
        public string? ListaPreciosId { get; set; }
        public string? ClienteId { get; set; }
        public VentaTipo? VentaTipo { get; set; }
        public string? VentaTipoId { get; set; }
        public string? NumeroCuenta { get; set; }
        public int? NumeroNotas { get; set; }
        public string? Folio { get; set; }
        public int? Personas { get; set; }
        public string? Nombre { get; set; }
        public string? Observaciones { get; set; }
        public float? SubTotal { get; set; } = 0;
        public string? DescuentoMotivo { get; set; }
        public float? DescuentoPorcentaje { get; set; }
        public float? Descuento { get; set; }
        public float? DescuentoProductos { get; set; }
        public float? Iva { get; set; }
        public float? Total { get; set; } = 0;
        public string? Estatus { get; set; }

        //TIEMPOS
        public DateTime? HoraImpresionTicket { get; set; }
        public string? Tiempo { get; set; }
        public string? TiempoImpresionTicket { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public ObservableCollection<VentaProducto> Productos { get; set; } = new ObservableCollection<VentaProducto>();
        public ObservableCollection<VentaNota> Notas { get; set; } = new ObservableCollection<VentaNota>();

        //true si hay una nota pagada
        public bool NotasPagada = false;

        //indica si es visible para el usuario que inicio sesion
        public bool Visible { get; set; } = false;

        public string DecuentoTexto
        {
            get
            {
                if (DescuentoPorcentaje > 0)
                {
                    return $"Descuento general {DescuentoPorcentaje ?? 0}%";
                }
                return $"Descuento general";
            }
        }

        public string NombreNumeroCuenta
        {
            get
            {
                return $"Cuenta {NumeroCuenta}";
            }
        }

        public string? ColorEstatus
        {
            get
            {
                string color = "#cccccc";
                int minutos = GetMinutosImpresion();
                if (TiempoImpresionTicket != null && minutos < 30)
                {
                    color = "#FFC400";
                }
                else if (minutos >= 30)
                {
                    color = "#f0245e";
                }
                return color;
            }
        }


        public string UsuarioTitulo
        {
            get
            {
                return $"{Usuario?.Codigo} {Usuario?.Alias}";
            }
        }


        public string TituloFormato
        {
            get
            {
                string titulo = $"{Tiempo}   {Personas}P   {NumeroNotas}N";
                if (Nombre != null)
                {
                    titulo += $"   {Nombre}";
                }
                if (VentaTipo != null)
                {
                    titulo += $"   {VentaTipo?.Nombre}";
                }

                return titulo;
            }
        }

        public string SubTotalFormato
        {
            get
            {
                return $"{(SubTotal ?? 0).ToString("C", CultureInfo.CurrentCulture)}";
            }
        }

        

        public string DescuentoProductosFormato
        {
            get
            {
                if (DescuentoProductos > 0)
                {
                    return $"-{(DescuentoProductos ?? 0).ToString("C", CultureInfo.CurrentCulture)}";
                }
                return "";
            }
        }

        public string DescuentoFormato
        {
            get
            {
                if (Descuento > 0)
                {
                    return $"-{(Descuento ?? 0).ToString("C", CultureInfo.CurrentCulture)}";
                }
                return "";
            }
        }

        public string DescuentoTotalFormato
        {
            get
            {
                if (Descuento > 0 || DescuentoProductos > 0)
                {
                    return $"-{((Descuento ?? 0) + (DescuentoProductos ?? 0)).ToString("C", CultureInfo.CurrentCulture)}";
                }
                return "";
            }
        }


        public string TotalFormato
        {
            get
            {
                return $"{(Total ?? 0).ToString("C", CultureInfo.CurrentCulture)}";
            }
        }


        public Venta() { }

        public Venta(string numeroCuenta) 
        {
            NumeroCuenta = numeroCuenta;
        }

        public Venta(JObject data)
        {
            Id = (string?)data["id"];
            UsuarioId = (string?)data["usuario_id"];
            ListaPreciosId = (string?)data["lista_precios_id"];
            ClienteId = (string?)data["cliente_id"];
            VentaTipoId = (string?)data["venta_tipo_id"];
            NumeroCuenta = (string?)data["num_cuenta"];
            NumeroNotas = (int?)data["num_notas"];
            Folio = (string?)data["folio"];
            Personas = (int?)data["personas"];
            Nombre = (string?)data["nombre"];
            Observaciones = (string?)data["observaciones"];
            SubTotal = (float?)data["subtotal"];
            DescuentoMotivo = (string?)data["descuento_motivo"];
            Descuento = (float?)data["descuento"];
            DescuentoPorcentaje = (float?)data["descuento_porcentaje"];
            Iva = (float?)data["iva"];
            Total = (float?)data["total"];
            Estatus = (string?)data["estatus"];

            //TIEMPOS
            HoraImpresionTicket = (DateTime?)data["hora_impresion_ticket"];
            Tiempo = (string?)data["tiempo"];
            TiempoImpresionTicket = (string?)data["tiempo_impresion_ticket"];

            CreatedAt = (DateTime?)data["created_at"];
            UpdatedAt = (DateTime?)data["updated_at"];
            DeletedAt = (DateTime?)data["deleted_at"];

            if (UsuarioId != null)
            {
                Usuario = Usuario.buscarId(UsuarioId);
            }

            if (VentaTipoId != null)
            {
                VentaTipo = VentaTipo.buscarId(VentaTipoId);
            }


            Productos = new ObservableCollection<VentaProducto>();
            var productosJson = (JArray?)data["productos"];
            if (productosJson != null && productosJson.Count > 0)
            {
                foreach (JObject productoJson in productosJson)
                {
                    Productos.Add(new VentaProducto(productoJson, NumeroCuenta));
                }
            }

            Notas = new ObservableCollection<VentaNota>();
            var notasJson = (JArray?)data["notas"];
            if (notasJson != null && notasJson.Count > 0)
            {
                foreach (JObject notaJson in notasJson)
                {
                    Notas.Add(new VentaNota(notaJson));
                }
            }

            NotasPagada = Notas.Where(item => item.IsPagada).Count() > 0;
        }

        public int GetMinutosImpresion()
        {
            int minutos = 0;
            if (TiempoImpresionTicket != null)
            {
                var tiempo = TiempoImpresionTicket.Split(":");
                minutos += int.Parse(tiempo[1]);
                minutos += int.Parse(tiempo[0]) * 60;
            }

            return minutos;
        }

        public float GetSutotal(string? numeroNota = null)
        {
            float subtotal = 0;
            var productosFiltro = Productos.Where(item => item.DeletedAt == null);
            if (numeroNota != null)
            {
                productosFiltro = productosFiltro.Where(item => $"{item.NumeroNota}" == $"{numeroNota}");
            }

            subtotal = productosFiltro.Sum(item => (item.Importe ?? 0));
            subtotal += GetProductosDescuentos(numeroNota);

            return subtotal;
        }

        public float GetProductosDescuentos(string? numeroNota = null)
        {
            float descuento = 0;

            var productosFiltro = Productos.Where(item => item.DeletedAt == null);

            if (numeroNota != null)
            {
                productosFiltro = productosFiltro.Where(item => $"{item.NumeroNota}" == $"{numeroNota}");
            }

            descuento = productosFiltro.Sum(item => (item.Descuento ?? 0));

            return descuento;
        }

        public float GetDescuento(string? numeroNota = null)
        {
            float descuento = 0;

            if (DescuentoPorcentaje > 0)
            {
                float subtotal = GetSutotal(numeroNota);
                float descuentoProductos = GetProductosDescuentos(numeroNota);
                descuento = (subtotal - descuentoProductos) * ((DescuentoPorcentaje ?? 0) / 100);
            }

            return descuento;
        }

        public float GetTotal(string? numeroNota = null)
        {
            float subtotal = GetSutotal(numeroNota);
            float descuentoProductos = GetProductosDescuentos(numeroNota);
            float descuento = GetDescuento(numeroNota);

            return subtotal - descuentoProductos - descuento;
        }


        public ObservableCollection<VentaProducto> GetProductosEnviados()
        {
            return new ObservableCollection<VentaProducto>(Productos.Where(item => item.TerminalId != null).ToList());
        }

        public void RestablecerProductosVenta()
        {
            var productosEnviados = GetProductosEnviados();
            if (productosEnviados.Count > 0)
            {
                Productos = productosEnviados;
            }
        }

        public ObservableCollection<VentaProducto> getComplementos(VentaProducto ventaProducto)
        {
            return new ObservableCollection<VentaProducto>(Productos.Where(item => item.VentaProductoPadreId == ventaProducto.Id).ToList());
        }

    }
}
