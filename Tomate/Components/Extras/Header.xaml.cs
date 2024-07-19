using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using Tomate.Models.Usuarios;
using Tomate.Utils;
using Tomate.Views;

namespace Tomate.Components.Extras
{
    /// <summary>
    /// Interaction logic for Header.xaml
    /// </summary>
    public partial class Header : UserControl
    {

        public static readonly DependencyProperty TituloProperty = DependencyProperty.Register(nameof(Usuario), typeof(Usuario), typeof(Header),
        new PropertyMetadata(new Usuario()));


        public Usuario Usuario
        {
            get
            {
                return (Usuario)GetValue(TituloProperty);
            }
            set
            {
                SetValue(TituloProperty, value);
            }

        }

        public static readonly DependencyProperty HoraProperty = DependencyProperty.Register(nameof(Hora), typeof(string), typeof(Header),
        new PropertyMetadata("--:--"));


        public string Hora
        {
            get
            {
                return (string)GetValue(HoraProperty);
            }
            set
            {
                SetValue(HoraProperty, value);
            }

        }

        public static readonly DependencyProperty FechaProperty = DependencyProperty.Register(nameof(Fecha), typeof(string), typeof(Header),
        new PropertyMetadata("-- --- ----"));


        public string Fecha
        {
            get
            {
                return (string)GetValue(FechaProperty);
            }
            set
            {
                SetValue(FechaProperty, value);
            }
        }

        private Timer? FechaHoraTimer;

        public Header()
        {
            InitializeComponent();
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

        private void Header_Loaded(object sender, RoutedEventArgs e)
        {
            IniciarFechaHoraTimer();
        }

        private void Header_Unloaded(object sender, RoutedEventArgs e)
        {
            DetenerFechaHoraTimer();
        }

        private void IniciarFechaHoraTimer()
        {
            FechaHoraTimer = new Timer();
            FechaHoraTimer.Elapsed += (sender, e) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    Fecha = DateTime.Now.ToString("dd MMM yyyy");
                    Hora = DateTime.Now.ToString("HH:mm");
                });
            };
            FechaHoraTimer.Interval = 1000;
            FechaHoraTimer.AutoReset = true;
            FechaHoraTimer.Start();
            this.Dispatcher.Invoke(() =>
            {
                Fecha = DateTime.Now.ToString("dd MMM yyyy");
                Hora = DateTime.Now.ToString("HH:mm");
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
    }
}
