using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tomate.Components.Dialogos.Teclado;
using Tomate.Models.Usuarios;
using Tomate.Models.Ventas;
using Tomate.Models.Caja;
using Tomate.Utils;
using Tomate.ViewModels.Caja;
using Newtonsoft.Json.Linq;
using Tomate.HttpRequest;
using Tomate.Statics;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Tomate.Components.Dialogos.Venta.Notas;
using System.Collections.ObjectModel;
using System.Timers;

namespace Tomate.Views.Caja
{
    /// <summary>
    /// Interaction logic for CajaView.xaml
    /// </summary>
    public partial class CajaView : UserControl
    {
        //Viewmodel de la clase con variables que se muestran en la vista
        public CajaViewModel ViewModel { get; set; }

        private Timer? CajaCorteTimer;

        //Constructor
        public CajaView()
        {
            ViewModel = new CajaViewModel();
            InitializeComponent();
            DataContext = ViewModel;
        }

        /**
         * MEJORAR PROCESO PARA CALCULAR EL SUBTOTAL CON Y SIN NOTAS
         */
        public void IniciarVista(Usuario? usuario)
        {
#pragma warning disable CS8601 // Possible null reference assignment.
            ContenidoControl.Visibility = Visibility.Collapsed;
            MaskAbrirCaja.Visibility = Visibility.Collapsed;
            ViewModel.Usuario = usuario;
            ViewModel.CargarFormasPagos();
            MostrarPagos(0);
            ViewModel.TotalBotones = 1;
            AperturaCajaBoton.Visibility = Visibility.Collapsed;
            EntradasSalidasBoton.Visibility = Visibility.Collapsed;
            ArqueoBoton.Visibility = Visibility.Collapsed;
            //RevisarPermisos();
            IniciarCajaCorteTimer();
            //MostrarAbrirCaja();
            //OcultarAbrirCaja();
        }

        private void MostrarAbrirCaja()
        {
            this.Dispatcher.Invoke(() =>
            {
                ContenidoControl.Visibility = Visibility.Visible;
                MaskAbrirCaja.Visibility = Visibility.Visible;
                var blur = new BlurEffect();
                blur.Radius = 20;
                ContenidoControl.Effect = blur;
                RevisarPermisos();

                if (ViewModel.Usuario.RevisarPermiso("pv.caja.entradas") || ViewModel.Usuario.RevisarPermiso("pv.caja.salidas"))
                {
                    ViewModel.TotalBotones -= 1;
                    EntradasSalidasBoton.Visibility = Visibility.Collapsed;
                }
                if (ViewModel.Usuario.RevisarPermiso("pv.caja.arqueo"))
                {
                    ViewModel.TotalBotones -= 1;
                    ArqueoBoton.Visibility = Visibility.Collapsed;
                }
            });
        }

        public void OcultarAbrirCaja()
        {
            this.Dispatcher.Invoke(() =>
            {
                ContenidoControl.Visibility = Visibility.Visible;
                MaskAbrirCaja.Visibility = Visibility.Collapsed;
                ContenidoControl.Effect = null;
                RevisarPermisos();
            });
        }

        public void LimpiarVista()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            ViewModel.Usuario = null;
            ViewModel.CajaCorte = null;
            ViewModel.CajaConsultada = false;
            ViewModel.FormaPagoIdFiltro = null;
            ViewModel.FormaPagoIndex = -1;
            CancelarCajaCorteTimer();
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        private void RevisarPermisos()
        {
            ViewModel.TotalBotones = 4;
            if (!ViewModel.Usuario.RevisarPermiso("pv.caja.apertura"))
            {
                ViewModel.TotalBotones -= 1;
                AperturaCajaBoton.Visibility = Visibility.Collapsed;
            }
            else
            {
                AperturaCajaBoton.Visibility = Visibility.Visible;
            }

            if (ViewModel.Usuario.RevisarPermiso("pv.caja.entradas") || ViewModel.Usuario.RevisarPermiso("pv.caja.salidas"))
            {
                EntradasSalidasBoton.Visibility = Visibility.Visible;
            }
            else
            {
                ViewModel.TotalBotones -= 1;
                EntradasSalidasBoton.Visibility = Visibility.Collapsed;
                
            }

           
            if (!ViewModel.Usuario.RevisarPermiso("pv.caja.arqueo"))
            {
                ViewModel.TotalBotones -= 1;
                ArqueoBoton.Visibility = Visibility.Collapsed;
            }
            else
            {
                ArqueoBoton.Visibility = Visibility.Visible;
            }
        }

        private void BotonAtras_Click(object sender, RoutedEventArgs e)
        {
            Atras();  
        }

        private void Atras()
        {
            this.Dispatcher.Invoke(() =>
            {
                getWindow().MostrarListadoVentas();
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

        private void ArqueoBoton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                getWindow().MostrarCajaArqueo(ViewModel.CajaCorte, CajaArqueoView.TIPO_ARQUEO);
            });
        }

        /*private void SalidaDineroBoton_OnClick(object sender, RoutedEventArgs e)
        {
            RegistrarEntradaSalida(false);
        }*/

        private void EntradasSalidasEfectivo_OnClick(object sender, RoutedEventArgs e)
        {

            MovimientosEfectivoModal.mostrar(ViewModel.Usuario, ViewModel.CajaCorte, delegate (Dictionary<string, object?> respuesta)
            {
                this.Dispatcher.Invoke(() =>
                {
                    ViewModel.CajaCorte = (CajaCorte)respuesta["CajaCorte"];
                });
            });
        }

        private void FormasPagoTabs_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (FormasPagoTabs.SelectedIndex > -1)
            {
                MostrarPagos(FormasPagoTabs.SelectedIndex);
            }
        }

        private void MostrarPagos(int formaPagoIndex)
        {
            if (formaPagoIndex != ViewModel.FormaPagoIndex)
            {
                
                
                if (ViewModel.FormaPagoIndex > -1)
                {
                    var formaPagoAnterior = ViewModel.FormasPagos[ViewModel.FormaPagoIndex];
                    formaPagoAnterior.Background = "#d0d0d0";

                    ViewModel.FormasPagos.RemoveAt(ViewModel.FormaPagoIndex);
                    ViewModel.FormasPagos.Insert(ViewModel.FormaPagoIndex, formaPagoAnterior);
                }

                var formaPago = ViewModel.FormasPagos[formaPagoIndex];
                formaPago.Background = "#FFFFFF";

                ViewModel.FormasPagos.RemoveAt(formaPagoIndex);
                ViewModel.FormasPagos.Insert(formaPagoIndex, formaPago);

                ViewModel.FormaPagoIdFiltro = formaPago.Id;
                ViewModel.FormaPagoIndex = formaPagoIndex;
            }

            

        }

        private void AperturaCajaBoton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.CajaConsultada)
            {
                this.Dispatcher.Invoke(() =>
                {
                    DialogoAlertaCm.show("Caja", "Consultando corte de caja");
                });
                return;
            }
            if (ViewModel.CajaCorte == null)
            {
                abrirCaja();
            }
            else
            {
                cerrarCaja();
            }
        }

        private void AbrirCajaModal_Click(object sender, RoutedEventArgs e)
        {
            abrirCaja();
        }

        private void CancelarCajaCorteTimer()
        {
            if (CajaCorteTimer != null)
            {
                CajaCorteTimer.Stop();
                CajaCorteTimer = null;
            }
        }

        /**
         * Obtiene el corte de cajaActual
         */
        private void IniciarCajaCorteTimer()
        {
            CancelarCajaCorteTimer();
            CajaCorteTimer = new Timer();
            CajaCorteTimer.Elapsed += (sender, e) =>
            {
                getCorteCaja();
            };
            CajaCorteTimer.Interval = 5000;
            CajaCorteTimer.AutoReset = true;
            CajaCorteTimer.Start();
            getCorteCaja();
        }

        private void getCorteCaja()
        {

            ApiClient.CajaCorte("1", delegate (JObject respuesta)
            {
                var error = (string?)respuesta["error"];
                if (error != null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        DialogoAlertaCm.show("Caja", error);
                    });
                    return;
                }
                else
                {
                    ViewModel.CajaConsultada = true;
                    this.Dispatcher.Invoke(() =>
                    {
                        var corteCajaJson = respuesta["caja_corte"];
                        if (corteCajaJson != null && corteCajaJson.Type == JTokenType.Object && corteCajaJson.HasValues)
                        {
                            ViewModel.CajaCorte = new Models.Caja.CajaCorte((JObject)corteCajaJson);
                            ViewModel.SumaFormasPagos = new ObservableCollection<CajaFormaPago>(ViewModel.CajaCorte.SumaFormasPagos);
                            OcultarAbrirCaja();
                        }
                        else
                        {
                            ViewModel.CajaCorte = null;
                            MostrarAbrirCaja();
                        }
                    });
                }
            });
        }

        private void abrirCaja()
        {
            this.Dispatcher.Invoke(() =>
            {
                getWindow().MostrarCajaArqueo(ViewModel.CajaCorte, CajaArqueoView.TIPO_APERTURA_CAJA);
            });

        }

        private void cerrarCaja()
        {
            this.Dispatcher.Invoke(() =>
            {
                getWindow().MostrarCajaArqueo(ViewModel.CajaCorte, CajaArqueoView.TIPO_CIERRE_CAJA);
            });
        }

        private void VentasNotasListado_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (VentasNotasListado.SelectedIndex > -1)
            {
                VentaNotaDetalleModal.mostrar(ViewModel.VentasNotas[VentasNotasListado.SelectedIndex], delegate (Dictionary<string, object?> respuesta)
                {
                });
            }
        }
    }
}
