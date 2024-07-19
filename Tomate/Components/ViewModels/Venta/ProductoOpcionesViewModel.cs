using Tomate.Models.Ventas;
using Tomate.Utils;

namespace Tomate.Components.ViewModels.Venta
{
    public class ProductoOpcionesViewModel : ObservableObject
    {
        private VentaProducto _VentaProducto { get; set; }

        public VentaProducto VentaProducto
        {
            get
            {
                return _VentaProducto;
            }
            set
            {
                _VentaProducto = value;
                OnPropertyChanged();
            }
        }

        private string _TituloImprimir = "No trabaja";

        public string TituloImprimir
        {
            get
            {
                return _TituloImprimir;
            }
            set
            {
                _TituloImprimir = value;
                OnPropertyChanged();
            }
        }

        public ProductoOpcionesViewModel()
        {

        }
    }
}
