using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Tomate.Components.ViewModels.Filtros;
using Tomate.Models.Usuarios;
using Tomate.Models.Ventas;
using Tomate.Models;
using Tomate.Utils;
using Tomate.Views;

namespace Tomate.Components.Dialogos.Filtros
{
    /// <summary>
    /// Interaction logic for FiltrosCuentasModal.xaml
    /// </summary>
    public partial class FiltrosCuentasModal : UserControl
    {

        public Dictionary<string, object?>? Extras { get; set; } = new Dictionary<string, object?>();

        public FiltrosCuentasViewModel ViewModel { get; set; }

        public bool DesactivarBlur { get; set; } = true;

        private Action<Dictionary<string, object?>>? OnEnterAction;

        public void setOnEnter(Action<Dictionary<string, object?>>? accion)
        {
            OnEnterAction = accion;
        }

        private Dictionary<string, string> Seleccionados = new Dictionary<string, string>();
        private List<Usuario> Usuarios { get; set; } = new List<Usuario>();
        private List<VentaTipo> VentasTipos { get; set; } = new List<VentaTipo>();
        private List<FiltroOrden> Ordenes { get; set; } = new List<FiltroOrden>();
        

        public FiltrosCuentasModal()
        {
            ViewModel = new FiltrosCuentasViewModel();

            InitializeComponent();

            DataContext = ViewModel;

            Ordenes.Add(new FiltroOrden("Número cuenta ascendente", "num_cuenta", "asc"));
            Ordenes.Add(new FiltroOrden("Número cuenta descendente", "num_cuenta", "desc"));
            Ordenes.Add(new FiltroOrden("Tiempo cuenta ascendente", "created_at", "desc"));
            Ordenes.Add(new FiltroOrden("Tiempo cuenta descendente", "created_at", "asc"));
        }

        public void mostrar(Dictionary<string, string> filtros, Action<Dictionary<string, object?>>? enter = null)
        {
            setOnEnter(enter);
            DesactivarBlur = true;

            Seleccionados = filtros;


            DialogoFiltrosCuentas.IsOpen = true;
            ActualizarEmpleados();
            ActualizarVentasTipos();
        }

        private void ActualizarEmpleados()
        {
            Usuarios = Usuario.todosAtiendeMesas().ToList();   

            this.Dispatcher.Invoke(() =>
            {
                UsuariosCombobox.IsEnabled = true;
                UsuariosCombobox.Items.Clear();
                UsuariosCombobox.Items.Add("Todos");
                string usuarioId = "";
                if (Seleccionados.ContainsKey("usuario_id"))
                {
                    usuarioId = Seleccionados["usuario_id"];
                }

                int indexUsuarioSelected = 0;
                int indexUsuario = 0;
                foreach (Usuario usuario in Usuarios)
                {
                    indexUsuario++;
                    if (usuarioId == usuario.Id)
                    {
                        indexUsuarioSelected = indexUsuario;
                    }
                    UsuariosCombobox.Items.Add($"{usuario.Codigo} {usuario.Alias}");
                }
                UsuariosCombobox.SelectedIndex = indexUsuarioSelected;

                string ordenSeleccionado = "";
                if (Seleccionados.ContainsKey("orden"))
                {
                    ordenSeleccionado = Seleccionados["orden"];
                }

                OrdenarCombobox.IsEnabled = true;
                OrdenarCombobox.Items.Clear();
                int indexOrdenSelected = 0;
                int indexOrden = 0;
                foreach (FiltroOrden orden in Ordenes)
                {
                    if (ordenSeleccionado == $"{orden.Columna}|{orden.Orden}")
                    {
                        indexOrdenSelected = indexOrden;
                    }
                    OrdenarCombobox.Items.Add(orden.Nombre);
                    indexOrden++;
                }
                OrdenarCombobox.SelectedIndex = indexOrdenSelected;
            });
            
        }

        private void ActualizarVentasTipos()
        {
            VentasTipos = VentaTipo.todas().ToList();

            this.Dispatcher.Invoke(() =>
            {
                VentasTiposCombobox.IsEnabled = true;
                VentasTiposCombobox.Items.Clear();
                VentasTiposCombobox.Items.Add("Todas");
                string ventaTipoId = "";
                if (Seleccionados.ContainsKey("venta_tipo_id"))
                {
                    ventaTipoId = Seleccionados["venta_tipo_id"];
                }

                int indexVentaTipoSelected = 0;
                int indexVentaTipo = 0;
                foreach (VentaTipo ventaTipo in VentasTipos)
                {
                    indexVentaTipo++;
                    if (ventaTipoId == ventaTipo.Id)
                    {
                        indexVentaTipoSelected = indexVentaTipo;
                    }
                    VentasTiposCombobox.Items.Add($"{ventaTipo.Nombre}");
                }
                VentasTiposCombobox.SelectedIndex = indexVentaTipoSelected;
            });
        }

        public void ocultar()
        {
                DialogoFiltrosCuentas.IsOpen = false;
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

        private void AplicarFiltros_Click(object sender, RoutedEventArgs e)
        {
            EnterFiltros();
        }

        private void EnterFiltros()
        {
            if (Extras == null)
            {
                Extras = new Dictionary<string, object?>();
            }

            
            if (VentasTiposCombobox.SelectedIndex > 0)
            {
                Seleccionados["venta_tipo_id"] = VentasTipos[VentasTiposCombobox.SelectedIndex - 1].Id;
            }
            else
            {
                Seleccionados.Remove("venta_tipo_id");
            }

            if (UsuariosCombobox.SelectedIndex > 0)
            {
                Seleccionados["usuario_id"] = Usuarios[UsuariosCombobox.SelectedIndex - 1].Id;
            }
            else
            {
                Seleccionados.Remove("usuario_id");
            }

            if (OrdenarCombobox.SelectedIndex > 0)
            {
                var orden = Ordenes[OrdenarCombobox.SelectedIndex];
                Seleccionados["orden"] = $"{orden.Columna}|{orden.Orden}";
            }
            else
            {
                Seleccionados.Remove("orden");
            }

            Extras["Filtros"] = Seleccionados;

            ocultar();
            if (OnEnterAction != null)
            {
                OnEnterAction.Invoke(Extras);
            }
        }
    }
}
