using DPFP;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Tomate.HttpRequest;
using Tomate.Models.Caja;
using Tomate.Models.Usuarios;
using Tomate.Models.Ventas;
using Tomate.Statics;
using Tomate.Utils;
using Tomate.ViewModels.Cobro;

namespace Tomate.Views.Cobro
{
    /// <summary>
    /// Interaction logic for CobroVentaView.xaml
    /// </summary>
    public partial class CobroVentaView : UserControl
    {
        //Viewmodel de la clase con variables que se muestran en la vista
        public CobroVentaViewModel ViewModel { get; set; }

        private bool SumarEfectivo = false;

        //Constructor
        public CobroVentaView()
        {
            ViewModel = new CobroVentaViewModel();
            InitializeComponent();
            DataContext = ViewModel;
        }

        /**
         * MEJORAR PROCESO PARA CALCULAR EL SUBTOTAL CON Y SIN NOTAS
         */
        public void IniciarVista(Usuario? usuario, Venta venta, VentaNota ventaNota, CajaCorte cajaCorte)
        {
#pragma warning disable CS8601 // Possible null reference assignment.
            ViewModel.VentaNota = ventaNota;
            ViewModel.Usuario = usuario;
            ViewModel.Venta = venta;
            ViewModel.SubtotalCobro = (float)ventaNota.Subtotal;
            ViewModel.DescuentoProductosCobro = venta.GetProductosDescuentos($"{ventaNota.NumeroNota}");
            if (ViewModel.DescuentoProductosCobro != null)
            {
                ViewModel.SubtotalCobro += (float)ViewModel.DescuentoProductosCobro;
            }
            ViewModel.DescuentoCobro = ventaNota.Descuento;
            
#pragma warning restore CS8601 // Possible null reference assignment.
            ViewModel.TotalPagos = 0;
            ViewModel.TotalPropina = 0;
            ViewModel.CalcularTotalDebe();

            ViewModel.IniciarFormasPagos();
            ViewModel.IniciarMonedas();
            getVentaPagos();
            ViewModel.CajaCorte = cajaCorte;
        }

        

        public void LimpiarVista()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            ViewModel.Usuario = null;
            ViewModel.Venta = null;
            ViewModel.CajaCorte = null;
            SumarEfectivo = false;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            ViewModel.VentaPagos = new ObservableCollection<VentaPago>();
            ViewModel.VentaPagosEliminados = new List<string>();
            RevisarVentaPagos();
        }




        private void BotonAtras_Click(object sender, RoutedEventArgs e)
        {
            if (BotonAplicarCuenta.Opacity == 1)
            {
                this.Dispatcher.Invoke(() =>
                {
                    DialogoAlertaCm.show("¿Descartar cambios?", true, delegate ()
                    {
                        Atras();
                    });
                });
            }
            else
            {
                Atras();
            }
            
           
        }

        private void Atras()
        {
            this.Dispatcher.Invoke(() =>
            {
                getWindow().MostrarVentaDetalle(ViewModel.Venta);
            });
        }

        /**
         * Minimiza la aplicación
         */
        private void BotonMinimizar_Click(object sender, RoutedEventArgs e)
        {

            this.Dispatcher.Invoke(() =>
            {
                getWindow().MinimizarVentana();
            });
        }

        /**
         * Muestra el dialogo para confirmar la salida del programa
         */
        private void BotonCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                getWindow().MostrarDialogoCerrarPrograma();
            });
        }

        /**
         * Obtiene la ventana principal del programa
         */
        private MainWindow getWindow()
        {
            return WindowUtils.getMainWindow();
        }

        /**
         * Desactivar Feedback Boundary de touch
         */
        private void ScrollViewer_ManipulationBoundaryFeedback(object sender, System.Windows.Input.ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        private void getVentaPagos()
        {

            ApiClient.CobranzaVentaPagos($"{ViewModel.Venta.Id}", $"{ViewModel.VentaNota.NumeroNota}", delegate (JObject respuesta)
            {
                var error = (string?)respuesta["error"];
                if (error != null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        DialogoAlertaCm.show("Pagos", error);
                    });
                    return;
                }
                else
                {

                    var ventaPagosJson = (JArray?)respuesta["venta_pagos"];
                    var ventas = new List<Venta>();
                    if (ventaPagosJson != null && ventaPagosJson.Count > 0)
                    {
                        foreach (JObject ventaPagoJson in ventaPagosJson)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                ViewModel.VentaPagos.Add(new VentaPago(ventaPagoJson));
                            });
                        }   
                        RevisarVentaPagos();
                        ViewModel.CalcularTotalDebe();
                    }
                }
            });
        }

        
        private void RevisarVentaPagos()
        {
            this.Dispatcher.Invoke(() =>
            {
                if (ViewModel.VentaPagos.Count > 0)
                {
                    ViewModel.MensajeNoPagos = Visibility.Collapsed;
                }
                else
                {
                    ViewModel.MensajeNoPagos = Visibility.Visible;
                }
                if (ViewModel.getPagosAgregados().Count > 0 || ViewModel.VentaPagosEliminados.Count > 0)
                {
                    BotonAplicarCuenta.Opacity = 1;
                }
                else
                {
                    BotonAplicarCuenta.Opacity = 0.8;
                }
            });

        }

        /**
         * Envia los pagos de la venta solo si se agregaron
         */
        private void BotonAplicarCuenta_Click(object sender, RoutedEventArgs e)
        {
            if (BotonAplicarCuenta.Opacity == 1)
            {

                this.Dispatcher.Invoke(() =>
                {
                    DialogoAlertaCm.show("¿Aplicar pagos?", true, delegate ()
                    {
                        AplicarPagos();
                    });
                });

            }
        }

        private void VentaPagosListado_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ViewModel.CajaCorte == null)
            {
                this.Dispatcher.Invoke(() =>
                {
                    DialogoAlertaCm.show("Es necesario abrir caja");
                });
                return;
            }
            var index = VentaPagosListado.SelectedIndex;
            if (index > -1)
            {
                var pago = ViewModel.VentaPagos[index];
                if (pago.Id == null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        DialogoAlertaCm.show("¿Eliminar pago?", true, delegate ()
                        {

                            if (index == (ViewModel.VentaPagos.Count - 1))
                            {
                                SumarEfectivo = false;
                            }
                            ViewModel.RemoverPago(index);
                            RevisarVentaPagos();
                        }, delegate ()
                        {
                            VentaPagosListado.SelectedIndex = -1;
                        });
                    });
                }
                
            }
        }

        private void MonedasListado_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ViewModel.CajaCorte == null)
            {
                this.Dispatcher.Invoke(() =>
                {
                    DialogoAlertaCm.show("Es necesario abrir caja");
                });
                return;
            }

            if (ViewModel.PagoCubierto)
            {
                this.Dispatcher.Invoke(() =>
                {
                    DialogoAlertaCm.show("El pago excede el total");
                });
                return;
            }
            var index = MonedasListado.SelectedIndex;
            if (index > -1)
            {
                var moneda = ViewModel.CobroMonedas[index];

                if (SumarEfectivo && ViewModel.VentaPagos.Count > 0)
                {
                    var indexPago = ViewModel.VentaPagos.Count - 1;
                    var ultimoPago = ViewModel.VentaPagos[indexPago];
                    if (ultimoPago.FormaPagoId != ViewModel.FormaPagoEfectivo.Id || ultimoPago.Id != null)
                    {
                        SumarEfectivo = false;
                        return;
                    }
                    else
                    {
                        ultimoPago.Importe += (float)moneda.Cantidad;
                        ViewModel.VentaPagos.RemoveAt(indexPago);
                        ViewModel.VentaPagos.Insert(indexPago, ultimoPago);
                        ViewModel.CalcularTotalDebe();
                        RevisarVentaPagos();
                    }
                   
                }
                else
                {
                    
                    AgregarPago(ViewModel.FormaPagoEfectivo.Nombre, ViewModel.FormaPagoEfectivo.Id, 0, (float)moneda.Cantidad);
                    SumarEfectivo = true;
                }
                

                /*var cantidad = ViewModel.PagosPredefinidos[index];
                if (cantidad.Propina)
                {
                    IngresarPropina(cantidad.FormaPago, cantidad.FormaPagoId, (float)cantidad.Importe);
                }
                else
                {
                    AgregarPago(cantidad.FormaPago, cantidad.FormaPagoId, 0, (float)cantidad.Importe);
                }*/
            }
                

        }

        private void FormasPagosListado_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ViewModel.CajaCorte == null)
            {
                this.Dispatcher.Invoke(() =>
                {
                    DialogoAlertaCm.show("Es necesario abrir caja");
                });
                return;
            }

            if (ViewModel.PagoCubierto)
            {
                this.Dispatcher.Invoke(() =>
                {
                    DialogoAlertaCm.show("El pago excede el total");
                });
                return;
            }
            var index = FormasPagosListado.SelectedIndex;
            if (index > -1)
            {
                var formaPago = ViewModel.FormasPagos[index];
                var datos = new Dictionary<string, object?>();
                datos["Titulo"] = "Ingresa total";
                datos["ExactoCantidad"] = ViewModel.TotalDebe;
                datos["Valor"] = $"{ViewModel.TotalDebe}";
                datos["ValorMinimo"] = (float)0.1;

                TecladoCobroModal.mostrar(datos, delegate (Dictionary<string, object?> respuesta)
                {
                    float total = 0;
                    if ((float?)respuesta["Valor"] != null)
                    {
                        total = (float)respuesta["Valor"];
                    }
                    if (total > 0)
                    {
                        if (formaPago.Propina)
                        {
                            IngresarPropina(formaPago.Nombre, formaPago.Id, total);
                        }
                        else
                        {
                            AgregarPago(formaPago.Nombre, formaPago.Id, 0, total);
                        }
                    }
                });
            }
            
        }

        private void IngresarPropina(string formaPago, string formaPagoId, float pago)
        {
            var datos = new Dictionary<string, object?>();
            datos["Titulo"] = "Ingresa propina";
            datos["Valor"] = null;

            TecladoCobroModal.mostrar(datos, delegate (Dictionary<string, object?> respuesta)
            {
                float total = 0;
                if ((float?)respuesta["Valor"] != null)
                {
                    total = (float)respuesta["Valor"];
                }
                AgregarPago(formaPago, formaPagoId, total, pago);
            });
        }

        private void AgregarPago(string formaPago, string formaPagoId, float propina, float pago)
        {
            var ventaPago = new VentaPago();
            ventaPago.FormaPago = formaPago;
            ventaPago.FormaPagoId = formaPagoId;
            ventaPago.Propina = propina;
            ventaPago.Importe = pago;
            ViewModel.AgregarPago(ventaPago);
            RevisarVentaPagos();
            SumarEfectivo = false;
        }

        private void AplicarPagos()
        {
            if (ViewModel.CajaCorte == null)
            {
                this.Dispatcher.Invoke(() =>
                {
                    DialogoAlertaCm.show("Es necesario abrir caja");
                });
                return;
            }
            var pagos = ViewModel.getPagosAgregados();

            if (ViewModel.PagoCubierto && ViewModel.TotalDebe < 0)
            {
                var index = pagos.Count - 1;
                pagos[index].Importe += ViewModel.TotalDebe;
                pagos[index].Cambio = ViewModel.TotalDebe * -1;
            }


            ApiClient.CobranzaVentaPagosRegistrar($"{ViewModel.Usuario.Id}", $"{ViewModel.Venta.Id}", $"{ViewModel.VentaNota.NumeroNota}", 
                $"{ViewModel.CajaCorte.Id}", pagos, 
                ViewModel.VentaPagosEliminados, delegate (JObject respuesta)
            {
                var error = (string?)respuesta["error"];
                if (error != null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        DialogoAlertaCm.show("Pagos", error);
                    });
                    return;
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        if ((string)respuesta["venta_estatus"] == "activo")
                        {
                            getWindow().MostrarVentaDetalle(ViewModel.Venta);
                        }
                        else
                        {
                            getWindow().MostrarListadoVentas();
                        }

                    });
                }
            });
        }
    }
}
