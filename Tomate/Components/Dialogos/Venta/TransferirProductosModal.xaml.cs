using MaterialDesignThemes.Wpf;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Tomate.Components.ViewModels.Venta;
using Tomate.HttpRequest;
using Tomate.Models.Ventas;
using Tomate.Utils;
using Tomate.Views;

namespace Tomate.Components.Dialogos.Venta
{
    /// <summary>
    /// Interaction logic for TransferirProductosModal.xaml
    /// </summary>
    public partial class TransferirProductosModal : UserControl
    {

        public Dictionary<string, object?>? Extras { get; set; } = new Dictionary<string, object?>();

        public TransferirProductosViewModel ViewModel { get; set; }

        public bool DesactivarBlur { get; set; } = true;

        private Action<Dictionary<string, object?>>? OnEnterAction;
        private Action<Dictionary<string, object?>>? OnCloseAction;

        private ObservableCollection<VentaProducto>? Productos { get; set; }

        public void setOnEnter(Action<Dictionary<string, object?>>? accion)
        {
            OnEnterAction = accion;
        }

        public void setOnClose(Action<Dictionary<string, object?>>? accion)
        {
            OnCloseAction = accion;
        }

        public TransferirProductosModal()
        {
            ViewModel = new TransferirProductosViewModel();

            InitializeComponent();

            DataContext = ViewModel;
        }

        public void mostrar(Models.Ventas.Venta venta, Models.Ventas.Venta transferir,
            ObservableCollection<VentaProducto> productos, int? indexMover = null,
            Action<Dictionary<string, object?>>? enter = null,
            Action<Dictionary<string, object?>>? cerrar = null)
        {
            setOnEnter(enter);
            setOnClose(cerrar);
            ViewModel.Venta = venta;
            ViewModel.VentaTransferir = transferir;
            Productos = new ObservableCollection<VentaProducto>(productos.Where(item => item.IsDisabled != null).ToList());
            ViewModel.VentaProductos = new ObservableCollection<VentaProducto>(Productos);
            ViewModel.MoverProductos = new ObservableCollection<VentaProducto>();
            DesactivarBlur = true;

            DialogoTransferir.IsOpen = true;
            if (indexMover != null)
            {
                TransferirProducto(productos[(int)indexMover]);
            }

            RevisarMensajes();
        }
        public void ocultar()
        {
            this.Dispatcher.Invoke(() =>
            {
                DialogoTransferir.IsOpen = false;
            });
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
            if (Extras == null)
            {
                Extras = new Dictionary<string, object?>();
            }
            if (OnCloseAction != null)
            {
                OnCloseAction.Invoke(Extras);
            }
            if (DesactivarBlur)
            {
                this.Dispatcher.Invoke(() =>
                {
                    getWindow().RemoverBlur();
                });
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

        private void TranferirTodos_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (IsLoader())
            {
                return;
            }
            if (ViewModel.VentaProductos.Count > 0)
            {
                var productos = ViewModel.VentaProductos
                                    .Where(item => item.VentaProductoPadreId == null && item.IsDisabled != null).ToList();

                foreach (VentaProducto producto in productos)
                {
                    TransferirProducto(producto);
                }
                RevisarMensajes();
            }

        }

        private void CuentaProductosListado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoader())
            {
                return;
            }
            if (CuentaProductosListado.SelectedIndex > -1)
            {
                TransferirProducto(ViewModel.VentaProductos[CuentaProductosListado.SelectedIndex]);
                CuentaProductosListado.SelectedIndex = -1;
                TransferirProductosListado.SelectedIndex = -1;
                RevisarMensajes();
            }
        }

        private void TransferirProductosListado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoader())
            {
                return;
            }
            if (TransferirProductosListado.SelectedIndex > -1)
            {
                RegresarProducto(ViewModel.MoverProductos[TransferirProductosListado.SelectedIndex]);
                CuentaProductosListado.SelectedIndex = -1;
                TransferirProductosListado.SelectedIndex = -1;
                RevisarMensajes();
            }

        }


        private void RegresarProducto(VentaProducto? producto)
        {
            if (producto != null && producto.IsExtra)
            {
                producto = ViewModel.MoverProductos.Where(item => item.Id == producto.VentaProductoPadreId).FirstOrDefault();
            }
            if (producto != null)
            {
                var productosExtras = ViewModel.MoverProductos
                                    .Where(item => item.VentaProductoPadreId == producto.Id).ToList();
                ViewModel.VentaProductos.Add(producto);

                int index = ViewModel.MoverProductos.Select((Producto, Index) => new { Producto, Index })
                        .First(item => item.Producto.Id == producto.Id)
                        .Index;

                ViewModel.MoverProductos.RemoveAt(ViewModel.GetIndexMoverProductos($"{producto.Id}"));
                foreach (var extra in productosExtras)
                {
                    ViewModel.VentaProductos.Add(extra);
                    int indexExtra = ViewModel.GetIndexMoverProductos($"{extra.Id}");
                    if (indexExtra > -1)
                    {
                        ViewModel.MoverProductos.RemoveAt(indexExtra);
                    }
                }
            }
        }

        private void TransferirProducto(VentaProducto? producto)
        {
            if (producto != null && producto.IsExtra)
            {
                producto = ViewModel.VentaProductos.Where(item => item.Id == producto.VentaProductoPadreId).FirstOrDefault();
            }
            if (producto != null)
            {
                var productosExtras = ViewModel.VentaProductos
                                    .Where(item => item.VentaProductoPadreId == producto.Id).ToList();
                ViewModel.MoverProductos.Add(producto);
                ViewModel.VentaProductos.RemoveAt(ViewModel.GetIndexVentaProductos($"{producto.Id}"));
                foreach (var extra in productosExtras)
                {
                    ViewModel.MoverProductos.Add(extra);
                    int indexExtra = ViewModel.GetIndexVentaProductos($"{extra.Id}");
                    if (indexExtra > -1)
                    {
                        ViewModel.VentaProductos.RemoveAt(indexExtra);
                    }
                }
            }
        }

        private void RevisarMensajes()
        {
            if (ViewModel.VentaProductos.Count > 0)
            {
                NoProductos.Visibility = Visibility.Collapsed;
            }
            else
            {
                NoProductos.Visibility = Visibility.Visible;
            }
            if (ViewModel.MoverProductos.Count > 0)
            {
                AgregaProductos.Visibility = Visibility.Collapsed;
            }
            else
            {
                AgregaProductos.Visibility = Visibility.Visible;
            }
        }

        private void GuardarProductos_Click(object sender, RoutedEventArgs e)
        {
            GuardarCambios();
        }

        private void GuardarCambios()
        {
            if (IsLoader())
            {
                return;
            }
            MostrarLoader();

            /**
             * APLICAR NUMERO DE NOTA 
             */
            ApiClient.VentaDetalleProductosTransferir(new List<VentaProducto>(ViewModel.MoverProductos), 
                $"{ViewModel.VentaTransferir.NumeroCuenta}",
                "1",
                delegate (JObject respuesta)
            {
                OcultarLoader();
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
                    EnterTransferir();
                }
            });

            
        }

        private void EnterTransferir()
        {
            if (Extras == null)
            {
                Extras = new Dictionary<string, object?>();
            }

            Extras["EliminarProductos"] = new List<VentaProducto>(ViewModel.MoverProductos);

            ocultar();
            if (OnEnterAction != null)
            {
                OnEnterAction.Invoke(Extras);
            }
        }

        private void RevertirMovimiento_OnClick(object sender, RoutedEventArgs e)
        {
            if (IsLoader())
            {
                return;
            }
            if (Productos != null)
            {
                ViewModel.VentaProductos = new ObservableCollection<VentaProducto>(Productos);
                ViewModel.MoverProductos = new ObservableCollection<VentaProducto>();
                RevisarMensajes();
            }
        }

        private bool IsLoader()
        {
            return Loader.Visibility == Visibility.Visible;
        }

        private void OcultarLoader()
        {
            this.Dispatcher.Invoke(() =>
            {
                Loader.Visibility = Visibility.Collapsed;
                BotonGuardar.Visibility = Visibility.Visible;
            });
        }

        private void MostrarLoader()
        {
            this.Dispatcher.Invoke(() =>
            {
                Loader.Visibility = Visibility.Visible;
                BotonGuardar.Visibility = Visibility.Collapsed;
            });
        }
    }
}
