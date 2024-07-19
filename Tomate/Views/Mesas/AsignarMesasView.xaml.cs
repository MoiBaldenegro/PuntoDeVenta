using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Tomate.Components.Dialogos.Filtros;
using Tomate.HttpRequest;
using Tomate.Models.Usuarios;
using Tomate.Models.Ventas;
using Tomate.Statics;
using Tomate.Utils;
using Tomate.ViewModels;
namespace Tomate.Views.Mesas
{
    /// <summary>
    /// Interaction logic for AsignarMesasView.xaml
    /// </summary>
    public partial class AsignarMesasView : UserControl
    {
        //Viewmodel de la clase con variables que se muestran en la vista
        public AsignarMesasViewModel ViewModel { get; set; }

        private Dictionary<string, ValorEditado> MesaUsuario = new Dictionary<string, ValorEditado>();
        private Dictionary<string, string> FiltrosEmpleados = new Dictionary<string, string>();

        public AsignarMesasView()
        {
            ViewModel = new AsignarMesasViewModel();
            //Texto por defecto en el tablero
            InitializeComponent();
            DataContext = ViewModel;

        }

        public void IniciarVista(Usuario? usuario)
        {
#pragma warning disable CS8601 // Possible null reference assignment.
            ViewModel.Usuario = usuario;
#pragma warning restore CS8601 // Possible null reference assignment.
            GetEmpleados();
            IniciarMesas();
        }

        public void LimpiarVista()
        {
            ScrollViewer scrollViewerMesas = (ScrollViewer)VisualTreeHelper.GetChild(MesasListado, 0);
            scrollViewerMesas.ScrollToTop();
            ScrollViewer scrollViewerEmpleado = (ScrollViewer)VisualTreeHelper.GetChild(EmpleadosListado, 0);
            scrollViewerEmpleado.ScrollToTop();

            MesaUsuario = new Dictionary<string, ValorEditado>();
            ViewModel.Mesas = new ObservableCollection<Mesa>();
            ViewModel.Empleados = new ObservableCollection<Usuario>();
            FiltrosEmpleados = new Dictionary<string, string>();
        }

        private void IniciarMesas()
        {
            MesaUsuario = new Dictionary<string, ValorEditado>();
            new Task(() =>
            {
                var mesas = Mesa.todas();

                this.Dispatcher.Invoke(() =>
                {
                    var ventas = VentasStatic.TodasVentas;
                    int index = 0;
                    foreach (var mesa in mesas)
                    {
                        var venta = ventas.Where(item => $"{item.NumeroCuenta}" == $"{mesa.NumeroMesa}").FirstOrDefault();
                        if (venta != null)
                        {
                            mesas[index].Venta = venta;
                            if (venta.Usuario != null)
                            {
                                mesas[index].UsuarioNombre = $"{venta.Usuario.Alias}";
                            }
                            
                        }
                        index++;
                    }

                    ViewModel.Mesas = mesas;
                    EmpleadosListado.SelectedIndex = -1;
                });
                foreach (var mesa in mesas)
                {
                    MesaUsuario[$"{mesa.NumeroMesa}"] = new ValorEditado(mesa.UsuarioId, mesa.UsuarioId, mesa);
                }

            }).Start();
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

                var empleados = Usuario.todosAtiendeMesas(perfilId, buscar);
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
         * Desactivar Feedback Boundary de touch
         */
        private void ScrollViewer_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
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

        private void AsignarMesa_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (MesasListado.SelectedIndex > -1 && EmpleadosListado.SelectedIndex > -1)
            {
                var usuario = ViewModel.Empleados[EmpleadosListado.SelectedIndex];
                var mesa = ViewModel.Mesas[MesasListado.SelectedIndex];

                if (mesa.Seleccionada)
                {
                    mesa.SetSeleccionada(false);
                    mesa.UsuarioId = null;
                    mesa.UsuarioNombre = null;
                }
                else
                {
                    mesa.SetSeleccionada(true);
                    mesa.UsuarioId = usuario.Id;
                    mesa.UsuarioNombre = usuario.Alias;
                }
                MesaUsuario[$"{mesa.NumeroMesa}"].ValorActual = mesa.UsuarioId;
                MesaUsuario[$"{mesa.NumeroMesa}"].Extra = mesa;
                SetMesa(mesa, MesasListado.SelectedIndex);
            }
        }

        private void SetMesa(Mesa mesa, int index)
        {
            ViewModel.Mesas.RemoveAt(index);
            ViewModel.Mesas.Insert(index, mesa);
        }

        private void ResetColoresMesa(Usuario? usuario)
        {
            var mesas = new ObservableCollection<Mesa>();

            for (int i = 0; i < ViewModel.Mesas.Count; i++)
            {
                var mesa = ViewModel.Mesas[i];
                if (usuario != null && mesa.UsuarioId == usuario.Id)
                {
                    mesa.SetSeleccionada(true);
                }
                else
                {
                    mesa.SetSeleccionada(false);
                }
                mesas.Add(mesa);
            }
            ViewModel.Mesas = mesas;
        }

        private void EmpleadosListado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EmpleadosListado.SelectedIndex > -1)
            {
                ResetColoresMesa(ViewModel.Empleados[EmpleadosListado.SelectedIndex]);
            }
            else
            {
                ResetColoresMesa(null);
            }
        }

        private void LimpiarTodo()
        {
            var mesas = new ObservableCollection<Mesa>();

            for (int i = 0; i < ViewModel.Mesas.Count; i++)
            {
                var mesa = ViewModel.Mesas[i];
                mesa.SetSeleccionada(false);
                if (mesa.Venta == null)
                {
                    mesa.UsuarioId = null;
                    mesa.UsuarioNombre = null;
                }
                MesaUsuario[$"{mesa.NumeroMesa}"].ValorActual = mesa.UsuarioId;
                MesaUsuario[$"{mesa.NumeroMesa}"].Extra = mesa;
                mesas.Add(mesa);
            }
            this.Dispatcher.Invoke(() =>
            {
                ViewModel.Mesas = mesas;
                EmpleadosListado.SelectedIndex = -1;
            });
        }
        private void Limpiar_OnClick(object sender, RoutedEventArgs e)
        {
            LimpiarTodo();
        }

        private void Revertir_OnClick(object sender, RoutedEventArgs e)
        {
            IniciarMesas();
        }

        private void Confirmar_OnClick(object sender, RoutedEventArgs e)
        {
            Dictionary<string, ValorEditado> mesasEditadas = MesaUsuario.Where(item => item.Value.Editado)
                .ToDictionary(item => item.Key, item => item.Value);
            if (mesasEditadas.Count > 0)
            {
                Dictionary<string, Mesa?> mesasActualizar = mesasEditadas.ToDictionary(item => item.Key, item => (Mesa?)item.Value.Extra);
                Dictionary<string, string?> mesasUsuariosEditadas = mesasEditadas.ToDictionary(item => item.Key, item => item.Value.ValorActual);

                ApiClient.MesasAsignar(mesasUsuariosEditadas, delegate (JObject respuesta)
                {
                    var error = (string?)respuesta["error"];
                    if (error != null)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            DialogoAlertaCm.show("Listado de cuentas", error);
                        });
                        return;
                    }
                    else
                    {
                        foreach (KeyValuePair<string, Mesa?> mesa in mesasActualizar)
                        {
                            if (mesa.Value != null)
                            {
                                mesa.Value.save();
                            }
                        }
                        this.Dispatcher.Invoke(() =>
                        {
                            EmpleadosListado.SelectedIndex = -1;
                        });
                        MesasStatic.CargarMesas();
                    }
                });
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    EmpleadosListado.SelectedIndex = -1;
                });
            }
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
