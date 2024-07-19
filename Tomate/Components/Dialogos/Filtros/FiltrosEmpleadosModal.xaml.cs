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
    /// Interaction logic for FiltrosEmpleadosModal.xaml
    /// </summary>
    public partial class FiltrosEmpleadosModal : UserControl
    {

        public Dictionary<string, object?>? Extras { get; set; } = new Dictionary<string, object?>();

        public FiltrosEmpleadosViewModel ViewModel { get; set; }

        public bool DesactivarBlur { get; set; } = true;

        private Action<Dictionary<string, object?>>? OnEnterAction;

        public void setOnEnter(Action<Dictionary<string, object?>>? accion)
        {
            OnEnterAction = accion;
        }

        private Dictionary<string, string> Seleccionados = new Dictionary<string, string>();
        private List<Perfil> Perfiles { get; set; } = new List<Perfil>();
        /*private List<VentaTipo> VentasTipos { get; set; } = new List<VentaTipo>();
        private List<FiltroOrden> Ordenes { get; set; } = new List<FiltroOrden>();*/
        

        public FiltrosEmpleadosModal()
        {
            ViewModel = new FiltrosEmpleadosViewModel();

            InitializeComponent();

            DataContext = ViewModel;

            /*Ordenes.Add(new FiltroOrden("Número cuenta ascendente", "num_cuenta", "asc"));
            Ordenes.Add(new FiltroOrden("Número cuenta descendente", "num_cuenta", "desc"));
            Ordenes.Add(new FiltroOrden("Tiempo cuenta ascendente", "created_at", "desc"));
            Ordenes.Add(new FiltroOrden("Tiempo cuenta descendente", "created_at", "asc"));*/
        }

        public void mostrar(Dictionary<string, string> filtros, Action<Dictionary<string, object?>>? enter = null)
        {
            setOnEnter(enter);
            DesactivarBlur = true;

            Seleccionados = filtros;


            DialogoFiltrosEmpleados.IsOpen = true;
            ActualizarPerfiles();;

            if (Seleccionados.ContainsKey("buscar"))
            {
                BuscarUsuario.Text = Seleccionados["buscar"];
            }
            else
            {
                BuscarUsuario.Text = "";
            }
        }

        private void ActualizarPerfiles()
        {
            Perfiles = Perfil.todos();  

            this.Dispatcher.Invoke(() =>
            {
                PerfilesCombobox.IsEnabled = true;
                PerfilesCombobox.Items.Clear();
                PerfilesCombobox.Items.Add("Todos");
                string perfilId = "";
                if (Seleccionados.ContainsKey("perfil_id"))
                {
                    perfilId = Seleccionados["perfil_id"];
                }
                
                int indexPerfilSelected = 0;
                int indexPerfil = 0;
                foreach (Perfil perfil in Perfiles)
                {
                    indexPerfil++;
                    if (perfilId == perfil.Id)
                    {
                        indexPerfilSelected = indexPerfil;
                    }
                    PerfilesCombobox.Items.Add($"{perfil.Nombre}");
                }
                PerfilesCombobox.SelectedIndex = indexPerfilSelected;

            });
            
        }

        public void ocultar()
        {
            DialogoFiltrosEmpleados.IsOpen = false;
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

        private void EditarBuscar_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DialogoFiltrosEmpleados.IsOpen = false;

            this.Dispatcher.Invoke(() =>
            {
                var datos = new Dictionary<string, object?>();
                datos["Titulo"] = "Ingresa buscar";
                datos["Valor"] = $"{BuscarUsuario.Text}";
                datos["DesactivarBlur"] = false;
                getWindow().MostrarTeclado(datos, delegate (Dictionary<string, object?> respuesta)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        BuscarUsuario.Text = $"{(string?)respuesta["Valor"]}";
                    });

                    DialogoFiltrosEmpleados.IsOpen = true;
                }, delegate (Dictionary<string, object?> respuesta)
                {
                    DialogoFiltrosEmpleados.IsOpen = true;
                });
            });
        }

        private void EnterFiltros()
        {
            if (Extras == null)
            {
                Extras = new Dictionary<string, object?>();
            }

            
            if (PerfilesCombobox.SelectedIndex > 0)
            {
                Seleccionados["perfil_id"] = Perfiles[PerfilesCombobox.SelectedIndex - 1].Id;
            }
            else
            {
                Seleccionados.Remove("perfil_id");
            }

            if (BuscarUsuario.Text != "" && BuscarUsuario.Text.Length > 0)
            {
                Seleccionados["buscar"] = BuscarUsuario.Text;
            }
            else
            {
                Seleccionados.Remove("buscar");
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
