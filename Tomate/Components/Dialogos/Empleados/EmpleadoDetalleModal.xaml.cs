using MaterialDesignThemes.Wpf;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Tomate.Components.ViewModels.Empleados;
using Tomate.HttpRequest;
using Tomate.Models.Usuarios;
using Tomate.Utils;
using Tomate.Views;

namespace Tomate.Components.Dialogos.Empleados
{
    /// <summary>
    /// Interaction logic for EmpleadoDetalleModal.xaml
    /// </summary>
    public partial class EmpleadoDetalleModal : UserControl
    {

        public static readonly RoutedEvent OnActualizarEvent = EventManager.RegisterRoutedEvent("OnActualizar", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(EmpleadoDetalleModal));

        public event RoutedEventHandler OnActualizar
        {
            add { AddHandler(OnActualizarEvent, value); }
            remove { RemoveHandler(OnActualizarEvent, value); }
        }

        EmpleadoDetalleViewModel ViewModel { get; set; }
        private List<Perfil> Perfiles { get; set; } = new List<Perfil>();

        public EmpleadoDetalleModal()
        {
            ViewModel = new EmpleadoDetalleViewModel();
            InitializeComponent();
            DataContext = ViewModel;
        }

        public void mostrar()
        {
            mostrar(null);
        }
        public void mostrar(Usuario? usuario)
        {

            Perfiles = Perfil.todos();

            this.Dispatcher.Invoke(() =>
            {
                PerfilComboBox.IsEnabled = true;
                PerfilComboBox.Items.Clear();

                foreach (Perfil perfil in Perfiles)
                {
                    PerfilComboBox.Items.Add(perfil.Nombre);
                }
                DialogoEmpleado.IsOpen = true;

                Password.Password = "";
                ConfirmarPassword.Password = "";
                ViewModel.ErrorCodigo = "";
                ViewModel.ErrorPerfil = "";
                ViewModel.ErrorNombre = "";
                ViewModel.ErrorAlias = "";
                ViewModel.ErrorGeneral = "";
                ViewModel.ErrorPassword = "";
                ViewModel.ErrorConfirmarPassword = "";
            });

            if (usuario == null)
            {
                ViewModel.Usuario = new Usuario();
                DialogoTitulo.Text = "Nuevo empleado";
            }
            else
            {
                DialogoTitulo.Text = "Editar empleado";
                ViewModel.Usuario = usuario;
                PerfilComboBox.SelectedValue = usuario.PerfilNombre;
                this.Dispatcher.Invoke(() =>
                {
                    if (getWindow().UsuarioSeleccionado?.Id == usuario.Id)
                    {
                        PerfilComboBox.IsEnabled = false;
                    }
                });
            }
        }

        public void ocultar()
        {
            DialogoEmpleado.IsOpen = false;
        }

        private void BotonCerrarModal_Click(object sender, RoutedEventArgs e)
        {
            ocultar();
        }

        private void DialogoTeclado_DialogOpened(object sender, DialogOpenedEventArgs eventArgs)
        {
            BackgroundModalEmpleado.Visibility = Visibility.Visible;
            this.Dispatcher.Invoke(() =>
            {
                getWindow().AgregarBlur();
            });
        }

        private void DialogoTeclado_DialogClosed(object sender, DialogClosedEventArgs eventArgs)
        {
            BackgroundModalEmpleado.Visibility = Visibility.Collapsed;
            this.Dispatcher.Invoke(() =>
            {
                getWindow().RemoverBlur();
            });
        }



        private void Teclado_OnClose(object sender, RoutedEventArgs e)
        {
            DialogoEmpleado.IsOpen = true;
        }

        private void EditarCodigo_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DialogoEmpleado.IsOpen = false;
            ViewModel.ErrorCodigo = "";
            ViewModel.ErrorGeneral = "";

            this.Dispatcher.Invoke(() =>
            {
                var datos = new Dictionary<string, object?>();
                datos["Titulo"] = "Ingresa código";
                datos["Valor"] = $"{ViewModel.Usuario.Codigo}";
                datos["Limite"] = 4;
                datos["DesactivarBlur"] = false;

                getWindow().MostrarTecladoNumerico(datos, delegate (Dictionary<string, object?> respuesta)
                {
                    string Valor = $"{(string?)respuesta["Valor"]}";
                    var usuarioCodigo = Usuario.buscarCodigo(Valor);
                    if (usuarioCodigo != null && usuarioCodigo.Id != ViewModel.Usuario.Id)
                    {
                        ViewModel.ErrorCodigo = "Ya existe el código";
                    }

                    ViewModel.Usuario.Codigo = Valor;
                    ViewModel.Usuario = ViewModel.Usuario;
                    DialogoEmpleado.IsOpen = true;
                }, delegate (Dictionary<string, object?> respuesta)
                {
                    DialogoEmpleado.IsOpen = true;
                });
            });
        }

        private void EditarNombre_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DialogoEmpleado.IsOpen = false;
            ViewModel.ErrorNombre = "";
            ViewModel.ErrorGeneral = "";

            this.Dispatcher.Invoke(() =>
            {
                var datos = new Dictionary<string, object?>();
                datos["Titulo"] = "Ingresa nombre";
                datos["Valor"] = $"{ViewModel.Usuario.Nombre}";
                datos["DesactivarBlur"] = false;
                getWindow().MostrarTeclado(datos, delegate (Dictionary<string, object?> respuesta)
                {
                    ViewModel.Usuario.Nombre = $"{(string?)respuesta["Valor"]}";
                    ViewModel.Usuario = ViewModel.Usuario;
                    DialogoEmpleado.IsOpen = true;
                }, delegate (Dictionary<string, object?> respuesta)
                {
                    DialogoEmpleado.IsOpen = true;
                });
            });

        }

        private void EditarAlias_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DialogoEmpleado.IsOpen = false;
            ViewModel.ErrorAlias = "";
            ViewModel.ErrorGeneral = "";
            this.Dispatcher.Invoke(() =>
            {
                var datos = new Dictionary<string, object?>();
                datos["Titulo"] = "Ingresa alias";
                datos["Valor"] = $"{ViewModel.Usuario.Alias}";
                datos["DesactivarBlur"] = false;
                datos["Limite"] = 10;
                getWindow().MostrarTeclado(datos, delegate (Dictionary<string, object?> respuesta)
                {
                    ViewModel.Usuario.Alias = $"{(string?)respuesta["Valor"]}";
                    ViewModel.Usuario = ViewModel.Usuario;
                    DialogoEmpleado.IsOpen = true;
                }, delegate (Dictionary<string, object?> respuesta)
                {
                    DialogoEmpleado.IsOpen = true;
                });
            });
        }

        private void EditarPassword_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DialogoEmpleado.IsOpen = false;
            ViewModel.ErrorPassword = "";
            ViewModel.ErrorGeneral = "";

            this.Dispatcher.Invoke(() =>
            {
                var datos = new Dictionary<string, object?>();
                datos["Titulo"] = "Ingresa contraseña";
                datos["Valor"] = null;
                datos["Limite"] = 4;
                datos["Secreto"] = true;
                datos["DesactivarBlur"] = false;

                getWindow().MostrarTecladoNumerico(datos, delegate (Dictionary<string, object?> respuesta)
                {
                    Password.Password = $"{(string?)respuesta["Valor"]}";
                    DialogoEmpleado.IsOpen = true;
                }, delegate (Dictionary<string, object?> respuesta)
                {
                    DialogoEmpleado.IsOpen = true;
                });
            });
        }

        private void EditarConfirmarPassword_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DialogoEmpleado.IsOpen = false;
            ViewModel.ErrorConfirmarPassword = "";
            ViewModel.ErrorGeneral = "";

            this.Dispatcher.Invoke(() =>
            {
                var datos = new Dictionary<string, object?>();
                datos["Titulo"] = "Confirmar contraseña";
                datos["Valor"] = null;
                datos["Limite"] = 4;
                datos["Secreto"] = true;
                datos["DesactivarBlur"] = false;

                getWindow().MostrarTecladoNumerico(datos, delegate (Dictionary<string, object?> respuesta)
                {
                    ConfirmarPassword.Password = $"{(string?)respuesta["Valor"]}";
                    DialogoEmpleado.IsOpen = true;
                }, delegate (Dictionary<string, object?> respuesta)
                {
                    DialogoEmpleado.IsOpen = true;
                });
            });
        }

        private void GuardarEmpleado_Click(object sender, RoutedEventArgs e)
        {
            if (Loader.Visibility != Visibility.Visible)
            {
                ViewModel.ErrorGeneral = "";
                ComprobarInformacion();
            }

        }

        private void ComprobarInformacion()
        {
            var password = Password.Password.ToString();
            var confirmarPassword = ConfirmarPassword.Password.ToString();

            if ((ViewModel.Usuario?.Codigo ?? "").Length < 4)
            {
                ViewModel.ErrorCodigo = "Ingresa código de 4 dígitos";
            }

            if (PerfilComboBox.SelectedIndex < 0)
            {
                ViewModel.ErrorPerfil = "Selecciona un perfil";
            }

            if ((ViewModel.Usuario?.Nombre ?? "").Length < 2)
            {
                ViewModel.ErrorNombre = "Ingresa nombre";
            }

            if ((ViewModel.Usuario?.Alias ?? "").Length < 2)
            {
                ViewModel.ErrorAlias = "Ingresa alias";
            }

            if (ViewModel.Usuario?.Id == null && password.Length < 4)
            {
                ViewModel.ErrorPassword = "Ingresa contraseña de 4 dígitos";
            }

            if (password.Length > 0 && password != confirmarPassword)
            {
                ViewModel.ErrorConfirmarPassword = "Contraseña no coincide";
            }

            if (ViewModel.ErrorCodigo.Length > 0 || ViewModel.ErrorPerfil.Length > 0 ||
                ViewModel.ErrorNombre.Length > 0 || ViewModel.ErrorAlias.Length > 0 ||
                ViewModel.ErrorPassword.Length > 0 || ViewModel.ErrorConfirmarPassword.Length > 0)
            {
                return;
            }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            ViewModel.Usuario.PerfilId = Perfiles[PerfilComboBox.SelectedIndex].Id;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            this.Dispatcher.Invoke(() =>
            {
                Loader.Visibility = Visibility.Visible;
            });

            ApiClient.GuardarUsuario(ViewModel.Usuario, password, delegate (JObject respuesta)
            {
                this.Dispatcher.Invoke(() =>
                {
                    Loader.Visibility = Visibility.Collapsed;
                });

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
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    var usuario = new Usuario((JObject?)respuesta["usuarios"]);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    usuario.save();
                    this.Dispatcher.Invoke(() =>
                    {
                        ocultar();
                        var args = new EventClickArgs(OnActualizarEvent);
                        RaiseEvent(args);
                    });

                }
            });

        }

        private void Perfil_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.ErrorPerfil = "";
            ViewModel.ErrorGeneral = "";
        }

        /**
         * Obtiene la ventana principal del programa
         */
        private MainWindow getWindow()
        {
            return WindowUtils.getMainWindow();
        }
    }
}
