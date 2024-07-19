using System.Collections.ObjectModel;
using Tomate.Models.Cobrar;
using Tomate.Utils;

namespace Tomate.Components.ViewModels.Caja
{
    public class FormasPagosViewModel : ObservableObject
    {

        private ObservableCollection<FormaPago> _FormasPagos;

        public ObservableCollection<FormaPago> FormasPagos
        {
            get { return _FormasPagos; }
            set
            {
                _FormasPagos = value;
                OnPropertyChanged();
            }
        }


        public FormasPagosViewModel()
        {
            FormasPagos = new ObservableCollection<FormaPago>();
        }

        public void IniciarFormasPagos()
        {
            FormasPagos = FormaPago.todas();
        }

    }
}
