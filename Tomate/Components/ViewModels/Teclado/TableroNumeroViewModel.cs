using Tomate.Utils;

namespace Tomate.Components.ViewModels.Teclado
{
    public class TableroNumeroViewModel : ObservableObject
    {

        private string _MensajeTeclado;
        public string MensajeTeclado
        {
            get { return _MensajeTeclado; }
            set
            {
                _MensajeTeclado = value;
                OnPropertyChanged();
            }
        }

        private string _TituloTeclado = "Ingresa un número";
        public string? TituloTeclado
        {
            get { return _TituloTeclado; }
            set
            {
                _TituloTeclado = value;
                OnPropertyChanged();
            }
        }

        public TableroNumeroViewModel()
        {

        }

    }
}
