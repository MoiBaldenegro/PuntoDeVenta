using Tomate.Utils;

namespace Tomate.Components.ViewModels.Teclado
{
    public class TableroDescuentoViewModel : ObservableObject
    {

        private string _MensajeTeclado;
        public string MensajeTeclado
        {
            get
            {
                return _MensajeTeclado;
            }
            set
            {
                _MensajeTeclado = value;
                OnPropertyChanged();
                MensajeTecladoDescuento = MensajeTecladoDescuento;
            }
        }

        public string MensajeTecladoDescuento
        {
            get
            {
                string mensajeCustom = "0";
                if (_MensajeTeclado != null && _MensajeTeclado.Length > 0)
                {
                    mensajeCustom = _MensajeTeclado;

                }
                if (IsPorcentaje)
                {
                    mensajeCustom = $"{mensajeCustom}%";
                }
                else
                {
                    mensajeCustom = $"${mensajeCustom}";
                }

                return mensajeCustom;
            }
            set
            {
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

        private bool _IsPorcentaje = true;
        public bool IsPorcentaje
        {
            get { return _IsPorcentaje; }
            set
            {
                _IsPorcentaje = value;
                OnPropertyChanged();
                MensajeTecladoDescuento = MensajeTecladoDescuento;
            }
        }

        public TableroDescuentoViewModel()
        {

        }

    }
}
