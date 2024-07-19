using Tomate.Utils;

namespace Tomate.Components.ViewModels.Empleados
{
    public class EmpleadoHuellasViewModel : ObservableObject
    {

        private string _ErrorGeneral = "";
        public string ErrorGeneral
        {
            get { return _ErrorGeneral; }
            set
            {
                _ErrorGeneral = value;
                OnPropertyChanged();
            }
        }

        private string _MensajeModal;
        public string MensajeModal
        {
            get { return _MensajeModal; }
            set
            {
                _MensajeModal = value;
                OnPropertyChanged();
            }
        }

        private string[] _ContadorBackground = new string[4];

        public string[] ContadorBackground
        {
            get
            {
                return _ContadorBackground;
            }
            set
            {
                _ContadorBackground = value;
                OnPropertyChanged();
            }
        }

        private string[] _ContadorForeground = new string[4];

        public string[] ContadorForeground
        {
            get
            {
                return _ContadorForeground;
            }
            set
            {
                _ContadorForeground = value;
                OnPropertyChanged();
            }
        }


        public EmpleadoHuellasViewModel()
        {

        }

        public void RestablecerColoresregistro()
        {
            var _Colores = new string[4];
            _Colores[0] = "#00000000";
            _Colores[1] = "#00000000";
            _Colores[2] = "#00000000";
            _Colores[3] = "#00000000";

            var _ColoresHover = new string[4];
            _ColoresHover[0] = "#333333";
            _ColoresHover[1] = "#333333";
            _ColoresHover[2] = "#333333";
            _ColoresHover[3] = "#333333";

            ContadorBackground = _Colores;
            ContadorForeground = _ColoresHover;
        }

        public void CambiarDedoContador(int numero)
        {
            ContadorBackground[numero - 1] = "#fc5656";
            ContadorForeground[numero - 1] = "#FFFFFF";
            ContadorBackground = ContadorBackground;
            ContadorForeground = ContadorForeground;
        }

    }
}
