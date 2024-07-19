using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using Squirrel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Tomate.HttpRequest;
using Tomate.Models;
using Tomate.Models.Usuarios;
using Tomate.Models.Ventas;
using Tomate.Models.Catalogo;
using Tomate.Utils;
using Tomate.ViewModels;
using Tomate.Views.Cobro;
using Tomate.Views.Empleados;
using Tomate.Views.Mesas;
using Tomate.Views.Ventas;
using Tomate.DBHelper;
using System.Diagnostics;
using Tomate.Views.Caja;
using Tomate.Views.Tableros;
using Tomate.Models.Cobrar;
using Tomate.Statics;
using Tomate.Models.Caja;

namespace Tomate.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// MATERIAL THEME
    /// ((MainWindow) Application.Current.MainWindow)
    /// http://materialdesigninxaml.net/
    /// https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit/releases
    public partial class MainWindow
    {

        public MainViewModel ViewModel { get; set; }
        private InicioView _InicioView { get; set; }
        private ListadoVentasView _ListadoVentasView { get; set; }
        private EmpleadosView _EmpleadosView { get; set; }
        private VentaDetalleView _VentaDetalleView { get; set; }
        private CobroVentaView _CobroVentaView { get; set; }
        private AsignarMesasView _AsignarMesasView { get; set; }
        private DesactivarProductosView _DesactivarProductosView { get; set; }
        private CajaView _CajaView { get; set; }
        private CajaArqueoView _CajaArqueoView { get; set; }

        //corre cada 15 segundo el sincronizar información con el servidor
        private Timer? SincronizarTimer { get; set; }

        //usuario que inició sesión
        public Usuario? UsuarioSeleccionado { get; set; }
        //venta seleccionada para cobrar
        public Venta? VentaSeleccionada { get; set; }
        public VentaNota? VentaNotaSeleccionada { get; set; }

        //controlador lector de huella
        public HuellaController HuellaController { get; set; }

        private string TipoArqueo = "";
        private CajaCorte? CajaCorteActual;

        private string _VistaActual = "Inicio";

        private string VistaActual
        {
            get
            {
                return _VistaActual;
            }
            set
            {
                MostarSeccion(value, _VistaActual);
                _VistaActual = value;
            }
        }

        private bool NoSincronizar = false;

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainViewModel();
            DataContext = ViewModel;
            SetPrimaryColor((Color)ColorConverter.ConvertFromString("#fc5656"));
            try
            {
                IniciarVistas();
                InitLectorHuella();

                ActualizarUsuariosLector();
                
                PermisosStatic.CargarPermisos();
                MesasStatic.CargarMesas();

                MostrarInicio();
                
            }
            catch {
                NoSincronizar = true;
                DesactivarSincronizacion();
                System.Threading.Thread.Sleep(2000);
                DbHelper.RestablecerTablas();
                Configuracion.removerValor(Configuracion.Key.ULTIMA_SINCRONIZACION);
                Application.Current.Shutdown();
                System.Windows.Forms.Application.Restart();
            }
            
        }

        public void CargarConfiguracion()
        {

            CargarConfiguracion(false);
        }

        public void CargarConfiguracion(bool activarSincronizacion)
        {
            if (Configuracion.isConfigurado())
            {
                ApiClient.SetConfiguracion(Configuracion.getValor(Configuracion.Key.DIRECCION_SERVIDOR),
                    Configuracion.getValor(Configuracion.Key.TOKEN_ACCESO));
                if (activarSincronizacion)
                {
                    ActivarSincronizacion();
                }
                //actualizar terminal
                TerminalStatic.NUMERO = Configuracion.getValor(Configuracion.Key.TERMINAL_NUMERO);
                TerminalStatic.NOMBRE = Configuracion.getValor(Configuracion.Key.TERMINAL_NOMBRE);
                TerminalStatic.SUCURSAL_ID = Configuracion.getValor(Configuracion.Key.TERMINAL_SUCURSAL_ID);
            }


        }

        public void AgregarBlur()
        {
            var blur = new BlurEffect();
            blur.Radius = 20;
            GeneralControl.Effect = blur;
        }

        public bool ExisteBlur()
        {
            return GeneralControl.Effect != null;
        }

        public void RemoverBlur()
        {
            GeneralControl.Effect = null;
        }

        private void IniciarVistas()
        {
            _InicioView = new InicioView();
            _EmpleadosView = new EmpleadosView();
            _ListadoVentasView = new ListadoVentasView();
            _VentaDetalleView = new VentaDetalleView();
            _CobroVentaView = new CobroVentaView();
            _AsignarMesasView = new AsignarMesasView();
            _DesactivarProductosView = new DesactivarProductosView();
            _CajaView = new CajaView();
            _CajaArqueoView = new CajaArqueoView();
        }

        public void MostarSeccion(string actual, string previa)
        {
            switch (previa)
            {
                case "Inicio":
                    break;
                case "ListadoVentas":
                    if (actual != "VentaDetalle")
                    {
                        _ListadoVentasView.CancelarVentasSincronizar();
                    }
                    if (actual == "Inicio")
                    {
                        _ListadoVentasView.LimpiarVista();
                    }
                    break;
                case "VentaDetalle":
                    _VentaDetalleView.LimpiarVista();
                    if (actual != "CobroVenta")
                    {
                        VentaSeleccionada = null;
                    }
                    break;
                case "CobroVenta":
                    _CobroVentaView.LimpiarVista();
                    break;
                case "Empleados":
                    _EmpleadosView.LimpiarVista();
                    break;
                case "AsignarMesas":
                    _AsignarMesasView.LimpiarVista();
                    break;
                case "DesactivarProductos":
                    _DesactivarProductosView.LimpiarVista();
                    break;
                case "Caja":
                    if (actual != "CajaArqueo")
                    {
                        _CajaView.LimpiarVista();
                    }
                    break;
                case "CajaArqueo":
                    _CajaArqueoView.LimpiarVista();
                    break;

            }

            switch (actual)
            {
                case "Inicio":
                    UsuarioSeleccionado = null;
                    break;
                case "ListadoVentas":
                    _ListadoVentasView.IniciarVista(UsuarioSeleccionado);
                    break;
                case "VentaDetalle":
                    _VentaDetalleView.IniciarVista(UsuarioSeleccionado, VentaSeleccionada);
                    break;
                case "CobroVenta":
                    _CobroVentaView.IniciarVista(UsuarioSeleccionado, (Venta)VentaSeleccionada, (VentaNota)VentaNotaSeleccionada, CajaCorteActual);
                    break;
                case "Empleados":
                    _EmpleadosView.IniciarVista(UsuarioSeleccionado);
                    break;
                case "AsignarMesas":
                    _AsignarMesasView.IniciarVista(UsuarioSeleccionado);
                    break;
                case "DesactivarProductos":
                    _DesactivarProductosView.IniciarVista(UsuarioSeleccionado);
                    break;
                case "Caja":
                    _CajaView.IniciarVista(UsuarioSeleccionado);
                    break;
                case "CajaArqueo":
                    _CajaArqueoView.IniciarVista(UsuarioSeleccionado, CajaCorteActual, TipoArqueo);
                    break;
            }
        }

        public void MostrarInicio()
        {
            VistaActual = "Inicio";
            //ViewModel.VistaActualControl = new EmpleadoDetalleModal(); ;
            ViewModel.VistaActualControl = _InicioView;
        }

        public void MostrarEmpleados()
        {
            VistaActual = "Empleados";
            ViewModel.VistaActualControl = _EmpleadosView;
        }

        public void MostrarListadoVentas()
        {
            MostrarListadoVentas(UsuarioSeleccionado);
        }

        public void MostrarListadoVentas(Usuario? usuario)
        {
            UsuarioSeleccionado = usuario;
            VistaActual = "ListadoVentas";
            ViewModel.VistaActualControl = _ListadoVentasView;
            //ContentControl.Content = _ListadoCuentasView;
        }

        public void ActualizarVentas()
        {
            _ListadoVentasView.ActualizarVentas();
        }

        public void MostrarVentaDetalle(Venta venta)
        {
            VentaSeleccionada = venta;
            VistaActual = "VentaDetalle";
            ViewModel.VistaActualControl = _VentaDetalleView;
            //ContentControl.Content = _CuentaDetalleView;
        }

        public void MostrarCobroVenta(Venta venta, VentaNota ventaNota, CajaCorte? cajaCorte)
        {
            CajaCorteActual = cajaCorte;
            VentaSeleccionada = venta;
            VentaNotaSeleccionada = ventaNota;
            VistaActual = "CobroVenta";
            ViewModel.VistaActualControl = _CobroVentaView;
        }

        public void MostrarAsignarMesas()
        {
            VistaActual = "AsignarMesas";
            ViewModel.VistaActualControl = _AsignarMesasView;
            //ContentControl.Content = _CuentaDetalleView;
        }

        public void MostrarDesactivarProductos()
        {
            VistaActual = "DesactivarProductos";
            ViewModel.VistaActualControl = _DesactivarProductosView;
        }

        public void MostrarCaja()
        {
            VistaActual = "Caja";
            ViewModel.VistaActualControl = _CajaView;
        }

        public void MostrarCajaArqueo(CajaCorte? cajaCorte, string tipo)
        {
            CajaCorteActual = cajaCorte;
            TipoArqueo = tipo;
            VistaActual = "CajaArqueo";
            ViewModel.VistaActualControl = _CajaArqueoView;
        }

        public void MostrarConfiguraciones(Action callback)
        {
            DesactivarSincronizacion();
            bool borrarBaseDatos = UsuarioSeleccionado != null && UsuarioSeleccionado.RevisarPermiso("pv.borrar-base-datos");
            ConfiguracionModal.mostrar(borrarBaseDatos, callback);
        }


        private static void SetPrimaryColor(Color color)
        {
            //This is the API in 4.2.1
            PaletteHelper paletteHelper = new PaletteHelper();
            //For versions after this point use MaterialDesignThemes.Wpf.Theming.ThemeManager

            //Get current theme
            var theme = paletteHelper.GetTheme();

            //Apply primary color
            theme.SetPrimaryColor(color);

            //Apply theme to application
            paletteHelper.SetTheme(theme);
        }

        public void MinimizarVentana()
        {
            this.WindowState = WindowState.Minimized;
        }


        public void ActivarSincronizacion()
        {
            DesactivarSincronizacion();
            SincronizarTimer = new Timer();
            SincronizarTimer.Elapsed += (sender, e) =>
            {
                #if RELEASE
                if (VistaActual == "Inicio")
                {
                    ActualizarPrograma();
                }
                #endif
                SincronizarInformacion();
            };
            SincronizarTimer.Interval = 4000;
            SincronizarTimer.AutoReset = true;
            SincronizarTimer.Start();

            SincronizarInformacion();
        }

        public void DesactivarSincronizacion()
        {
            if (SincronizarTimer != null)
            {
                SincronizarTimer.Stop();
                SincronizarTimer = null;
            }
        }

        /**
         * Envia los datos al servidor 
         */
        private void SincronizarInformacion()
        {
            SincronizarInformacion(false, null);
        }

        public void SincronizarInformacion(bool restablecerBaseDatos, Action<bool, string>? callback)
        {
            if (NoSincronizar)
            {
                return;
            }
            string? ultimaSincronizacion = Configuracion.getValor(Configuracion.Key.ULTIMA_SINCRONIZACION);

            if (restablecerBaseDatos)
            {
                ultimaSincronizacion = null;
            }

            ApiClient.Sincronizar(ultimaSincronizacion, delegate (JObject respuesta)
            {
                if (NoSincronizar)
                {
                    Configuracion.removerValor(Configuracion.Key.ULTIMA_SINCRONIZACION);
                    return;
                }

               
                var actualizarUsuariosHuellas = false;
                var actualizarPermisos = false;
                var actualizarMesas = false;

                var error = (string?)respuesta["error"];
                if (error != null)
                {

                    if (callback != null)
                    {
                        callback.Invoke(false, error);
                    }
                    else
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            MostrarDialogoAlerta(null, error, false, null, null, false);
                            ViewModel.MensajeErrorSincronizacion = error;
                            //BarError.IsActive = true;
                        });
                    }
                    return;
                }
                else
                {
                    if (restablecerBaseDatos)
                    {
                        DbHelper.RestablecerTablas();
                        DbHelper.CorrerMigraciones();
                    }

                    this.Dispatcher.Invoke(() =>
                    {
                        if (!string.IsNullOrEmpty(ViewModel.MensajeErrorSincronizacion))
                        {
                            ViewModel.MensajeErrorSincronizacion = "";
                            DialogoAlerta.IsOpen = false;
                            //BarError.IsActive = false;
                        }
                    });
                    
                    var perfilesJson = (JArray?)respuesta["perfiles"];
                    if (perfilesJson != null && perfilesJson.Count > 0)
                    {
                        foreach (JObject perfilJson in perfilesJson)
                        {
                            var perfil = new Perfil(perfilJson);
                            perfil.save();
                        }
                    }

                    var perfilesPermisosJson = (JArray?)respuesta["perfiles_permisos"];
                    if (perfilesPermisosJson != null && perfilesPermisosJson.Count > 0)
                    {
                        foreach (JObject perfilPermisoJson in perfilesPermisosJson)
                        {
                            var permiso = new PerfilPermiso(perfilPermisoJson);
                            permiso.save();
                        }
                        actualizarPermisos = true;
                    }

                    var usuariosJson = (JArray?)respuesta["usuarios"];
                    if (usuariosJson != null && usuariosJson.Count > 0)
                    {
                        foreach (JObject usuarioJson in usuariosJson)
                        {
                            var usuario = new Usuario(usuarioJson);
                            usuario.save();
                        }
                        actualizarMesas = true;
                    }

                    var usuariosHuellasJson = (JArray?)respuesta["usuarios_huellas"];
                    if (usuariosHuellasJson != null && usuariosHuellasJson.Count > 0)
                    {
                        foreach (JObject usuarioHuellaJson in usuariosHuellasJson)
                        {
                            var usuarioHuella = new UsuarioHuella(usuarioHuellaJson);
                            usuarioHuella.save();
                        }
                        actualizarUsuariosHuellas = true;
                    }

                    var usuariosSucursalesJson = (JArray?)respuesta["usuarios_sucursales"];
                    if (usuariosSucursalesJson != null && usuariosSucursalesJson.Count > 0)
                    {
                        foreach (JObject usuarioSucursalJson in usuariosSucursalesJson)
                        {
                            var usuarioSucursal = new UsuarioSucursal(usuarioSucursalJson);
                            usuarioSucursal.save();
                        }
                        actualizarUsuariosHuellas = true;
                    }

                    var categoriasJson = (JArray?)respuesta["categorias"];
                    if (categoriasJson != null && categoriasJson.Count > 0)
                    {
                        foreach (JObject categoriaJson in categoriasJson)
                        {
                            var categoria = new Categoria(categoriaJson);
                            categoria.save();
                        }
                    }

                    var productosJson = (JArray?)respuesta["productos"];
                    if (productosJson != null && productosJson.Count > 0)
                    {
                        foreach (JObject productoJson in productosJson)
                        {
                            var producto = new Producto(productoJson);
                            producto.save();
                        }
                    }

                    var listasPreciosJson = (JArray?)respuesta["listas_precios"];
                    if (listasPreciosJson != null && listasPreciosJson.Count > 0)
                    {
                        foreach (JObject listaPreciosJson in listasPreciosJson)
                        {
                            var listaPrecios = new ListaPrecios(listaPreciosJson);
                            listaPrecios.save();
                        }
                    }

                    var listasPreciosProductosJson = (JArray?)respuesta["listas_precios_productos"];
                    if (listasPreciosProductosJson != null && listasPreciosProductosJson.Count > 0)
                    {
                        foreach (JObject listaPreciosProductoJson in listasPreciosProductosJson)
                        {
                            var listaPrecioProducto = new ListaPreciosProducto(listaPreciosProductoJson);
                            listaPrecioProducto.save();
                        }
                    }

                    var categoriasExtrasJson = (JArray?)respuesta["categorias_extras"];
                    if (categoriasExtrasJson != null && categoriasExtrasJson.Count > 0)
                    {
                        foreach (JObject categoriaExtraJson in categoriasExtrasJson)
                        {
                            var categoriaExtra = new CategoriaExtra(categoriaExtraJson);
                            categoriaExtra.save();
                        }
                    }

                    var modificadoresJson = (JArray?)respuesta["modificadores"];
                    if (modificadoresJson != null && modificadoresJson.Count > 0)
                    {
                        foreach (JObject modificadorJson in modificadoresJson)
                        {
                            var modificador = new Modificador(modificadorJson);
                            modificador.save();
                        }
                    }

                    var formasPagosJson = (JArray?)respuesta["formas_pago"];
                    if (formasPagosJson != null && formasPagosJson.Count > 0)
                    {
                        foreach (JObject formaPagoJson in formasPagosJson)
                        {
                            var formaPago = new FormaPago(formaPagoJson);
                            formaPago.save();
                        }
                    }

                    var pagosPredefinidosJson = (JArray?)respuesta["pagos_predefinidos"];
                    if (pagosPredefinidosJson != null && pagosPredefinidosJson.Count > 0)
                    {
                        foreach (JObject pagoPredefinidoJson in pagosPredefinidosJson)
                        {
                            var pagoPredefinido = new PagoPredefinido(pagoPredefinidoJson);
                            pagoPredefinido.save();
                        }
                    }


                    var ventasTiposJson = (JArray?)respuesta["ventas_tipos"];
                    if (ventasTiposJson != null && ventasTiposJson.Count > 0)
                    {
                        foreach (JObject ventaTipoJson in ventasTiposJson)
                        {
                            var ventaTipo = new VentaTipo(ventaTipoJson);
                            ventaTipo.save();
                        }
                    }

                    var motivosCancelacionJson = (JArray?)respuesta["motivos_cancelacion"];
                    if (motivosCancelacionJson != null && motivosCancelacionJson.Count > 0)
                    {
                        foreach (JObject motivoCancelacionJson in motivosCancelacionJson)
                        {
                            var motivoCancelacion = new MotivoCancelacion(motivoCancelacionJson);
                            motivoCancelacion.save();
                        }
                    }

                    var mesasJson = (JArray?)respuesta["mesas"];
                    if (mesasJson != null && mesasJson.Count > 0)
                    {
                        foreach (JObject mesaJSon in mesasJson)
                        {
                            var mesa = new Mesa(mesaJSon);
                            mesa.save();
                        }
                        actualizarMesas = true;
                    }
                    var tablerosJson = (JArray?)respuesta["tableros"];
                    if (tablerosJson != null && tablerosJson.Count > 0)
                    {
                        foreach (JObject tableroJson in tablerosJson)
                        {
                            var tablero = new Tablero(tableroJson);
                            tablero.save();
                        }
                    }
                    var tableroProductosJson = (JArray?)respuesta["tableros_productos"];
                    if (tableroProductosJson != null && tableroProductosJson.Count > 0)
                    {
                        foreach (JObject tableroProductoJson in tableroProductosJson)
                        {
                            var tableroProducto = new TableroProducto(tableroProductoJson);
                            tableroProducto.save();
                        }
                    }

                    var terminalJson = (JObject?)respuesta["terminal"];
                    if (terminalJson != null)
                    {
                        if ((string?)terminalJson["num_terminal"] != TerminalStatic.NUMERO)
                        {
                            Configuracion.setValor(Configuracion.Key.TERMINAL_NUMERO, $"{(string?)terminalJson["num_terminal"]}");
                            TerminalStatic.NUMERO = (string?)terminalJson["num_terminal"];
                        }
                        if ((string?)terminalJson["nombre"] != TerminalStatic.NOMBRE)
                        {
                            Configuracion.setValor(Configuracion.Key.TERMINAL_NOMBRE, $"{(string?)terminalJson["nombre"]}");
                            TerminalStatic.NOMBRE = (string?)terminalJson["nombre"];
                        }
                        if ((string?)terminalJson["sucursal_id"] != TerminalStatic.SUCURSAL_ID)
                        {
                            Configuracion.setValor(Configuracion.Key.TERMINAL_SUCURSAL_ID, $"{(string?)terminalJson["sucursal_id"]}");
                            TerminalStatic.SUCURSAL_ID = (string?)terminalJson["sucursal_id"];
                            actualizarUsuariosHuellas = true;
                            actualizarMesas = true;
                        }
                    }

                    Configuracion.setValor(Configuracion.Key.ULTIMA_SINCRONIZACION, $"{(string?)respuesta["sync_time"]}");

                    if (actualizarUsuariosHuellas)
                    {
                        ActualizarUsuariosLector();
                    }

                    if (actualizarPermisos)
                    {
                        PermisosStatic.CargarPermisos();
                    }

                    if (actualizarMesas)
                    {
                        MesasStatic.CargarMesas();
                    }

                    if (callback != null)
                    {
                        callback.Invoke(true, "");
                    }

                }
            });

        }

        //LECTOR DE HUELLA EVENTOS  

        private List<UsuarioHuella> HuellasUsuarios = new List<UsuarioHuella>();
        public void ActualizarUsuariosLector()
        {
            var huellas = UsuarioHuella.todas();
            foreach (var huella in huellas)
            {
                var index = HuellasUsuarios.FindIndex(item => huella.Id == item.Id);
                if (index > -1)
                {
                    HuellasUsuarios[index] = huella;
                }
                else
                {
                    HuellasUsuarios.Add(huella);
                }
            }

            HuellaController.validarMultipleHuella(HuellasUsuarios);
        }

        /**
         * Inicia el lector de huellas DigitalPersona
         */
        private void InitLectorHuella()
        {
            if (HuellaController == null)
            {
                HuellaController = new HuellaController();
                HuellaController.OnHuellaValidarEvent += OnHuellaValidarEvent;
                HuellaController.OnHuellaValidarProgresoEvent += OnHuellaValidarProgresoEvent;
                HuellaController.OnDeviceChangeEvent += OnDeviceChangeEvent;
                HuellaController.OnHuellaRegistroEvent += HuellaRegistrada;
                HuellaController.OnHuellaRegistroCounterEvent += RegistroHuellaContador;
                HuellaController.InitSDK();
            }

        }

        /**
         * Revisa que el lector de huella esté disponible
         */
        public bool LectorHuellaDisponible()
        {
            return HuellaController.Available;
        }

        public void CancelarRegistrarHuella()
        {
            HuellaController.DetenerRegistrarHuella();
            HuellaController.validarMultipleHuella(HuellasUsuarios);
        }

        public void RegistrarHuella()
        {
            if (HuellaController.Available)
            {
                HuellaController.registrarHuella();
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    DialogoAlertaCm.show("Huella", "Lector de huella no disponible");
                });
            }
        }

        /**
         * Huella registrada correctamente
         */
        private void HuellaRegistrada(bool success, byte[]? huella)
        {

            if (VistaActual == "Empleados")
            {
                _EmpleadosView.HuellaRegistrada(success, huella);
            }

        }

        /**
         * Indica el número de veces que has registrado la huella
         */
        private void RegistroHuellaContador(int counter, int total)
        {
            if (VistaActual == "Empleados")
            {
                _EmpleadosView.RegistroHuellaContador(counter);
            }
        }

        /**
        * Funcion para saber si el lector ha sido desconectado
        */
        private void OnDeviceChangeEvent(bool available)
        {
            HuellaController.validarMultipleHuella(HuellasUsuarios);
            if (VistaActual == "Inicio")
            {
                _InicioView.LectorHuellaDisponible(available);
            }
            /*if (!available)
            {
                HuellaController.Cancelar();
            }*/

        }

        /**
        * Funcion para saber si el lector está procesando una huella
        */
        private void OnHuellaValidarProgresoEvent()
        {
            if (VistaActual == "Inicio")
            {
                _InicioView.MostrarLoader();
            }
        }

        /**
         * Funcion cuando una huella ha sido escaneada 
         */
        private void OnHuellaValidarEvent(bool success, UsuarioHuella? huella)
        {
            if (VistaActual == "Inicio")
            {
                if (huella != null)
                {
                    var index = HuellasUsuarios.FindIndex(item => huella.Id == item.Id);
                    if (index > 0)
                    {
                        HuellasUsuarios.RemoveAt(index);
                        HuellasUsuarios.Insert(0, huella);
                        HuellaController.validarMultipleHuella(HuellasUsuarios);
                    }
                }


                _InicioView.BuscarUsuarioById(huella?.UsuarioId);
            }

        }

        /**
         * Registra la aplicacion en aplicaciones de inicio windows
         */
        public void RegistrarInicioWindows(bool agregar)
        {
            bool registrar = true;
#if RELEASE
            registrar = false;
#endif
            if (registrar)
            {
                try
                {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    RegistryKey reg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                    Assembly curAssembly = Assembly.GetExecutingAssembly();
                    
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    if (agregar && reg.GetValue(curAssembly.GetName().Name) == null)
                    {
                        reg.SetValue(curAssembly.GetName().Name, $"{Directory.GetCurrentDirectory()}\\Tomate.exe");
                    }
                    else if (!agregar && reg.GetValue(curAssembly.GetName().Name) != null)
                    {
                        reg.DeleteValue($"{curAssembly.GetName().Name}");
                    }
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                }
                catch { }
            }

        }

        /**
         * Confirmación para cerrar el programa
         */
        public void MostrarDialogoCerrarPrograma()
        {
            this.Dispatcher.Invoke(() =>
            {
                DialogoAlertaCm.show("¿Cerrar el programa?", true, delegate ()
                {
                    this.Close();
                });
            });
        }

        // FUNCIONES RELACIONADAS A MOSTRAR NOTIFICACION TOAST
        private Timer? CancelarToastTimer;
        public void showToast(string mensaje, int duracion)
        {
            if (CancelarToastTimer != null)
            {
                CancelarToastTimer.Stop();
                CancelarToastTimer = null;
            }
            this.Dispatcher.Invoke(() =>
            {
                NotificacionToast.Visibility = Visibility.Visible;
                NotificacionToastTitulo.Text = mensaje;
            });

            CancelarToastTimer = new Timer();
            CancelarToastTimer.Elapsed += (sender, e) =>
            {
                hideToast();
                CancelarToastTimer.Stop();
                CancelarToastTimer = null;
            };
            CancelarToastTimer.Interval = duracion;
            CancelarToastTimer.AutoReset = false;
            CancelarToastTimer.Start();
        }

        public void hideToast()
        {
            this.Dispatcher.Invoke(() =>
            {
                NotificacionToast.Visibility = Visibility.Collapsed;
                NotificacionToastTitulo.Text = "";
            });
        }

        // FUNCIONES RELACIONADAS A MOSTRAR DIALOGO DE ALERTA

        private Action? DialogoAlertaAceptar;
        private Action? DialogoAlertaCancelar;


        public void MostrarDialogoAlerta(string? titulo, string mensaje, bool cancelarBoton,
            Action? aceptarAction, Action? cancelarAction, bool cancelable = true)
        {
            if (!string.IsNullOrEmpty(ViewModel.MensajeErrorSincronizacion))
            {
                return;
            }
            DialogoAlertaAceptar = aceptarAction;
            DialogoAlertaCancelar = cancelarAction;

            this.Dispatcher.Invoke(() =>
            {
                DialogoTitulo.Text = titulo;
                DialogoDescripcion.Text = mensaje;

                if (cancelarBoton)
                {
                    DialogoCancelar.Visibility = Visibility.Visible;
                }
                else
                {
                    DialogoCancelar.Visibility = Visibility.Collapsed;
                }
                if (cancelable)
                {
                    DialogoBotones.Visibility = Visibility.Visible;
                }
                else
                {
                    DialogoBotones.Visibility = Visibility.Collapsed;
                }
                DialogoAlerta.CloseOnClickAway = cancelable;
                DialogoAlerta.IsOpen = true;
            });
        }


        /**
         * Cancelar dialogo alerta
         */
        private void BotonCancelarDialogo_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                DialogoAlerta.IsOpen = false;
            });
        }

        /**
        * Aceptar dialogo alerta
        */
        private void BotonAceptarDialogo_Click(object sender, RoutedEventArgs e)
        {
            DialogoAlertaCancelar = null;
            if (DialogoAlertaAceptar != null)
            {
                DialogoAlertaAceptar.Invoke();
            }
            this.Dispatcher.Invoke(() =>
            {
                DialogoAlerta.IsOpen = false;
            });

        }

        private bool AlertaRemoverBlur = true;
        private void DialogoAlerta_DialogClosed(object sender, DialogClosedEventArgs eventArgs)
        {
            if (DialogoAlertaCancelar != null)
            {
                DialogoAlertaCancelar.Invoke();
            }
            DialogoAlertaBackground.Visibility = Visibility.Collapsed;

            if (AlertaRemoverBlur)
            {
                RemoverBlur();
            }

        }

        private void DialogoAlerta_DialogOpened(object sender, DialogOpenedEventArgs eventArgs)
        {
            DialogoAlertaBackground.Visibility = Visibility.Visible;
            AlertaRemoverBlur = !ExisteBlur();
            AgregarBlur();
        }

        /**
         * Componentes generales teclado
         */
        public void MostrarTeclado(Dictionary<string, object?> datos,
        Action<Dictionary<string, object?>> OnEnter, Action<Dictionary<string, object?>>? OnClose)
        {
            TecladoGeneralModal.mostrar(datos);
            TecladoGeneralModal.setOnEnter(OnEnter);
            TecladoGeneralModal.setOnClose(OnClose);
        }

        /**
         * Componentes generales teclado númerico
         */
        public void MostrarTecladoNumerico(Dictionary<string, object?> datos,
            Action<Dictionary<string, object?>> OnEnter, 
            Action<Dictionary<string, object?>>? OnClose = null)
        {
            TecladoNumeroGeneralModal.mostrar(datos);
            TecladoNumeroGeneralModal.setOnEnter(OnEnter);
            TecladoNumeroGeneralModal.setOnClose(OnClose);
        }


        /**
         * Componentes generales teclado númerico
         */
        public void MostrarTecladoDescuento(Dictionary<string, object?> datos,
            Action<Dictionary<string, object?>> OnEnter, Action<Dictionary<string, object?>>? OnClose)
        {
            TecladoDescuentoGeneralModal.mostrar(datos);
            TecladoDescuentoGeneralModal.setOnEnter(OnEnter);
            TecladoDescuentoGeneralModal.setOnClose(OnClose);
        }

        /**
         * Componentes generales venta tipo
         */
        public void MostrarVentaTipos(Dictionary<string, object?> datos,
            Action<Dictionary<string, object?>> OnEnter, Action<Dictionary<string, object?>>? OnClose)
        {
            VentaTiposGeneralModal.mostrar(datos);
            VentaTiposGeneralModal.setOnEnter(OnEnter);
            VentaTiposGeneralModal.setOnClose(OnClose);
        }


        /**
         * Revisa las actualizaciones de software y actualiza la app
         */
        private async Task ActualizarPrograma()
        {
            if (HttpRequest.HttpRequest.SERVIDOR_IP != null)
            {
                using var mgr = new UpdateManager($"{HttpRequest.HttpRequest.SERVIDOR_IP}/pv");
                var actualizaciones = await mgr.CheckForUpdate();

                if (actualizaciones.CurrentlyInstalledVersion == null)
                {
                    //Seems like this is not a squirrel installation, running within VisualStudio perhaps?
                    //MessageBox.Show("Unknown current version - possibly application was not installed correctly");
                }
                else if (actualizaciones.ReleasesToApply.Any())
                {
                    NoSincronizar = true;
                    DesactivarSincronizacion();
                    //Squirrel installation with updates to apply
                    this.Dispatcher.Invoke(() =>
                    {
                        AgregarBlur();
                        panelActualizando.Visibility = Visibility.Visible;
                        ActualizandoPuntoVenta.Value = 0;
                    });
                    await mgr.DownloadReleases(actualizaciones.ReleasesToApply, (int progreso) =>
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            TextoActualizar.Text = $"Descargando actualizaciones";
                            ActualizandoPuntoVenta.Value = progreso;
                            
                        });
                    });
                    this.Dispatcher.Invoke(() =>
                    {
                        ActualizandoPuntoVenta.Value = 0;
                    });
                    await mgr.ApplyReleases(actualizaciones, (int progreso) => {
                        this.Dispatcher.Invoke(() =>
                        {
                            TextoActualizar.Text = $"Actualizando punto de venta";
                            ActualizandoPuntoVenta.Value = progreso;

                        });
                    });
                    this.Dispatcher.Invoke(() =>
                    {
                        panelActualizando.Visibility = Visibility.Collapsed;
                        RemoverBlur();
                    });
                    System.Threading.Thread.Sleep(2000);
                    DbHelper.RestablecerTablas();
                    await mgr.UpdateApp();
                    UpdateManager.RestartApp();
                }
            }
            
        }

    }
}
