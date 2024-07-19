using MaterialDesignThemes.Wpf;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
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
    /// Interaction logic for EmpleadoHuellasModal.xaml
    /// </summary>
    public partial class EmpleadoHuellasModal : UserControl
    {

        public EmpleadoHuellasViewModel ViewModel { get; set; }

        private string? UsuarioId = null;
        private int? DedoSeleccionado = null;
        private ObservableCollection<UsuarioHuella> Huellas = new ObservableCollection<UsuarioHuella>();

        public EmpleadoHuellasModal()
        {

            ViewModel = new EmpleadoHuellasViewModel();
            InitializeComponent();
            DataContext = ViewModel;
        }

        public void mostrar(string usuarioId)
        {
            UsuarioId = usuarioId;
            DialogoHuellas.IsOpen = true;
            RestablecerHuellaRegistro();
        }

        public void ocultar()
        {
            DialogoHuellas.IsOpen = false;
        }

        private void ActualizarDedos()
        {
            Huellas = UsuarioHuella.todas($"{UsuarioId}");
            int[] dedosAgregados = new int[Huellas.Count];
            int index = 0;
            foreach (UsuarioHuella huella in Huellas)
            {
#pragma warning disable CS8629 // Nullable value type may be null.
                dedosAgregados[index] = (int)huella.Posicion;
#pragma warning restore CS8629 // Nullable value type may be null.
                index++;
            }
            ManoView.SetDedosAgregados(dedosAgregados);
        }

        private void BotonCerrarModal_Click(object sender, RoutedEventArgs e)
        {
            ocultar();
        }

        private void DialogoTeclado_DialogOpened(object sender, DialogOpenedEventArgs eventArgs)
        {
            BackgroundModal.Visibility = Visibility.Visible;
            this.Dispatcher.Invoke(() =>
            {
                getWindow().AgregarBlur();
            });
        }

        private void DialogoTeclado_DialogClosed(object sender, DialogClosedEventArgs eventArgs)
        {
            BackgroundModal.Visibility = Visibility.Collapsed;
            if (CancelarRegistroHuella.Visibility == Visibility.Visible)
            {
                CancelarRegistro();
            }
            this.Dispatcher.Invoke(() =>
            {
                getWindow().RemoverBlur();
            });
        }

        private void Mano_OnSeleccionarDedo(object sender, RoutedEventArgs e)
        {
            EventClickArgs extras = (EventClickArgs)e;
#pragma warning disable CS8605 // Unboxing a possibly null value.
            DedoSeleccionado = (int)extras.Extras["NumeroDedo"];
#pragma warning restore CS8605 // Unboxing a possibly null value.

            RegistrarHuella();
        }

        public void ContadorRegistroHuella(int numero)
        {
            ViewModel.CambiarDedoContador(numero);
        }

        public void GuardarHuella(bool success, byte[]? huella)
        {

            if (!success)
            {
                this.Dispatcher.Invoke(() =>
                {
                    //RestablecerHuellaRegistro();
                    ViewModel.ErrorGeneral = "No se pudo registrar la huella";
                });

            }
            else
            {
#pragma warning disable CS8604 // Possible null reference argument.
                string huellaBase64 = $"{Convert.ToBase64String(huella)}";
#pragma warning restore CS8604 // Possible null reference argument.
                ApiClient.UsuarioNuevaHuella($"{UsuarioId}", huellaBase64, (int?)DedoSeleccionado, delegate (JObject respuesta)
                {
                    var error = (string?)respuesta["error"];
                    if (error != null)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            ViewModel.ErrorGeneral = error;
                        });
                        return;
                    }
                    else
                    {
#pragma warning disable CS8604 // Possible null reference argument.
                        var usuarioHuella = new UsuarioHuella(respuesta["huella"] as JObject);
#pragma warning restore CS8604 // Possible null reference argument.
                        usuarioHuella.save();
                        this.Dispatcher.Invoke(() =>
                        {
                            RestablecerHuellaRegistro();
                            ActualizarDedos();
                            getWindow().ActualizarUsuariosLector();
                        });
                    }
                });
            }
        }

        private void RestablecerHuellaRegistro()
        {
            ViewModel.ErrorGeneral = "";
            ListadoNumeroDedo.Visibility = Visibility.Hidden;
            CancelarRegistroHuella.Visibility = Visibility.Hidden;
            ViewModel.RestablecerColoresregistro();
            ViewModel.MensajeModal = "Registrar huellas de acceso";
            ActualizarDedos();
        }

        private void RegistrarHuella()
        {
            ViewModel.ErrorGeneral = "";
            CancelarRegistro();
            RestablecerHuellaRegistro();
            ListadoNumeroDedo.Visibility = Visibility.Visible;
            CancelarRegistroHuella.Visibility = Visibility.Visible;
            getWindow().RegistrarHuella();
#pragma warning disable CS8629 // Nullable value type may be null.
            ManoView.SetModificarHuella((int)DedoSeleccionado);
#pragma warning restore CS8629 // Nullable value type may be null.
        }

        private void CancelarRegistroHuella_Click(object sender, RoutedEventArgs e)
        {
            CancelarRegistro();
        }

        private void CancelarRegistro()
        {
            getWindow().CancelarRegistrarHuella();
            this.Dispatcher.Invoke(() =>
            {

                RestablecerHuellaRegistro();
            });
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
