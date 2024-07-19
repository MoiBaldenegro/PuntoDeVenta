using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Tomate.Models.Ventas;
using Tomate.Models.Caja;
using Tomate.Models.Usuarios;
using Tomate.Models.Cobrar;
using Tomate.Utils;
using System.Collections.Generic;
using System.Windows;

namespace Tomate.ViewModels.Caja
{
    public class CajaViewModel : ObservableObject
    {

        private Usuario _Usuario;

        public Usuario Usuario
        {
            get { return _Usuario; }
            set
            {
                _Usuario = value;
                OnPropertyChanged();
            }
        }

        private int _TotalBotones = 5;
        public int TotalBotones
        {
            get { return _TotalBotones; }
            set
            {
                _TotalBotones = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<CajaFormaPago> _FormasPagosApertura;
        public ObservableCollection<CajaFormaPago> FormasPagosApertura
        {
            get { return _FormasPagosApertura; }
            set
            {
                _FormasPagosApertura = value;
                OnPropertyChanged();
                //OnPropertyChanged(nameof(AperturaCajaTotal));
            }
        }

        private ObservableCollection<CajaFormaPago> _SumaFormasPagos;
        public ObservableCollection<CajaFormaPago> SumaFormasPagos
        {
            get { return _SumaFormasPagos; }
            set
            {
                _SumaFormasPagos = value;
                OnPropertyChanged();
                //OnPropertyChanged(nameof(CierreCajaTotal));
            }
        }



        private string? _FormaPagoIdFiltro = null;
        public string? FormaPagoIdFiltro
        {
            get { return _FormaPagoIdFiltro; }
            set
            {
                _FormaPagoIdFiltro = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(VentasPagos));
            }
        }

        private List<VentaPago> VentasNotasPagos = new List<VentaPago>();

        public ObservableCollection<VentaPago> VentasPagos
        {
            get {
                var filtro = VentasNotasPagos.Where(item => FormaPagoIdFiltro == null || $"{FormaPagoIdFiltro}" == item.FormaPagoId);
                var ventasPagosFiltros = new ObservableCollection<VentaPago>(filtro.ToList());
                if (ventasPagosFiltros.Count > 0)
                {
                    NoPagos = Visibility.Collapsed;
                }
                else
                {
                    NoPagos = Visibility.Visible;
                }
                return ventasPagosFiltros;
            }
        }

        public ObservableCollection<VentaNota> VentasNotas
        {
            get
            {
                VentasNotasPagos = new List<VentaPago>();
                NoVentas = Visibility.Visible;
                if (CajaCorte != null)
                {
                    if (CajaCorte.VentasNotas.Count > 0)
                    {
                        NoVentas = Visibility.Collapsed;

                        foreach(var nota in CajaCorte.VentasNotas)
                        {
                            foreach (var pago in nota.VentaPagos)
                            {
                                VentasNotasPagos.Add(pago);
                            }
                        }
                    }
                    OnPropertyChanged(nameof(VentasPagos));
                    return new ObservableCollection<VentaNota>(CajaCorte.VentasNotas);
                }
                OnPropertyChanged(nameof(VentasPagos));
                return new ObservableCollection<VentaNota>();
            }
        }

        private Visibility _NoPagos = Visibility.Visible;
        public Visibility NoPagos
        {
            get { return _NoPagos; }
            set
            {
                _NoPagos = value;
                OnPropertyChanged();
            }
        }

        private Visibility _NoVentas = Visibility.Visible;
        public Visibility NoVentas
        {
            get { return _NoVentas; }
            set
            {
                _NoVentas = value;
                OnPropertyChanged();
            }
        }

        

        public int FormaPagoIndex = -1;


        private ObservableCollection<FormaPago> _FormasPagos;
        public ObservableCollection<FormaPago> FormasPagos
        {
            get { return _FormasPagos; }
            set
            {
                _FormasPagos = value;
                OnPropertyChanged();
            }
        }

        public string CajaInicialFormato
        {
            get
            {

                return $"{(CajaCorte?.CajaInicial ?? 0).ToString("C", CultureInfo.CurrentCulture)}";
            }
        }

        /*public string CajaEntradasFormato
        {
            get
            {

                return $"{(CajaCorte?.CajaEntradas ?? 0).ToString("C", CultureInfo.CurrentCulture)}";
            }
        }

        public string CajaSalidasFormato
        {
            get
            {

                return $"{(CajaCorte?.CajaSalidas ?? 0).ToString("C", CultureInfo.CurrentCulture)}";
            }
        }*/

        public string CajaMovimientosEfectivoFormato
        {
            get
            {

                return $"{(CajaCorte?.CajaMovimientosEfectivo ?? 0).ToString("C", CultureInfo.CurrentCulture)}";
            }
        }

        

        public string CajaAjustesFormato
        {
            get
            {

                return $"{(CajaCorte?.CajaAjustes ?? 0).ToString("C", CultureInfo.CurrentCulture)}";
            }
        }

        public string CajaActualFormato
        {
            get
            {

                return $"{(CajaCorte?.CajaFinal ?? 0).ToString("C", CultureInfo.CurrentCulture)}";
            }
        }

        public string CajaPagosFormato
        {
            get
            {

                return $"{(CajaCorte?.Pagos ?? 0).ToString("C", CultureInfo.CurrentCulture)}";
            }
        }

        public string CajaPagosEfectivoFormato
        {
            get
            {

                return $"{(CajaCorte?.PagosEfectivo ?? 0).ToString("C", CultureInfo.CurrentCulture)}";
            }
        }

        public string EncargadoCaja
        {
            get
            {

                return $"{CajaCorte?.Cajero?.Codigo} {CajaCorte?.Cajero?.Alias}";
            }
        }

        public string HoraAperturaCaja
        {
            get
            {
                if (CajaCorte != null)
                {
                    return $"{CajaCorte.FechaInicio?.ToString("dd MMM yyyy HH:mm")}";
                }
                return "";
            }
        }


        /*private bool _MostrarVentasActivas = true;
        public bool MostrarVentasActivas
        {
            get { return _MostrarVentasActivas; }
            set
            {
                _MostrarVentasActivas = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Ventas));
                OnPropertyChanged(nameof(VentasActivasFondo));
                OnPropertyChanged(nameof(VentasFinalizadasFondo));
            }
        }*/

        /*public string VentasActivasFondo
        {
            get { 
                return MostrarVentasActivas ? "#FFF" : "#d0d0d0"; 
            }
        }

        public string VentasFinalizadasFondo
        {
            get
            {
                return !MostrarVentasActivas ? "#FFF" : "#d0d0d0";
            }
        }*/


        private string _CajaCorteTitulo = "Apertura caja";

        public string CajaCorteTitulo
        {
            get { return _CajaCorteTitulo; }
            set
            {
                _CajaCorteTitulo = value;
                OnPropertyChanged();
            }
        }

        private CajaCorte? _CajaCorte = null;

        public CajaCorte? CajaCorte
    {
            get { return _CajaCorte; }
            set
            {
                _CajaCorte = value;
                if (_CajaCorte == null)
                {
                    CajaCorteTitulo = "Apertura caja";
                }
                else
                {
                    CajaCorteTitulo = "Cierre caja";
                }
                OnPropertyChanged();
                OnPropertyChanged(nameof(VentasNotas));
                OnPropertyChanged(nameof(CajaInicialFormato));
                OnPropertyChanged(nameof(CajaMovimientosEfectivoFormato));
                OnPropertyChanged(nameof(CajaAjustesFormato));
                OnPropertyChanged(nameof(CajaActualFormato));
                OnPropertyChanged(nameof(CajaPagosFormato));
                OnPropertyChanged(nameof(CajaPagosEfectivoFormato));
                OnPropertyChanged(nameof(EncargadoCaja));
                OnPropertyChanged(nameof(HoraAperturaCaja));
            }
        }

        public bool CajaConsultada = false;

        public CajaViewModel()
        {
            FormasPagos = new ObservableCollection<FormaPago>();
            /*VentasPagos = new ObservableCollection<VentaPago>();*/
        }

        public void CargarFormasPagos()
        {
            FormasPagos = FormaPago.todas();
            var formaPagosTodos = new FormaPago();
            formaPagosTodos.Id = null;
            formaPagosTodos.Nombre = "Todos";
            FormasPagos.Insert(0, formaPagosTodos);
        }

        public void NotifyChange(string name)
        {
            OnPropertyChanged(name);
        }


    }
}
