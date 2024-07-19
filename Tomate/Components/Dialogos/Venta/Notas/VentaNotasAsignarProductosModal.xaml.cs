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
using Tomate.HttpRequest;
using Tomate.Models;
using Tomate.Models.Ventas;
using Tomate.Utils;
using Tomate.Views;
using Tomate.Components.ViewModels.Venta.Notas;

namespace Tomate.Components.Dialogos.Venta.Notas
{
    /// <summary>
    /// Interaction logic for VentaNotasAsignarProductosModal.xaml
    /// </summary>
    public partial class VentaNotasAsignarProductosModal : UserControl
    {

        public Dictionary<string, object?>? Extras { get; set; } = new Dictionary<string, object?>();

        public VentaNotasAsignarProductosViewModel ViewModel { get; set; }

        public bool DesactivarBlur { get; set; } = true;

        private Action<Dictionary<string, object?>>? OnEnterAction;
        private Action<Dictionary<string, object?>>? OnCloseAction;

        private Dictionary<string, int> ProductosNotaRevertir { get; set; } = new Dictionary<string, int>();

        private string? NumeroNotaSeleccionada { get; set; } = null;

        private bool ActualizandoNumeroNotas = false;

        public void setOnEnter(Action<Dictionary<string, object?>>? accion)
        {
            OnEnterAction = accion;
        }

        public void setOnClose(Action<Dictionary<string, object?>>? accion)
        {
            OnCloseAction = accion;
        }



        public VentaNotasAsignarProductosModal()
        {
            ViewModel = new VentaNotasAsignarProductosViewModel();

            InitializeComponent();

            DataContext = ViewModel;
        }

        public void mostrar(Models.Ventas.Venta venta, ObservableCollection<VentaNota> ventaNotas,
            ObservableCollection<VentaProducto> productos,
            Action<Dictionary<string, object?>>? enter = null,
            Action<Dictionary<string, object?>>? close = null)
        {
            setOnEnter(enter);
            setOnClose(close);
            ViewModel.Venta = venta;
            ViewModel.VentaNotas = ventaNotas;

            ViewModel.VentaProductos = new ObservableCollection<VentaProducto>(productos.Where(item => item.IsDisabled != null));
            ViewModel.ActualizarTotalNotas();
            SetProductosRevertir(new List<VentaProducto>(ViewModel.VentaProductos));

            //ActualizarNumeroNotas((int)Venta.NumeroNotas, true);

            DesactivarBlur = true;
            this.Dispatcher.Invoke(() =>
            {
                DialogoSeleccionarNotas.IsOpen = true;
            });
        }

        /*private void ActualizarNumeroNotas(int numeroNotas, bool actualizarTotales = false)
        {
            new Task(() =>
            {
                var notas = new ObservableCollection<VentaNota>(ViewModel.VentaNotas);
                if (notas.Count + 1 > numeroNotas)
                {
                    for (int i = notas.Count; notas.Count != numeroNotas; i--)
                    {
                        notas.RemoveAt(i - 1);
                    }
                }
                else
                {
                    for (int i = notas.Count + 1; i <= numeroNotas; i++)
                    {
                        notas.Add(new VentaNota($"{i}"));
                    }
                }
               
                this.Dispatcher.Invoke(() =>
                {
                    ViewModel.VentaNotas = notas;
                });
                if (actualizarTotales)
                {
                    ActualizarTotales();
                }
                
            }).Start();
        }*/

        private void SetProductosRevertir(List<VentaProducto> productos)
        {
            foreach (var producto in productos)
            {
                ProductosNotaRevertir[$"{producto.Id}"] = (int)producto.NumeroNota;
            }
            if (Extras == null)
            {
                Extras = new Dictionary<string, object?>();
            }
            Extras["NumeroNotas"] = ViewModel.Venta.NumeroNotas;
            var productosActualizados = new List<VentaProducto>();
            foreach (var productoNota in ViewModel.VentaProductos)
            {
                productosActualizados.Add(new VentaProducto(productoNota));
            }
            Extras["VentaProductos"] = productosActualizados;
            /*if (OnEnterAction != null)
            {
                OnEnterAction.Invoke(Extras);
            }*/
        }

        private void RestablecerCuentas()
        {
            NumeroNotaSeleccionada = null;
            foreach (var revertir in ProductosNotaRevertir)
            {
                var producto = ViewModel.VentaProductos.Where(item => item.Id == revertir.Key).First();
                MoverProductoNota($"{producto.NumeroNota}", $"{revertir.Value}", producto);
            }
        }

        public void ocultar()
        {
            DialogoSeleccionarNotas.IsOpen = false;
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
            if (!ActualizandoNumeroNotas)
            {
                NumeroNotaSeleccionada = null;
                ViewModel.VentaProductos = new ObservableCollection<VentaProducto>();
                ViewModel.VentaNotas = new ObservableCollection<VentaNota>();
            }
            
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
                Extras["NumeroNotas"] = ViewModel.Venta.NumeroNotas;
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

        private void NotasListado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NotasListado.SelectedIndex > -1)
            {
                var nota = ViewModel.VentaNotas[NotasListado.SelectedIndex];
                if (nota.IsPagada)
                {
                    ProductosListado.SelectedIndex = -1;
                    NotasListado.SelectedIndex = -1;
                    ToastNotification.show("Nota pagada", ToastNotification.SHORT);
                    if (NumeroNotaSeleccionada != null)
                    {
                        NumeroNotaSeleccionada = null;
                        ViewModel.VentaProductos = ProductosNotaSeleccionados(new ObservableCollection<VentaProducto>(ViewModel.VentaProductos));
                    }
                    return;
                }
                if (NumeroNotaSeleccionada != nota.NumeroNota)
                {
                    ProductosListado.SelectedIndex = -1;
                    NumeroNotaSeleccionada = nota.NumeroNota;
                    ViewModel.VentaProductos = ProductosNotaSeleccionados(new ObservableCollection<VentaProducto>(ViewModel.VentaProductos));
                }
            }
        }

        private ObservableCollection<VentaProducto> ProductosNotaSeleccionados(ObservableCollection<VentaProducto> productos)
        {

            return new ObservableCollection<VentaProducto>(productos
                .Select(item =>
                {
                    item.Seleccionado = $"{item.NumeroNota}" == NumeroNotaSeleccionada && NumeroNotaSeleccionada != null; 
                    return item;
                }).ToList());

        }

        private void VentaProductoItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (IsLoader())
            {
                return;
            }
            EventClickArgs extras = (EventClickArgs)e;
            int index = (int)extras.Extras["Index"];
            if (NotasListado.SelectedIndex > -1)
            {
                int indexNota = NotasListado.SelectedIndex;
                var nota = ViewModel.VentaNotas[NotasListado.SelectedIndex];
                var producto = ViewModel.VentaProductos[index];
                string anteriorNota = $"{producto.NumeroNota}";
                string nuevaNota = $"{nota.NumeroNota}";
                if ($"{producto.NumeroNota}" != nuevaNota)
                {
                    if (producto.IsExtra)
                    {
                        producto = ViewModel.VentaProductos.Where(item => item.Id == producto.VentaProductoPadreId).First();
                    }

                    MoverProductoNota(anteriorNota, nuevaNota, producto);

                }

            }

        }

        private void MoverProductoNota(string anteriorNota, string nuevaNota, VentaProducto ventaProducto)
        {
            var listadoProductos = new ObservableCollection<VentaProducto>(ViewModel.VentaProductos);
            int productoIndex = ViewModel.GetIndexProducto($"{ventaProducto.Id}");
            ventaProducto.Seleccionado = true;
            ventaProducto.NumeroNota = int.Parse(nuevaNota);
            listadoProductos.RemoveAt(productoIndex);
            listadoProductos.Insert(productoIndex, ventaProducto);

            var extras = listadoProductos.Where(item => item.VentaProductoPadreId == ventaProducto.Id).ToList();
            foreach (var extra in extras)
            {
                int indexExtra = ViewModel.GetIndexProducto($"{extra.Id}");
                extra.Seleccionado = true;
                extra.NumeroNota = int.Parse(nuevaNota);

                listadoProductos.RemoveAt(indexExtra);
                listadoProductos.Insert(indexExtra, extra);
            }
            ProductosListado.SelectedIndex = -1;
            ViewModel.VentaProductos = ProductosNotaSeleccionados(listadoProductos);

            ViewModel.ActualizarNotas(anteriorNota, $"{nuevaNota}");
            if (NumeroNotaSeleccionada != null)
            {
                NotasListado.SelectedIndex = ViewModel.GetIndexNota(nuevaNota);
            }

        }

        /**
         * Obtiene la ventana principal del programa
         */
        private MainWindow getWindow()
        {
            return WindowUtils.getMainWindow();
        }


        private void RevertirMovimiento_OnClick(object sender, RoutedEventArgs e)
        {
            if (IsLoader())
            {
                return;
            }
            RestablecerCuentas();
        }

        private void NumeroNotas_OnClick(object sender, RoutedEventArgs e)
        {
            if (IsLoader())
            {
                return;
            }
            AbrirDividirCuentaNumero();
        }

        private void GuardarProductos_Click(object sender, RoutedEventArgs e)
        {
            GuardarProductosMesas();
        }

        private void AbrirDividirCuentaNumero()
        {
            ActualizandoNumeroNotas = true;
            DialogoSeleccionarNotas.IsOpen = false;

            var datos = new Dictionary<string, object?>();
            datos["Titulo"] = "Ingresa número de notas";
            datos["Valor"] = $"{ViewModel.Venta.NumeroNotas}";
            datos["LimiteCantidad"] = 99;
            datos["DesactivarBlur"] = false;
            
            getWindow().MostrarTecladoNumerico(datos, delegate (Dictionary<string, object?> respuesta)
            {
                int numeroNotas = int.Parse($"{(string?)respuesta["Valor"]}");
                GuardarNumeroNotas(numeroNotas);
            }, delegate (Dictionary<string, object?> respuesta)
            {
                ActualizandoNumeroNotas = false;
                DialogoSeleccionarNotas.IsOpen = true;
            });
        }

        private void GuardarNumeroNotas(int numeroNotas)
        {
            ApiClient.VentaDetalleNumeroNotas($"{ViewModel.Venta.Id}", numeroNotas, delegate (JObject respuesta)
            {
                var error = (string?)respuesta["error"];
                if (error != null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        ToastNotification.show(error, ToastNotification.LONG);
                    });
                    return;
                }
                else
                {
                    ViewModel.Venta = new Models.Ventas.Venta((JObject)respuesta["venta"]);
                    this.Dispatcher.Invoke(() =>
                    {
                        ViewModel.VentaNotas = ViewModel.Venta.Notas;
                        ViewModel.ActualizarTotalNotas();
                    });
                }
            });
        }

        private void GuardarProductosMesas()
        {
            if (IsLoader())
            {
                return;
            }

            MostrarLoader();

            ApiClient.VentaDetalleProductosNotas(new List<VentaProducto>(ViewModel.VentaProductos),  delegate (JObject respuesta)
            {
                OcultarLoader();
                var error = (string?)respuesta["error"];
                if (error != null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        ToastNotification.show(error, ToastNotification.SHORT);
                    });
                    return;
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        if (OnEnterAction != null)
                        {
                            if (Extras == null)
                            {
                                Extras = new Dictionary<string, object?>();
                            }
                            OnEnterAction.Invoke(Extras);
                        }
                        ocultar();
                    });
                    //SetProductosRevertir(new List<VentaProducto>(ViewModel.VentaProductos));
                }
            });

        }

        private bool IsLoader()
        {
            return Loader.Visibility == Visibility.Visible;
        }

        private void OcultarLoader()
        {
            this.Dispatcher.Invoke(() =>
            {
                Loader.Visibility = Visibility.Collapsed;
                BotonGuardar.Visibility = Visibility.Visible;
            });
        }

        private void MostrarLoader()
        {
            this.Dispatcher.Invoke(() =>
            {
                Loader.Visibility = Visibility.Visible;
                BotonGuardar.Visibility = Visibility.Collapsed;
            });
        }


    }
}
