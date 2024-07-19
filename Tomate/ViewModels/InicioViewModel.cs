using Tomate.Utils;

namespace Tomate.ViewModels
{
    public class InicioViewModel : ObservableObject
    {

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

        private string _TituloTablero;
        public string TituloTablero
        {
            get { return _TituloTablero; }
            set
            {
                _TituloTablero = value;
                OnPropertyChanged();
            }
        }

        private string _VersionSoftware = "v.1.0.0.0";
        public string VersionSoftware
        {
            get { return _VersionSoftware; }
            set
            {
                _VersionSoftware = value;
                OnPropertyChanged();
            }
        }

        private string _FechaHora = "--/--/---- --:--";
        public string FechaHora
        {
            get { return _FechaHora; }
            set
            {
                _FechaHora = value;
                OnPropertyChanged();
            }
        }


        public InicioViewModel()
        {

        }
    }
}
