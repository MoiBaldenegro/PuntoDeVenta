using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Tomate.Components.ViewModels.Caja;
using Tomate.Utils;
using Tomate.Views;

namespace Tomate.Components.Dialogos.Caja
{
    /// <summary>
    /// Interaction logic for FormasPagosModal.xaml
    /// </summary>
    public partial class FormasPagosModal : UserControl
    {

        public Dictionary<string, object?>? Extras { get; set; } = new Dictionary<string, object?>();

        public FormasPagosViewModel ViewModel { get; set; }

        public bool DesactivarBlur { get; set; } = true;

        private Action<Dictionary<string, object?>>? OnEnterAction;

        public void setOnEnter(Action<Dictionary<string, object?>>? accion)
        {
            OnEnterAction = accion;
        }

        public FormasPagosModal()
        {
            ViewModel = new FormasPagosViewModel();

            InitializeComponent();

            DataContext = ViewModel;
        }

        public void mostrar(Action<Dictionary<string, object?>>? enter = null)
        {
            setOnEnter(enter);
            DesactivarBlur = true;

            ViewModel.IniciarFormasPagos();

            DialogoFormasPagos.IsOpen = true;
        }

        public void ocultar()
        {
            DialogoFormasPagos.IsOpen = false;
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
            //ViewModel.VentaProductos = new ObservableCollection<VentaProducto>();
            //ViewModel.Notas = new ObservableCollection<Nota>();
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

       

        private void EnterFormaPago(string formaPagoId)
        {
            if (Extras == null)
            {
                Extras = new Dictionary<string, object?>();
            }

            Extras["FormaPagoId"] = formaPagoId;

            ocultar();
            if (OnEnterAction != null)
            {
                OnEnterAction.Invoke(Extras);
            }
        }

        private void FormasPagosListado_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (FormasPagosListado.SelectedIndex > -1)
            {
                EnterFormaPago($"{ViewModel.FormasPagos[FormasPagosListado.SelectedIndex].Id}");
            }
        }
    }
}
