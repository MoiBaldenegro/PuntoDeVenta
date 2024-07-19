using Tomate.Utils;

namespace Tomate.ViewModels
{
    public class MainViewModel : ObservableObject
    {

        private object _VistaActualControl;

        public object VistaActualControl
        {
            get { return _VistaActualControl; }
            set
            {
                _VistaActualControl = value;
                OnPropertyChanged();
            }
        }

        private string? _MensajeErrorSincronizacion;

        public string? MensajeErrorSincronizacion
        {
            get { return _MensajeErrorSincronizacion; }
            set
            {
                _MensajeErrorSincronizacion = value;
                OnPropertyChanged();
            }
        }



        public MainViewModel()
        {

        }
    }
}
