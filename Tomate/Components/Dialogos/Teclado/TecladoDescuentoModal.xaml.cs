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
    /// Lógica de interacción para TecladoDescuentoModal.xaml
    /// </summary>
    /// 
    public partial class TecladoDescuentoModal : UserControl
    {

        public Dictionary<string, object?> Extras { get; set; } = new Dictionary<string, object?>();

        public TableroDescuentoViewModel ViewModel { get; set; }

        public static readonly RoutedEvent OnEnterEvent = EventManager.RegisterRoutedEvent("OnEnter", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(TecladoDescuentoModal));

        public event RoutedEventHandler OnEnter
        {
            add { AddHandler(OnEnterEvent, value); }
            remove { RemoveHandler(OnEnterEvent, value); }
        }

        public static readonly RoutedEvent OnCloseEvent = EventManager.RegisterRoutedEvent("OnClose", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(TecladoDescuentoModal));

        public event RoutedEventHandler OnClose
        {
            add { AddHandler(OnCloseEvent, value); }
            remove { RemoveHandler(OnCloseEvent, value); }
        }

        private Action<Dictionary<string, object?>>? OnEnterAction;
        private Action<Dictionary<string, object?>>? OnCloseAction;
        private string? KeyModal { get; set; }
        private bool TeclaIngresada { get; set; } = false;
        private int LimiteDigitos { get; set; } = 7;
        private float? LimiteCantidad { get; set; }
        public bool DesactivarBlur { get; set; } = true;
        public bool IsTextoInicial { get; set; } = true;
        private KeyEventHandler? KeyDownEvento { get; set; }
        public TecladoDescuentoModal()
        {
            ViewModel = new TableroDescuentoViewModel();
            InitializeComponent();
            DataContext = ViewModel;
        }

        public void mostrar(Dictionary<string, object?> datos)
        {
            bool soloPorcentaje = false;
            ViewModel.IsPorcentaje = (bool)datos["IsPorcentaje"];
            mostrar(null, (string?)datos["Titulo"], (string?)datos["Valor"]);
            if (datos.ContainsKey("Limite") && (int?)datos["Limite"] != null)
            {
#pragma warning disable CS8605 // Unboxing a possibly null value.
                LimiteDigitos = (int)datos["Limite"];
#pragma warning restore CS8605 // Unboxing a possibly null value.
            }
            if (datos.ContainsKey("LimiteCantidad") && (float?)datos["LimiteCantidad"] != null)
            {
#pragma warning disable CS8605 // Unboxing a possibly null value.
                LimiteCantidad = (float)datos["LimiteCantidad"];
#pragma warning restore CS8605 // Unboxing a possibly null value.
            }
            if (datos.ContainsKey("DesactivarBlur"))
            {
#pragma warning disable CS8605 // Unboxing a possibly null value.
                DesactivarBlur = (bool)datos["DesactivarBlur"];
#pragma warning restore CS8605 // Unboxing a possibly null value.
            }
            if (datos.ContainsKey("SoloPorcentaje"))
            {
#pragma warning disable CS8605 // Unboxing a possibly null value.
                soloPorcentaje = (bool)datos["SoloPorcentaje"];
#pragma warning restore CS8605 // Unboxing a possibly null value.
            }
            if (datos.ContainsKey("KeyModal"))
            {
                KeyModal = (string?)datos["KeyModal"];
            }
            Extras = datos;

            if (soloPorcentaje)
            {
                btnPorcentaje.IsEnabled = false;
                btnPorcentaje.Opacity = 0.7;
                btnCantidad.IsEnabled = false;
                btnCantidad.Opacity = 0.7;
            }
            else
            {
                btnPorcentaje.IsEnabled = true;
                btnPorcentaje.Opacity = 1;
                btnCantidad.IsEnabled = true;
                btnCantidad.Opacity = 1;
            }
        }

        public void setOnEnter(Action<Dictionary<string, object?>>? accion)
        {
            OnEnterAction = accion;
        }

        public void setOnClose(Action<Dictionary<string, object?>>? accion)
        {
            OnCloseAction = accion;
        }

        public void mostrar(string? key, string? titulo, string? valor)
        {
            //setTipoDescuentoPorcentaje(true);
            setOnEnter(null);
            setOnClose(null);
            LimiteCantidad = null;
            LimiteDigitos = 7;
            DesactivarBlur = true;
            ResetTeclado();
            if (valor != null)
            {
                NumeroTeclado(valor);
            }
            ViewModel.TituloTeclado = titulo;
            KeyModal = key;
            DialogoTecladoDescuento.IsOpen = true;
            IsTextoInicial = true;
        }

        public void ocultar()
        {
            ResetTeclado();
            DialogoTecladoDescuento.IsOpen = false;
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
            /*if (BotonAceptar.Opacity != 1)
            {
                return;
            }*/
            var args = new EventClickArgs(OnEnterEvent);
            if (Extras == null)
            {
                Extras = new Dictionary<string, object?>();
            }

            float? valor = null;
            if (ViewModel.MensajeTeclado.Length > 0)
            {
                valor = float.Parse(ViewModel.MensajeTeclado);
            }

            Extras["Valor"] = valor;
            Extras["IsProcentaje"] = ViewModel.IsPorcentaje;
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
            //TextoDialogo.Visibility = Visibility.Collapsed;
            //BotonAceptar.IsEnabled = false;
            //BotonAceptar.Opacity = 0.8;
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
        * Funcion cuando se hace click en los simbolos del tablero
        */
        private void BotonEspecialTable_Click(object sender, RoutedEventArgs e)
        {
            EventClickArgs extras = (EventClickArgs)e;
            string? valorBoton = (string?)extras.Extras["Valor"];
            if (valorBoton == ".")
            {
                AgregarPunto();
            }
            else
            {
                setTipoDescuentoPorcentaje(valorBoton == "%");
            }
        }

        private void AgregarPunto()
        {
            if (!TeclaIngresada)
            {
                NumeroTeclado("0.");
            }
            else if (!ViewModel.MensajeTeclado.Contains("."))
            {

                float totalNumerico = float.Parse(ViewModel.MensajeTeclado);

                if (ViewModel.IsPorcentaje)
                {
                    if (totalNumerico < 100)
                    {
                        NumeroTeclado(".");
                    }
                }
                else
                {
                    if (LimiteCantidad != null && totalNumerico < LimiteCantidad)
                    {
                        NumeroTeclado(".");
                    }
                }

            }
        }

        private void setTipoDescuentoPorcentaje(bool isPorcentaje)
        {
            if (ViewModel.IsPorcentaje != isPorcentaje)
            {
                ViewModel.IsPorcentaje = isPorcentaje;
                RevisarLimiteCantidad();
            }

        }


        /**
         * Agregar un numero al teclado
         */
        private void NumeroTeclado(string? valorBoton)
        {

            if (!TeclaIngresada || IsTextoInicial)
            {
                ViewModel.MensajeTeclado = $"{valorBoton}";
                TeclaIngresada = true;
                IsTextoInicial = false;
                //TextoDialogo.Visibility = Visibility.Visible;
                //BotonAceptar.IsEnabled = true;
                //BotonAceptar.Opacity = 1;
            }
            else
            {
                if (ViewModel.MensajeTeclado.Length < LimiteDigitos)
                {
                    if (ViewModel.MensajeTeclado.Contains("."))
                    {
                        if (ViewModel.MensajeTeclado.Substring(ViewModel.MensajeTeclado.LastIndexOf('.') + 1).Length > 1)
                        {
                            return;
                        }
                    }
                    ViewModel.MensajeTeclado += valorBoton;
                }

            }

            RevisarLimiteCantidad();
        }

        private void RevisarLimiteCantidad()
        {
            if (ViewModel.MensajeTeclado.Length > 0)
            {
                float totalNumerico = float.Parse(ViewModel.MensajeTeclado);
                string ultimoCaracter = ViewModel.MensajeTeclado.Substring(ViewModel.MensajeTeclado.Length - 1);
                if (ViewModel.MensajeTeclado.Length > 1 && ultimoCaracter != ".")
                {
                    ViewModel.MensajeTeclado = $"{totalNumerico}";
                }

                if (ViewModel.IsPorcentaje)
                {
                    if (totalNumerico > 100)
                    {
                        ViewModel.MensajeTeclado = "100";
                    }
                }
                else
                {
                    if (LimiteCantidad != null && totalNumerico > LimiteCantidad)
                    {
                        ViewModel.MensajeTeclado = $"{LimiteCantidad}";
                    }
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
            else if (e.Key == Key.OemPeriod || e.Key == Key.Decimal)
            {
                AgregarPunto();
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

    }
}
