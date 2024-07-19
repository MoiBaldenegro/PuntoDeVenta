
using System.Collections.ObjectModel;
using System.Windows;
using Tomate.Models.Ventas;
using Tomate.Utils;

namespace Tomate.Components.ViewModels.Venta.Notas
{
    public class VentaNotaDetalleViewModel : ObservableObject
    {

        public ObservableCollection<VentaProducto> VentaProductos
        {
            get
            {
                if (VentaNota != null)
                {
                    return new ObservableCollection<VentaProducto>(VentaNota.VentaProductos);
                }
                return new ObservableCollection<VentaProducto>();
            }

        }


        public ObservableCollection<VentaPago> VentaPagos
        {
            get {
                if (VentaNota != null)
                {
                    return new ObservableCollection<VentaPago>(VentaNota.VentaPagos);
                }
                return new ObservableCollection<VentaPago>(); 
            }
            
        }

        private VentaNota _VentaNota;
        public VentaNota VentaNota
        {
            get { return _VentaNota; }
            set
            {
                _VentaNota = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(VentaPagos));
                OnPropertyChanged(nameof(VentaProductos));
                OnPropertyChanged(nameof(Titulo));                

                if (VentaPagos.Count > 0)
                {
                    NoPagos = Visibility.Collapsed;
                }
                else
                {
                    NoPagos = Visibility.Visible;
                }
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

        public string Titulo
        {
            get
            {
                if (VentaNota != null)
                {
                    if (VentaNota.Venta.NumeroNotas > 1)
                    {
                        return $"Cuenta {VentaNota.Venta.NumeroCuenta} - Nota {VentaNota.NumeroNota}";
                    }
                    return $"Cuenta {VentaNota.Venta.NumeroCuenta}";
                }
                return "Venta nota";
            }
        }


        public VentaNotaDetalleViewModel()
        {

        }
    }
}
