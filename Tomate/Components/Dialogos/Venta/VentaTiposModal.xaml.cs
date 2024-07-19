using MaterialDesignThemes.Wpf;
using System;
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
    /// Interaction logic for VentaTiposModal.xaml
    /// </summary>
    public partial class VentaTiposModal : UserControl
    {

        public Dictionary<string, object?>? Extras { get; set; } = new Dictionary<string, object?>();

        public VentaTiposViewModel ViewModel { get; set; }

        public static readonly RoutedEvent OnCloseEvent = EventManager.RegisterRoutedEvent("OnClose", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(VentaTiposModal));

        public event RoutedEventHandler OnClose
        {
            add { AddHandler(OnCloseEvent, value); }
            remove { RemoveHandler(OnCloseEvent, value); }
        }

        public static readonly RoutedEvent OnVentaTipoEvent = EventManager.RegisterRoutedEvent("OnVentaTipo", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(VentaTiposModal));

        public event RoutedEventHandler OnVentaTipo
        {
            add { AddHandler(OnVentaTipoEvent, value); }
            remove { RemoveHandler(OnVentaTipoEvent, value); }
        }

        private Action<Dictionary<string, object?>>? OnEnterAction;
        private Action<Dictionary<string, object?>>? OnCloseAction;

        public bool DesactivarBlur { get; set; } = true;

        public bool Predeterminado { get; set; } = true;

        public VentaTiposModal()
        {
            ViewModel = new VentaTiposViewModel();

            InitializeComponent();

            DataContext = ViewModel;
        }

        public void mostrar(Dictionary<string, object?> datos)
        {
            mostrar();
            if (datos.ContainsKey("Predeterminado"))
            {
#pragma warning disable CS8605 // Unboxing a possibly null value.
                Predeterminado = (bool)datos["Predeterminado"];
#pragma warning restore CS8605 // Unboxing a possibly null value.
            }
            Extras = datos;
            GetVentaTipos();
        }

        public void mostrar()
        {
            Predeterminado = true;
            setOnEnter(null);
            setOnClose(null);
            DesactivarBlur = true;
            DialogoVentaTipos.IsOpen = true;
        }

        public void setOnEnter(Action<Dictionary<string, object?>>? accion)
        {
            OnEnterAction = accion;
        }

        public void setOnClose(Action<Dictionary<string, object?>>? accion)
        {
            OnCloseAction = accion;
        }

        private void GetVentaTipos()
        {
            new Task(() =>
            {
                var ventaTipos = VentaTipo.todas(Predeterminado);
                this.Dispatcher.Invoke(() =>
                {
                    ViewModel.VentaTipos = ventaTipos;
                });
            }).Start();
        }


        public void ocultar()
        {
            DialogoVentaTipos.IsOpen = false;
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
            if (Extras == null)
            {
                Extras = new Dictionary<string, object?>();
            }
            args.Extras = Extras;
            RaiseEvent(args);

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

        private void VentaTipo_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (VentaTipos.SelectedIndex > -1)
            {
                EnviarVentaTipo(ViewModel.VentaTipos[VentaTipos.SelectedIndex].Id);
            }
        }

        private void EnviarVentaTipo(string ventaTipoId)
        {
            var args = new EventClickArgs(OnVentaTipoEvent);
            if (Extras == null)
            {
                Extras = new Dictionary<string, object?>();
            }
            Extras["VentaTipoId"] = ventaTipoId;
            args.Extras = Extras;
            ocultar();
            RaiseEvent(args);
            if (OnEnterAction != null)
            {
                OnEnterAction.Invoke(Extras);
            }
        }
    }
}
