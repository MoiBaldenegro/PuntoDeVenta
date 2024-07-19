using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tomate.Components.ViewModels.Teclado;
using Tomate.Utils;
using Tomate.Views;

namespace Tomate.Components.Dialogos.Teclado
{
    /// <summary>
    /// Lógica de interacción para ModalTablero.xaml
    /// </summary>
    /// 
    public partial class TecladoNumeroModal : UserControl
    {

        public Dictionary<string, object?> Extras { get; set; } = new Dictionary<string, object?>();

        public TableroNumeroViewModel ViewModel { get; set; }

        public static readonly RoutedEvent OnEnterEvent = EventManager.RegisterRoutedEvent("OnEnter", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(TecladoNumeroModal));

        public event RoutedEventHandler OnEnter
        {
            add { AddHandler(OnEnterEvent, value); }
            remove { RemoveHandler(OnEnterEvent, value); }
        }

        public static readonly RoutedEvent OnCloseEvent = EventManager.RegisterRoutedEvent("OnClose", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(TecladoNumeroModal));

        public event RoutedEventHandler OnClose
        {
            add { AddHandler(OnCloseEvent, value); }
            remove { RemoveHandler(OnCloseEvent, value); }
        }

        private Action<Dictionary<string, object?>>? OnEnterAction;
        private Action<Dictionary<string, object?>>? OnCloseAction;

        private string TextoIngresado { get; set; } = "";
        private string? ValorDefecto { get; set; } = null;
        private string? KeyModal { get; set; }
        private bool TeclaIngresada { get; set; } = false;
        private bool AutoEnter { get; set; } = false;
        private bool CerrarClickFuera { get; set; } = true;
        private int LimiteDigitos { get; set; } = 2;
        private int? LimiteCantidad { get; set; }
        private bool TextoSecreto { get; set; } = false;
        public bool DesactivarBlur { get; set; } = true;
        public bool IsTextoInicial { get; set; } = true;
        private KeyEventHandler? KeyDownEvento { get; set; }
        public TecladoNumeroModal()
        {
            ViewModel = new TableroNumeroViewModel();
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
            if (datos.ContainsKey("LimiteCantidad") && (int?)datos["LimiteCantidad"] != null)
            {
#pragma warning disable CS8605 // Unboxing a possibly null value.
                LimiteCantidad = (int)datos["LimiteCantidad"];
#pragma warning restore CS8605 // Unboxing a possibly null value.
            }
            if (datos.ContainsKey("Secreto"))
            {
#pragma warning disable CS8605 // Unboxing a possibly null value.
                TextoSecreto = (bool)datos["Secreto"];
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
                KeyModal = (string?)datos["KeyModal"];
            }
            if (datos.ContainsKey("ValorDefecto"))
            {
                ValorDefecto = (string?)datos["ValorDefecto"];
            }
            if (datos.ContainsKey("AutoEnter"))
            {
#pragma warning disable CS8605 // Conversión unboxing a un valor posiblemente NULL.
                AutoEnter = (bool)datos["AutoEnter"];
#pragma warning restore CS8605 // Conversión unboxing a un valor posiblemente NULL.
            }
            if (datos.ContainsKey("CerrarClickFuera"))
            {
#pragma warning disable CS8605 // Conversión unboxing a un valor posiblemente NULL.
                CerrarClickFuera = (bool)datos["CerrarClickFuera"];
#pragma warning restore CS8605 // Conversión unboxing a un valor posiblemente NULL.
            }
            Extras = datos;

            DialogoTeclado.CloseOnClickAway = CerrarClickFuera;
        }

        public void setOnEnter(Action<Dictionary<string, object?>>? accion)
        {
            OnEnterAction = accion;
        }

        public void setOnClose(Action<Dictionary<string, object?>>? accion)
        {
            OnCloseAction = accion;
        }

        public void mostrar(string key, string titulo, string? valor, int limite, bool secreto)
        {
            mostrar(key, titulo, valor, limite);
            TextoSecreto = secreto;
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
            LimiteDigitos = 2;
            TextoSecreto = false;
            DesactivarBlur = true;
            ValorDefecto = null;
            AutoEnter = false;
            CerrarClickFuera = true;
            ResetTeclado();
            if (valor != null)
            {
                NumeroTeclado(valor);
            }
            ViewModel.TituloTeclado = titulo;
            KeyModal = key;
            DialogoTeclado.IsOpen = true;
            IsTextoInicial = true;
        }

        public void ocultar()
        {
            ResetTeclado();
            DialogoTeclado.IsOpen = false;
        }

        private void BotonCerrarModal_Click(object sender, RoutedEventArgs e)
        {
            ocultar();
        }

        private void BotonAceptar_Click(object sender, RoutedEventArgs e)
        {
            EnterTeclado();
        }

        private void EnterTeclado()
        {
            if (BotonAceptar.Opacity != 1)
            {
                return;
            }
            var args = new EventClickArgs(OnEnterEvent);
            if (Extras == null)
            {
                Extras = new Dictionary<string, object?>();
            }

            Extras["Valor"] = TextoIngresado;
            Extras["KeyModal"] = KeyModal;
            args.Extras = Extras;

            ocultar();

            RaiseEvent(args);

            if (OnEnterAction != null)
            {
                OnEnterAction.Invoke(Extras);
            }
        }

        private void ResetTeclado()
        {
            TeclaIngresada = false;

            ViewModel.MensajeTeclado = "";
            TextoIngresado = "";

            TextoDialogo.Visibility = Visibility.Collapsed;
            BotonAceptar.IsEnabled = false;
            BotonAceptar.Opacity = 0.8;


            if (ValorDefecto != null)
            {
                NumeroTeclado($"{ValorDefecto}");
            }


        }


        /**
        * Funcion cuando se hace click en los numero del tablero
        */
        private void BotonTable_Click(object sender, RoutedEventArgs e)
        {
            EventClickArgs extras = (EventClickArgs)e;
            string? valorBoton = (string?)extras.Extras["Valor"];
            NumeroTeclado(valorBoton);
        }

        /**
         * Agregar un numero al teclado
         */
        private void NumeroTeclado(string? valorBoton)
        {

            if ((!TextoSecreto && !TeclaIngresada && valorBoton == "0") ||
                (LimiteDigitos == 1 && valorBoton == "0") ||
                (!TextoSecreto && IsTextoInicial && valorBoton == "0"))
            {
                return;
            }

            if (!TeclaIngresada || LimiteDigitos == 1 || IsTextoInicial)
            {
                TextoIngresado = $"{valorBoton}";
                if (TextoSecreto)
                {
                    ViewModel.MensajeTeclado = "*";
                }
                else
                {
                    ViewModel.MensajeTeclado = $"{TextoIngresado}";
                }
                TeclaIngresada = true;
                IsTextoInicial = false;
                TextoDialogo.Visibility = Visibility.Visible;
                BotonAceptar.IsEnabled = true;
                BotonAceptar.Opacity = 1;
            }
            else
            {
                if (ViewModel.MensajeTeclado.Length < LimiteDigitos)
                {
                    TextoIngresado += valorBoton;
                    if (TextoSecreto)
                    {
                        ViewModel.MensajeTeclado += "*";
                    }
                    else
                    {
                        ViewModel.MensajeTeclado = TextoIngresado;
                    };
                }

            }
            if (!TextoSecreto)
            {
                RevisarLimiteCantidad();
            }

            if (ViewModel.MensajeTeclado.Length  == LimiteDigitos && AutoEnter)
            {
                EnterTeclado();
            }

        }

        private void RevisarLimiteCantidad()
        {
            if (ViewModel.MensajeTeclado.Length > 0)
            {
                float totalNumerico = int.Parse(ViewModel.MensajeTeclado);
                if (ViewModel.MensajeTeclado.Length > 1)
                {
                    ViewModel.MensajeTeclado = $"{totalNumerico}";
                }

                if (LimiteCantidad != null && totalNumerico > LimiteCantidad)
                {
                    ViewModel.MensajeTeclado = $"{LimiteCantidad}";
                }

            }

        }

        /**
        * Funcion cuando se hace click en boton borrar del tablero
        */
        private void BotonBorrar_Click(object sender, RoutedEventArgs e)
        {
            ResetTeclado();
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

        /**
         * Obtiene la ventana principal del programa
         */
        private MainWindow getWindow()
        {
            return WindowUtils.getMainWindow();
        }

        private void BorrarTexto()
        {
            if (TeclaIngresada)
            {
                if (TeclaIngresada && ViewModel.MensajeTeclado.Length > 1)
                {
                    ViewModel.MensajeTeclado = ViewModel.MensajeTeclado.Substring(0, ViewModel.MensajeTeclado.Length - 1);
                    TextoIngresado = TextoIngresado.Substring(0, TextoIngresado.Length - 1);
                }
                else
                {
                    ResetTeclado();
                }
            }
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
                EnterTeclado();
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
                if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                {
                    NumeroTeclado(TeclaKeyboard.Replace("numpad", ""));
                }
                else if (e.Key >= Key.D0 && e.Key <= Key.D9)
                {
                    NumeroTeclado(TeclaKeyboard.Replace("d", ""));
                }
            }
        }
    }
}
