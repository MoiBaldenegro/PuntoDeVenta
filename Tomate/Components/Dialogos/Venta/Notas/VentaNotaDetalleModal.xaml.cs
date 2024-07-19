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
using Tomate.Components.ViewModels.Venta.Notas;
using Tomate.HttpRequest;
using Tomate.Models;
using Tomate.Models.Caja;
using Tomate.Models.Usuarios;
using Tomate.Models.Ventas;
using Tomate.Utils;
using Tomate.Views;

namespace Tomate.Components.Dialogos.Venta.Notas
{
    /// <summary>
    /// Interaction logic for VentaNotaDetalleModal.xaml.xaml
    /// </summary>
    public partial class VentaNotaDetalleModal : UserControl
    {

        public Dictionary<string, object?>? Extras { get; set; } = new Dictionary<string, object?>();

        public VentaNotaDetalleViewModel ViewModel { get; set; }

        public bool DesactivarBlur { get; set; } = true;

        private Action<Dictionary<string, object?>>? OnEnterAction;
        private Action<Dictionary<string, object?>>? OnCloseAction;

        public void setOnEnter(Action<Dictionary<string, object?>>? accion)
        {
            OnEnterAction = accion;
        }

        public void setOnClose(Action<Dictionary<string, object?>>? accion)
        {
            OnCloseAction = accion;
        }


        public VentaNotaDetalleModal()
        {
            ViewModel = new VentaNotaDetalleViewModel();

            InitializeComponent();

            DataContext = ViewModel;
        }

        public void mostrar(VentaNota ventaNota, Action<Dictionary<string, object?>>? enter = null,
            Action<Dictionary<string, object?>>? close = null)
        {
            setOnEnter(enter);
            setOnClose(close);

            ViewModel.VentaNota = ventaNota;

            DesactivarBlur = true;
            this.Dispatcher.Invoke(() =>
            {
                DialogoVentaDetalle.IsOpen = true;
            });
        }

       
        public void ocultar()
        {
            DialogoVentaDetalle.IsOpen = false;
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
    }
}
