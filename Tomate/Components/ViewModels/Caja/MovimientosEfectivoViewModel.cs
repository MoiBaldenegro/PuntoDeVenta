using System.Collections.ObjectModel;
using System.Windows;
using Tomate.Models.Caja;
using Tomate.Models.Cobrar;
using Tomate.Models.Usuarios;
using Tomate.Models.Ventas;
using Tomate.Utils;

namespace Tomate.Components.ViewModels.Caja
{
    public class MovimientosEfectivoViewModel : ObservableObject
    {

        /*private ObservableCollection<CajaMovimientoEfectivo> _Movimientos;

        public ObservableCollection<CajaMovimientoEfectivo> Movimientos
        {
            get { return _Movimientos; }
            set
            {
                _Movimientos = value;
                if (Movimientos.Count > 0)
                {
                    NoMovimientos = Visibility.Collapsed;
                }
                else
                {
                    NoMovimientos = Visibility.Visible;
                }
                OnPropertyChanged();
            }
        }*/

        public ObservableCollection<CajaMovimientoEfectivo> Movimientos
        {
            get
            {
                var listado = new ObservableCollection<CajaMovimientoEfectivo>();
                if (CajaCorte != null)
                {
                    listado = new ObservableCollection<CajaMovimientoEfectivo>(CajaCorte.MovimientosEfectivo);
                }
                if (listado.Count > 0)
                {
                    NoMovimientos = Visibility.Collapsed;
                }
                else
                {
                    NoMovimientos = Visibility.Visible;
                }
                return listado;
            }

        }

        private Visibility _NoMovimientos = Visibility.Visible;
        public Visibility NoMovimientos
        {
            get { return _NoMovimientos; }
            set
            {
                _NoMovimientos = value;
                OnPropertyChanged();
            }
        }

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

        private CajaCorte _CajaCorte;

        public CajaCorte CajaCorte
        {
            get { return _CajaCorte; }
            set
            {
                _CajaCorte = value;
                OnPropertyChanged(nameof(Movimientos));
                OnPropertyChanged();
            }
        }


        public MovimientosEfectivoViewModel()
        {
        }

        public void ExistenMovimientos()
        {
            if (Movimientos.Count > 0)
            {
                NoMovimientos = Visibility.Collapsed;
            }
            else
            {
                NoMovimientos = Visibility.Visible;
            }
        }


    }
}
