using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Tomate.HttpRequest;
using Tomate.Models.Catalogo;
using Tomate.Models.Usuarios;
using Tomate.Utils;
using Tomate.ViewModels.Tableros;

namespace Tomate.Views.Tableros
{
    /// <summary>
    /// Interaction logic for DesactivarProductosView.xaml
    /// </summary>
    public partial class DesactivarProductosView : UserControl
    {
        //Viewmodel de la clase con variables que se muestran en la vista
        public DesactivarProductosViewModel ViewModel { get; set; }

        //Constructor
        public DesactivarProductosView()
        {
            ViewModel = new DesactivarProductosViewModel();
            InitializeComponent();
            DataContext = ViewModel;
        }

        public void IniciarVista(Usuario? usuario)
        {
#pragma warning disable CS8601 // Possible null reference assignment.
            ViewModel.Usuario = usuario;
#pragma warning restore CS8601 // Possible null reference assignment.

            getTableros();
        }

        public void LimpiarVista()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            ViewModel.Usuario = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            ViewModel.ProductosModificados = new Dictionary<string, bool>();
            ViewModel.ProductosEstatus = new Dictionary<string, bool>();
            ViewModel.ProductosDesactivados = new ObservableCollection<Producto>();
            ViewModel.Productos = new ObservableCollection<Producto>();
        }

        private void getTableros()
        {
            new Task(() =>
            {
                ViewModel.CargarProductosDesactivados();
            }).Start();

            new Task(() =>
            {
                ViewModel.CargarTableros(true);
            }).Start();

            //new Task(() =>
            //{
                //ViewModel.CargarProductosEstatus();
            //}).Start();

        }

        private void BotonAtras_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.ProductosModificados.Count > 0)
            {
                this.Dispatcher.Invoke(() =>
                {
                    DialogoAlertaCm.show("¿Descartar cambios?", true, delegate ()
                    {
                        Atras();
                    });
                });
            }
            else
            {
                Atras();
            }
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

        private void SeleccionarTablero_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ViewModel.setTableroSeleccionado(TablerosListado.SelectedIndex);
        }

        private void DesactivarProducto_OnClick(object sender, RoutedEventArgs e)
        {
            EventClickArgs args = (EventClickArgs)e;
            var Extras = args.Extras;

            int index = (int)Extras["Index"];
            var producto = ViewModel.Productos[index];
            if (!producto.TableroActivo)
            {
                producto.TableroActivo = true;
                var indexDesactivado = ViewModel.ProductosDesactivados
                    .IndexOf(ViewModel.ProductosDesactivados
                    .Where(item => $"{item.TableroProductoId}" == $"{producto.TableroProductoId}").FirstOrDefault());
                if (indexDesactivado > -1)
                {
                    ViewModel.ProductosDesactivados.RemoveAt(indexDesactivado);
                }
            }
            else
            {
                producto.TableroActivo = false;
                ViewModel.ProductosDesactivados.Insert(0, producto);
            }

            if (producto.TableroActivo != ViewModel.GetProductoActivo($"{producto.TableroProductoId}"))
            {
                if (!ViewModel.ProductosModificados.ContainsKey($"{producto.TableroProductoId}"))
                {
                    ViewModel.ProductosModificados.Add($"{producto.TableroProductoId}", producto.TableroActivo);
                }
            }
            else
            {
                ViewModel.ProductosModificados.Remove($"{producto.TableroProductoId}");
            }
           
            ViewModel.Productos.RemoveAt(index);
            ViewModel.Productos.Insert(index, producto);

        }

        private void ActivarProducto_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var index = ProductosDesactivadosListado.SelectedIndex;
            if (index > -1)
            {
                var productoDesactivado = ViewModel.ProductosDesactivados[index];
                ViewModel.ProductosDesactivados.RemoveAt(index);
                if (ViewModel.ProductosModificados.ContainsKey($"{productoDesactivado.TableroProductoId}"))
                {
                    ViewModel.ProductosModificados.Remove($"{productoDesactivado.TableroProductoId}");
                }
                else
                {
                    ViewModel.ProductosModificados.Add($"{productoDesactivado.TableroProductoId}", true);
                }

                int indexTablero = ViewModel.Productos.IndexOf(ViewModel.Productos.Where(item => $"{item.TableroProductoId}" 
                == $"{productoDesactivado.TableroProductoId}").FirstOrDefault());
                if (indexTablero > -1)
                {
                    var producto = ViewModel.Productos[indexTablero];
                    producto.TableroActivo = true;
                    ViewModel.Productos.RemoveAt(indexTablero);
                    ViewModel.Productos.Insert(indexTablero, producto);
                }
            }
        }


        private void BotonGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.ProductosModificados.Count > 0)
            {
                ApiClient.TablerosProductosActivarDesactivar(ViewModel.ProductosModificados, delegate (JObject respuesta)
                {
                    var error = (string?)respuesta["error"];
                    if (error != null)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            DialogoAlertaCm.show("Tablero productos", error);
                        });
                        return;
                    }
                    else
                    {
                        foreach (var tablero in ViewModel.ProductosModificados)
                        {
                            TableroProducto.actualizarActivo(tablero.Key, tablero.Value);
                        }
                        Atras();
                    }
                });
            }
            else
            {
                Atras();
            }
        }
    }
}
