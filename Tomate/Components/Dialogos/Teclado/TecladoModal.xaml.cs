using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tomate.Components.ViewModels.Teclado;
using Tomate.Utils;
using Tomate.Views;

namespace Tomate.Components.Dialogos.Teclado
{
    /// <summary>
    /// Interaction logic for TecladoModal.xaml
    /// </summary>
    public partial class TecladoModal : UserControl
    {

        public Dictionary<string, object?>? Extras { get; set; } = new Dictionary<string, object?>();

        public TableroViewModel ViewModel { get; set; }

        public static readonly RoutedEvent OnEnterEvent = EventManager.RegisterRoutedEvent("OnEnter", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(TecladoModal));

        public event RoutedEventHandler OnEnter
        {
            add { AddHandler(OnEnterEvent, value); }
            remove { RemoveHandler(OnEnterEvent, value); }
        }

        public static readonly RoutedEvent OnCloseEvent = EventManager.RegisterRoutedEvent("OnClose", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(TecladoModal));

        public event RoutedEventHandler OnClose
        {
            add { AddHandler(OnCloseEvent, value); }
            remove { RemoveHandler(OnCloseEvent, value); }
        }

        private Action<Dictionary<string, object?>>? OnEnterAction;
        private Action<Dictionary<string, object?>>? OnCloseAction;

        private string? KeyModal { get; set; }
        private bool TeclaIngresada { get; set; } = false;

        private int? LimiteDigitos { get; set; } = 50;
        private int? MinimoDigitos { get; set; }

        public bool DesactivarBlur { get; set; } = true;

        private KeyEventHandler? KeyDownEvento { get; set; }

        public TecladoModal()
        {
            ViewModel = new TableroViewModel();
            InitializeComponent();
            DataContext = ViewModel;
        }

        public void mostrar(Dictionary<string, object?> datos)
        {
            mostrar(null, (string?)datos["Titulo"], (string?)datos["Valor"]);
            if (datos.ContainsKey("Limite") && (int?)datos["Limite"] != null)
            {
#pragma warning disable CS8605 // Unboxing a possibly null value.
                LimiteDigitos = (int)datos["Limite"];
#pragma warning restore CS8605 // Unboxing a possibly null value.
            }
            if (datos.ContainsKey("MinimoDigitos") && (int?)datos["MinimoDigitos"] != null)
            {
#pragma warning disable CS8605 // Unboxing a possibly null value.
                MinimoDigitos = (int)datos["MinimoDigitos"];
#pragma warning restore CS8605 // Unboxing a possibly null value.
            }
            if (datos.ContainsKey("DesactivarBlur"))
            {
#pragma warning disable CS8605 // Unboxing a possibly null value.
                DesactivarBlur = (bool)datos["DesactivarBlur"];
#pragma warning restore CS8605 // Unboxing a possibly null value.
            }
            if (datos.ContainsKey("KeyModal"))
            {
                KeyModal = datos["KeyModal"] as string;
            }
            Extras = datos;
        }

        public void mostrar(string key, string titulo, string? valor, int limite)
        {
            mostrar(key, titulo, valor);
            LimiteDigitos = limite;
        }

        public void mostrar(string? key, string? titulo, string? valor)
        {
            setOnEnter(null);
            setOnClose(null);
            LimiteDigitos = 60;
            MinimoDigitos = null;
            DesactivarBlur = true;
            ResetTeclado();
            if (valor != null)
            {
                IngresarTeclado(valor);
            }

            ViewModel.SetLetras(valor == null || valor.Length == 0);
            TeclaIngresada = valor != null && valor.Length > 0;
            ViewModel.TituloTeclado = $"{titulo}";
            KeyModal = key;
            DialogoTeclado.IsOpen = true;
        }

        public void setOnEnter(Action<Dictionary<string, object?>>? accion)
        {
            OnEnterAction = accion;
        }

        public void setOnClose(Action<Dictionary<string, object?>>? accion)
        {
            OnCloseAction = accion;
        }

        public void ocultar()
        {
            ResetTeclado();
            DialogoTeclado.IsOpen = false;
        }

        private void EnterTeclado_Click(object sender, RoutedEventArgs e)
        {
            EnterTeclado();
        }

        private void EnterTeclado()
        {
            if (MinimoDigitos != null && $"{MinimoDigitos}".Length > $"{ViewModel.MensajeTeclado}".Length)
            {
                return;
            }
            var args = new EventClickArgs(OnEnterEvent);
            if (Extras == null)
            {
                Extras = new Dictionary<string, object?>();
            }
            Extras["KeyModal"] = KeyModal;
            Extras["Valor"] = ViewModel.MensajeTeclado;
            args.Extras = Extras;
            ocultar();
            RaiseEvent(args);
            if (OnEnterAction != null)
            {
                OnEnterAction.Invoke(Extras);
            }
        }

        /**
        * Funcion cuando se hace click en los numero del tablero
        */
        private void BotonTeclado_Click(object sender, RoutedEventArgs e)
        {
            EventClickArgs extras = (EventClickArgs)e;
            IngresarTeclado((string?)extras.Extras["Valor"]);
        }

        /**
         * Agregar un numero al teclado
         */
        private void IngresarTeclado(string? valorBoton)
        {

            if (!TeclaIngresada)
            {

                ViewModel.MensajeTeclado = $"{valorBoton}";

                TeclaIngresada = true;
                TextoDialogo.Visibility = Visibility.Visible;
                ViewModel.SetLetras(false);
            }
            else
            {

                if (LimiteDigitos == null || ViewModel.MensajeTeclado.Length < LimiteDigitos)
                {
                    ViewModel.MensajeTeclado += valorBoton;
                }
            }

            //TextoDialogo.ScrollToRightEnd();
        }

        private void ResetTeclado()
        {
            this.Dispatcher.Invoke(() =>
            {
                TeclaIngresada = false;
                ViewModel.MensajeTeclado = "";
                TextoDialogo.Visibility = Visibility.Collapsed;
                ViewModel.SetLetras(true);
            });
            CancelarBorrarTexto();
        }

        private void BotonCerrarModal_Click(object sender, RoutedEventArgs e)
        {
            ocultar();
        }

        private void EspacioTecla_Click(object sender, RoutedEventArgs e)
        {
            if (TeclaIngresada)
            {
                IngresarTeclado(" ");
            }
        }

        /**
        * Funcion cuando se hace click en boton borrar lo último agregado
        */
        private void BorrarTecla_Click(object sender, RoutedEventArgs e)
        {
            BorrarTexto();
        }

        private void BorrarTexto()
        {
            if (TeclaIngresada)
            {
                if (TeclaIngresada && ViewModel.MensajeTeclado.Length > 1)
                {
                    ViewModel.MensajeTeclado = ViewModel.MensajeTeclado.Substring(0, ViewModel.MensajeTeclado.Length - 1);
                }
                else
                {
                    ResetTeclado();
                }
            }

        }

        private void DialogoTeclado_DialogOpened(object sender, DialogOpenedEventArgs eventArgs)
        {
            AgregarKeyDownEvento();
            BackgroundDialogo.Visibility = Visibility.Visible;
            this.Dispatcher.Invoke(() =>
            {
                getWindow().AgregarBlur();
            });
        }

        private void DialogoTeclado_DialogClosed(object sender, DialogClosedEventArgs eventArgs)
        {
            RemoverKeyDownEvento();
            BackgroundDialogo.Visibility = Visibility.Collapsed;
            var args = new EventClickArgs(OnCloseEvent);
            if (Extras == null)
            {
                Extras = new Dictionary<string, object?>();
            }
            Extras["KeyModal"] = KeyModal;
            //Extras["Valor"] = ViewModel.MensajeTeclado;
            args.Extras = Extras;
            RaiseEvent(args);
            if (OnCloseAction != null)
            {
                OnCloseAction.Invoke(Extras);
            }

            if (DesactivarBlur)
            {
                this.Dispatcher.Invoke(() =>
                {
                    getWindow().RemoverBlur();
                });
            }
        }



        private void BorrarTeclado_Click(object sender, RoutedEventArgs e)
        {
            ResetTeclado();
        }

        private void ActivarMayusculas_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SetLetras(!ViewModel.Mayusculas);
        }

        /**
         * Obtiene la ventana principal del programa
         */
        private MainWindow getWindow()
        {
            return WindowUtils.getMainWindow();
        }

        private Timer? BorrarTimer;
        private void BotonBorrar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BorrarTextoTimer();
        }
        private void BotonBorrar_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CancelarBorrarTexto();
        }
        private void BotonBorrar_TouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            BorrarTextoTimer();
        }

        private void BotonBorrar_TouchUp(object sender, System.Windows.Input.TouchEventArgs e)
        {
            CancelarBorrarTexto();
        }

        private void BorrarTextoTimer()
        {
            CancelarBorrarTexto();
            BorrarTimer = new Timer();
            BorrarTimer.Elapsed += (sender, e) =>
            {
                BorrarTexto();
            };
            BorrarTimer.Interval = 150;
            BorrarTimer.AutoReset = true;
            BorrarTimer.Start();
            BorrarTexto();
        }

        private void CancelarBorrarTexto()
        {
            if (BorrarTimer != null)
            {
                BorrarTimer.Stop();
                BorrarTimer = null;
            }
        }

        private void AgregarKeyDownEvento()
        {
            if (KeyDownEvento == null)
            {
                KeyDownEvento = new KeyEventHandler(HandleKeyPress);
            }
            var window = Window.GetWindow(this);
            window.KeyDown += new KeyEventHandler(HandleKeyPress);
        }
        private void RemoverKeyDownEvento()
        {
            var window = Window.GetWindow(this);
            if (KeyDownEvento != null)
            {
                window.KeyDown -= new KeyEventHandler(HandleKeyPress);
            }
        }

        private void HandleKeyPress(object sender, KeyEventArgs e)
        {
            string TeclaKeyboard = e.Key.ToString().ToLower();
            if (e.Key == Key.Return)
            {
                EnterTeclado();
            }
            else if (e.Key == Key.Space)
            {
                IngresarTeclado(" ");
            }
            else if (e.Key == Key.Back)
            {
                BorrarTexto();
            }
            else if (e.Key == Key.Escape)
            {
                ocultar();
            }
            else
            {
                if ((Keyboard.GetKeyStates(Key.CapsLock) & KeyStates.Toggled) == KeyStates.Toggled)
                {
                    ViewModel.SetLetras(true);
                }
                else
                {
                    ViewModel.SetLetras(false);
                }

                if (e.Key >= Key.A && e.Key <= Key.Z)
                {
                    if (ViewModel.Mayusculas)
                    {
                        TeclaKeyboard = TeclaKeyboard.ToUpper();
                    }
                    IngresarTeclado(TeclaKeyboard);
                }
                else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                {
                    IngresarTeclado(TeclaKeyboard.Replace("numpad", ""));
                }
                else if (e.Key >= Key.D0 && e.Key <= Key.D9)
                {
                    IngresarTeclado(TeclaKeyboard.Replace("d", ""));
                }
            }
        }
    }
}
