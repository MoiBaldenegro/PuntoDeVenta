
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Tomate.Utils;
using Tomate.Models.Ventas;

namespace Tomate.Components.ViewModels.Venta
{
    public class SeleccionarCuentaViewModel : ObservableObject
    {

        private Models.Ventas.Venta _Venta;

        public Models.Ventas.Venta Venta
        {
            get { return _Venta; }
            set
            {
                _Venta = value;
                OnPropertyChanged();
            }
        }

        private Visibility _MensajeNoCuentas = Visibility.Visible;
        public Visibility MensajeNoCuentas
        {
            get { return _MensajeNoCuentas; }
            set
            {
                _MensajeNoCuentas = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Models.Ventas.Venta> _Ventas = new ObservableCollection<Models.Ventas.Venta>();
        public ObservableCollection<Models.Ventas.Venta> Ventas
        {
            get
            {
                return new ObservableCollection<Models.Ventas.Venta>(_Ventas.Where(item => item.NumeroCuenta != _Venta.NumeroCuenta).ToList());
            }
            set
            {
                _Ventas = value;
                revisarNumeroCuentas();
                OnPropertyChanged();
            }
        }
        private string _MensajeTablero;
        public string MensajeTablero
        {
            get { return _MensajeTablero; }
            set
            {
                _MensajeTablero = value;
                OnPropertyChanged();
            }
        }
        private string _MensajeTableroColor = "#a3a3a3";
        public string MensajeTableroColor
        {
            get { return _MensajeTableroColor; }
            set
            {
                _MensajeTableroColor = value;
                OnPropertyChanged();
            }
        }

        private int _MensajeTableroSize = 23;
        public int MensajeTableroSize
        {
            get { return _MensajeTableroSize; }
            set
            {
                _MensajeTableroSize = value;
                OnPropertyChanged();
            }
        }
        public SeleccionarCuentaViewModel()
        {

        }

        private void revisarNumeroCuentas()
        {
            if (Ventas.Count == 0 && MensajeNoCuentas != Visibility.Visible)
            {
                MensajeNoCuentas = Visibility.Visible;
            }
            else if (Ventas.Count > 0 && MensajeNoCuentas != Visibility.Hidden)
            {
                MensajeNoCuentas = Visibility.Hidden;
            }
        }

        public Models.Ventas.Venta? getVenta(string numeroCuenta)
        {
            return Ventas.Where(item => item.NumeroCuenta == numeroCuenta).FirstOrDefault();
        }
    }
}
