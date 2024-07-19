using System.Collections.ObjectModel;
using System.Windows;
using Tomate.Models.Catalogo;
using Tomate.Utils;

namespace Tomate.Components.ViewModels.Venta
{
    public class ProductoModificadoresViewModel : ObservableObject
    {
        private ObservableCollection<Modificador> _Modificadores;

        public ObservableCollection<Modificador> Modificadores
        {
            get { return _Modificadores; }
            set
            {
                _Modificadores = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Modificador> _ModificadoresAgregados;

        public ObservableCollection<Modificador> ModificadoresAgregados
        {
            get { return _ModificadoresAgregados; }
            set
            {
                _ModificadoresAgregados = value;
                OnPropertyChanged();
            }
        }

        public ProductoModificadoresViewModel()
        {

        }
    }
}
