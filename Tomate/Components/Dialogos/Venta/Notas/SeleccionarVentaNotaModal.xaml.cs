using MaterialDesignThemes.Wpf;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Tomate.Components.ViewModels.Venta.Notas;
using Tomate.HttpRequest;
using Tomate.Models.Ventas;
using Tomate.Utils;
using Tomate.Views;

namespace Tomate.Components.Dialogos.Venta.Notas
{
    /// <summary>
    /// Interaction logic for SeleccionarVentaNotaModal.xaml
    /// </summary>
    public partial class SeleccionarVentaNotaModal : UserControl
    {

        public Dictionary<string, object?>? Extras { get; set; } = new Dictionary<string, object?>();

        public SeleccionarVentaNotaViewModel ViewModel { get; set; }

        public bool DesactivarBlur { get; set; } = true;

        public bool SeleccionarNotasNoProductos { get; set; } = true;

        private Action<Dictionary<string, object?>>? OnEnterAction;

        public void setOnEnter(Action<Dictionary<string, object?>>? accion)
        {
            OnEnterAction = accion;
        }



        public SeleccionarVentaNotaModal()
        {
            ViewModel = new SeleccionarVentaNotaViewModel();

            InitializeComponent();

            DataContext = ViewModel;
        }

        /**
         * permitirSeleccionarNotasCero permite seleccionar las notas con valor 0
         */
        public void mostrar(int numeroNotas, ObservableCollection<VentaNota> ventaNotas, 
            ObservableCollection<VentaProducto> productos,
            string titulo, bool seleccionarNotasNoProductos,
            Action<Dictionary<string, object?>>? enter = null)
        {
            setOnEnter(enter);

            TituloModal.Text = titulo;
            SeleccionarNotasNoProductos = seleccionarNotasNoProductos;
            ViewModel.NumeroNotas = numeroNotas;
            ViewModel.VentaNotas = ventaNotas;
            ViewModel.VentaProductos = new ObservableCollection<VentaProducto>(productos);

            /*new Task(() =>
            {
                var notas = new ObservableCollection<VentaNota>();
                for (int i = 1; i <= numeroNotas; i++)
                {
                    notas.Add(new VentaNota($"{i}"));
                }
                this.Dispatcher.Invoke(() =>
                {
                    ViewModel.VentaNotas = notas;
                });
                ActualizarTotales();
            }).Start();*/

            DesactivarBlur = true;

            DialogoSeleccionarNota.IsOpen = true;
        }

        
       

        /*private void ActualizarTotales()
        {
            new Task(() =>
            {
                var notas = ViewModel.CalcularNotasTotales();
                this.Dispatcher.Invoke(() =>
                {
                    ViewModel.VentaNotas = notas;
                });
            }).Start();
        }*/

        public void ocultar()
        {
            DialogoSeleccionarNota.IsOpen = false;
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
            ViewModel.VentaProductos = new ObservableCollection<VentaProducto>();
            ViewModel.VentaNotas = new ObservableCollection<VentaNota>();
            if (DesactivarBlur)
            {
                this.Dispatcher.Invoke(() =>
                {
                    getWindow().RemoverBlur();
                });
            }

        }

      
        private void VentaNotasListado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VentaNotasListado.SelectedIndex > -1)
            {
                EnterSeleccionarVentaNota();
            }
        }

        private void EnterSeleccionarVentaNota()
        {
            VentaNota ventaNota = ViewModel.VentaNotas[VentaNotasListado.SelectedIndex];
            VentaNotasListado.SelectedIndex = -1;

            string? error = null;
            if (!SeleccionarNotasNoProductos && ventaNota.Subtotal == 0)
            {
                error = "Nota sin productos";
            }
            else if (ventaNota.IsPagada)
            {
                error = "Nota pagada";
            }

            if (error != null)
            {
                this.Dispatcher.Invoke(() =>
                {
                    ToastNotification.show(error, ToastNotification.SHORT);
                });
                return;
            }

            
            if (Extras == null)
            {
                Extras = new Dictionary<string, object?>();
            }
            Extras["TotalNota"] = (float)ventaNota.Total;
            Extras["VentaNota"] = ventaNota;
            Extras["NumeroNota"] = $"{ventaNota.NumeroNota}";
            ocultar();
            if (OnEnterAction != null)
            {
                OnEnterAction.Invoke(Extras);
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
    }
}
