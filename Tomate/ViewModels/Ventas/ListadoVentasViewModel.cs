using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Tomate.Models.Usuarios;
using Tomate.Models.Ventas;
using Tomate.Statics;
using Tomate.Utils;

namespace Tomate.ViewModels.Ventas
{
    public class ListadoVentasViewModel : ObservableObject
    {

        private Usuario _Usuario;

        public Usuario Usuario
        {
            get { return _Usuario; }
            set
            {
                _Usuario = value;
                if (_Usuario != null)
                {
                    UsuarioAusente = _Usuario.Ausente;
                }
                OnPropertyChanged();
            }
        }

        public string TituloSeccion
        {
            get
            {
                return $"Cuentas abiertas {Ventas.Count}";
            }
        }

        private List<TurnoAbierto> _TodosTurnosAbiertos;
        public List<TurnoAbierto> TodosTurnosAbiertos
        {
            get { return _TodosTurnosAbiertos; }
            set
            {
                _TodosTurnosAbiertos = value;
            }
        }

        private ObservableCollection<Venta> _Ventas;
        public ObservableCollection<Venta> Ventas
        {
            get { return _Ventas; }
            set
            {
                _Ventas = value;
                revisarNumeroCuentas();
                OnPropertyChanged();
                OnPropertyChanged(nameof(TituloSeccion));
            }
        }

        private Visibility _MostrarFiltros = Visibility.Collapsed;
        public Visibility MostrarFiltros
        {
            get { return _MostrarFiltros; }
            set
            {
                _MostrarFiltros = value;
                OnPropertyChanged();
            }
        }

        private Visibility _MensajeNoCuentas = Visibility.Visible;
        public Visibility MensajeNoCuentas
        {
            get { return _MensajeNoCuentas; }
            set
            {
                _MensajeNoCuentas = value;
                OnPropertyChanged();
            }
        }

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

        private string _MensajeTableroColor = "#a3a3a3";
        public string MensajeTableroColor
        {
            get { return _MensajeTableroColor; }
            set
            {
                _MensajeTableroColor = value;
                OnPropertyChanged();
            }
        }

        private int _MensajeTableroSize = 23;
        public int MensajeTableroSize
        {
            get { return _MensajeTableroSize; }
            set
            {
                _MensajeTableroSize = value;
                OnPropertyChanged();
            }
        }

        private bool _UsuarioAusente = false;

        public bool UsuarioAusente
        {
            get
            {
                return _UsuarioAusente;
            }
            set
            {
                _UsuarioAusente = value;

                if (_UsuarioAusente)
                {
                    AusenteBackground = "#fc5656";
                    AusenteForeground = "#FFFFFF";
                    AusenteIcono = "/Resources/Iconos/Nuevos/ic_candado_abierto_white.png";
                }
                else
                {
                    AusenteBackground = "#FFFFFF";
                    AusenteForeground = "#333333";
                    AusenteIcono = "/Resources/Iconos/Nuevos/ic_candado_black.png";
                }
            }
        }

        private string _AusenteIcono = "/Resources/Iconos/Nuevos/ic_candado_black.png";
        public string AusenteIcono
        {
            get { return _AusenteIcono; }
            set
            {
                _AusenteIcono = value;
                OnPropertyChanged();
            }
        }

        private string _AusenteBackground = "#FFFFFF";
        public string AusenteBackground
        {
            get { return _AusenteBackground; }
            set
            {
                _AusenteBackground = value;
                OnPropertyChanged();
            }
        }
        private string _AusenteForeground = "#333333";
        public string AusenteForeground
        {
            get { return _AusenteForeground; }
            set
            {
                _AusenteForeground = value;
                OnPropertyChanged();
            }
        }

        

        private int _TotalBotones = 8;
        public int TotalBotones
        {
            get { return _TotalBotones; }
            set
            {
                _TotalBotones = value;
                OnPropertyChanged();
            }
        }

        private bool _TurnoIniciado = false;
        public bool TurnoIniciado
        {
            get
            {
                return _TurnoIniciado;
            }
            set
            {
                _TurnoIniciado = value;
                OnPropertyChanged(nameof(TituloTurno));
            }
        }

        public string TituloTurno
        {
            get
            {
                return TurnoIniciado ? "Finalizar turno" : "Iniciar turno";
            }
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ListadoVentasViewModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            Ventas = new ObservableCollection<Venta>();
        }

        public void addVenta(Venta cuenta)
        {
            VentasStatic.TodasVentas.Insert(0, cuenta);
            revisarNumeroCuentas();
        }

        public void setVentas(List<Venta> TodasVentas)
        {
            Ventas = new ObservableCollection<Venta>(TodasVentas.Where(item => item.Visible).ToList());
        }

        /*public Venta? getVenta(string numeroCuenta)
        {
            return TodasVentas.Where(item => item.NumeroCuenta == numeroCuenta).FirstOrDefault();
        }*/

        private void revisarNumeroCuentas()
        {
            if (Ventas.Count == 0 && MensajeNoCuentas != Visibility.Visible)
            {
                MensajeNoCuentas = Visibility.Visible;
            }
            else if (Ventas.Count > 0 && MensajeNoCuentas != Visibility.Hidden)
            {
                MensajeNoCuentas = Visibility.Hidden;
            }
        }

        /*public void RevisarVentasVisibles()
        {
            for (int i = 0; i < TodasVentas.Count; i++)
            {
                TodasVentas[i].Visible = Usuario.RevisarPermiso("pv.todas-ventas") || Usuario.BuscarMesa($"{TodasVentas[i].NumeroCuenta}");
            }
            TodasVentas = TodasVentas;
        }*/

        public void SetTurnosAbiertos(List<TurnoAbierto> turnos)
        {
            TodosTurnosAbiertos = turnos;
            if (Usuario != null)
            {
                var existeUsuario = TodosTurnosAbiertos.Where(item => item.UsuarioId == Usuario.Id).FirstOrDefault();
                TurnoIniciado = existeUsuario != null;
            }
            
        }
    }
}
