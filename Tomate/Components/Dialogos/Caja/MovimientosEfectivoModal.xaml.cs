using MaterialDesignThemes.Wpf;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tomate.Components.ViewModels.Caja;
using Tomate.HttpRequest;
using Tomate.Models;
using Tomate.Models.Caja;
using Tomate.Models.Usuarios;
using Tomate.Models.Ventas;
using Tomate.Utils;
using Tomate.Views;

namespace Tomate.Components.Dialogos.Caja
{
    /// <summary>
    /// Interaction logic for MovimientosEfectivoModal.xaml
    /// </summary>
    public partial class MovimientosEfectivoModal : UserControl
    {

        public Dictionary<string, object?>? Extras { get; set; } = new Dictionary<string, object?>();

        public MovimientosEfectivoViewModel ViewModel { get; set; }

        public bool DesactivarBlur { get; set; } = true;

        //private Action<Dictionary<string, object?>>? OnEnterAction;
        private Action<Dictionary<string, object?>>? OnCloseAction;

        /*public void setOnEnter(Action<Dictionary<string, object?>>? accion)
        {
            OnEnterAction = accion;
        }*/

        public void setOnClose(Action<Dictionary<string, object?>>? accion)
        {
            OnCloseAction = accion;
        }


        public MovimientosEfectivoModal()
        {
            ViewModel = new MovimientosEfectivoViewModel();

            InitializeComponent();

            DataContext = ViewModel;
        }

        public void mostrar(Usuario Usuario, CajaCorte CajaCorte, Action<Dictionary<string, object?>>? close = null)
        {
            //setOnEnter(enter);
            setOnClose(close);

            ViewModel.Usuario = Usuario;
            ViewModel.CajaCorte = CajaCorte;

            DesactivarBlur = true;
            this.Dispatcher.Invoke(() =>
            {
                DialogoEntradasSalidas.IsOpen = true;
            });
        }

       
        public void ocultar()
        {
            DialogoEntradasSalidas.IsOpen = false;
        }

        private void BotonCerrarModal_Click(object sender, RoutedEventArgs e)
        {
            ocultar();
        }

        private void DialogoTeclado_DialogOpened(object sender, DialogOpenedEventArgs eventArgs)
        {
            BackgroundDialogo.Visibility = Visibility.Visible;
            this.Dispatcher.Invoke(() =>
            {
                getWindow().AgregarBlur();
            });
        }
        private void DialogoTeclado_DialogClosed(object sender, DialogClosedEventArgs eventArgs)
        {
            BackgroundDialogo.Visibility = Visibility.Collapsed;
            
            if (DesactivarBlur)
            {
                this.Dispatcher.Invoke(() =>
                {
                    getWindow().RemoverBlur();
                });
            }

            if (OnCloseAction != null)
            {
                if (Extras == null)
                {
                    Extras = new Dictionary<string, object?>();
                }
                Extras["CajaCorte"] = ViewModel.CajaCorte;
                OnCloseAction.Invoke(Extras);
            }

        }

        /**
         * Desactivar Feedback Boundary de touch
         */
        private void ScrollViewer_ManipulationBoundaryFeedback(object sender, System.Windows.Input.ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }


        /**
         * Obtiene la ventana principal del programa
         */
        private MainWindow getWindow()
        {
            return WindowUtils.getMainWindow();
        }


        private void EntradaEfectivo_OnClick(object sender, RoutedEventArgs e)
        {
            RegistrarEntradaSalida(true);
        }

        private void SalidaEfectivo_OnClick(object sender, RoutedEventArgs e)
        {
            RegistrarEntradaSalida(false);
        }

        private void RegistrarEntradaSalida(bool isEntrada, int? index = null)
        {

            var datos = new Dictionary<string, object?>();
            datos["Titulo"] = $"Ingresa {(isEntrada ? "entrada" : "salida")}";
            datos["Valor"] = null;
            datos["ValorMinimo"] = (float)0.1;
            datos["TeclaExacto"] = false;
            datos["DesactivarBlur"] = false;
            CajaMovimientoEfectivo? movimiento = null;
            if (index > -1)
            {
                movimiento = ViewModel.Movimientos[(int)index];
            }
            if (movimiento != null)
            {
                datos["Valor"] = $"{movimiento.Importe}";
            }
            OcultarDialogo();

            TecladoCobroModal.mostrar(datos, delegate (Dictionary<string, object?> respuesta)
            {
                OcultarDialogo();
                float importe = 0;
                if ((float?)respuesta["Valor"] != null)
                {
                    importe = (float)respuesta["Valor"];
                }
                if (importe > 0)
                {
                    var datos = new Dictionary<string, object?>();
                    datos["Titulo"] = "Ingresa motivo";
                    datos["Valor"] = null;
                    datos["DesactivarBlur"] = false;
                    if (movimiento != null)
                    {
                        datos["Valor"] = $"{movimiento.Descripcion}";
                    }
                    getWindow().MostrarTeclado(datos, delegate (Dictionary<string, object?> respuesta)
                    {
                        if (!isEntrada)
                        {
                            importe = importe * -1;
                        }
                        MovimientoRegistrar($"{(string?)respuesta["Valor"]}", importe);
                        /*if (movimiento == null)
                        {
                            movimiento = new CajaMovimientoEfectivo();
                            movimiento.CreatedAt = DateTime.Now;
                        }
               
                        movimiento.UsuarioId = $"{ViewModel.Usuario.Id}";
                        movimiento.CajaCorteId = $"{ViewModel.CajaCorte.Id}";
                        movimiento.Usuario = ViewModel.Usuario;
                        movimiento.Descripcion = $"{(string?)respuesta["Valor"]}";
                        movimiento.Importe = total;

                        this.Dispatcher.Invoke(() =>
                        {*/
                        /*if (index > -1)
                        {
                            ViewModel.Movimientos.RemoveAt((int)index);
                            ViewModel.Movimientos.Insert((int)index, movimiento);
                        }
                        else
                        {
                            ViewModel.Movimientos.Insert(0, movimiento);
                        }*/

                        /*ViewModel.ExistenMovimientos();
                    });*/

                        //enviar peticion crear formapago

                    }, delegate (Dictionary<string, object?> respuesta)
                    {
                        MostrarDialogo();
                    });
                }
            }, delegate (Dictionary<string, object?> respuesta)
            {
                MostrarDialogo();
            });
        }

        private void OcultarDialogo()
        {
            DesactivarBlur = false;
            DialogoEntradasSalidas.IsOpen = false;
        }

        private void MostrarDialogo()
        {
            DesactivarBlur = true;
            DialogoEntradasSalidas.IsOpen = true;
        }

        private void MovimientoEditar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var index = MovimientosListado.SelectedIndex;
            if (index > -1)
            {
                var movimiento = ViewModel.Movimientos[MovimientosListado.SelectedIndex];
                RegistrarEntradaSalida(movimiento.Importe > 0, index);
            }
        }

        private void MovimientoEliminar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var index = MovimientosListado.SelectedIndex;
            if (index > -1)
            {
                var movimiento = ViewModel.Movimientos[MovimientosListado.SelectedIndex];
                var titulo = $"¿Eliminar {(movimiento.Importe < 0 ? "salida" : "entrada")}?";
                OcultarDialogo();
                this.Dispatcher.Invoke(() =>
                {
                    DialogoAlertaCm.show(titulo, true, delegate ()
                    {
                        MostrarDialogo();
                        MovimientoCancelar(index);
                    }, delegate ()
                    {
                        MostrarDialogo();
                    });
                });
            }
            
        }

        private void MovimientoRegistrar(string descripcion, float importe)
        {
            ApiClient.CajaMovimientoEfectivoRegistrar($"{ViewModel.Usuario.Id}", "1", descripcion, importe, delegate (JObject respuesta)
            {
                var error = (string?)respuesta["error"];
                if (error != null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        ToastNotification.show(error, ToastNotification.SHORT);
                    });
                    return;
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        var corteCajaJson = respuesta["caja_corte"];
                        if (corteCajaJson != null && corteCajaJson.Type == JTokenType.Object && corteCajaJson.HasValues)
                        {
                            ViewModel.CajaCorte = new Models.Caja.CajaCorte((JObject)corteCajaJson);
                        }
                        ViewModel.ExistenMovimientos();
                    });
                    
                }
            });
        }

        private void MovimientoCancelar(int index)
        {
            var movimientoId = ViewModel.Movimientos[index].Id;
            ApiClient.CajaMovimientoEfectivoCancelar($"{ViewModel.Usuario.Id}", "1", movimientoId, delegate (JObject respuesta)
            {
                var error = (string?)respuesta["error"];
                if (error != null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        ToastNotification.show(error, ToastNotification.SHORT);
                    });
                    return;
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        var corteCajaJson = respuesta["caja_corte"];
                        if (corteCajaJson != null && corteCajaJson.Type == JTokenType.Object && corteCajaJson.HasValues)
                        {
                            ViewModel.CajaCorte = new Models.Caja.CajaCorte((JObject)corteCajaJson);
                        }
                        ViewModel.ExistenMovimientos();
                    });
                }
            });
        }

    }
}
