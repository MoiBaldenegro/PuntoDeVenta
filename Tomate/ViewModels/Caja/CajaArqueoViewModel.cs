using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Tomate.Models.Caja;
using Tomate.Models.Usuarios;
using Tomate.Utils;

namespace Tomate.ViewModels.Caja
{
    public class CajaArqueoViewModel : ObservableObject
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

        private CajaCorte? _CajaCorte;

        public CajaCorte? CajaCorte
        {
            get { return _CajaCorte; }
            set
            {
                _CajaCorte = value;
                OnPropertyChanged();
                //EfectivoTotal = _CajaCorte?.PagosEfectivo ?? 0;
                EfectivoTotal = _CajaCorte?.CajaFinal ?? 0;
            }
        }

        private bool _IsCierreArqueo = true;

        public bool IsCierreArqueo
        {
            get { return _IsCierreArqueo; }
            set
            {
                _IsCierreArqueo = value;
                OnPropertyChanged();
            }
        }


        public float _EfectivoTotal = 0;

        public float EfectivoTotal
        {
            get
            {
                return _EfectivoTotal;
            }
            set
            {
                _EfectivoTotal = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EfectivoCobradoFormato));
                OnPropertyChanged(nameof(EfectivoTotalFormato));
                OnPropertyChanged(nameof(EfectivoDiferencia));
                OnPropertyChanged(nameof(EfectivoDiferenciaFormato));
                OnPropertyChanged(nameof(EfectivoDiferenciaBackground));
            }
        }

        public float _EfectivoCobrado = 0;
        public float EfectivoCobrado
        {
            get
            {
                return _EfectivoCobrado;
            }
            set
            {
                _EfectivoCobrado = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EfectivoCobradoFormato));
                OnPropertyChanged(nameof(EfectivoTotalFormato));
                OnPropertyChanged(nameof(EfectivoDiferencia));
                OnPropertyChanged(nameof(EfectivoDiferenciaFormato));
                OnPropertyChanged(nameof(EfectivoDiferenciaBackground));
            }
        }

        public string EfectivoCobradoFormato
        {
            get
            {
                return $"{EfectivoCobrado.ToString("C", CultureInfo.CurrentCulture)}";
            }
        }

        public string EfectivoTotalFormato
        {
            get
            {
                return $"{EfectivoTotal.ToString("C", CultureInfo.CurrentCulture)}";
            }
        }

        public float EfectivoDiferencia
        {
            get
            {
                return EfectivoCobrado - EfectivoTotal;
            }
        }
        public string EfectivoDiferenciaFormato
        {
            get
            {
                return $"{(EfectivoDiferencia).ToString("C", CultureInfo.CurrentCulture)}";
            }
        }

        public string EfectivoDiferenciaBackground
        {
            get
            {
                if (EfectivoDiferencia >= 0)
                {
                    return "#333333";
                }
                return "#FC5656";
            }
        }

        private ObservableCollection<ArqueoMoneda> _ArqueoMonedas;
        public ObservableCollection<ArqueoMoneda> ArqueoMonedas
        {
            get { return _ArqueoMonedas; }
            set
            {
                _ArqueoMonedas = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ArqueoFormaPago> _ArqueoFormasPagos;
        public ObservableCollection<ArqueoFormaPago> ArqueoFormasPagos
        {
            get { return _ArqueoFormasPagos; }
            set
            {
                _ArqueoFormasPagos = value;
                OnPropertyChanged();
            }
        }

        private string _TituloArqueo;
        public string TituloArqueo
        {
            get { return _TituloArqueo; }
            set
            {
                _TituloArqueo = value;
                OnPropertyChanged();
            }
        }

        public string TipoInput = "moneda";
        public int IndexFocus = 0;

        private int _ColumnSpanMonedas = 1;
        public int ColumnSpanMonedas
        {
            get { return _ColumnSpanMonedas; }
            set
            {
                _ColumnSpanMonedas = value;
                OnPropertyChanged();
            }
        }

        

        public CajaArqueoViewModel()
        {
      
        }

        public void CargarMonedas()
        {
            ArqueoMonedas = new ObservableCollection<ArqueoMoneda> {
                new ArqueoMoneda(1000, 0, true),
                new ArqueoMoneda(500, 1, false),
                new ArqueoMoneda(200, 2, false),
                new ArqueoMoneda(100, 3, false),
                new ArqueoMoneda(50, 4, false),
                new ArqueoMoneda(20, 5, false),
                new ArqueoMoneda(10, 6, false),
                new ArqueoMoneda(5, 7, false),
                new ArqueoMoneda(2, 8, false),
                new ArqueoMoneda(1, 9, false),
                new ArqueoMoneda((float)0.5, 10, false)
            };

            
            IndexFocus = 0;
            TipoInput = "moneda";
            ActualizarTotales();
        }

        public void ActualizarMonedas(int index)
        {
            var moneda = ArqueoMonedas[index];
            ArqueoMonedas[index] = moneda;
            ArqueoMonedas.RemoveAt(index);
            ArqueoMonedas.Insert(index, moneda);
            ActualizarTotales();
        }

        public void ActualizarFormasPagos(int index)
        {
            var formaPago = ArqueoFormasPagos[index];
            ArqueoFormasPagos[index] = formaPago;
            ArqueoFormasPagos.RemoveAt(index);
            ArqueoFormasPagos.Insert(index, formaPago);
            ActualizarTotales();
        }

        public void ActualizarTotales()
        {
            EfectivoCobrado = ArqueoMonedas.Sum(item => item.Importe);
        }
    }
}
