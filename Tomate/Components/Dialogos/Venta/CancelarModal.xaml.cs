using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Tomate.Components.ViewModels.Venta;
using Tomate.Models.Ventas;
using Tomate.Utils;
using Tomate.Views;

namespace Tomate.Components.Dialogos.Venta
{
    /// <summary>
    /// Interaction logic for CancelarModal.xaml
    /// </summary>
    public partial class CancelarModal : UserControl
    {

        public Dictionary<string, object?>? Extras { get; set; } = new Dictionary<string, object?>();

        public CancelarViewModel ViewModel { get; set; }

        public static readonly RoutedEvent OnCloseEvent = EventManager.RegisterRoutedEvent("OnClose", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(CancelarModal));

        public event RoutedEventHandler OnClose
        {
            add { AddHandler(OnCloseEvent, value); }
            remove { RemoveHandler(OnCloseEvent, value); }
        }

        public static readonly RoutedEvent OnMotivoEvent = EventManager.RegisterRoutedEvent("OnMotivo", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(CancelarModal));

        public event RoutedEventHandler OnMotivo
        {
            add { AddHandler(OnMotivoEvent, value); }
            remove { RemoveHandler(OnMotivoEvent, value); }
        }

        private int? Index { get; set; }
        private bool IsVenta { get; set; }
        public bool DesactivarBlur { get; set; } = true;

        public CancelarModal()
        {
            ViewModel = new CancelarViewModel();

            InitializeComponent();

            DataContext = ViewModel;
        }
        public void mostrar()
        {
            mostrar(true, null, null);
        }
        public void mostrar(bool isVenta, VentaProducto? ventaProducto, int? index)
        {
            Index = index;
            IsVenta = isVenta;
            DesactivarBlur = true;
            ViewModel.VentaProducto = ventaProducto;
            if (IsVenta)
            {
                TituloModal.Text = "Cancelar cuenta";
            }
            else
            {
                TituloModal.Text = "Cancelar producto";
            }
            GetMotivosCancelacion();
            DialogoCancelar.IsOpen = true;

        }

        private void GetMotivosCancelacion()
        {
            new Task(() =>
            {
                var motivos = MotivoCancelacion.todos();
                this.Dispatcher.Invoke(() =>
                {
                    ViewModel.MotivosCancelacion = motivos;
                    var OtroMotivo = new MotivoCancelacion();
                    OtroMotivo.Nombre = "Otro";
                    ViewModel.MotivosCancelacion.Add(OtroMotivo);
                });
            }).Start();
        }


        public void ocultar()
        {
            DialogoCancelar.IsOpen = false;
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

            var args = new EventClickArgs(OnCloseEvent);
            RaiseEvent(args);
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

        private void MotivosCancelacion_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (MotivosCancelacion.SelectedIndex > -1)
            {
                EnviarMotivo(ViewModel.MotivosCancelacion[MotivosCancelacion.SelectedIndex].Id);
            }
        }

        private void EnviarMotivo(string? motivoId)
        {
            var args = new EventClickArgs(OnMotivoEvent);
            if (Extras == null)
            {
                Extras = new Dictionary<string, object?>();
            }
            Extras["IsVenta"] = IsVenta;
            if (!IsVenta)
            {
                Extras["Index"] = Index;
            }
            Extras["MotivoId"] = motivoId;
            DesactivarBlur = motivoId != null;
            args.Extras = Extras;
            ocultar();
            RaiseEvent(args);
        }
    }
}
