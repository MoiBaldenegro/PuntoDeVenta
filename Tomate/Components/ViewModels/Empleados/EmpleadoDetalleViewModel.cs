using Tomate.Models.Usuarios;
using Tomate.Utils;

namespace Tomate.Components.ViewModels.Empleados
{
    public class EmpleadoDetalleViewModel : ObservableObject
    {

        private Usuario _Usuario = new Usuario();
        public Usuario Usuario
        {
            get { return _Usuario; }
            set
            {
                _Usuario = value;
                OnPropertyChanged();
            }
        }



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

        private string _ErrorCodigo = "";
        public string ErrorCodigo
        {
            get { return _ErrorCodigo; }
            set
            {
                _ErrorCodigo = value;
                OnPropertyChanged();
            }
        }

        private string _ErrorPerfil = "";
        public string ErrorPerfil
        {
            get { return _ErrorPerfil; }
            set
            {
                _ErrorPerfil = value;
                OnPropertyChanged();
            }
        }

        private string _ErrorNombre = "";
        public string ErrorNombre
        {
            get { return _ErrorNombre; }
            set
            {
                _ErrorNombre = value;
                OnPropertyChanged();
            }
        }

        private string _ErrorAlias = "";
        public string ErrorAlias
        {
            get { return _ErrorAlias; }
            set
            {
                _ErrorAlias = value;
                OnPropertyChanged();
            }
        }

        private string _ErrorPassword = "";
        public string ErrorPassword
        {
            get { return _ErrorPassword; }
            set
            {
                _ErrorPassword = value;
                OnPropertyChanged();
            }
        }

        private string _ErrorConfirmarPassword = "";
        public string ErrorConfirmarPassword
        {
            get { return _ErrorConfirmarPassword; }
            set
            {
                _ErrorConfirmarPassword = value;
                OnPropertyChanged();
            }
        }



        public EmpleadoDetalleViewModel()
        {

        }


    }
}
