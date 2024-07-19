using System.Collections.ObjectModel;
using System.Windows;
using Tomate.Models.Catalogo;
using Tomate.Models.Ventas;
using Tomate.Utils;

namespace Tomate.Components.ViewModels.Venta
{
    public class ProductoExtrasViewModel : ObservableObject
    {
        private ObservableCollection<Producto> _Complementos;

        public ObservableCollection<Producto> Complementos
        {
            get { return _Complementos; }
            set
            {
                _Complementos = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<VentaProducto> _ComplementosAgregados;

        public ObservableCollection<VentaProducto> ComplementosAgregados
        {
            get { return _ComplementosAgregados; }
            set
            {
                _ComplementosAgregados = value;
                OnPropertyChanged();
            }
        }

        private Visibility _MensajeNoExtras = Visibility.Visible;
        public Visibility MensajeNoExtras
        {
            get { return _MensajeNoExtras; }
            set
            {
                _MensajeNoExtras = value;
                OnPropertyChanged();
            }
        }

        public ProductoExtrasViewModel()
        {

        }
    }
}
