using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tomate.Models.Ventas;
using Tomate.Models.Usuarios;

namespace Tomate.HttpRequest
{
    public class ApiClient
    {


        /*----------- SINCRONIZACION ----------*/

        public static void Sincronizar(string? ultimaSincronizacion, Action<JObject> callback)
        {
            new Task(() =>
            {
                var request = new RestRequest("/api/v1/sync", Method.Get);
                if (ultimaSincronizacion != null)
                {
                    if (ultimaSincronizacion != null)
                    {
                        request.AddParameter("last_sync", ultimaSincronizacion);
                    }

                }
                callback.Invoke(HttpRequest.send(request));
            }).Start();
        }

        public static bool ComprobarConexion(string direccion, string token)
        {
            return true;
        }

        /*-------------- USUARIOS -------------*/

        /**
        * Nuevo/editar usuario
        */
        public static void GuardarUsuario(Usuario usuario, string? password, Action<JObject> callback)
        {
            new Task(() =>
            {
                var request = new RestRequest("/api/v1/usuarios/guardar", Method.Post);
                request.AddParameter("nombre", usuario.Nombre);
                request.AddParameter("alias", usuario.Alias);
                request.AddParameter("codigo", usuario.Codigo);
                request.AddParameter("perfil_id", usuario.PerfilId);
                if (usuario.Id != null)
                {
                    request.AddParameter("id", usuario.Id);
                }
                if (password != null && password.Length > 0)
                {
                    request.AddParameter("password", password);
                }

                callback.Invoke(HttpRequest.send(request));
            }).Start();
        }

        /**
        * usuario ausente
        */
        public static void UsuarioAusente(string usuarioId, bool ausente, Action<JObject> callback)
        {
            new Task(() =>
            {
                var request = new RestRequest("/api/v1/usuarios/ausente", Method.Post);
                request.AddParameter("usuario_id", usuarioId);
                request.AddParameter("ausente", ausente ? "true" : "false");
                callback.Invoke(HttpRequest.send(request));
            }).Start();
        }

        /**
        * Eliminar usuario
        */
        public static void UsuarioEliminar(string usuarioId, Action<JObject> callback)
        {
            new Task(() =>
            {
                var request = new RestRequest("/api/v1/usuarios/eliminar", Method.Delete);
                request.AddParameter("id", usuarioId);
                callback.Invoke(HttpRequest.send(request));
            }).Start();
        }

        /**
         * Crear huella
         */
        public static void UsuarioNuevaHuella(string usuarioId, string huella, int? posicion, Action<JObject> callback)
        {
            new Task(() =>
            {
                var request = new RestRequest("/api/v1/usuarios/nueva-huella", Method.Post);
                request.AddParameter("usuario_id", usuarioId);
                request.AddParameter("huella", huella);
                request.AddParameter("posicion", $"{posicion}");
                callback.Invoke(HttpRequest.send(request));
            }).Start();
        }

        /**
         * Consulta el listado de turnos abiertos
         */
        public static void TurnosAbiertos(Action<JObject> callback)
        {
            new Task(() =>
            {
                var request = new RestRequest("/api/v1/asistencias/turnos-abiertos", Method.Post);
                callback.Invoke(HttpRequest.send(request));
            }).Start();
        }

        /**
         * Registrar hora de entrada
         */
        public static void UsuarioAsistenciaRegistrarEntrada(string usuarioId, Action<JObject> callback)
        {
            new Task(() =>
            {
                var request = new RestRequest("/api/v1/asistencias/registrar-entrada", Method.Post);
                request.AddParameter("usuario_id", usuarioId);
                callback.Invoke(HttpRequest.send(request));
            }).Start();
        }

        /**
         * Registrar hora de salida
         */
        public static void UsuarioAsistenciaRegistrarSalida(string usuarioId, Action<JObject> callback)
        {
            new Task(() =>
            {
                var request = new RestRequest("/api/v1/asistencias/registrar-salida", Method.Post);
                request.AddParameter("usuario_id", usuarioId);
                callback.Invoke(HttpRequest.send(request));
            }).Start();
        }

        /*-------------- CATALOGO -------------*/


        public static void TablerosProductosActivarDesactivar(Dictionary<string, bool> productos, Action<JObject> callback)
        {
            new Task(() =>
            {
                var request = new RestRequest("/api/v1/tableros/activar-desactivar-productos", Method.Post);
                request.AddParameter("tableros_productos", JsonConvert.SerializeObject(productos));
                callback.Invoke(HttpRequest.send(request));
            }).Start();
        }

        /*--------------- VENTAS --------------*/

        /**
         * Asignar mesa a usuario
         */
        public static void MesasAsignar(Dictionary<string, string?> mesas, Action<JObject> callback)
        {
            new Task(() =>
            {
                var request = new RestRequest("/api/v1/mesas/asignar-mesas", Method.Post);
                string json = JsonConvert.SerializeObject(mesas);
                request.AddParameter("mesas", JsonConvert.SerializeObject(mesas));
                callback.Invoke(HttpRequest.send(request));
            }).Start();
        }

        public static void VentaTicketImprimir(string ventaId, string usuarioId, string numNota, Action<JObject> callback)
        {
            new Task(() =>
            {
                var request = new RestRequest("/api/v1/ventas/imprimir", Method.Get);
                request.AddParameter("venta_id", ventaId);
                request.AddParameter("usuario_id", usuarioId);
                request.AddParameter("num_nota", numNota);
                callback.Invoke(HttpRequest.send(request));
            }).Start();
        }

        /**
         * Cancelar producto venta
         */
        public static void VentaProductoCancelar(string usuarioId, string ventaProductoId, string? motivoId, string? otro, Action<JObject> callback)
        {
            new Task(() =>
            {
                var request = new RestRequest("/api/v1/ventas/cancelar-producto", Method.Post);
                request.AddParameter("usuario_id", usuarioId);
                request.AddParameter("venta_producto_id", ventaProductoId);
                if (motivoId != null)
                {
                    request.AddParameter("motivo_cancelacion_id", motivoId);
                }
                if (otro != null)
                {
                    request.AddParameter("otro", otro);
                }
                callback.Invoke(HttpRequest.send(request));
            }).Start();
        }

        /**
         * Consulta el listado de cuentas de un usuario
         */
        public static void Ventas(string? orden, Action<JObject> callback)
        {
            new Task(() =>
            {
                var request = new RestRequest("/api/v1/ventas/ventas-activas", Method.Get);
                if (orden != null)
                {
                    request.AddParameter("orden", orden);
                }

                callback.Invoke(HttpRequest.send(request));
            }).Start();
        }

        /**
         * Crea una venta
         */
        public static void VentaNueva(string usuarioId, string numeroCuenta, int personas, string? ventaTipoId, Action<JObject> callback)
        {
            new Task(() =>
            {
                var request = new RestRequest("/api/v1/ventas/nueva-venta", Method.Post);
                request.AddParameter("usuario_id", usuarioId);
                request.AddParameter("num_cuenta", numeroCuenta);
                request.AddParameter("personas", personas);
                if (ventaTipoId != null)
                {
                    request.AddParameter("venta_tipo_id", ventaTipoId);
                }
                callback.Invoke(HttpRequest.send(request));
            }).Start();
        }

        /**
         * Detalle una venta
         */
        public static void VentaDetalle(Venta cuenta, Action<JObject> callback)
        {
            new Task(() =>
            {
                var request = new RestRequest("/api/v1/ventas/venta", Method.Get);
                request.AddParameter("id", cuenta.Id);
                callback.Invoke(HttpRequest.send(request));
            }).Start();
        }

        /**
         * Edita una venta
         */
        public static void VentaDetalleActualizar(Venta venta, Action<JObject> callback)
        {
            new Task(() =>
            {
                var request = new RestRequest("/api/v1/ventas/editar-venta", Method.Post);
                request.AddParameter("id", venta.Id);
                request.AddParameter("nombre", venta.Nombre ?? "");
                request.AddParameter("observaciones", venta.Observaciones ?? "");
                request.AddParameter("personas", $"{venta.Personas}");
                //request.AddParameter("num_notas", $"{venta.NumeroNotas}");
                callback.Invoke(HttpRequest.send(request));
            }).Start();
        }

        /**
         * Asigna numero de notas de una venta
         */
        public static void VentaDetalleNumeroNotas(string ventaId, int numeroNotas, Action<JObject> callback)
        {
            new Task(() =>
            {
                var request = new RestRequest("/api/v1/ventas/numero-notas", Method.Post);
                request.AddParameter("venta_id", ventaId);
                request.AddParameter("num_notas", $"{numeroNotas}");
                callback.Invoke(HttpRequest.send(request));
            }).Start();
        }

        /**
         * Cambiar tipo de venta de una venta
         */
        public static void VentaDetalleTipo(string ventaId, string ventaTipoId, Action<JObject> callback)
        {
            new Task(() =>
            {
                var request = new RestRequest("/api/v1/ventas/cambiar-venta-tipo", Method.Post);
                request.AddParameter("venta_id", ventaId);
                request.AddParameter("venta_tipo_id", ventaTipoId);
                callback.Invoke(HttpRequest.send(request));
            }).Start();
        }

        /**
        * Cambiar tipo de venta de una venta
        */
        public static void VentaDetalleProductoDescuento(string usuarioId, string ventaProductoId, string motivo, bool isPorcentaje, float? cantidadDescuento, Action<JObject> callback)
        {
            new Task(() =>
            {
                var request = new RestRequest("/api/v1/ventas/aplicar-descuento-producto", Method.Post);
                request.AddParameter("usuario_id", usuarioId);
                request.AddParameter("venta_producto_id", ventaProductoId);
                request.AddParameter("descuento_motivo", motivo);

                string cantidad = "0";
                if (cantidadDescuento != null)
                {
                    cantidad = $"{cantidadDescuento}";
                }

                if (isPorcentaje)
                {
                    request.AddParameter("descuento_porcentaje", cantidad);
                }
                else
                {
                    request.AddParameter("descuento_importe", cantidad);
                }

                callback.Invoke(HttpRequest.send(request));
            }).Start();
        }

        /**
        * Cambiar tipo de venta de una venta
        */
        public static void VentaDetalleDescuento(string usuarioId, string ventaId, string motivo, bool isPorcentaje, float? cantidadDescuento, Action<JObject> callback)
        {
            new Task(() =>
            {
                var request = new RestRequest("/api/v1/ventas/aplicar-descuento", Method.Post);
                request.AddParameter("usuario_id", usuarioId);
                request.AddParameter("venta_id", ventaId);
                request.AddParameter("descuento_motivo", motivo);

                string cantidad = "0";
                if (cantidadDescuento != null)
                {
                    cantidad = $"{cantidadDescuento}";
                }

                if (isPorcentaje)
                {
                    request.AddParameter("descuento_porcentaje", cantidad);
                }
                else
                {
                    request.AddParameter("descuento_importe", cantidad);
                }

                callback.Invoke(HttpRequest.send(request));
            }).Start();
        }

        /**
        * Tranferir venta a otra mesa
        */
        public static void VentaDetalleTransferir(string usuarioId, string ventaId, string numeroCuenta, Action<JObject> callback)
        {
            new Task(() =>
            {
                var request = new RestRequest("/api/v1/ventas/transferir-cuenta", Method.Post);
                request.AddParameter("usuario_id", usuarioId);
                request.AddParameter("venta_id", ventaId);
                request.AddParameter("num_cuenta", numeroCuenta);

                callback.Invoke(HttpRequest.send(request));
            }).Start();
        }

        /**
        * Mover productos de cuenta 
        */
        public static void VentaDetalleProductosTransferir(List<VentaProducto> productos, string numeroCuenta, string numeroNota, Action<JObject> callback)
        {
            new Task(() =>
            {
                var request = new RestRequest("/api/v1/ventas/transferir-productos", Method.Post);

                List<string> productosCuenta = new List<string>();
                foreach (var producto in productos)
                {
                    if (producto.TerminalId != null)
                    {
                        productosCuenta.Add($"{producto.Id}");
                    }
                }
                request.AddParameter("num_cuenta", numeroCuenta);
                request.AddParameter("num_nota", numeroNota);
                request.AddParameter("venta_productos", JsonConvert.SerializeObject(productosCuenta));

                callback.Invoke(HttpRequest.send(request));
            }).Start();
        }

        /** 
        * Asigna el número de nota a los productos
        */
        public static void VentaDetalleProductosNotas(List<VentaProducto> productos, Action<JObject> callback)
        {
            new Task(() =>
            {
                var request = new RestRequest("/api/v1/ventas/asignar-productos-num-nota", Method.Post);

                Dictionary<string, int> productosNotas = new Dictionary<string, int>();
                foreach (var producto in productos)
                {
                    if (producto.TerminalId != null)
                    {
                        productosNotas[$"{producto.Id}"] = (int)producto.NumeroNota;
                    }

                }
                request.AddParameter("venta_productos", JsonConvert.SerializeObject(productosNotas));

                callback.Invoke(HttpRequest.send(request));
            }).Start();
        }

        /**
         * Cancelar una venta
         */
        public static void VentaCancelar(string usuarioId, string ventaId, string? motivoId, string? otro, Action<JObject> callback)
        {
            new Task(() =>
            {
                var request = new RestRequest("/api/v1/ventas/cancelar-venta", Method.Post);
                request.AddParameter("usuario_id", usuarioId);
                request.AddParameter("venta_id", ventaId);
                if (motivoId != null)
                {
                    request.AddParameter("motivo_cancelacion_id", motivoId);
                }
                if (otro != null)
                {
                    request.AddParameter("otro", otro);
                }
                callback.Invoke(HttpRequest.send(request));
            }).Start();
        }

        /**
         * Pedir productos en una venta
         */
        public static void VentaDetalleProductosPedir(string ventaId, string usuarioId, List<VentaProducto> productos,
            Action<JObject> callback)
        {
            new Task(() =>
            {
                var request = new RestRequest("/api/v1/ventas/pedir-productos", Method.Post);
                request.AddParameter("venta_id", ventaId);
                request.AddParameter("usuario_id", usuarioId);
                List<Dictionary<string, object?>> enviarProductos = new List<Dictionary<string, object?>>();
                foreach (VentaProducto producto in productos)
                {
                    var agregarProducto = new Dictionary<string, object?>();
                    agregarProducto["producto_id"] = producto.ProductoId;
                    agregarProducto["cantidad"] = producto.Cantidad;
                    if (!string.IsNullOrEmpty(producto.Id))
                    {
                        agregarProducto["id"] = producto.Id;
                    }
                    if (!string.IsNullOrEmpty(producto.VentaProductoPadreId))
                    {
                        agregarProducto["venta_producto_padre_id"] = producto.VentaProductoPadreId;
                    }
                    if (!string.IsNullOrEmpty(producto.Modificadores))
                    {
                        agregarProducto["modificadores"] = producto.Modificadores;
                    }
                    if (!string.IsNullOrEmpty(producto.DescuentoMotivo))
                    {
                        agregarProducto["descuento_motivo"] = producto.DescuentoMotivo;
                    }
                    if (producto.DescuentoPorcentaje != null)
                    {
                        agregarProducto["descuento_porcentaje"] = producto.DescuentoPorcentaje;
                    }
                    else if (producto.Descuento != null)
                    {
                        agregarProducto["descuento"] = producto.Descuento;
                    }

                    if (producto.NumeroNota != null)
                    {
                        agregarProducto["num_nota"] = producto.NumeroNota;
                    }
                    if (producto.NoImprimir)
                    {
                        agregarProducto["no_imprimir"] = true;
                    }
                    enviarProductos.Add(agregarProducto);
                }
                request.AddParameter("productos", JsonConvert.SerializeObject(enviarProductos));
                callback.Invoke(HttpRequest.send(request));
            }).Start();
        }

        /*-------------- COBRANZA -------------*/

        /**
         * Obtiene el listado de pagos realizados en una nota 
         */
        public static void CobranzaVentaPagos(string ventaId, string numNota, Action<JObject> callback)
        {
            new Task(() =>
            {
                var request = new RestRequest("/api/v1/cobranza/venta-pagos", Method.Get);
                request.AddParameter("venta_id", ventaId);
                request.AddParameter("num_nota", numNota);
                callback.Invoke(HttpRequest.send(request));
            }).Start();
        }

        public static void CobranzaVentaPagosRegistrar(string usuarioId, string ventaId, string numNota,
            string cajaCorteId,
           List<VentaPago> pagos, List<string> pagosEliminados, Action<JObject> callback)
        {
            new Task(() =>
            {
                var request = new RestRequest("/api/v1/cobranza/registrar-pagos", Method.Post);
                request.AddParameter("usuario_id", usuarioId);
                request.AddParameter("venta_id", ventaId);
                request.AddParameter("caja_corte_id", cajaCorteId);
                request.AddParameter("num_nota", numNota);

                List<Dictionary<string, object?>> nuevosPagos = new List<Dictionary<string, object?>>();
                foreach (VentaPago pago in pagos)
                {
                    var nuevoPago = new Dictionary<string, object?>();
                    nuevoPago["forma_pago_id"] = pago.FormaPagoId;
                    nuevoPago["importe"] = pago.Importe;
                    nuevoPago["propina"] = pago.Propina;
                    nuevoPago["cambio"] = pago.Cambio;

                    nuevosPagos.Add(nuevoPago);
                }
                request.AddParameter("venta_pagos", JsonConvert.SerializeObject(nuevosPagos));

                //request.AddParameter("cancelar_pagos", JsonConvert.SerializeObject(pagosEliminados));


                callback.Invoke(HttpRequest.send(request));
            }).Start();
        }

        /*---------------- CAJA ---------------*/
        

        /**
        * Obtiene el listado de pagos realizados en una nota 
        */
        public static void CajaCorte(string numCaja, Action<JObject> callback)
        {
            new Task(() =>
            {
                var request = new RestRequest("/api/v1/caja/corte-actual", Method.Get);
                //request.AddParameter("sucursal_id", sucursalId);
                request.AddParameter("num_caja", numCaja);
                callback.Invoke(HttpRequest.send(request));
            }).Start();
        }

        public static void CorteCajaAbrir(string usuarioId, string numCaja,
            float efectivoInicial,
            Action<JObject> callback)
        {
            new Task(() =>
            {
                var request = new RestRequest("/api/v1/caja/abrir-caja", Method.Post);
                request.AddParameter("caja_inicial", efectivoInicial);
                request.AddParameter("usuario_id", usuarioId);
                //request.AddParameter("sucursal_id", sucursalId);
                request.AddParameter("num_caja", numCaja);

                callback.Invoke(HttpRequest.send(request));
            }).Start();
        }

        public static void CorteCajaCerrar(string usuarioId, string numCaja, float efectivoFinal,
           Action<JObject> callback)
        {
            new Task(() =>
            {
                var request = new RestRequest("/api/v1/caja/cerrar-caja", Method.Post);
                request.AddParameter("caja_final", efectivoFinal);
                request.AddParameter("usuario_id", usuarioId);
                request.AddParameter("num_caja", numCaja);

                callback.Invoke(HttpRequest.send(request));
            }).Start();
        }

        public static void CajaArqueoRegistrar(string usuarioId, string numCaja, float importe,
           Action<JObject> callback)
        {
            new Task(() =>
            {
                var request = new RestRequest("/api/v1/caja/registrar-arqueo-caja", Method.Post);
                request.AddParameter("usuario_id", usuarioId);
                request.AddParameter("num_caja", numCaja);
                request.AddParameter("importe", importe);
                callback.Invoke(HttpRequest.send(request));
            }).Start();
        }

        /*---------------- MOVIMIENTOS ---------------*/

        public static void CajaMovimientoEfectivoRegistrar(string usuarioId, string numCaja, string descripcion,
            float importe,
           Action<JObject> callback)
        {
            new Task(() =>
            {
                var request = new RestRequest("/api/v1/caja/registrar-movimiento-efectivo", Method.Post);                
                request.AddParameter("usuario_id", usuarioId);
                request.AddParameter("num_caja", numCaja);
                request.AddParameter("descripcion", descripcion);
                request.AddParameter("importe", importe);
                callback.Invoke(HttpRequest.send(request));
            }).Start();
        }

        public static void CajaMovimientoEfectivoCancelar(string usuarioId, string numCaja, string movimientoEfectivoId,
           Action<JObject> callback)
        {
            new Task(() =>
            {
                var request = new RestRequest("/api/v1/caja/cancelar-movimiento-efectivo", Method.Post);
                request.AddParameter("usuario_id", usuarioId);
                request.AddParameter("num_caja", numCaja);
                request.AddParameter("movimiento_efectivo_id", movimientoEfectivoId);
                callback.Invoke(HttpRequest.send(request));
            }).Start();
        }



        /*-------- CONFIGURACION HTTP ---------*/

        public static void SetConfiguracion(string? servidorIp, string? tokenAcceso)
        {
            HttpRequest.SERVIDOR_IP = servidorIp;
            HttpRequest.TOKEN_ACCESO = tokenAcceso;
        }
        public static void RemoverConfiguracion()
        {
            HttpRequest.SERVIDOR_IP = null;
            HttpRequest.TOKEN_ACCESO = null;
        }

    }
}
