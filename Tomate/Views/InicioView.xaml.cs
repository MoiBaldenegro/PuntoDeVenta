using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tomate.Lang;
using Tomate.Models;
using Tomate.Models.Usuarios;
using Tomate.Utils;
using Tomate.ViewModels;

namespace Tomate.Views
{
    /// <summary>
    /// Interaction logic for InicioView.xaml
    /// </summary>
    public partial class InicioView : UserControl
    {

        //Viewmodel de la clase con variables que se muestran en la vista
        public InicioViewModel ViewModel { get; set; }
        //Controlador de la huella
        //public HuellaController HuellaController { get; set; }

        //Funciona para saber si se ha iniciado a ingresar un folio
        private bool CodigoIngresado = false;
        //Arreglo para comparar huellas y obtener el folio
        //public Dictionary<string, byte[]> HuellasUsuarios { get; set; } = new Dictionary<string, byte[]>();

        private Timer? ResetTableroTimer;
        private Timer? FechaHoraTimer;

        private KeyEventHandler? KeyDownEvento { get; set; }

        private bool ConfiguracionAbierta = false;
        public InicioView()
        {

            ViewModel = new InicioViewModel();
            //Texto por defecto en el tablero
            ViewModel.MensajeTablero = Mensajes.Textos["ingresa_codigo_id"];
            //Versión actual del programa
            ViewModel.VersionSoftware = $"v.{Assembly.GetExecutingAssembly().GetName().Version}";

            ViewModel.VersionSoftware = ViewModel.VersionSoftware.Substring(0, ViewModel.VersionSoftware.Length - 2);

            InitializeComponent();
            DataContext = ViewModel;

            //InitHuella();

            //actualizarUsuarios();
            RevisarConfigurado();
        }

        public void RevisarConfigurado()
        {
            if (!Configuracion.isConfigurado())
            {
                ConfiguracionAbierta = true;
                RemoverKeyDownEvento();
                getWindow().MostrarConfiguraciones(delegate ()
                {
                    ConfiguracionAbierta = false;
                    AgregarKeyDownEvento();
                });
            }
            else
            {
                getWindow().CargarConfiguracion(true);
            }
        }

        /**
         * Comprueba si el loader está activo
         */
        private bool isLoader()
        {
            return Loader.Visibility == Visibility.Visible;
        }

        public void LectorHuellaDisponible(bool disponible)
        {
            resetMensajeTablero();
        }

        /**
         * Muestra el loder del tablero
         */
        public void MostrarLoader()
        {
            this.Dispatcher.Invoke(() =>
            {
                TextoTablero.Visibility = Visibility.Hidden;
                Loader.Visibility = Visibility.Visible;
            });
        }

        /**
         * Oculta el loder del tablero
         */
        private void ocultarLoader()
        {
            this.Dispatcher.Invoke(() =>
            {
                TextoTablero.Visibility = Visibility.Visible;
                Loader.Visibility = Visibility.Hidden;
            });
        }

        /**
         * Restablece el mensaje del tablero
         */
        private void resetMensajeTablero()
        {
            this.Dispatcher.Invoke(() =>
            {
                if (getWindow().HuellaController.Available)
                {
                    ViewModel.MensajeTablero = Mensajes.Textos["ingresa_codigo_huella"];
                }
                else
                {
                    ViewModel.MensajeTablero = Mensajes.Textos["ingresa_codigo_id"];
                }
                CodigoIngresado = false;
            });
            cancelarResetMensajetableroAsync();
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
            if (isLoader())
            {
                return;
            }

            resetMensajetableroAsync();

            if (!CodigoIngresado)
            {
                ViewModel.MensajeTablero = valorBoton;
                CodigoIngresado = true;
            }
            else
            {
                if (ViewModel.MensajeTablero.Length < 4)
                {
                    ViewModel.MensajeTablero += valorBoton;
                }
                if (ViewModel.MensajeTablero.Length == 4)
                {
                    BuscarUsuarioCodigo(ViewModel.MensajeTablero);
                }

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
            if (isLoader())
            {
                return;
            }
            cancelarResetMensajetableroAsync();
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
         * Valida que se haya ingresado un folio
         */
        private void EnterTablero()
        {
            if (isLoader())
            {
                return;
            }
            cancelarResetMensajetableroAsync();
            if (CodigoIngresado)
            {
                MostrarLoader();
                BuscarUsuarioCodigo(ViewModel.MensajeTablero);
            }
        }

        public void BuscarUsuarioById(string? id)
        {
            ComprobarUsuario(Usuario.buscarId($"{id}"));
        }

        /**
         * Busca un empleado por codigo para abrir la siguiente seccion
         */
        private void BuscarUsuarioCodigo(string? codigo)
        {
            ComprobarUsuario(Usuario.buscarCodigo($"{codigo}", true), true);
        }

        private void ComprobarUsuario(Usuario? usuario, bool validarPass = false)
        {
            resetMensajeTablero();
            if (usuario != null)
            {
                RemoverKeyDownEvento();
                if (validarPass && usuario.Password != null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        var datos = new Dictionary<string, object?>();
                        datos["Titulo"] = "Ingresa contraseña";
                        datos["Valor"] = null;
                        datos["Limite"] = 4;
                        datos["Secreto"] = true;
                        datos["AutoEnter"] = true;
                        datos["CerrarClickFuera"] = false;

                        getWindow().MostrarTecladoNumerico(datos, delegate (Dictionary<string, object?> respuesta)
                        {
                            if ($"{(string?)respuesta["Valor"]}" == $"{usuario.Password}")
                            {
                                RemoverKeyDownEvento();
                                MostrarVentasEmpleado(usuario);
                            }
                            else
                            {
                                this.Dispatcher.Invoke(() =>
                                {
                                    ViewModel.MensajeTablero = Mensajes.Textos["ingresa_empleado_password_error"];
                                    CodigoIngresado = false;
                                });
                                resetMensajetableroAsync();
                            }
                        }, delegate (Dictionary<string, object?> respuesta)
                        {
                            AgregarKeyDownEvento();
                        });
                    });
                }
                else
                {
                    MostrarVentasEmpleado(usuario);
                }
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    ViewModel.MensajeTablero = Mensajes.Textos["ingresa_empleado_error"];
                    CodigoIngresado = false;
                });
                resetMensajetableroAsync();
            }
            ocultarLoader();
        }


        private void resetMensajetableroAsync()
        {

            cancelarResetMensajetableroAsync();

            ResetTableroTimer = new Timer();
            ResetTableroTimer.Elapsed += (sender, e) =>
            {
                resetMensajeTablero();
            };
            ResetTableroTimer.Interval = 5000;
            ResetTableroTimer.AutoReset = false;
            ResetTableroTimer.Start();
        }

        private void cancelarResetMensajetableroAsync()
        {
            if (ResetTableroTimer != null)
            {
                ResetTableroTimer.Stop();
                ResetTableroTimer = null;
            }

        }

        /**
         * Muestra la vista de ventas si se encuentra el empleado
         */
        private void MostrarVentasEmpleado(Usuario empleado)
        {
            new Task(() =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    getWindow().MostrarListadoVentas(empleado);
                });
            }).Start();


            DetenerFechaHoraTimer();
            resetMensajeTablero();

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

        private void Inicio_Loaded(object sender, RoutedEventArgs e)
        {
            AgregarKeyDownEvento();
            IniciarFechaHoraTimer();
        }

        private void IniciarFechaHoraTimer()
        {
            FechaHoraTimer = new Timer();
            FechaHoraTimer.Elapsed += (sender, e) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    ViewModel.FechaHora = DateTime.Now.ToString("dd MMM yyyy HH:mm");
                });
            };
            FechaHoraTimer.Interval = 1000;
            FechaHoraTimer.AutoReset = true;
            FechaHoraTimer.Start();
            this.Dispatcher.Invoke(() =>
            {
                ViewModel.FechaHora = DateTime.Now.ToString("dd MMM yyyy HH:mm");
            });
        }

        private void DetenerFechaHoraTimer()
        {
            if (FechaHoraTimer != null)
            {
                FechaHoraTimer.Stop();
                FechaHoraTimer = null;
            }
        }

        private void AgregarKeyDownEvento()
        {
            if (!ConfiguracionAbierta)
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (KeyDownEvento == null)
                    {
                        KeyDownEvento = new KeyEventHandler(HandleKeyPress);
                    }
                    var window = Window.GetWindow(this);
                    window.KeyDown += KeyDownEvento;
                });
            }

        }
        private void RemoverKeyDownEvento()
        {
            this.Dispatcher.Invoke(() =>
            {
                var window = Window.GetWindow(this);
                if (KeyDownEvento != null)
                {
                    window.KeyDown -= KeyDownEvento;
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

    }
}
