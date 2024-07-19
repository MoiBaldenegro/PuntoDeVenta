using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using Tomate.Models.Usuarios;
using Tomate.Models.Cobrar;
using Tomate.Models.Ventas;
using Tomate.Utils;
using Tomate.Models;
using Tomate.Models.Caja;

namespace Tomate.ViewModels.Cobro
{
    public class CobroVentaViewModel : ObservableObject
    {

        public VentaNota? VentaNota { get; set; }

        public bool PagoCubierto = false;

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

        private Venta _Venta;

        public Venta Venta
        {
            get { return _Venta; }
            set
            {
                _Venta = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<VentaPago> _VentaPagos;

        public ObservableCollection<VentaPago> VentaPagos
        {
            get { return _VentaPagos; }
            set
            {
                _VentaPagos = value;
                OnPropertyChanged();
            }
        }

        public List<string> VentaPagosEliminados { get; set; } = new List<string>();

        private ObservableCollection<PagoPredefinido> _PagosPredefinidos;

        public ObservableCollection<PagoPredefinido> PagosPredefinidos
        {
            get { return _PagosPredefinidos; }
            set
            {
                _PagosPredefinidos = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<CobroMoneda> _CobroMonedas;

        public ObservableCollection<CobroMoneda> CobroMonedas
        {
            get { return _CobroMonedas; }
            set
            {
                _CobroMonedas = value;
                OnPropertyChanged();
            }
        }


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

        private FormaPago _FormaPagoEfectivo;

        public FormaPago FormaPagoEfectivo
        {
            get { return _FormaPagoEfectivo; }
            set
            {
                _FormaPagoEfectivo = value;
                OnPropertyChanged();
            }
        }

        private Visibility _MensajeNoPagos = Visibility.Visible;
        public Visibility MensajeNoPagos
        {
            get { return _MensajeNoPagos; }
            set
            {
                _MensajeNoPagos = value;
                OnPropertyChanged();
            }
        }

        private float _SubtotalCobro = 0;

        public float SubtotalCobro
        {
            get { return _SubtotalCobro; }
            set
            {
                _SubtotalCobro = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SubtotalCobroFormato));
            }
        }

        public string SubtotalCobroFormato
        {
            get
            {
                return $"{SubtotalCobro.ToString("C", CultureInfo.CurrentCulture)}";
            }
        }

        private float? _DescuentoProductosCobro;

        public float? DescuentoProductosCobro
        {
            get { return _DescuentoProductosCobro; }
            set
            {
                _DescuentoProductosCobro = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DescuentoProductosCobroFormato));
            }
        }

        public string DescuentoProductosCobroFormato
        {
            get
            {
                string total = $"{(DescuentoProductosCobro ?? 0).ToString("C", CultureInfo.CurrentCulture)}";
                if ((DescuentoProductosCobro ?? 0) > 0)
                {
                    total = $"-{total}";
                }
                return total;
            }
        }

        private float? _DescuentoCobro;

        public float? DescuentoCobro
        {
            get { return _DescuentoCobro; }
            set
            {
                _DescuentoCobro = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DescuentoCobroFormato));
            }
        }

        public string DescuentoCobroFormato
        {
            get
            {
                string total = $"{(DescuentoCobro ?? 0).ToString("C", CultureInfo.CurrentCulture)}";
                if ((DescuentoCobro ?? 0) > 0)
                {
                    total = $"-{total}";
                }
                return total;
            }
        }

        private float _TotalPagos = 0; 

        public float TotalPagos
        {
            get { return _TotalPagos; }
            set
            {
                _TotalPagos = value;
                OnPropertyChanged(nameof(TotalPagosFormato));
                OnPropertyChanged();
            }
        }

        public string TotalPagosFormato
        {
            get
            {
                return $"{TotalPagos.ToString("C", CultureInfo.CurrentCulture)}";
            }
        }

        private float _TotalPropina = 0;

        public float TotalPropina
        {
            get { return _TotalPropina; }
            set
            {
                _TotalPropina = value;
                OnPropertyChanged(nameof(TotalPropinaFormato));
                OnPropertyChanged();
            }
        }

        public string TotalPropinaFormato
        {
            get
            {
                return $"{TotalPropina.ToString("C", CultureInfo.CurrentCulture)}";
            }
        }

        private float _TotalDebe = 0;

        public float TotalDebe
        {
            get { return _TotalDebe; }
            set
            {
                _TotalDebe = value;
                if (_TotalDebe <= 0)
                {

                    PagoCubierto = true;
                    TotalDebeTitulo = "Cambio";
                }
                else
                {
                    PagoCubierto = false;
                    TotalDebeTitulo = "Total por pagar";
                }
                OnPropertyChanged(nameof(TotalDebeFormato));
                OnPropertyChanged();
            }
        }

        public string TotalDebeFormato
        {
            get
            {
                float total = TotalDebe;
                if (PagoCubierto && TotalDebe < 0)
                {
                    total = TotalDebe * -1;
                }
                return $"{total.ToString("C", CultureInfo.CurrentCulture)}";
            }
        }

        private string _TotalDebeTitulo = "Total por pagar";

        public string TotalDebeTitulo
        {
            get { return _TotalDebeTitulo; }
            set
            {
                _TotalDebeTitulo = value;
                OnPropertyChanged();
            }
        }

        public CajaCorte? CajaCorte { get; set; } = null;

        public CobroVentaViewModel()
        {
            VentaPagos = new ObservableCollection<VentaPago>();
            FormasPagos = new ObservableCollection<FormaPago>();
            PagosPredefinidos = new ObservableCollection<PagoPredefinido>();
        }

        public void AgregarPago(VentaPago pago)
        {
            VentaPagos.Add(pago);
            this.CalcularTotalDebe();
        }

        public void RemoverPago(int index)
        {
            if (VentaPagos[index].Id != null)
            {
                VentaPagosEliminados.Add($"{VentaPagos[index].Id}");
            }
            
            VentaPagos.RemoveAt(index);
            this.CalcularTotalDebe();
        }

        

        public List<VentaPago> getPagosAgregados()
        {
            return VentaPagos.Where(item => item.Id == null).ToList();
        }

        public void CalcularTotalDebe()
        {
            float totalDebe = SubtotalCobro;
            float pagos = 0;
            float propinas = 0;
            foreach (VentaPago pago in VentaPagos)
            {
                pagos += (float)pago.Importe;
                propinas += (float)pago.Propina;
            }
            totalDebe -= DescuentoProductosCobro ?? 0;
            totalDebe -= DescuentoCobro ?? 0;
            totalDebe -= pagos;
            this.TotalPagos = pagos;
            this.TotalPropina = propinas;
            this.TotalDebe = totalDebe;
        }

        public void IniciarFormasPagos()
        {
            FormasPagos = FormaPago.todas();
            FormaPagoEfectivo = FormaPago.Efectivo(); 
        }

        public void IniciarMonedas()
        {
            CobroMonedas = new ObservableCollection<CobroMoneda> { 
                            new CobroMoneda(1000),
                            new CobroMoneda(500),
                            new CobroMoneda(200),
                            new CobroMoneda(100),
                            new CobroMoneda(50),
                            new CobroMoneda(20),
                            new CobroMoneda(10),
                            new CobroMoneda(5),
                            new CobroMoneda(2),
                            new CobroMoneda(1),
                            new CobroMoneda((float)0.5)
                        };
            //PagosPredefinidos = PagoPredefinido.todos();
        }

    }
}
