using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Tomate.Components.ViewModels.Venta;
using Tomate.Models.Ventas;
using Tomate.Models.Catalogo;
using Tomate.Utils;
using Tomate.Views;

namespace Tomate.Components.Dialogos.Venta
{
    /// <summary>
    /// Interaction logic for ProductoExtrasModal.xaml
    /// </summary>
    public partial class ProductoExtrasModal : UserControl
    {

        public Dictionary<string, object?>? Extras { get; set; } = new Dictionary<string, object?>();

        public ProductoExtrasViewModel ViewModel { get; set; }

        private Action<Dictionary<string, object?>>? OnEnterAction;
        private Action<Dictionary<string, object?>>? OnCloseAction;
        public bool DesactivarBlur { get; set; } = true;

        private VentaProducto VentaProducto;
        private Models.Ventas.Venta Venta;

        public ProductoExtrasModal()
        {
            ViewModel = new ProductoExtrasViewModel();
            InitializeComponent();
            DataContext = ViewModel;
        }

        public void mostrar(Models.Ventas.Venta venta, VentaProducto ventaProducto, ObservableCollection<VentaProducto> extras,
            Action<Dictionary<string, object?>>? enter,
            Action<Dictionary<string, object?>>? cerrar)
        {
            VentaProducto = ventaProducto;
            Venta = venta;
            GetExtras($"{ventaProducto.Producto?.CategoriaId}");
            setOnEnter(enter);
            setOnClose(cerrar);
            TituloExtras.Visibility = Visibility.Visible;
            ViewModel.ComplementosAgregados = new ObservableCollection<VentaProducto>();
            if (extras != null && extras.Count > 0)
            {
                foreach (var producto in extras)
                {
                    AgregarExtra(producto.Producto);
                }
            }
            
            DesactivarBlur = true;
            DialogoExtras.IsOpen = true;

        }

        private void GetExtras(string categoriaId)
        {
            new Thread(() =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    ViewModel.Complementos = Producto.todosExtras(categoriaId, Venta.ListaPreciosId);
                    if (ViewModel.Complementos.Count == 0)
                    {
                        ViewModel.MensajeNoExtras = Visibility.Visible;
                    }
                    else
                    {
                        ViewModel.MensajeNoExtras = Visibility.Collapsed;
                    }
                });
            }).Start();
            
        }

        public void setOnEnter(Action<Dictionary<string, object?>>? accion)
        {
            OnEnterAction = accion;
        }

        public void setOnClose(Action<Dictionary<string, object?>>? accion)
        {
            OnCloseAction = accion;
        }

        public void ocultar()
        {
            DialogoExtras.IsOpen = false;
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

        private void AgregarExtra_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ExtrasListado.SelectedIndex > -1)
            {
                AgregarExtra(ViewModel.Complementos[ExtrasListado.SelectedIndex]);
            }
        }

        private void AgregarExtra(Producto producto)
        {


            var ventaProducto = new VentaProducto();
            ventaProducto.VentaId = VentaProducto.VentaId;
            ventaProducto.UsuarioId = VentaProducto.UsuarioId;
            ventaProducto.VentaProductoPadreId = VentaProducto.Id;
            ventaProducto.ProductoId = $"{producto.Id}";
            ventaProducto.Producto = producto;
            ventaProducto.Precio = producto.Precio;
            ventaProducto.Cantidad = 1;
            ventaProducto.Importe = 1 * producto.Precio;

            ViewModel.ComplementosAgregados.Add(ventaProducto);
            if (ViewModel.ComplementosAgregados.Count > 0)
            {
                TituloExtras.Visibility = Visibility.Collapsed;
            }

            ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(ExtrasAgregadosListado, 0);
            scrollViewer.ScrollToBottom();

        }

        private void RemoverExtra(int index)
        {
            if (index < ViewModel.ComplementosAgregados.Count)
            {
                ViewModel.ComplementosAgregados.RemoveAt(index);
            }

            if (ViewModel.ComplementosAgregados.Count == 0)
            {
                TituloExtras.Visibility = Visibility.Visible;
            }
        }

        private void RemoverExtra_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ExtrasAgregadosListado.SelectedIndex > -1)
            {
                RemoverExtra(ExtrasAgregadosListado.SelectedIndex);
            }
        }


       
        private void GuardarExtras_Click(object sender, RoutedEventArgs e)
        {
            EnterExtras();
        }

      
        private void EnterExtras()
        {
            if (Extras == null)
            {
                Extras = new Dictionary<string, object?>();
            }

            Extras["Complementos"] = ViewModel.ComplementosAgregados;

            ocultar();
            if (OnEnterAction != null)
            {
                OnEnterAction.Invoke(Extras);
            }
        }

        private void EliminarExtras_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ComplementosAgregados.Clear();
        }

    }
}
