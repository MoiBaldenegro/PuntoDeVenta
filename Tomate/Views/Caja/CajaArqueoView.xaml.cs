using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tomate.Components.Caja;
using Tomate.HttpRequest;
using Tomate.Models.Caja;
using Tomate.Models.Usuarios;
using Tomate.Statics;
using Tomate.Utils;
using Tomate.ViewModels.Caja;

namespace Tomate.Views.Caja
{
    /// <summary>
    /// Interaction logic for CajaArqueoView.xaml
    /// </summary>
    public partial class CajaArqueoView : UserControl
    {

        public static string TIPO_APERTURA_CAJA = "apertura_caja";
        public static string TIPO_CIERRE_CAJA = "cierre_caja";
        public static string TIPO_ARQUEO = "arqueo_caja";

        //Viewmodel de la clase con variables que se muestran en la vista
        public CajaArqueoViewModel ViewModel { get; set; }
        private KeyEventHandler? KeyDownEvento { get; set; }

        private string TipoArqueo = TIPO_ARQUEO;

        //Constructor
        public CajaArqueoView()
        {
            ViewModel = new CajaArqueoViewModel();
            InitializeComponent();
            DataContext = ViewModel;
        }

        /**
         * MEJORAR PROCESO PARA CALCULAR EL SUBTOTAL CON Y SIN NOTAS
         */
        public void IniciarVista(Usuario? usuario, CajaCorte? CajaCorteActual, string tipo)
        {
            ViewModel.CargarMonedas();
            TipoArqueo = tipo;
#pragma warning disable CS8601 // Possible null reference assignment.
            ViewModel.Usuario = usuario;
            ViewModel.CajaCorte = CajaCorteActual;

            ViewModel.ArqueoFormasPagos = new ObservableCollection<ArqueoFormaPago>();
            if (CajaCorteActual != null)
            {
                foreach (var item in CajaCorteActual.SumaFormasPagos)
                {
                    if (item.Nombre != "Efectivo")
                    {
                        ViewModel.ArqueoFormasPagos.Add(new ArqueoFormaPago($"{item.Nombre}", item.Total ?? 0));
                    }
                    
                }
            }

            ActualizarComponentes();
        }

        public void LimpiarVista()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            ViewModel.Usuario = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            RemoverKeyDownEvento();
        }

        private void ActualizarComponentes()
        {
            ViewModel.IsCierreArqueo = TipoArqueo != TIPO_APERTURA_CAJA;
            if (TipoArqueo == TIPO_APERTURA_CAJA)
            {
                ViewModel.EfectivoTotal = 0;
                ViewModel.TituloArqueo = "Apertura caja";
                GridFormasPagos.Visibility = Visibility.Collapsed;
                ViewModel.ColumnSpanMonedas = 2;
                GridMonedas.HorizontalAlignment = HorizontalAlignment.Center;
                GridMonedas.MinWidth = 600;
            }
            else if (TipoArqueo == TIPO_CIERRE_CAJA)
            {
                ViewModel.TituloArqueo = "Cierre caja";
                GridFormasPagos.Visibility = Visibility.Visible;
  
                
                ViewModel.ColumnSpanMonedas = 1;
                GridMonedas.HorizontalAlignment = HorizontalAlignment.Stretch;
                GridMonedas.MinWidth = 0;
            }
            else
            {
                ViewModel.TituloArqueo = "Arqueo caja";
                ViewModel.ColumnSpanMonedas = 1;
                GridFormasPagos.Visibility = Visibility.Visible;
                ViewModel.ColumnSpanMonedas = 1;
                GridMonedas.HorizontalAlignment = HorizontalAlignment.Stretch;
                GridMonedas.MinWidth = 0;
            }
        }

        private void BotonAtras_Click(object sender, RoutedEventArgs e)
        {
            var existeModoficacionMonedas = ViewModel.ArqueoMonedas.Where(item => item.Cantidad != null).Count() > 0;
            var existeModoficacionFormasPagos = ViewModel.ArqueoFormasPagos.Where(item => item.Cantidad != null).Count() > 0;

            if (existeModoficacionMonedas || existeModoficacionFormasPagos)
            {
                this.Dispatcher.Invoke(() =>
                {
                    DialogoAlertaCm.show("¿Descartar cambios?", true, delegate ()
                    {
                        Atras();
                    });
                });
            }
            else
            {
                Atras();
            }
        }

        private void Atras()
        {
            this.Dispatcher.Invoke(() =>
            {
                getWindow().MostrarCaja();
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

        private void SoloNumeros(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SoloNumerosPunto(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Monedas_TextChanged(object sender, TextChangedEventArgs e)
        {
            var item = ViewModel.ArqueoMonedas[0];
            Debug.WriteLine(item);
        }


        private void CajaArqueo_Loaded(object sender, RoutedEventArgs e)
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
            if (e.Key == Key.Return || e.Key == Key.Tab)
            {
                EnterTablero();
            }
            else if (e.Key == Key.Back)
            {
                BorrarCantidad();
            }
            else if (e.Key == Key.OemPeriod || e.Key == Key.Decimal)
            {
                if (ViewModel.TipoInput == "formas_pago")
                {
                    AgregarPunto();
                }

            }
            else
            {
                if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                {
                    NumeroInputTablero(TeclaKeyboard.Replace("numpad", ""));
                }
                else if (e.Key >= Key.D0 && e.Key <= Key.D9)
                {
                    NumeroInputTablero(TeclaKeyboard.Replace("d", ""));
                }
            }
        }

        private void SeleccionarMoneda_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        { 
            if(MonedasListado.SelectedIndex > -1)
            {
                setFocus("moneda", MonedasListado.SelectedIndex);
            }
        }
        private void SeleccionarFormaPago_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (FormasPagosListado.SelectedIndex > -1)
            {
                setFocus("formas_pago", FormasPagosListado.SelectedIndex);
            }
        }
        

        private void setFocus(string tipo, int index)
        {
            if (ViewModel.TipoInput == "moneda")
            {
                ViewModel.ArqueoMonedas[ViewModel.IndexFocus].IsFocus = false;
                ViewModel.ActualizarMonedas(ViewModel.IndexFocus);
            }
            else
            {
                ViewModel.ArqueoFormasPagos[ViewModel.IndexFocus].IsFocus = false;
                ViewModel.ActualizarFormasPagos(ViewModel.IndexFocus);
            }
            if (tipo == "moneda")
            {
                ViewModel.ArqueoMonedas[index].IsFocus = true;
                ViewModel.ActualizarMonedas(index);
            }
            else
            {
                ViewModel.ArqueoFormasPagos[index].IsFocus = true;
                ViewModel.ActualizarFormasPagos(index);
            }
            ViewModel.TipoInput = tipo;
            ViewModel.IndexFocus = index;
        }

        /**
         * Funcion cuando se hace click en los numero del tablero
         */
        private void BotonTable_Click(object sender, RoutedEventArgs e)
        {
            EventClickArgs extras = (EventClickArgs)e;
            NumeroInputTablero($"{(string?)extras.Extras["Valor"]}");
        }

        /**
         * Funcion cuando se hace click en boton entrar del tablero
         */
        private void BotonEnter_Click(object sender, RoutedEventArgs e)
        {
            EnterTablero();
        }

        /**
         * Funcion cuando se hace click en boton borrar del tablero
         */
        private void BotonBorrar_Click(object sender, RoutedEventArgs e)
        {
            BorrarCantidad(true);
        }

        private void EnterTablero()
        {
            if (ViewModel.TipoInput == "moneda")
            {
                if (ViewModel.IndexFocus < ViewModel.ArqueoMonedas.Count - 1)
                {
                    setFocus(ViewModel.TipoInput, ViewModel.IndexFocus + 1);
                }
                else
                {
                    if (ViewModel.ArqueoFormasPagos.Count > 0)
                    {
                        setFocus("formas_pago", 0);
                    }
                    else
                    {
                        AplicarArqueoCaja();
                    }
                }
            }
            else
            {
                if (ViewModel.IndexFocus < ViewModel.ArqueoFormasPagos.Count - 1)
                {
                    setFocus(ViewModel.TipoInput, ViewModel.IndexFocus + 1);
                }
                else
                {
                    AplicarArqueoCaja();
                }
                
            }
        }

        private void BorrarCantidad(bool borrarCompleto = false)
        {


            if (ViewModel.TipoInput == "moneda")
            {
                var moneda = ViewModel.ArqueoMonedas[ViewModel.IndexFocus];
                if (borrarCompleto)
                {
                    moneda.Cantidad = null;
                }
                else
                {
                    if ($"{moneda.Cantidad}".Length > 1)
                    {
                        moneda.Cantidad = $"{moneda.Cantidad}".Substring(0, $"{moneda.Cantidad}".Length - 1);
                    }
                    else
                    {
                        BorrarCantidad(true);
                    }
                }
                ViewModel.ArqueoMonedas[ViewModel.IndexFocus] = moneda;
                ViewModel.ActualizarMonedas(ViewModel.IndexFocus);
            }
            else
            {
                var formaPago = ViewModel.ArqueoFormasPagos[ViewModel.IndexFocus];
                if (borrarCompleto)
                {
                    formaPago.Cantidad = null;
                }
                else
                {
                    if ($"{formaPago.Cantidad}".Length > 1)
                    {
                        formaPago.Cantidad = $"{formaPago.Cantidad}".Substring(0, $"{formaPago.Cantidad}".Length - 1);
                    }
                    else
                    {
                        BorrarCantidad(true);
                    }
                }
                ViewModel.ArqueoFormasPagos[ViewModel.IndexFocus] = formaPago;
                ViewModel.ActualizarFormasPagos(ViewModel.IndexFocus);

            }
        }

        /**
         * Agregar un numero al teclado
         */
        private void NumeroInputTablero(string valorBoton)
        {
            if (ViewModel.TipoInput == "moneda")
            {
                var moneda = ViewModel.ArqueoMonedas[ViewModel.IndexFocus];
                var cantidadFinal = $"{moneda.Cantidad}{valorBoton}";
                if (cantidadFinal.Length > 5)
                {
                    return;
                }
                try
                {
                    moneda.Cantidad = $"{int.Parse(cantidadFinal)}";
                    ViewModel.ArqueoMonedas[ViewModel.IndexFocus] = moneda;
                    ViewModel.ActualizarMonedas(ViewModel.IndexFocus);
                }
                catch
                {

                }
            }
            else
            {
                var formaPago = ViewModel.ArqueoFormasPagos[ViewModel.IndexFocus];
                var cantidadFinal = $"{formaPago.Cantidad}{valorBoton}";
                if (cantidadFinal.Length > 9)
                {
                    return;
                }
                try
                {
                    formaPago.Cantidad = $"{float.Parse(cantidadFinal)}";
                    ViewModel.ArqueoFormasPagos[ViewModel.IndexFocus] = formaPago;
                    ViewModel.ActualizarFormasPagos(ViewModel.IndexFocus);
                }
                catch
                {

                }
            }
            
        }
        private void AgregarPunto()
        {
            var formaPago = ViewModel.ArqueoFormasPagos[ViewModel.IndexFocus];

            if (formaPago.Cantidad == null || formaPago.Cantidad.Length == 0)
            {
                ViewModel.ArqueoFormasPagos[ViewModel.IndexFocus].Cantidad = "0.";
                ViewModel.ActualizarFormasPagos(ViewModel.IndexFocus);
            }
            else if (!formaPago.Cantidad.Contains("."))
            {
                ViewModel.ArqueoFormasPagos[ViewModel.IndexFocus].Cantidad = $"{formaPago.Cantidad}.";
                ViewModel.ActualizarFormasPagos(ViewModel.IndexFocus);
            }
        }

        private void AplicarArqueo_OnClick(object sender, RoutedEventArgs e)
        {
            AplicarArqueoCaja();
        }

        private void AplicarArqueoCaja()
        {
            this.Dispatcher.Invoke(() =>
            {
                string titulo = "¿Aplicar arqueo?";
                if (TIPO_APERTURA_CAJA == TipoArqueo)
                {
                    titulo = "¿Abrir caja?";
                }
                else if (TIPO_CIERRE_CAJA == TipoArqueo)
                {
                    titulo = "¿Cerrar caja?";
                }
                DialogoAlertaCm.show(titulo, true, delegate ()
                {
                    if (TIPO_APERTURA_CAJA == TipoArqueo)
                    {
                        abrirCaja();
                    }
                    else if (TIPO_CIERRE_CAJA == TipoArqueo)
                    {
                        cerrarCaja();
                    }
                    else
                    {
                        aplicarArqueo();
                    }
                });
            });
        }

        private void abrirCaja()
        {
            float efectivoInicial = ViewModel.ArqueoMonedas.Sum(Item => Item.Importe);

            ApiClient.CorteCajaAbrir($"{ViewModel.Usuario.Id}", "1", 
                efectivoInicial, delegate (JObject respuesta) {
                var error = (string?)respuesta["error"];
                if (error != null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        DialogoAlertaCm.show("Apertura caja", error);
                    });
                    return;
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        Atras();
                    });
                }
            });
        }

        private void cerrarCaja()
        {

            float efectivoFinal = ViewModel.ArqueoMonedas.Sum(Item => Item.Importe);

            ApiClient.CorteCajaCerrar($"{ViewModel.Usuario.Id}", "1",
                efectivoFinal, delegate (JObject respuesta) {
                    var error = (string?)respuesta["error"];
                    if (error != null)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            DialogoAlertaCm.show("Cierre caja", error);
                        });
                        return;
                    }
                    else
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            Atras();
                        });
                    }
                });
        }

        private void aplicarArqueo()
        {
            float efectivoInicial = ViewModel.ArqueoMonedas.Sum(Item => Item.Importe);

            ApiClient.CajaArqueoRegistrar($"{ViewModel.Usuario.Id}", "1",
                efectivoInicial, delegate (JObject respuesta) {
                    var error = (string?)respuesta["error"];
                    if (error != null)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            DialogoAlertaCm.show("Arqueo de caja", error);
                        });
                        return;
                    }
                    else
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            Atras();
                        });
                    }
                });
            
        }
    }
}
