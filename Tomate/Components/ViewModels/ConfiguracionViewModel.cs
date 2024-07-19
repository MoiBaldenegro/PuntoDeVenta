using Tomate.Utils;

namespace Tomate.Components.ViewModels
{
    public class ConfiguracionViewModel : ObservableObject
    {

        private string _DireccionServidor;

        public string DireccionServidor
        {
            get { return _DireccionServidor; }
            set
            {
                _DireccionServidor = value;
                OnPropertyChanged();
            }
        }

        private string _TokenAcceso;

        public string TokenAcceso
        {
            get { return _TokenAcceso; }
            set
            {
                _TokenAcceso = value;
                OnPropertyChanged();
            }
        }

        private bool _RegistrarInicio;

        public bool RegistrarInicio
        {
            get { return _RegistrarInicio; }
            set
            {
                _RegistrarInicio = value;
                OnPropertyChanged();
            }
        }

        private string _ErrorGeneral;

        public string ErrorGeneral
        {
            get { return _ErrorGeneral; }
            set
            {
                _ErrorGeneral = value;
                OnPropertyChanged();
            }
        }

        private string _ErrorDireccion;

        public string ErrorDireccion
        {
            get { return _ErrorDireccion; }
            set
            {
                _ErrorDireccion = value;
                OnPropertyChanged();
            }
        }

        private string _ErrorToken;

        public string ErrorToken
        {
            get { return _ErrorToken; }
            set
            {
                _ErrorToken = value;
                OnPropertyChanged();
            }
        }


        public ConfiguracionViewModel()
        {

        }

    }
}
