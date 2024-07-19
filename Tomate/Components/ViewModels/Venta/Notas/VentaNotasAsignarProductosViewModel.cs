using System.Collections.ObjectModel;
using System.Linq;
using Tomate.Models.Ventas;
using Tomate.Utils;

namespace Tomate.Components.ViewModels.Venta.Notas
{
    public class VentaNotasAsignarProductosViewModel : ObservableObject
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

        public Models.Ventas.Venta? Venta { get; set; } = null;

        public VentaNotasAsignarProductosViewModel()
        {

        }

        public void ActualizarTotalNotas()
        {
            int index = 0;
            foreach (var nota in VentaNotas)
            {
                if (!nota.IsPagada)
                {
                    VentaNotas[index].Total = getTotalNota(nota);
                }
                
                index++;
            }
            OnPropertyChanged(nameof(VentaNotas));
        }

        public void ActualizarNotas(string numeroNotaAnterior, string numeroNotaActual)
        {
            int indexNotaAnterior = GetIndexNota(numeroNotaAnterior);
            var notaAnterior = VentaNotas[indexNotaAnterior];
            VentaNotas.RemoveAt(indexNotaAnterior);
            notaAnterior.Total = getTotalNota(notaAnterior);
            VentaNotas.Insert(indexNotaAnterior, notaAnterior);

            int indexNotaActual = GetIndexNota(numeroNotaActual);
            var notaActual = VentaNotas[indexNotaActual];
            VentaNotas.RemoveAt(indexNotaActual);
            notaActual.Total = getTotalNota(notaActual);
            VentaNotas.Insert(indexNotaActual, notaActual);
        }

        public int GetIndexProducto(string id)
        {
            return VentaProductos.Select((Producto, Index) => new { Producto, Index })
                        .First(item => item.Producto.Id == id)
                        .Index;
        }

        public int GetIndexNota(string numero)
        {
            return VentaNotas.Select((Nota, Index) => new { Nota, Index })
                        .First(item => item.Nota.NumeroNota == numero)
                        .Index;
        }
        
        public float getTotalNota(VentaNota ventaNota)
        {
            float descuento = 0;
            var totalProductos = (float)VentaProductos.Where(item => $"{item.NumeroNota}" == $"{ventaNota.NumeroNota}" && item.DeletedAt == null).Sum(item => item.Importe);
            if (totalProductos > 0 && Venta != null && Venta.DescuentoPorcentaje != null)
            {
                descuento = totalProductos * ((float)Venta.DescuentoPorcentaje / 100);
            }
            totalProductos -= descuento;
            return totalProductos;
        }

        /*public ObservableCollection<VentaNota> CalcularNotasTotales()
        {
            for (int i = 0; i < VentaNotas.Count; i++)
            {
                VentaNotas[i].Total = getTotalNota($"{VentaNotas[i].NumeroNota}");
            }
            return new ObservableCollection<VentaNota>(VentaNotas);
        }*/
    }
}
