using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tomate.HttpRequest;
using Tomate.Lang;
using Tomate.Models.Ventas;
using Tomate.Models.Usuarios;
using Tomate.Utils;
using Tomate.ViewModels.Ventas;
using Tomate.Statics;

namespace Tomate.Views.Ventas
{
    /// <summary>
    /// Lógica de interacción para ListadoVentasView.xaml
    /// </summary>
    /// IDisposable
    public partial class ListadoVentasView : UserControl
    {

        //Viewmodel de la clase con variables que se muestran en la vista
        public ListadoVentasViewModel ViewModel { get; set; }
        //Funciona para saber si se ha iniciado a ingresar una venta
        private bool CodigoIngresado = false;

        private Timer? GetVentasTimer;
        private KeyEventHandler? KeyDownEvento { get; set; }

        private Dictionary<string, string> FiltrosCuenta = new Dictionary<string, string>();

        public ListadoVentasView()
        {

            ViewModel = new ListadoVentasViewModel();
            //Texto por defecto en el tablero
            ViewModel.MensajeTablero = Mensajes.Textos["ingresa_cuenta"];
            InitializeComponent();
            DataContext = ViewModel;
        }

        public void IniciarVista(Usuario? usuario)
        {
            FiltrosCuenta = new Dictionary<string, string>();
            SetUsuario(usuario);
            VentasStatic.RevisarVentasVisibles(usuario);
            GetVentasSincronizar();
            RevisarPermisos();
            resetMensajeTablero();
        }

        public void SetUsuario(Usuario? usuario)
        {
#pragma warning disable CS8601 // Possible null reference assignment.
            ViewModel.Usuario = usuario;
#pragma warning restore CS8601 // Possible null reference assignment.
        }

        private void RevisarPermisos()
        {
            ViewModel.MostrarFiltros = ViewModel.Usuario.RevisarPermiso("pv.todas-ventas") ? Visibility.Visible : Visibility.Collapsed;
            ViewModel.TotalBotones = 8;
            if (!ViewModel.Usuario.RevisarPermiso("pv.asignar-mesas"))
            {
                ViewModel.TotalBotones -= 1;
                AsignarMesasBoton.Visibility = Visibility.Collapsed;
            }
            else
            {
                AsignarMesasBoton.Visibility = Visibility.Visible;
            }
            if (!ViewModel.Usuario.RevisarPermiso("pv.empleados"))
            {
                ViewModel.TotalBotones -= 1;
                EmpleadosBoton.Visibility = Visibility.Collapsed;
            }
            else
            {
                EmpleadosBoton.Visibility = Visibility.Visible;
            }
            if (!ViewModel.Usuario.RevisarPermiso("pv.configuraciones"))
            {
                ViewModel.TotalBotones -= 1;
                ConfigurarBoton.Visibility = Visibility.Collapsed;
            }
            else
            {
                ConfigurarBoton.Visibility = Visibility.Visible;
            }
            if (!ViewModel.Usuario.RevisarPermiso("pv.caja"))
            {
                ViewModel.TotalBotones -= 1;
                CajaBoton.Visibility = Visibility.Collapsed;
            }
            else
            {
                CajaBoton.Visibility = Visibility.Visible;
            }
            if (!ViewModel.Usuario.RevisarPermiso("pv.desactivar-productos"))
            {
                ViewModel.TotalBotones -= 1;
                DesactivarProductosBoton.Visibility = Visibility.Collapsed;
            }
            else
            {
                DesactivarProductosBoton.Visibility = Visibility.Visible;
            }
        }

        public void LimpiarVista()
        {
            ViewModel.TurnoIniciado = false;
            FiltrosCuenta = new Dictionary<string, string>();
            CancelarVentasSincronizar();
            resetMensajeTablero();
        }

        /**
         * Obtiene los empleados de la API
         */
        private void GetVentasSincronizar()
        {
            CancelarVentasSincronizar();
            GetVentasTimer = new Timer();
            GetVentasTimer.Elapsed += (sender, e) =>
            {
                GetVentas();
                GetTurnosAbiertos();
            };
            GetVentasTimer.Interval = 10000;
            GetVentasTimer.AutoReset = true;
            GetVentasTimer.Start();
            GetVentas();
            GetTurnosAbiertos();
        }

        public void CancelarVentasSincronizar()
        {
            if (GetVentasTimer != null)
            {
                GetVentasTimer.Stop();
                GetVentasTimer = null;
            }
        }

        public void ActualizarVentas()
        {
            GetVentas();
        }
        private void GetVentas()
        {
            string? orden = null;
            if (FiltrosCuenta.ContainsKey("orden"))
            {
                orden = FiltrosCuenta["orden"];
            }
            ApiClient.Ventas(orden, delegate(JObject respuesta)
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
                    var ventasJson = (JArray?)respuesta["ventas"];
                    var ventas = new List<Venta>();
                    if (ventasJson != null && ventasJson.Count > 0)
                    {
                        foreach (JObject ventaJson in ventasJson)
                        {
                            var venta = new Venta(ventaJson);

                            bool visible = ViewModel.Usuario.RevisarPermiso("pv.todas-ventas") || ViewModel.Usuario.BuscarMesa($"{venta.NumeroCuenta}");

                            if (FiltrosCuenta.ContainsKey("usuario_id") && visible)
                            {
                                visible = FiltrosCuenta["usuario_id"] == $"{venta.UsuarioId}";
                            }

                            if (FiltrosCuenta.ContainsKey("venta_tipo_id") && visible)
                            {
                                visible = FiltrosCuenta["venta_tipo_id"] == $"{venta.VentaTipoId}";
                            }

                            venta.Visible = visible;
                            ventas.Add(venta);
                        }
                    }
                    VentasStatic.TodasVentas = ventas;
                    this.Dispatcher.Invoke(() =>
                    {
                        ViewModel.setVentas(VentasStatic.TodasVentas);
                    });
                }
            });
        }

        private void GetTurnosAbiertos()
        {
            ApiClient.TurnosAbiertos(delegate (JObject respuesta)
            {
                var error = (string?)respuesta["error"];
                if (error != null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        DialogoAlertaCm.show("Turnos abiertos", error);
                    });
                    return;
                }
                else
                {
                    var turnosAbiertosJson = (JArray?)respuesta["turnos_abiertos"];
                    var TurnosAbiertos = new List<TurnoAbierto>();
                    if (turnosAbiertosJson != null && turnosAbiertosJson.Count > 0)
                    {
                        foreach (JObject turnoAbiertoJson in turnosAbiertosJson)
                        {
                            var turno = new TurnoAbierto(turnoAbiertoJson);
                            TurnosAbiertos.Add(turno);
                        }
                    }
                    ViewModel.SetTurnosAbiertos(TurnosAbiertos);
                }
            });
        }

        /**
         * Restablece el mensaje del tablero
         */
        private void resetMensajeTablero()
        {
            resetMensajeTablero(null);
        }
        private void resetMensajeTablero(string? mensaje)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (mensaje != null)
                {
                    ViewModel.MensajeTablero = mensaje;
                }
                else
                {
                    ViewModel.MensajeTablero = Mensajes.Textos["ingresa_cuenta"];
                }

                ViewModel.MensajeTableroColor = "#a3a3a3";
                ViewModel.MensajeTableroSize = 23;
                CodigoIngresado = false;
            });
        }

        /**
         * Funcion cuando se hace click en los numero del tablero
         */
        private void BotonTable_Click(object sender, RoutedEventArgs e)
        {
            EventClickArgs extras = (EventClickArgs)e;
            NumeroTablero($"{(string?)extras.Extras["Valor"]}");
        }

        /**
         * Agregar un numero al teclado
         */
        private void NumeroTablero(string valorBoton)
        {

            if (!CodigoIngresado)
            {
                if (valorBoton != "0")
                {
                    ViewModel.MensajeTablero = valorBoton;
                    CodigoIngresado = true;
                    ViewModel.MensajeTableroColor = "#333333";
                    ViewModel.MensajeTableroSize = 60;
                }
                
            }
            else if (ViewModel.MensajeTablero.Length < 4)
            {
                ViewModel.MensajeTablero += valorBoton;
            }
        }

        /**
         * Funcion cuando se hace click en boton borrar del tablero
         */
        private void BotonBorrar_Click(object sender, RoutedEventArgs e)
        {
            resetMensajeTablero();
        }

        /**
         * Borra numeros del tablero
         */
        private void BorrarTablero()
        {

            if (CodigoIngresado && ViewModel.MensajeTablero.Length > 1)
            {
                ViewModel.MensajeTablero = ViewModel.MensajeTablero.Substring(0, ViewModel.MensajeTablero.Length - 1);
            }
            else
            {
                resetMensajeTablero();
            }
        }

        /**
         * Funcion cuando se hace click en boton entrar del tablero
         */
        private void BotonEnter_Click(object sender, RoutedEventArgs e)
        {
            EnterTablero();
        }

        /**
        * Funcion cuando se hace click en boton entrar del tablero
        */
        private void BotonAtrasTablero_Click(object sender, RoutedEventArgs e)
        {
            if (CodigoIngresado)
            {
                ViewModel.MensajeTablero = ViewModel.MensajeTablero.Substring(0, ViewModel.MensajeTablero.Length - 1);
                if (ViewModel.MensajeTablero.Length == 0)
                {
                    resetMensajeTablero();
                }
            }
        }



        /**
         * Valida que se haya ingresado un folio de venta
         */
        private void EnterTablero()
        {

            if (CodigoIngresado)
            {
                BuscaVentaNumero(ViewModel.MensajeTablero);
            }
        }

        public ObservableCollection<Venta> getVentas()
        {
            return ViewModel.Ventas;
        }
        public bool RevisarExisteCuenta(string numeroCuenta)
        {
            return VentasStatic.getVenta(numeroCuenta) != null;
        }

        public void BuscaVentaNumero(string numeroVenta)
        {
            resetMensajeTablero();

            if (!ViewModel.Usuario.RevisarPermiso("pv.todas-ventas") && !ViewModel.Usuario.BuscarMesa(numeroVenta))
            {
                this.Dispatcher.Invoke(() =>
                {
                    DialogoAlertaCm.show("Listado de cuentas", $"No tienes asignada la mesa \"{numeroVenta}\"");
                });
                return;
            }
            else
            {
                Venta? venta = VentasStatic.getVenta(numeroVenta);

                if (venta == null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        if (MesasStatic.IsMesaValida($"{numeroVenta}"))
                        {
                            var datos = new Dictionary<string, object?>();
                            datos["Titulo"] = "Ingresa comensales";
                            datos["Valor"] = null;
                            RemoverKeyDownEvento();
                            getWindow().MostrarTecladoNumerico(datos, delegate (Dictionary<string, object?> respuesta)
                            {
                                int personas = int.Parse($"{(string?)respuesta["Valor"]}");
                                CrearVenta(numeroVenta, personas);
                            }, delegate (Dictionary<string, object?> cerrar)
                            {
                                AgregarKeyDownEvento();
                            });
                        }
                        else
                        {
                            var datos = new Dictionary<string, object?>();
                            datos["Predeterminado"] = false;
                            getWindow().MostrarVentaTipos(datos, delegate (Dictionary<string, object?> respuesta)
                            {
                                string Valor = $"{(string?)respuesta["VentaTipoId"]}";
                                if (!string.IsNullOrEmpty(Valor))
                                {
                                    CrearVenta(numeroVenta, 1, Valor);
                                }
                            }, null);
                        }    
                    });
                }
                else
                {
                    VentaDetalle(venta);
                }
            }

        }

        private void BotonAtras_Click(object sender, RoutedEventArgs e)
        {
            RemoverKeyDownEvento();
            this.Dispatcher.Invoke(() =>
            {
                getWindow().MostrarInicio();
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

        private void CuentaDetalle_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            if (CuentasListado.SelectedIndex > -1)
            {
                VentaDetalle(ViewModel.Ventas[CuentasListado.SelectedIndex]);
            }
        }

        private void VentaDetalle(Venta venta)
        {
            RemoverKeyDownEvento();
            this.Dispatcher.Invoke(() =>
            {
                getWindow().MostrarVentaDetalle(venta);
            });
        }

        private void CrearVenta(string numeroCuenta, int personas, string? ventaTipoId = null)
        {
            string usuarioId = $"{ViewModel.Usuario.Id}";
            if (ViewModel.Usuario.RevisarPermiso("pv.todas-ventas"))
            {
                string? mesaUsuarioId = MesasStatic.getMesaUsuarioId(numeroCuenta);
                if (mesaUsuarioId != null)
                {
                    usuarioId = mesaUsuarioId;
                }
            }
            ApiClient.VentaNueva(usuarioId, numeroCuenta, personas, ventaTipoId, delegate (JObject respuesta)
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
                    resetMensajeTablero();
                    var venta = new Venta((JObject?)respuesta["venta"]);
                    this.Dispatcher.Invoke(() =>
                    {
                        ViewModel.addVenta(venta);
                    });
                    VentaDetalle(venta);
                }
            });

        }

        private void BotonAusente_Click(object sender, RoutedEventArgs e)
        {

            ApiClient.UsuarioAusente($"{ViewModel.Usuario.Id}", !ViewModel.UsuarioAusente, delegate (JObject respuesta) {
                var error = (string?)respuesta["error"];
                if (error != null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        DialogoAlertaCm.show("Usuario ausente", error);
                    });
                    return;
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        ViewModel.UsuarioAusente = !ViewModel.UsuarioAusente;
                        ViewModel.Usuario.Ausente = ViewModel.UsuarioAusente;
                        ViewModel.Usuario.save();
                        getWindow().UsuarioSeleccionado = ViewModel.Usuario;
                        MesasStatic.CargarMesas();
                    });
                }
            });
        }

        private void MostrarConfiguraciones_OnClick(object sender, RoutedEventArgs e)
        {
            RemoverKeyDownEvento();
            this.Dispatcher.Invoke(() =>
            {
                getWindow().MostrarConfiguraciones(delegate ()
                {
                    AgregarKeyDownEvento();
                });
            });
        }

        private void Empleados_OnClick(object sender, RoutedEventArgs e)
        {
            RemoverKeyDownEvento();
            this.Dispatcher.Invoke(() =>
            {
                getWindow().MostrarEmpleados();
            });
        }

        private void AsignarMesas_Click(object sender, RoutedEventArgs e)
        {
            RemoverKeyDownEvento();
            this.Dispatcher.Invoke(() =>
            {
                getWindow().MostrarAsignarMesas();
            });
        }

        
        private void DesactivarProductos_Click(object sender, RoutedEventArgs e)
        {
            RemoverKeyDownEvento();
            this.Dispatcher.Invoke(() =>
            {
                getWindow().MostrarDesactivarProductos();
            });
        }
        private void Caja_Click(object sender, RoutedEventArgs e)
        {
            RemoverKeyDownEvento();
            this.Dispatcher.Invoke(() =>
            {
                getWindow().MostrarCaja();
            });
        }

        private void ListadoVentas_Loaded(object sender, RoutedEventArgs e)
        {
            AgregarKeyDownEvento();
        }

        private void AgregarKeyDownEvento()
        {
            this.Dispatcher.Invoke(() =>
            {
                if (KeyDownEvento == null)
                {
                    KeyDownEvento = new KeyEventHandler(HandleKeyPress);
                }
                var window = Window.GetWindow(this);
                window.KeyDown += new KeyEventHandler(HandleKeyPress);
            });
        }
        private void RemoverKeyDownEvento()
        {
            this.Dispatcher.Invoke(() =>
            {
                var window = Window.GetWindow(this);
                if (KeyDownEvento != null)
                {
                    window.KeyDown -= new KeyEventHandler(HandleKeyPress);
                }
            });

        }

        private void HandleKeyPress(object sender, KeyEventArgs e)
        {
            string TeclaKeyboard = e.Key.ToString().ToLower();
            if (e.Key == Key.Return)
            {
                EnterTablero();
            }
            else if (e.Key == Key.Back)
            {
                BorrarTablero();
            }
            else
            {
                if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                {
                    NumeroTablero(TeclaKeyboard.Replace("numpad", ""));
                }
                else if (e.Key >= Key.D0 && e.Key <= Key.D9)
                {
                    NumeroTablero(TeclaKeyboard.Replace("d", ""));
                }
            }
        }

        /**
         * Registro de turno
         */
        private void RegistroTurno_OnClick(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                string titulo = ViewModel.TurnoIniciado ? "¿Finalizar turno?" : "¿Iniciar turno?";
                DialogoAlertaCm.show(titulo, true, delegate ()
                {
                    if (ViewModel.TurnoIniciado)
                    {
                        finalizarTurno();
                    }
                    else
                    {
                        iniciarTurno();
                    }
                });
            });

            
        }

        private void iniciarTurno()
        {
            ApiClient.UsuarioAsistenciaRegistrarEntrada($"{ViewModel.Usuario.Id}", delegate (JObject respuesta){
                var error = (string?)respuesta["error"];
                if (error != null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        DialogoAlertaCm.show("Registro asistencia", error);
                    });
                    return;
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        ViewModel.TurnoIniciado = true;
                        DialogoAlertaCm.show("Registro asistencia", "Turno iniciado");
                    });
                }
            });
        }

        private void finalizarTurno()
        {
            ApiClient.UsuarioAsistenciaRegistrarSalida($"{ViewModel.Usuario.Id}", delegate (JObject respuesta) {
                var error = (string?)respuesta["error"];
                if (error != null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        DialogoAlertaCm.show("Registro asistencia", error);
                    });
                    return;
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        ViewModel.TurnoIniciado = false;
                        DialogoAlertaCm.show("Registro asistencia", "Turno finalizado");
                    });
                }
            });
        }

        private void HeaderBotonFiltros_Click(object sender, RoutedEventArgs e)
        {
            FiltrosCuentasModal.mostrar(FiltrosCuenta, delegate(Dictionary<string, object?> respuesta){
                FiltrosCuenta = (Dictionary<string, string>)respuesta["Filtros"];
                GetVentas();
            });
        }
    }

}
