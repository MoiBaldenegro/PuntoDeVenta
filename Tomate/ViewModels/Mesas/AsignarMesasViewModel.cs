using System.Collections.ObjectModel;
using System.Windows;
using Tomate.Models.Usuarios;
using Tomate.Utils;
using Tomate.Models.Ventas;

namespace Tomate.ViewModels
{
    public class AsignarMesasViewModel : ObservableObject
    {
        private Usuario _Usuario;

        public Usuario Usuario
        {
            get { return _Usuario; }
            set
            {
                _Usuario = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Usuario> _Empleados;

        public ObservableCollection<Usuario> Empleados
        {
            get { return _Empleados; }
            set
            {
                _Empleados = value;
                RevisarNumeroEmpleados();
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Mesa> _Mesas;

        public ObservableCollection<Mesa> Mesas
        {
            get { return _Mesas; }
            set
            {
                _Mesas = value;
                OnPropertyChanged();
            }
        }

        private Visibility _MensajeNoEmpleados = Visibility.Visible;
        public Visibility MensajeNoEmpleados
        {
            get { return _MensajeNoEmpleados; }
            set
            {
                _MensajeNoEmpleados = value;
                OnPropertyChanged();
            }
        }

        public AsignarMesasViewModel()
        {
            Empleados = new ObservableCollection<Usuario>();
        }

        public void RevisarNumeroEmpleados()
        {
            if (Empleados.Count == 0 && MensajeNoEmpleados != Visibility.Visible)
            {
                MensajeNoEmpleados = Visibility.Visible;
            }
            else if (Empleados.Count > 0 && MensajeNoEmpleados != Visibility.Hidden)
            {
                MensajeNoEmpleados = Visibility.Hidden;
            }
        }
    }
}
