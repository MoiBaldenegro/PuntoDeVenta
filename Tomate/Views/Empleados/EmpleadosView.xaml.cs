using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tomate.Components.Dialogos.Filtros;
using Tomate.HttpRequest;
using Tomate.Models.Usuarios;
using Tomate.Utils;
using Tomate.ViewModels.Cuentas;

namespace Tomate.Views.Empleados
{
    /// <summary>
    /// Lógica de interacción para EmpleadosView.xaml
    /// </summary>
    public partial class EmpleadosView : UserControl
    {

        //Viewmodel de la clase con variables que se muestran en la vista
        public EmpleadosViewModel ViewModel { get; set; }

        private Dictionary<string, string> FiltrosEmpleados = new Dictionary<string, string>();

        public EmpleadosView()
        {
            ViewModel = new EmpleadosViewModel();
            InitializeComponent();
            DataContext = ViewModel;
        }

        public void IniciarVista(Usuario? usuario)
        {
#pragma warning disable CS8601 // Possible null reference assignment.
            ViewModel.Usuario = usuario;
#pragma warning restore CS8601 // Possible null reference assignment.
            GetEmpleados();
        }

        public void LimpiarVista()
        {
            FiltrosEmpleados = new Dictionary<string, string>();
            ViewModel.Empleados = new ObservableCollection<Usuario>();
        }

        private void GetEmpleados()
        {
            new Task(() =>
            {
                string? perfilId = null;
                string? buscar = null;

                if (FiltrosEmpleados.ContainsKey("perfil_id"))
                {
                    perfilId = FiltrosEmpleados["perfil_id"];
                }

                if (FiltrosEmpleados.ContainsKey("buscar"))
                {
                    buscar = FiltrosEmpleados["buscar"];
                }

                var empleados = Usuario.todos(false, perfilId, buscar);
                this.Dispatcher.Invoke(() =>
                {
                    ViewModel.Empleados = empleados;
                });
            }).Start();
        }

        private void BotonAtras_Click(object sender, RoutedEventArgs e)
        {
            Atras();
        }

        private void Atras()
        {
            this.Dispatcher.Invoke(() =>
            {
                getWindow().MostrarListadoVentas();
            });
        }

        /**
         * Minimiza la aplicación
         */
        private void BotonMinimizar_Click(object sender, RoutedEventArgs e)
        {

            this.Dispatcher.Invoke(() =>
            {
                getWindow().MinimizarVentana();
            });
        }

        /**
         * Muestra el dialogo para confirmar la salida del programa
         */
        private void BotonCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                getWindow().MostrarDialogoCerrarPrograma();
            });
        }

        /**
         * Obtiene la ventana principal del programa
         */
        private MainWindow getWindow()
        {
            return WindowUtils.getMainWindow();
        }

        /**
         * Desactivar Feedback Boundary de touch
         */
        private void ScrollViewer_ManipulationBoundaryFeedback(object sender, System.Windows.Input.ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        private void EmpleadoEditar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (EmpleadosListado.SelectedIndex > -1)
            {
                this.Dispatcher.Invoke(() =>
                {

                    EmpleadoDetalleModal.mostrar(ViewModel.Empleados[EmpleadosListado.SelectedIndex]);
                });
            }
        }

        private void EmpleadoHuellas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (EmpleadosListado.SelectedIndex > -1)
            {
                this.Dispatcher.Invoke(() =>
                {

                    if (getWindow().LectorHuellaDisponible())
                    {
                        EmpleadoHuellaModal.mostrar($"{ViewModel.Empleados[EmpleadosListado.SelectedIndex].Id}");
                    }
                    else
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            DialogoAlertaCm.show("Lector de huellas no disponible");
                        });
                    }
                });
            }

        }

        public void RegistroHuellaContador(int contador)
        {
            EmpleadoHuellaModal.ContadorRegistroHuella(contador);
        }

        public void HuellaRegistrada(bool success, byte[]? huella)
        {
            EmpleadoHuellaModal.GuardarHuella(success, huella);
        }

        private void EmpleadoEliminar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                DialogoAlertaCm.show("¿Eliminar empleado?", true, delegate ()
                {
                    EliminarEmpleado($"{ViewModel.Empleados[EmpleadosListado.SelectedIndex].Id}");
                });
            });
        }

        private void EliminarEmpleado(string usuarioId)
        {
            ApiClient.UsuarioEliminar(usuarioId, delegate (JObject respuesta)
            {
                var error = (string?)respuesta["error"];
                if (error != null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        DialogoAlertaCm.show("Eliminar usuario", error);
                    });
                    return;
                }
                else
                {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    var usuario = new Usuario((JObject)respuesta["usuario"]);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                    usuario.save();
                    this.Dispatcher.Invoke(() =>
                    {
                        GetEmpleados();
                    });
                }
            });
        }

        private void NuevoEmpleado_OnClick(object sender, RoutedEventArgs e)
        {
            EmpleadoDetalleModal.mostrar();
        }

        private void EmpleadoDetalleModal_OnActualizar(object sender, RoutedEventArgs e)
        {
            GetEmpleados();
        }

        private void HeaderBotonFiltros_Click(object sender, RoutedEventArgs e)
        {
            FiltrosEmpleadosModal.mostrar(FiltrosEmpleados, delegate (Dictionary<string, object?> respuesta) {
                FiltrosEmpleados = (Dictionary<string, string>)respuesta["Filtros"];
                GetEmpleados();
            });
        }
    }
}
