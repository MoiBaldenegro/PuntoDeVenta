using System.Collections.ObjectModel;
using Tomate.Models.Ventas;
using Tomate.Utils;

namespace Tomate.Components.ViewModels.Venta
{
    public class VentaTiposViewModel : ObservableObject
    {
        private ObservableCollection<VentaTipo> _VentaTipos;

        public ObservableCollection<VentaTipo> VentaTipos
        {
            get { return _VentaTipos; }
            set
            {
                _VentaTipos = value;
                OnPropertyChanged();
            }
        }

        public VentaTiposViewModel()
        {

        }
    }
}
