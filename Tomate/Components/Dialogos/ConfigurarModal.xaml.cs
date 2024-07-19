using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Tomate.Components.ViewModels;
using Tomate.DBHelper;
using Tomate.HttpRequest;
using Tomate.Models;
using Tomate.Utils;
using Tomate.Views;

namespace Tomate.Components.Dialogos
{
    /// <summary>
    /// Lógica de interacción para ConfigurarModal.xaml
    /// </summary>
    /// 
    public partial class ConfigurarModal : UserControl
    {
        public ConfiguracionViewModel ViewModel { get; set; }

        private bool? UltimoRegistrarInicio = null;
        public bool DesactivarBlur { get; set; } = true;

        public static readonly RoutedEvent OnGuardadoEvent = EventManager.RegisterRoutedEvent("OnGuardado", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(ConfigurarModal));

        public event RoutedEventHandler OnGuardado
        {
            add { AddHandler(OnGuardadoEvent, value); }
            remove { RemoveHandler(OnGuardadoEvent, value); }
        }

        private string? UltimaDireccionServidor { get; set; } = null;

        private Action? Callback { get; set; }

        public ConfigurarModal()
        {
            ViewModel = new ConfiguracionViewModel();

            InitializeComponent();

            DataContext = ViewModel;
        }

        public void mostrar(bool borrarBaseDatos, Action callback)
        {
            Callback = callback;
            Dialogo.IsOpen = true;
            DesactivarBlur = true;

            if (borrarBaseDatos)
            {
                BorrarBaseDatosBoton.Visibility = Visibility.Visible;
            }
            else
            {
                BorrarBaseDatosBoton.Visibility = Visibility.Collapsed;
            }

            if (Configuracion.isConfigurado())
            {
                Dialogo.CloseOnClickAway = true;
                BotonCerrar.Visibility = Visibility.Visible;

                ViewModel.DireccionServidor = Configuracion.getValor(Configuracion.Key.DIRECCION_SERVIDOR);
                ViewModel.TokenAcceso = Configuracion.getValor(Configuracion.Key.TOKEN_ACCESO);
                UltimoRegistrarInicio = Configuracion.getValor(Configuracion.Key.INICIAR_WINDOWS) == "1";

                UltimaDireccionServidor = ViewModel.DireccionServidor;
            }
            else
            {
                UltimaDireccionServidor = null;
                UltimoRegistrarInicio = null;
                Dialogo.CloseOnClickAway = false;
                BotonCerrar.Visibility = Visibility.Collapsed;
                BorrarBaseDatosBoton.Visibility = Visibility.Collapsed;
            }
            
        }

        public void ocultar()
        {
            this.Dispatcher.Invoke(() =>
            {
                Dialogo.IsOpen = false;
            });
        }

        private void guardado()
        {
            var args = new EventClickArgs(OnGuardadoEvent);
            var Extras = new Dictionary<string, object?>();
            args.Extras = Extras;
            this.Dispatcher.Invoke(() =>
            {
                RaiseEvent(args);
            });
        }

        private void BotonCerrarModal_Click(object sender, RoutedEventArgs e)
        {
            ocultar();
        }

        private void GuardarConfiguracion_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.DireccionServidor == null || ViewModel.DireccionServidor.Length < 15)
            {
                ViewModel.ErrorDireccion = "Agrega una dirección IP";
            }
            if (ViewModel.TokenAcceso == null || ViewModel.TokenAcceso.Length < 5)
            {
                ViewModel.ErrorToken = "Agrega un token de acceso válido";
            }

            if (ViewModel.ErrorToken.Length > 0 && ViewModel.ErrorDireccion.Length > 0)
            {
                return;
            }

            ApiClient.SetConfiguracion(ViewModel.DireccionServidor, ViewModel.TokenAcceso);

            this.Dispatcher.Invoke(() =>
            {
                Loader.Visibility = Visibility.Visible;
            });


            try
            {
                getWindow().SincronizarInformacion((UltimaDireccionServidor != null && UltimaDireccionServidor != ViewModel.DireccionServidor), 
                    delegate (bool success, string mensaje)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        Loader.Visibility = Visibility.Collapsed;
                    });

                    if (success)
                    {
                        Configuracion.setValor(Configuracion.Key.DIRECCION_SERVIDOR, $"{ViewModel.DireccionServidor}");
                        Configuracion.setValor(Configuracion.Key.TOKEN_ACCESO, $"{ViewModel.TokenAcceso}");
                        Configuracion.setValor(Configuracion.Key.INICIAR_WINDOWS, ViewModel.RegistrarInicio ? "1" : "0");
                        Configuracion.setConfigurado("1");

                        if (UltimoRegistrarInicio != ViewModel.RegistrarInicio)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                getWindow().RegistrarInicioWindows(ViewModel.RegistrarInicio);
                            });
                        }

                        ocultar();
                        guardado();
                    }
                    else
                    {
                        ViewModel.ErrorGeneral = mensaje;
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }




        }

        private void BorrarConfiguracion_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                DesactivarBlur = false;
                Dialogo.IsOpen = false;
                DialogoAlertaCm.show("¿Borrar base de datos?", true, delegate ()
                {
                    DbHelper.RestablecerTablas();
                    Application.Current.Shutdown();
                    System.Windows.Forms.Application.Restart();
                }, delegate ()
                {
                    Dialogo.IsOpen = true;
                    DesactivarBlur = true;
                });
            });
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
            if (Callback != null)
            {
                Callback.Invoke();
            }
            BackgroundDialogo.Visibility = Visibility.Collapsed;
            this.Dispatcher.Invoke(() =>
            {
                if (DesactivarBlur)
                {
                    getWindow().RemoverBlur();
                }
                
                getWindow().CargarConfiguracion(true);
            });

        }

        /**
         * Obtiene la ventana principal del programa
         */
        private MainWindow getWindow()
        {
            return WindowUtils.getMainWindow();
        }

        private void Direccion_TextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.ErrorGeneral = "";
            ViewModel.ErrorDireccion = "";
        }

        private void Token_TextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.ErrorGeneral = "";
            ViewModel.ErrorToken = "";

        }
    }
}
