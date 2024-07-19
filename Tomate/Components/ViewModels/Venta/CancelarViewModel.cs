using System.Collections.ObjectModel;
using Tomate.Models.Ventas;
using Tomate.Utils;

namespace Tomate.Components.ViewModels.Venta
{
    public class CancelarViewModel : ObservableObject
    {
        private ObservableCollection<MotivoCancelacion> _MotivosCancelacion;

        public ObservableCollection<MotivoCancelacion> MotivosCancelacion
        {
            get { return _MotivosCancelacion; }
            set
            {
                _MotivosCancelacion = value;
                OnPropertyChanged();
            }
        }

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
        public CancelarViewModel()
        {

        }
    }
}
