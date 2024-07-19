using System.Collections.ObjectModel;
using System.Linq;
using Tomate.Models.Ventas;
using Tomate.Utils;

namespace Tomate.Components.ViewModels.Venta.Notas
{
    public class SeleccionarVentaNotaViewModel : ObservableObject
    {

        private ObservableCollection<VentaProducto> _VentaProductos = new ObservableCollection<VentaProducto>();

        public ObservableCollection<VentaProducto> VentaProductos
        {
            get { return _VentaProductos; }
            set
            {
                _VentaProductos = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<VentaNota> _VentaNotas = new ObservableCollection<VentaNota>();

        public ObservableCollection<VentaNota> VentaNotas
        {
            get { return _VentaNotas; }
            set
            {
                _VentaNotas = value;
                OnPropertyChanged();
            }
        }

        public int NumeroNotas { get; set; } = 1;

        public SeleccionarVentaNotaViewModel()
        {

        }

        public float getTotalNota(string numeroNota)
        {
            return (float)VentaProductos.Where(item => $"{item.NumeroNota}" == numeroNota && item.DeletedAt == null).Sum(item => item.Importe);
        }

        public ObservableCollection<VentaNota> CalcularNotasTotales()
        {
            for (int i = 0; i < VentaNotas.Count; i++)
            {
                VentaNotas[i].Total = getTotalNota($"{VentaNotas[i].NumeroNota}");
            }
            return new ObservableCollection<VentaNota>(VentaNotas);
        }

    }
}
