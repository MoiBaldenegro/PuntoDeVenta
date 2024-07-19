using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Tomate.Components.ViewModels.Venta;
using Tomate.Models.Ventas;
using Tomate.Utils;
using Tomate.Views;

namespace Tomate.Components.Dialogos.Venta
{
    /// <summary>
    /// Interaction logic for ProductosOpcionesModal.xaml
    /// </summary>
    public partial class ProductosOpcionesModal : UserControl
    {

        public Dictionary<string, object?>? Extras { get; set; } = new Dictionary<string, object?>();

        public ProductoOpcionesViewModel ViewModel { get; set; }

        public static readonly RoutedEvent OnCloseEvent = EventManager.RegisterRoutedEvent("OnClose", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(ProductosOpcionesModal));

        public event RoutedEventHandler OnClose
        {
            add { AddHandler(OnCloseEvent, value); }
            remove { RemoveHandler(OnCloseEvent, value); }
        }

        private Action<Dictionary<string, object?>>? OnEnterAction;
        public bool DesactivarBlur { get; set; } = true;

        public bool OpcionSeleccionada { get; set; } = false;

        public ProductosOpcionesModal()
        {
            ViewModel = new ProductoOpcionesViewModel();

            InitializeComponent();

            DataContext = ViewModel;
        }


        public void mostrar(VentaProducto ventaProducto, Action<Dictionary<string, object?>>? aceptar)
        {
            DialogoProductoOpciones.CloseOnClickAway = false;
            setOnEnter(aceptar);
            OpcionSeleccionada = false;
            DesactivarBlur = true;
            ViewModel.VentaProducto = ventaProducto;

            if (ventaProducto.TerminalId == null)
            {
                BotonModificadores.Visibility = Visibility.Visible;
                BotonExtras.Visibility = Visibility.Visible;
                BotonNoImprimir.Visibility = Visibility.Visible;
                BotonReordenar.Visibility = Visibility.Collapsed;
                BotonMoverMesa.Visibility = Visibility.Collapsed;
            }
            else
            {
                BotonModificadores.Visibility = Visibility.Collapsed;
                BotonExtras.Visibility = Visibility.Collapsed;
                BotonNoImprimir.Visibility = Visibility.Collapsed;
                BotonReordenar.Visibility = Visibility.Visible;
                BotonMoverMesa.Visibility = Visibility.Visible;
            }

            BotonMoverNota.Visibility = Visibility.Collapsed;

            if (ventaProducto.NoImprimir)
            {
                ViewModel.TituloImprimir = "Si trabaja";
            }
            else
            {
                ViewModel.TituloImprimir = "No trabaja";
            }

            if (ventaProducto.IsExtra)
            {
                BotonExtras.Visibility = Visibility.Collapsed;
            }

            DialogoProductoOpciones.IsOpen = true;

            this.Dispatcher.Invoke(() =>
            {
                if (!getWindow().UsuarioSeleccionado.RevisarPermiso("pv.venta.cancelar-productos") && ventaProducto.Folio != null)
                {
                    BotonEliminar.Visibility = Visibility.Collapsed;
                }
                else
                {
                    BotonEliminar.Visibility = Visibility.Visible;
                }
            });
        }

        public void setOnEnter(Action<Dictionary<string, object?>>? accion)
        {
            OnEnterAction = accion;
        }

        public void ocultar()
        {
            DialogoProductoOpciones.IsOpen = false;
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

            new Thread(() =>
            {
                Thread.Sleep(500);
                this.Dispatcher.Invoke(() =>
                {
                    DialogoProductoOpciones.CloseOnClickAway = true;
                });
            }).Start();
        }

        private void DialogoTeclado_DialogClosed(object sender, DialogClosedEventArgs eventArgs)
        {
            BackgroundDialogo.Visibility = Visibility.Collapsed;

            var args = new EventClickArgs(OnCloseEvent);
            RaiseEvent(args);
            if (DesactivarBlur)
            {
                this.Dispatcher.Invoke(() =>
                {
                    getWindow().RemoverBlur();
                });
            }

            if (!OpcionSeleccionada)
            {

                EnterOpcion("CerrarOpciones");
            }
        }

        /**
         * Obtiene la ventana principal del programa
         */
        private MainWindow getWindow()
        {
            return WindowUtils.getMainWindow();
        }

        private void BotonEliminar_Click(object sender, RoutedEventArgs e)
        {
            EnterOpcion("CancelarProducto");
        }

        private void BotonModificadores_Click(object sender, RoutedEventArgs e)
        {
            EnterOpcion("ModificadoresProducto");
        }

        private void BotonExtras_Click(object sender, RoutedEventArgs e)
        {
            EnterOpcion("ExtrasProducto");
        }

        private void BotonNoImprimir_Click(object sender, RoutedEventArgs e)
        {
            EnterOpcion("NoImprimirProducto");
        }


        private void BotonTransferir_Click(object sender, RoutedEventArgs e)
        {
            EnterOpcion("TransferirProducto");
        }
        private void BotonTransferirNota_Click(object sender, RoutedEventArgs e)
        {
            EnterOpcion("TransferirNotaProducto");
        }
        private void BotonAplicarDescuento_Click(object sender, RoutedEventArgs e)
        {
            EnterOpcion("AplicarDescuentoProducto");
        }

        private void BotonReordenar_Click(object sender, RoutedEventArgs e)
        {
            EnterOpcion("ReordenarProducto");
        }

        private void EnterOpcion(string accion)
        {
            OpcionSeleccionada = true;
            if (Extras == null)
            {
                Extras = new Dictionary<string, object?>();
            }
            Extras["VentaProducto"] = ViewModel.VentaProducto;
            Extras["Accion"] = accion;
            ocultar();
            if (OnEnterAction != null)
            {
                OnEnterAction.Invoke(Extras);
            }
        }
    }
}
