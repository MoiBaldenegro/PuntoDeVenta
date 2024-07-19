using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Tomate.Models.Usuarios;
using Tomate.Models.Ventas;

namespace Tomate.Models.Caja
{
    public class CajaCorte
    {
       
        public string? Id { get; set; }
        public string? SucursalId { get; set; }
        public string? CajeroId { get; set; }
        public string? NumeroCaja { get; set; }
        public float? CajaInicial { get; set; }
        public float? Pagos { get; set; }
        public float? PagosEfectivo { get; set; }
        public float? CajaFinal { get; set; }
        /*public float? CajaEntradas { get; set; }
        public float? CajaSalidas { get; set; }*/

        public float? CajaMovimientosEfectivo { get; set; }
        public float? CajaAjustes { get; set; }

        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Usuario? Cajero { get; set; }

        public List<VentaPago> VentasPagos = new List<VentaPago>();
        public List<VentaNota> VentasNotas = new List<VentaNota>();
        public List<CajaMovimientoEfectivo> MovimientosEfectivo = new List<CajaMovimientoEfectivo>();

        public List<CajaFormaPago> SumaFormasPagos = new List<CajaFormaPago>();

        public string CajaInicialFormato
        {
            get
            {
                return $"{(CajaInicial ?? 0).ToString("C", CultureInfo.CurrentCulture)}";
            }
        }

        public string CajaFinalFormato
        {
            get
            {
                return $"{(CajaFinal ?? 0).ToString("C", CultureInfo.CurrentCulture)}";
            }
        }

        public CajaCorte() { }


        public CajaCorte(JObject data)
        {
            Id = (string?)data["id"];
            SucursalId = (string?)data["sucursal_id"];
            CajeroId = (string?)data["cajero_id"];
            NumeroCaja = (string?)data["num_caja"];
            CajaInicial = (float?)data["caja_inicial"];
            CajaMovimientosEfectivo = (float?)data["total_movimientos_efectivo"];
            Pagos = (float?)data["pagos"];
            PagosEfectivo = (float?)data["pagos_efectivo"];
            CajaFinal = (float?)data["caja_final"];

            FechaInicio = (DateTime?)data["fecha_inicio"];
            FechaFin = (DateTime?)data["fecha_fin"];
            CreatedAt = (DateTime?)data["created_at"];
            UpdatedAt = (DateTime?)data["updated_at"];
            DeletedAt = (DateTime?)data["deleted_at"];

            try
            {
                var ventaPagosJson = (JArray?)data["ventaPagos"];
                if (ventaPagosJson != null && ventaPagosJson.Count > 0)
                {
                    foreach (JObject ventaPagoJson in ventaPagosJson)
                    {
                        VentasPagos.Add(new VentaPago(ventaPagoJson));
                    }
                }
            }
            catch (Exception)
            {
                VentasPagos = new List<VentaPago>();
            }

            try
            {
                var ventaNotasJson = (JArray?)data["ventaNotas"];
                if (ventaNotasJson != null && ventaNotasJson.Count > 0)
                {
                    foreach (JObject ventaNotaJson in ventaNotasJson)
                    {
                        VentasNotas.Add(new VentaNota(ventaNotaJson));
                    }
                }
            }
            catch (Exception)
            {
                VentasNotas = new List<VentaNota>();
            }

            try
            {
                var movimientosEfectivoJson = (JArray?)data["movimientosEfectivo"];
                if (movimientosEfectivoJson != null && movimientosEfectivoJson.Count > 0)
                {
                    foreach (JObject movimientoEfectivoJson in movimientosEfectivoJson)
                    {
                        MovimientosEfectivo.Add(new CajaMovimientoEfectivo(movimientoEfectivoJson));
                    }
                }
            }
            catch (Exception)
            {
                MovimientosEfectivo = new List<CajaMovimientoEfectivo>();
            }

            

            try
            {
                var sumaFormasPagosJson = (JArray?)data["suma_pagos"];
                if (sumaFormasPagosJson != null && sumaFormasPagosJson.Count > 0)
                {
                    foreach (JObject sumaFormaPagoJson in sumaFormasPagosJson)
                    {
                        SumaFormasPagos.Add(new CajaFormaPago(sumaFormaPagoJson));
                    }
                }
            }
            catch (Exception)
            {
                SumaFormasPagos = new List<CajaFormaPago>();
            }

            try
            {
                Cajero = new Usuario((JObject?)data["cajero"]);
            }
            catch (Exception)
            {
                Cajero = null;
            }

        }

    }
}
