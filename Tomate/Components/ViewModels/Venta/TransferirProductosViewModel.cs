using System.Collections.ObjectModel;
using System.Linq;
using Tomate.Models.Ventas;
using Tomate.Utils;

namespace Tomate.Components.ViewModels.Venta
{
    public class TransferirProductosViewModel : ObservableObject
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

        private Models.Ventas.Venta _VentaTransferir;

        public Models.Ventas.Venta VentaTransferir
        {
            get { return _VentaTransferir; }
            set
            {
                _VentaTransferir = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<VentaProducto> _VentaProductos = new ObservableCollection<VentaProducto>();

        public ObservableCollection<VentaProducto> VentaProductos
        {
            get { return _VentaProductos; }
            set
            {
                _VentaProductos = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<VentaProducto> _MoverProductos = new ObservableCollection<VentaProducto>();

        public ObservableCollection<VentaProducto> MoverProductos
        {
            get { return _MoverProductos; }
            set
            {
                _MoverProductos = value;
                OnPropertyChanged();
            }
        }

        public int GetIndexMoverProductos(string id)
        {
            return MoverProductos.Select((Producto, Index) => new { Producto, Index })
                        .First(item => item.Producto.Id == id)
                        .Index;
        }

        public int GetIndexVentaProductos(string id)
        {
            return VentaProductos.Select((Producto, Index) => new { Producto, Index })
                        .First(item => item.Producto.Id == id)
                        .Index;
        }

        public TransferirProductosViewModel()
        {

        }
    }
}
