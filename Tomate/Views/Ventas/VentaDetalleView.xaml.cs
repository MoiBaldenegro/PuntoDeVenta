using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Tomate.Components.Dialogos.Venta;
using Tomate.HttpRequest;
using Tomate.Models.Ventas;
using Tomate.Models.Usuarios;
using Tomate.Models.Catalogo;
using Tomate.Utils;
using Tomate.ViewModels.Ventas;
using Tomate.Components.Dialogos.Venta.Notas;
using Tomate.Statics;

namespace Tomate.Views.Ventas
{
    /// <summary>
    /// Interaction logic for CuentaDetalleView.xaml
    /// </summary>
    public partial class VentaDetalleView : UserControl
    {
        //Viewmodel de la clase con variables que se muestran en la vista
        public VentaDetalleViewModel ViewModel { get; set; }

        private bool ProductoCantidadModificada = false;

        //Constructor
        public VentaDetalleView()
        {
            ViewModel = new VentaDetalleViewModel();
            InitializeComponent();
            DataContext = ViewModel;
        }

        /**
         * Inicia la vista, obtiene los datos necesarios 
         */
        public void IniciarVista(Usuario? usuario, Venta? venta)
        {
#pragma warning disable CS8601 // Possible null reference assignment.
            ViewModel.Usuario = usuario;
            ViewModel.Venta = venta;
#pragma warning restore CS8601 // Possible null reference assignment.

            getTableros();
            getVentaDetalle();
            IniciarRelojVenta();
            RevisarPermisos();
            getCajaCorte();
        }

        /**
         * Oculta botones sin permiso
         */
        private void RevisarPermisos()
        {
            ViewModel.TotalBotones = 11;
            if (!ViewModel.Usuario.RevisarPermiso("pv.venta.agregar-descuento"))
            {
                ViewModel.TotalBotones -= 1;
                DescuentoBoton.Visibility = Visibility.Collapsed;
            }
            else
            {
                DescuentoBoton.Visibility = Visibility.Visible;
            }
            if (!ViewModel.Usuario.RevisarPermiso("pv.venta.cancelar-venta"))
            {
                ViewModel.TotalBotones -= 1;
                CancelarBoton.Visibility = Visibility.Collapsed;
            }
            else
            {
                CancelarBoton.Visibility = Visibility.Visible;
            }
        }

        /**
         * Libera variables y cancela intervalos
         */
        public void LimpiarVista()
        {
            DetenerRelojVenta();
            ProductoCantidadModificada = false;
            ViewModel.CantidadProducto = 1;
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            ViewModel.Usuario = null;
            ViewModel.Venta = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            ViewModel.VentaProductos = new ObservableCollection<VentaProducto>();
        }

        /**
         * Valida si se han agregado productos nuevos para enviarlos
         */
        private void RevisarEnviarProductos()
        {
            this.Dispatcher.Invoke(() =>
            {
                ViewModel.TextoEnviar = "Enviar";
                if (ViewModel.getProductosAgregados().Count > 0)
                {
                    BotonEnviarCuenta.Opacity = 1;
                }
                else
                {
                    if (ViewModel.Usuario.RevisarPermiso("pv.venta.cobrar") && ViewModel.VentaProductos.Count > 0)
                    {
                        ViewModel.TextoEnviar = "Cobrar";
                        BotonEnviarCuenta.Opacity = 1;
                    }
                    else
                    {
                        BotonEnviarCuenta.Opacity = 0.8;
                    }

                }
            });

        }

        private void getTableros()
        {
            new Task(() =>
            {
                ViewModel.CargarTableros(true);
            }).Start();
            
        }

        private void getVentaDetalle()
        {

            ApiClient.VentaDetalle(ViewModel.Venta, delegate (JObject respuesta)
            {
                var error = (string?)respuesta["error"];
                if (error != null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        DialogoAlertaCm.show("Detalle de venta", error);
                    });
                    return;
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        getWindow().VentaSeleccionada = ViewModel.ActualizarVentaDetalle(respuesta);
                        ScrollUltimoProducto();
                    });
                }
                RevisarEnviarProductos();
            });
        }

        private void AgregarProductoPlaceholder()
        {
            var placeholder = new VentaProducto();
            placeholder.Cantidad = 1;
            placeholder.Producto = new Producto();
            ViewModel.VentaProductos.Add(placeholder);
            ScrollUltimoProducto();
        }

        private void SetCantidadPlaceholder(int cantidad)
        {
            this.Dispatcher.Invoke(() =>
            {
                int index = ViewModel.VentaProductos.Count - 1;

                ViewModel.VentaProductos[index].Cantidad = cantidad;
                ViewModel.NotificarActualizarProducto(index);
            });
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

        private void AgregarProducto_OnClick(object sender, RoutedEventArgs e)
        {
            EventClickArgs args = (EventClickArgs)e;
            var Extras = args.Extras;

            int index = (int)Extras["Index"];
            var producto = ViewModel.Productos[index];
            if (producto != null)
            {
                if (!producto.TableroActivo)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        DialogoAlertaCm.show("Producto desactivado");
                    });
                    return;
                }

                int? indexProducto = null;

                if (ProductoCantidadModificada)
                {
                    indexProducto = ViewModel.VentaProductos.Count - 1;
                }

                int cantidad = ViewModel.CantidadProducto;
                var ventaProducto = new VentaProducto();
                ventaProducto.VentaId = ViewModel.Venta.Id;
                ventaProducto.UsuarioId = ViewModel.Usuario.Id;
                ventaProducto.ProductoId = $"{producto.Id}";
                ventaProducto.Producto = producto;
                ventaProducto.Precio = producto.Precio;
                ventaProducto.Cantidad = cantidad;
                ventaProducto.Importe = cantidad * producto.Precio;

                //Los extra se agregan desde otra sección
                bool isExtra = false;

                if (!isExtra && ViewModel.Venta.NumeroNotas > 1)
                {
                    SeleccionarVentaNotaModal.mostrar((int)ViewModel.Venta.NumeroNotas, ViewModel.VentaNotas, ViewModel.VentaProductos,
                        "Asignar número de nota", true,
                        delegate (Dictionary<string, object?> respuesta)
                    {
                        int numeroNota = int.Parse($"{(string?)respuesta["NumeroNota"]}");
                        ventaProducto.NumeroNota = numeroNota;
                        AgregarProductoVenta(ventaProducto, indexProducto, isExtra);
                    });
                }
                else
                {
                    AgregarProductoVenta(ventaProducto, indexProducto, isExtra);
                }
                
            }
        }

        private void AgregarProductoVenta(VentaProducto ventaProducto, int? index = null, bool extra = false)
        {
            ViewModel.AgregarProducto(ventaProducto, index, extra);
            LimpiarProductoCantidad();
            RevisarEnviarProductos();
            ScrollUltimoProducto();
        }

        private void ScrollUltimoProducto()
        {
            try
            {
                if (ViewModel.VentaProductos.Count > 5)
                {
                    ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(CuentaProductosListado, 0);
                    scrollViewer.ScrollToBottom();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

        }


        private void ModificarCantidadProductoCuenta(int index, int cantidad)
        {
            ViewModel.VentaProductos[index].Cantidad = cantidad;
            ViewModel.VentaProductos[index].Importe = cantidad * ViewModel.VentaProductos[index].Precio;

            ViewModel.NotificarActualizarProducto(index);
            ViewModel.CalcularVentaTotal();
        }

        private void EliminarProducto(int index)
        {
            if (index < ViewModel.VentaProductos.Count)
            {
                var producto = ViewModel.VentaProductos[index];
                var productosEliminar = ViewModel.VentaProductos.Where(item => item.VentaProductoPadreId == producto.Id).ToList();
                productosEliminar.Add(producto);

                if (producto.TerminalId != null)
                {
                    if (ViewModel.Usuario.RevisarPermiso("pv.venta.cancelar-productos"))
                    {
                        CancelarModal.mostrar(false, ViewModel.VentaProductos[index], index);
                        CancelarModal.Extras["Productos"] = productosEliminar;
                    }
                }
                else
                {
                    RemoverProductos(productosEliminar);
                }
            }
        }

        private void RemoverProductos(List<VentaProducto> productos)
        {
            foreach (var producto in productos)
            {
                RemoverProducto(ViewModel.GetIndexVentaProductos($"{producto.Id}"));
            }
            ViewModel.CalcularVentaTotal();
        }

        private void RemoverProducto(int index)
        {
            this.Dispatcher.Invoke(() =>
            {
                ViewModel.VentaProductos.RemoveAt(index);
                ViewModel.CalcularVentaTotal();
                RevisarEnviarProductos();
            });
            ViewModel.CalcularVentaTotal();
        }

        private void VentaProducto_OnEditarCantidad(object sender, RoutedEventArgs e)
        {
            CuentaProductosListado.SelectedIndex = -1;
            EventClickArgs extras = (EventClickArgs)e;
            //int index = (int)extras.Extras["Index"];
            string Id = (string)extras.Extras["Id"];
            int index = ViewModel.GetIndexVentaProductos(Id);
            var ventaProducto = ViewModel.VentaProductos[index];
            if (ventaProducto.Pagado)
            {
                this.Dispatcher.Invoke(() =>
                {
                    ToastNotification.show("Producto pagado", ToastNotification.SHORT);
                });
                return;
            }

            this.Dispatcher.Invoke(() =>
            {
                var datos = new Dictionary<string, object?>();
                datos["Titulo"] = "Ingresa la cantidad";
                datos["Valor"] = null;
                getWindow().MostrarTecladoNumerico(datos, delegate (Dictionary<string, object?> respuesta)
                {
                    int cantidad = int.Parse($"{(string?)respuesta["Valor"]}");
                    //int index = (int)extras.Extras["Index"];
                    ModificarCantidadProductoCuenta(index, cantidad);
                }, null);
            });

        }

        private void VentaProducto_OnEliminar(object sender, RoutedEventArgs e)
        {
            EventClickArgs extras = (EventClickArgs)e;
            //int index = (int)extras.Extras["Index"];
            string Id = (string)extras.Extras["Id"];
            int index = ViewModel.GetIndexVentaProductos(Id);
            var ventaProducto = ViewModel.VentaProductos[index];
            if (ventaProducto.Pagado)
            {
                this.Dispatcher.Invoke(() =>
                {
                    ToastNotification.show("Producto pagado", ToastNotification.SHORT);
                });
                return;
            }
            
            EliminarProducto(index);
        }


        private void BotonCantidadProducto_OnClick(object sender, RoutedEventArgs e)
        {
            EventClickArgs extras = (EventClickArgs)e;
            string valorBoton = $"{(string?)extras.Extras["Valor"]}";

            if (valorBoton == "0" && !ProductoCantidadModificada)
            {
                return;
            }

            if ($"{ViewModel.CantidadProducto}".Length < 2)
            {
                int cantidad = int.Parse($"{ViewModel.CantidadProducto}{valorBoton}");
                if (!ProductoCantidadModificada)
                {
                    AgregarProductoPlaceholder();
                    cantidad = int.Parse(valorBoton);
                    ProductoCantidadModificada = true;
                }
                ViewModel.CantidadProducto = cantidad;
                SetCantidadPlaceholder(ViewModel.CantidadProducto);
            }

        }

        private void LimpiarProductoCantidad()
        {
            if (ProductoCantidadModificada)
            {
                ViewModel.CantidadProducto = 1;
                ProductoCantidadModificada = false;
                this.Dispatcher.Invoke(() =>
                {
                    ViewModel.VentaProductos.RemoveAt(ViewModel.VentaProductos.Count - 1);
                });
                ScrollUltimoProducto();
            }

        }

        private void BotonCantidadProductoCancelar_OnClick(object sender, RoutedEventArgs e)
        {
            LimpiarProductoCantidad();
        }

        private void SeleccionarTablero_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CuentaProductosListado.SelectedIndex = -1;
            ViewModel.setTableroSeleccionado(TablerosListado.SelectedIndex);
        }

        private void AsignarNombre_OnClick(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                var datos = new Dictionary<string, object?>();
                datos["Titulo"] = "Ingresa nombre";
                datos["Valor"] = $"{ViewModel.Venta.Nombre}";
                datos["Limite"] = 20;
                getWindow().MostrarTeclado(datos, delegate (Dictionary<string, object?> respuesta)
                {
                    ViewModel.Venta.Nombre = $"{(string?)respuesta["Valor"]}";
                    ViewModel.Venta = ViewModel.Venta;
                    ActualizarVenta();
                }, null);
            });
        }

        private void ImprimirVenta_OnClick(object sender, RoutedEventArgs e)
        {
            ImprimirVenta();
        }

        private void AsignarObservaciones_OnClick(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                var datos = new Dictionary<string, object?>();
                datos["Titulo"] = "Ingresa comentarios";
                datos["Valor"] = $"{ViewModel.Venta.Observaciones}";
                getWindow().MostrarTeclado(datos, delegate (Dictionary<string, object?> respuesta)
                {
                    ViewModel.Venta.Observaciones = $"{(string?)respuesta["Valor"]}";
                    ViewModel.Venta = ViewModel.Venta;
                    ActualizarVenta();
                }, null);
            });
        }

        private void NumeroPersonas_OnClick(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                var datos = new Dictionary<string, object?>();
                datos["Titulo"] = "Ingresa comensales";
                datos["Valor"] = $"{ViewModel.Venta.Personas}";
                getWindow().MostrarTecladoNumerico(datos, delegate (Dictionary<string, object?> respuesta)
                {
                    int personas = int.Parse($"{(string?)respuesta["Valor"]}");
                    ViewModel.Venta.Personas = personas;
                    ViewModel.Venta = ViewModel.Venta;
                    ActualizarVenta();

                }, null);
            });
        }

        private void TipoCuenta_OnClick(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                var datos = new Dictionary<string, object?>();
                getWindow().MostrarVentaTipos(datos, delegate (Dictionary<string, object?> respuesta)
                {
                    string Valor = $"{(string?)respuesta["VentaTipoId"]}";
                    if (ViewModel.Venta.VentaTipoId != Valor)
                    {
                        CambiarVentaTipo(Valor);
                    }
                }, null);
            });
        }


        private void CancelarCuenta_OnClick(object sender, RoutedEventArgs e)
        {
            CancelarModal.mostrar();
        }

        private void ActualizarVenta()
        {
            ApiClient.VentaDetalleActualizar(ViewModel.Venta, delegate (JObject respuesta)
            {
                var error = (string?)respuesta["error"];
                if (error != null)
                {
                   
                    this.Dispatcher.Invoke(() =>
                    {
                        DialogoAlertaCm.show(error);
                    });

                    return;
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        var venta = new Venta((JObject?)respuesta["venta"]);
                        ViewModel.Venta.Personas = venta.Personas;
                        ViewModel.Venta.Observaciones = venta.Observaciones;
                        ViewModel.Venta.Nombre = venta.Nombre;
                        ViewModel.ActualizarVentaDetalle();
                        getWindow().VentaSeleccionada = ViewModel.Venta;
                    });
                }
            });

        }

        private void ImprimirVenta()
        {

            if (ViewModel.Venta.NumeroNotas == 1)
            {
                mandarImprimirVenta();
            }
            else
            {

                SeleccionarVentaNotaModal.mostrar((int)ViewModel.Venta.NumeroNotas, ViewModel.VentaNotas, ViewModel.VentaProductos,
                    "Selecciona nota", true,
                delegate (Dictionary<string, object?> respuesta)
                {
                    mandarImprimirVenta($"{(string?)respuesta["NumeroNota"]}");
                });
            }    
        }
        private void mandarImprimirVenta(string numNota = "1")
        {
            ApiClient.VentaTicketImprimir($"{ViewModel.Venta.Id}", $"{ViewModel.Usuario.Id}", numNota, delegate (JObject respuesta)
            {
                var error = (string?)respuesta["error"];
                if (error != null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        DialogoAlertaCm.show(error);
                    });
                    return;
                }
            });
        }

        private void CancelarProductoVenta(List<VentaProducto> productosEliminar, string? motivoId, string? otro)
        {
            foreach (var productoEliminar in productosEliminar)
            {
                ApiClient.VentaProductoCancelar($"{ViewModel.Usuario.Id}", $"{productoEliminar.Id}", motivoId, otro, delegate (JObject respuesta)
                {
                    var error = (string?)respuesta["error"];
                    if (error != null)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            DialogoAlertaCm.show(error);
                        });
                        return;
                    }
                    else
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            productoEliminar.DeletedAt = DateTime.Now;
                            ViewModel.ReemplazarProducto(productoEliminar);
                            ViewModel.CalcularVentaTotal();
                        });
                       
                    }
                });
            }

        }

        private void CambiarVentaTipo(string ventaTipoId)
        {
            ApiClient.VentaDetalleTipo(ViewModel.Venta.Id, ventaTipoId, delegate (JObject respuesta)
            {
                var error = (string?)respuesta["error"];
                if (error != null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        DialogoAlertaCm.show(error);
                    });
                    return;
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        ViewModel.setTableroSeleccionado(ViewModel.TableroSeleccionado, true);
                        getWindow().VentaSeleccionada = ViewModel.ActualizarVentaDetalle(respuesta);
                    });
                }
            });
        }

        private void AplicarDescuentoProductoVenta(string motivo, bool isPorcentaje, float? cantidad, int? index)
        {
            if (ViewModel.VentaProductos[(int)index].TerminalId == null)
            {
                ViewModel.VentaProductos[(int)index].SetDescuento(motivo, isPorcentaje, cantidad); ;
                ViewModel.NotificarActualizarProducto((int)index);
                ViewModel.CalcularVentaTotal();
            }
            else
            {
                ApiClient.VentaDetalleProductoDescuento($"{ViewModel.Usuario.Id}", $"{ViewModel.VentaProductos[(int)index].Id}",
                motivo, isPorcentaje, cantidad, delegate (JObject respuesta)
                {
                    var error = (string?)respuesta["error"];
                    if (error != null)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            DialogoAlertaCm.show(error);
                        });
                        return;
                    }
                    else
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            getWindow().VentaSeleccionada = ViewModel.ActualizarVentaDetalle(respuesta);
                        });
                    }
                });
            }

        }

        private void AplicarDescuentoVenta(string motivo, bool isPorcentaje, float? cantidad)
        {
            ApiClient.VentaDetalleDescuento($"{ViewModel.Usuario.Id}", $"{ViewModel.Venta.Id}",
                motivo, isPorcentaje, cantidad, delegate (JObject respuesta)
            {
                var error = (string?)respuesta["error"];
                if (error != null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        DialogoAlertaCm.show(error);
                    });
                    return;
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        getWindow().VentaSeleccionada = ViewModel.ActualizarVentaDetalle(respuesta);
                    });
                }
            });
        }



        private void CancelarVenta(string? motivoId, string? otro)
        {
            ApiClient.VentaCancelar(ViewModel.Usuario.Id, ViewModel.Venta.Id, motivoId, otro, delegate (JObject respuesta)
            {
                var error = (string?)respuesta["error"];
                if (error != null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        DialogoAlertaCm.show(error);
                    });
                    return;
                }
                else
                {
                    Atras();
                }
            });
        }


        /**
         * Envia la venta solo si se agregaron productos
         */
        private void BotonEnviarCuenta_Click(object sender, RoutedEventArgs e)
        {
            if (BotonEnviarCuenta.Opacity == 1)
            {
                if (ViewModel.TextoEnviar == "Cobrar")
                {
                    MostrarCobrarVentaNota();
                }
                else
                {
                    EnviarVentaProductos();
                }

            }
        }

        /**
         * Elimina los productos de la cuenta que no se han enviado
         */
        private void LimpiarVenta_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.TableroSeleccionado == -1)
            {
                ViewModel.setTableroSeleccionado(0);
            }
            LimpiarProductoCantidad();
            ViewModel.RestablecerProductosVenta();
            RevisarEnviarProductos();
        }

        private void EnviarVentaProductos()
        {
            ApiClient.VentaDetalleProductosPedir(ViewModel.Venta.Id, ViewModel.Usuario.Id,
                ViewModel.getProductosAgregados(), delegate (JObject respuesta)
            {
                var error = (string?)respuesta["error"];
                if (error != null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        DialogoAlertaCm.show(error);
                    });
                    return;
                }
                else
                {
                    Atras();
                }
            });
        }


        private void MostrarModificadores(int index)
        {
            ProductoModificadoresModal.mostrar(ViewModel.VentaProductos[index], delegate (Dictionary<string, object?> respuesta)
            {
                string? modificadores = (string?)respuesta["Valor"];
                this.Dispatcher.Invoke(() =>
                {
                    ViewModel.SetModificadores(index, modificadores);
                });

            }, delegate (Dictionary<string, object?> respuesta)
            {
                CuentaProductosListado.SelectedIndex = -1;
            });
        }

        private void MostrarComplementos(int index)
        {

            ProductoExtrasModal.mostrar(ViewModel.Venta, ViewModel.VentaProductos[index], ViewModel.getComplementos(ViewModel.VentaProductos[index]), 
                delegate (Dictionary<string, object?> respuesta)
            {
                this.Dispatcher.Invoke(() =>
                {
                    ViewModel.RemoverComplementos(index);
                    var complementos = (ObservableCollection<VentaProducto>)respuesta["Complementos"];
                    foreach (var complemento in complementos)
                    {
                        AgregarProductoVenta(complemento, index, true);
                    }
                });

            }, delegate (Dictionary<string, object?> respuesta)
            {
                CuentaProductosListado.SelectedIndex = -1;
            });
        }

        private void VentaProductoItem_OnClick(object sender, RoutedEventArgs e)
        {
            EventClickArgs extras = (EventClickArgs)e;
            //int index = (int)extras.Extras["Index"];
            string Id = (string)extras.Extras["Id"];
            int index = ViewModel.GetIndexVentaProductos(Id);
            var ventaProducto = ViewModel.VentaProductos[index];

            if (ventaProducto.Pagado)
            {
                this.Dispatcher.Invoke(() =>
                {
                    ToastNotification.show("Producto pagado", ToastNotification.SHORT);
                });
                return;
            }

            ProductoOpcionesModal.mostrar(ViewModel.VentaProductos[index], delegate (Dictionary<string, object?> respuesta)
            {
                switch ($"{(string?)respuesta["Accion"]}")
                {
                    case "ExtrasProducto":
                        MostrarComplementos(index);
                        break;
                    case "ModificadoresProducto":
                        MostrarModificadores(index);
                        break;
                    case "CancelarProducto":
                        EliminarProducto(index);
                        break;
                    case "ReordenarProducto":
                        var producto = TableroProducto.buscarProductoId($"{ventaProducto.ProductoId}");
                        if (producto == null || !producto.TableroActivo)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                DialogoAlertaCm.show("El producto no está activo");
                            });
                            return;
                        }

                        var productoReordenar = ventaProducto.Reordenar();
                        AgregarProductoVenta(productoReordenar);
                        break;
                    case "TransferirProducto":
                        TransferirSeleccionarCuenta(index);
                        break;
                    case "TransferirNotaProducto":
                        if (ViewModel.Venta.NumeroNotas == 1)
                        {
                            AbrirDividirCuentaNumero();
                        }
                        else
                        {
                            AsignarNotasProductos();
                        }
                        break;

                    case "AplicarDescuentoProducto":
                        AplicarDescuentoCantidad(index);
                        break;
                    case "NoImprimirProducto":
                        ViewModel.VentaProductos[index].NoImprimir = !ViewModel.VentaProductos[index].NoImprimir;
                        ViewModel.NotificarActualizarProducto(index);
                        break;
                    case "CerrarOpciones":
                        CuentaProductosListado.SelectedIndex = -1;
                        break;
                }

            });
        }










        private void CuentaProductosListado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CuentaProductosListado.SelectedIndex == -1 && ViewModel.TableroSeleccionado == -1)
            {
                ViewModel.setTableroSeleccionado(0);
            }
        }

        //Funciones cancelar producto o cuenta
        private void CancelarModal_OnMotivo(object sender, RoutedEventArgs e)
        {
            EventClickArgs extras = (EventClickArgs)e;
            string? MotivoId = (string?)extras.Extras["MotivoId"];
            if (MotivoId != null)
            {
                if ((bool)extras.Extras["IsVenta"])
                {
                    CancelarVenta(MotivoId, null);
                }
                else
                {
                    CancelarProductoVenta((List<VentaProducto>)extras.Extras["Productos"], MotivoId, null);
                }
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    var datos = new Dictionary<string, object?>();
                    datos["Titulo"] = "Ingresa motivo cancelación";
                    datos["MinimoDigitos"] = 1;
                    datos["Valor"] = null;
                    getWindow().MostrarTeclado(datos, delegate (Dictionary<string, object?> respuesta)
                    {
                        string Valor = $"{(string?)respuesta["Valor"]}";
                        if ((bool)extras.Extras["IsVenta"])
                        {
                            CancelarVenta(null, Valor);
                        }
                        else
                        {
                            CancelarProductoVenta((List<VentaProducto>)extras.Extras["Productos"], null, Valor);
                        }
                    }, null);
                });
            }
        }

        //Funciones dividir cuenta
        private void DividirCuenta_OnClick(object sender, RoutedEventArgs e)
        {
            AbrirDividirCuentaNumero();
        }

        private void AbrirDividirCuentaNumero()
        {
            if (ViewModel.Venta.NumeroNotas == 1)
            {
                var datos = new Dictionary<string, object?>();
                datos["Titulo"] = "Ingresa número de notas";
                datos["Valor"] = $"{ViewModel.Venta.NumeroNotas}";
                datos["LimiteCantidad"] = 99;
                getWindow().MostrarTecladoNumerico(datos, delegate (Dictionary<string, object?> respuesta)
                {
                    int numeroNotas = int.Parse($"{(string?)respuesta["Valor"]}");
                    ActualizarNumeroNotas(numeroNotas, delegate(string? error)
                    {
                        if (error != null)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                DialogoAlertaCm.show("Detalle cuenta", $"{error}");
                            });
                        }
                        else
                        {
                            AsignarNotasProductos();
                        }
                    });

                }, null);
            }
            else
            {
                AsignarNotasProductos();
            }
            
        }



        //Funciones transferr productos a otra nota

        private void ActualizarNumeroNotas(int numeroNotas, Action<string?>? callback = null)
        {
            ApiClient.VentaDetalleNumeroNotas($"{ViewModel.Venta.Id}", numeroNotas, delegate (JObject respuesta)
            {
                var error = (string?)respuesta["error"];
                if (error != null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        DialogoAlertaCm.show(error);
                    });

                    return;
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        getWindow().VentaSeleccionada = ViewModel.ActualizarVentaDetalle(respuesta);
                        AsignarNotasProductos();
                    });
                }
            });
        }

        /**
         * Abre el listado de productos y notas para asignarlos
         */
        private void AsignarNotasProductos()
        {
            VentaNotasAsignarProductosModal.mostrar((Venta)ViewModel.Venta, ViewModel.VentaNotas, ViewModel.VentaProductos,
                delegate (Dictionary<string, object?> respuesta)
            {
                getVentaDetalle();
            }, delegate (Dictionary<string, object?> respuesta) {

                if ((int?)respuesta["NumeroNotas"] != ViewModel.Venta.NumeroNotas)
                {
                    getVentaDetalle();
                }
            });
        }

        //Funciones transferir productos a otra cuenta
        /**
         * Abre el modal para seleccionar la cuenta a transferir
         */
        private void TransferirSeleccionarCuenta(int? index)
        {
            var datos = new Dictionary<string, object?>();
            datos["Titulo"] = "Ingresa número de cuenta";
            datos["Limite"] = 4;
            datos["Valor"] = null;
            getWindow().MostrarTecladoNumerico(datos, delegate (Dictionary<string, object?> respuesta)
            {

                var nuevaCuenta = $"{(string?)respuesta["Valor"]}";
                if (nuevaCuenta == $"{ViewModel.Venta.NumeroCuenta}")
                {
                    DialogoAlertaCm.show($"No se puede transferir la cuenta a la misma cuenta");
                }
                else if (VentasStatic.getVenta(nuevaCuenta) == null)
                {
                    DialogoAlertaCm.show($"No se puede transferir a una cuenta cerrada");
                }
                else
                {
                    TransferirSeleccionarProductos(new Venta(nuevaCuenta), index);
                }
            }, null);
        }

        /**
         * Selecciona los productos a transferir 
         * index es para indicar cual producto se está transfiriendo
         */
        private void TransferirSeleccionarProductos(Venta transferir, int? index)
        {
            TransferirProductosModal.mostrar(ViewModel.Venta, transferir, ViewModel.GetProductosEnviados(), index,
                delegate (Dictionary<string, object?> respuesta)
                {

                    List<VentaProducto>? eliminarProductos = (List<VentaProducto>?)respuesta["EliminarProductos"];
                    if (eliminarProductos != null && eliminarProductos.Count > 0)
                    {
                        RemoverProductos(eliminarProductos);
                    }

                }, null);
        }

        private void TransferirProductos_OnClick(object sender, RoutedEventArgs e)
        {
            TransferirSeleccionarCuenta(null);
        }

           

        //Funciones tranferir cuenta
        /**
         * Click menu inferir de cuenta
         */
        private void TransferirCuentas_OnClick(object sender, RoutedEventArgs e)
        {
            TransferirCuenta();
        }

        private void TransferirCuenta()
        {
            var datos = new Dictionary<string, object?>();
            datos["Titulo"] = "Ingresa número de cuenta";
            datos["Valor"] = null;
            datos["Limite"] = 4;
            //datos["DesactivarBlur"] = false;
            getWindow().MostrarTecladoNumerico(datos, delegate (Dictionary<string, object?> respuesta)
            {
                var nuevaCuenta = $"{(string?)respuesta["Valor"]}";
                if (nuevaCuenta == $"{ViewModel.Venta.NumeroCuenta}")
                {
                    DialogoAlertaCm.show($"No se puede transferir la cuenta a la misma cuenta");
                }
                else if (VentasStatic.getVenta(nuevaCuenta) != null)
                {
                    DialogoAlertaCm.show($"No se puede transferir a una cuenta abierta");
                }
                else
                {
                    DialogoAlertaCm.show($"¿Transferir cuenta {ViewModel.Venta.NumeroCuenta} a {nuevaCuenta}?", true, delegate ()
                    {
                        TransferirCuenta(nuevaCuenta);
                    });
                }

            }, null);
        }

        /**
         * Transfiere la cuenta actual a otra mesa
         */
        private void TransferirCuenta(string numeroCuenta)
        {

            if (!ViewModel.Usuario.RevisarPermiso("pv.todas-ventas") && !ViewModel.Usuario.BuscarMesa(numeroCuenta))
            {
                this.Dispatcher.Invoke(() =>
                {
                    DialogoAlertaCm.show("Listado de cuentas", $"No tienes asignada la mesa \"{numeroCuenta}\"");
                });
                return;
            }
            else if (VentasStatic.getVenta(numeroCuenta) != null)
            {
                this.Dispatcher.Invoke(() =>
                {
                    DialogoAlertaCm.show("Listado de cuentas", $"La cuenta \"{numeroCuenta}\" ya existe");
                });
                return;
            }

            ApiClient.VentaDetalleTransferir(ViewModel.Usuario.Id, ViewModel.Venta.Id, numeroCuenta, delegate (JObject respuesta)
            {
                var error = (string?)respuesta["error"];
                if (error != null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        DialogoAlertaCm.show(error);
                    });
                    return;
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        getWindow().ActualizarVentas();
                        getWindow().VentaSeleccionada = ViewModel.ActualizarVentaDetalle(respuesta);
                    });
                }
            });
        }

        //Funciones descuento venta y producto

        /**
         * Click en el boton inferir de descuento (cuenta)
         */
        private void Descuento_OnClick(object sender, RoutedEventArgs e)
        {
            AplicarDescuentoCantidad(null);

        }

        /**
         * Abre el dialogo para ingresar motivo de descuento
         * Si el index es null, es descuento a la cuenta
         */
        private void AplicarDescuentoCantidad(int? index)
        {
            this.Dispatcher.Invoke(() =>
            {

                if (index == null && ViewModel.Venta.NotasPagada)
                {
                    DialogoAlertaCm.show("Hay notas pagadas");
                    return;
                }

                float? limiteCantidad = null;

                bool isPorcentaje = false;
                string? valor = null;

                if (index != null)
                {
                    limiteCantidad = ViewModel.VentaProductos[(int)index].SubTotal;

                    if (ViewModel.VentaProductos[(int)index].DescuentoPorcentaje > 0)
                    {
                        valor = $"{ViewModel.VentaProductos[(int)index].DescuentoPorcentaje}";
                        isPorcentaje = true;
                    }
                    else if (ViewModel.VentaProductos[(int)index].Descuento > 0)
                    {
                        valor = $"{ViewModel.VentaProductos[(int)index].Descuento}";
                    }

                }
                else
                {
                    limiteCantidad = ViewModel.Venta.SubTotal;

                    if (ViewModel.Venta.DescuentoPorcentaje > 0)
                    {
                        valor = $"{ViewModel.Venta.DescuentoPorcentaje}";
                    }
                    else if (ViewModel.Venta.Descuento > 0)
                    {
                        valor = $"{ViewModel.Venta.Descuento}";
                    }
                    isPorcentaje = true;
                }

                var datos = new Dictionary<string, object?>();
                datos["Titulo"] = "Ingresa descuento";
                datos["LimiteCantidad"] = limiteCantidad;
                datos["SoloPorcentaje"] = index == null;

                datos["IsPorcentaje"] = isPorcentaje;
                datos["Valor"] = valor;

                getWindow().MostrarTecladoDescuento(datos, delegate (Dictionary<string, object?> respuesta)
                {
                    AplicarDescuentoMotivo((bool)respuesta["IsProcentaje"], (float?)respuesta["Valor"], index);
                }, null);
            });
            
        }

        /**
         * Abre el teclado para ingresar el descuento
         * Si el index es null significa que se está haciendo descuento a la cuenta
         */
        private void AplicarDescuentoMotivo(bool isPorcentaje, float? total, int? index = null)
        {
            this.Dispatcher.Invoke(() =>
            {
                string? motivo = null;
                if (index != null)
                {
                    motivo = ViewModel.VentaProductos[(int)index].DescuentoMotivo;
                }
                else
                {
                    motivo = ViewModel.Venta.DescuentoMotivo;
                }

                var datos = new Dictionary<string, object?>();
                datos["Titulo"] = "Ingresa motivo descuento";
                datos["Valor"] = motivo;
                getWindow().MostrarTeclado(datos, delegate (Dictionary<string, object?> respuesta)
                {
                    string motivo = $"{(string?)respuesta["Valor"]}";
                    if (index != null)
                    {
                        AplicarDescuentoProductoVenta(motivo, isPorcentaje, total, index);
                    }
                    else
                    {
                        AplicarDescuentoVenta(motivo, isPorcentaje, total);
                    }
                }, null);
            });
        }

        //Timer para calcular minutos transcurridos
        private Timer? RelojTimer = null;
        /**
         * Inicia el timer para subar minutos a la venta de manera local
         */
        private void IniciarRelojVenta()
        {
            DetenerRelojVenta();
            RelojTimer = new Timer();
            RelojTimer.Elapsed += (sender, e) =>
            {
                var Venta = ViewModel.Venta;
                TimeSpan diferenciaCuenta = DateTime.Now - (DateTime)ViewModel.Venta.CreatedAt;
                Venta.Tiempo = String.Format("{0:D2}:{1:D2}", (int)diferenciaCuenta.TotalHours, (int)diferenciaCuenta.Minutes);
                if (ViewModel.Venta.TiempoImpresionTicket != null)
                {
                    TimeSpan diferenciaTicket = DateTime.Now - (DateTime)ViewModel.Venta.HoraImpresionTicket;
                    Venta.TiempoImpresionTicket = String.Format("{0:D2}:{1:D2}", (int)diferenciaTicket.TotalHours, (int)diferenciaTicket.Minutes);
                }
                this.Dispatcher.Invoke(() =>
                {
                    ViewModel.Venta = Venta;
                });
            };
            RelojTimer.Interval = 5000;
            RelojTimer.AutoReset = true;
            RelojTimer.Start();
        }

        /**
         * Detiene el timer que calcula los minutos de la venta
         */
        private void DetenerRelojVenta()
        {
            if (RelojTimer != null)
            {
                RelojTimer.Stop();
                RelojTimer = null;
            }
        }


        /**
         * Abre el modal correspondiente para cobrar la venta
         */
        private void MostrarCobrarVentaNota()
        {
            if (ViewModel.CajaCorte == null)
            {
                this.Dispatcher.Invoke(() =>
                {
                    DialogoAlertaCm.show("Es necesario abrir caja");
                });
                return;
            }

            if (ViewModel.Venta.NumeroNotas == 1)
            {
                MostrarCobrarVenta(ViewModel.VentaNotas[0]);
            }
            else
            {

                SeleccionarVentaNotaModal.mostrar((int)ViewModel.Venta.NumeroNotas, ViewModel.VentaNotas, ViewModel.VentaProductos,
                    "Selecciona nota", false,
                delegate (Dictionary<string, object?> respuesta)
                {
                    MostrarCobrarVenta((VentaNota)respuesta["VentaNota"]);
                });
            }
        }

        private void MostrarCobrarVenta(VentaNota ventaNota)
        {
            this.Dispatcher.Invoke(() =>
            {
                getWindow().MostrarCobroVenta(ViewModel.Venta, ventaNota, ViewModel.CajaCorte);
            });
        }

        //Funciones caja y cortes

        private void getCajaCorte()
        {

            ApiClient.CajaCorte("1", delegate (JObject respuesta)
            {
                var error = (string?)respuesta["error"];
                if (error != null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        DialogoAlertaCm.show("Venta detalle", error);
                    });
                    return;
                }
                else
                {
                    var corteCajaJson = respuesta["caja_corte"];
                    if (corteCajaJson != null && corteCajaJson.Type == JTokenType.Object && corteCajaJson.HasValues)
                    {
                        ViewModel.CajaCorte = new Models.Caja.CajaCorte((JObject)corteCajaJson);
                    }
                    else
                    {
                        ViewModel.CajaCorte = null;
                    }
                }
            });
        }

    }
}
