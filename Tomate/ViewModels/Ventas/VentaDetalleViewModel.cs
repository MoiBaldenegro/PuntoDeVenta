using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Tomate.Models.Ventas;
using Tomate.Models.Usuarios;
using Tomate.Models.Catalogo;
using Tomate.Utils;
using Tomate.Models.Caja;

namespace Tomate.ViewModels.Ventas
{
    public class VentaDetalleViewModel : ObservableObject
    {

        /*---------------- VARIABLES GENERALES -------------*/

        //USUARIO QUE ABRIO LA SECCION
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

        public CajaCorte? CajaCorte { get; set; } = null;

        /*---------------- VARIABLES COMPONENTES VISUALES -------------*/

        //TOTAL BOTONES BOTTOM
        private int _TotalBotones = 11;
        public int TotalBotones
        {
            get { return _TotalBotones; }
            set
            {
                _TotalBotones = value;
                OnPropertyChanged();
            }
        }

        //TEXTO BOTON ENVIAR/COBRAR
        private string _TextoEnviar = "Enviar";
        public string TextoEnviar
        {
            get { return _TextoEnviar; }
            set
            {
                _TextoEnviar = value;
                OnPropertyChanged();
            }
        }
        
        //CANTIDAD PRODUCTOS A AGREGAR
        private int _CantidadProducto = 1;
        public int CantidadProducto
        {
            get { return _CantidadProducto; }
            set
            {
                _CantidadProducto = value;
                OnPropertyChanged();
            }
        }

        //MUESTRA MENSAJE NO HAY PRODUCTOS
        private Visibility _MensajeNoProductos = Visibility.Visible;
        public Visibility MensajeNoProductos
        {
            get { return _MensajeNoProductos; }
            set
            {
                _MensajeNoProductos = value;
                OnPropertyChanged();
            }
        }

        /*---------------- VARIABLES TABLERO -------------*/

        public int TableroSeleccionado = -1;

        //TABLEROS INFERIORES
        private ObservableCollection<Tablero> _Tableros;
        public ObservableCollection<Tablero> Tableros
        {
            get { return _Tableros; }
            set
            {
                _Tableros = value;
                OnPropertyChanged();
            }
        }

        //LISTADO DE PRODUCTOS POR TABLERO
        private ObservableCollection<Producto> _Productos;
        public ObservableCollection<Producto> Productos
        {
            get { return _Productos; }
            set
            {
                _Productos = value;
                OnPropertyChanged();
            }
        }


        /*---------------- VARIABLES VENTAS -------------*/
        
        //VENTA ACTUAL
        private Venta _Venta;
        public Venta Venta
        {
            get { return _Venta; }
            set
            {
                _Venta = value;
                OnPropertyChanged();
            }
        }

        //PRODUCTOS VENTA ACTUAL
        private ObservableCollection<VentaProducto> _VentaProductos;
        public ObservableCollection<VentaProducto> VentaProductos
        {
            get { return _VentaProductos; }
            set
            {
                _VentaProductos = value;
                CalcularVentaTotal();
                OnPropertyChanged();
            }
        }

        //NOTAS VENTA ACTUAL
        private ObservableCollection<VentaNota> _VentaNotas;
        public ObservableCollection<VentaNota> VentaNotas
        {
            get { return _VentaNotas; }
            set
            {
                _VentaNotas = value;
                OnPropertyChanged();
            }
        }



        /*---------------- CONSTRUCTORES -------------*/
        public VentaDetalleViewModel()
        {
            VentaProductos = new ObservableCollection<VentaProducto>();
            Tableros = new ObservableCollection<Tablero>();
            Productos = new ObservableCollection<Producto>();
            VentaNotas = new ObservableCollection<VentaNota>();
        }

        /*---------------- FUNCIONES TABLEROS -------------*/

        /**
         * Limpia los tablero
         */
        public ObservableCollection<Tablero> ResetTableros(int filas = 2)
        {
            var tablerosListados = new ObservableCollection<Tablero>();
            for (int i = 0; i < (8 * filas); i++)
            {
                tablerosListados.Add(new Tablero());
            }
            return tablerosListados;
        }

        /**
         * Limpia los productos del tablero 8*8
         */
        public ObservableCollection<Producto> ResetProductos()
        {
            var productosListados = new ObservableCollection<Producto>();
            for (int i = 0; i < (8 * 8); i++)
            {
                productosListados.Add(new Producto());
            }
            return productosListados;
        }

        /**
         * Carga los productos por tablero
         */
        public void CargarTableros(bool recargar = false)
        {
            var tablerosListado = Tablero.todos();
            int filas = 1;
            if (tablerosListado.Count > 8)
            {
                filas = 2;
            }
            var listado = ResetTableros(filas);

            foreach (var tablero in tablerosListado)
            {
                int x = (int)tablero.PosX - 1;
                int y = (int)tablero.PosY - 1;
                int row = y * 8;
                int index = row + x;
                listado[index] = tablero;
            }
            Tableros = listado;
            setTableroSeleccionado(0, recargar);
        }

        /**
         * Muestra productos por tablero index
         */
        public void setTableroSeleccionado(int tableroIndex, bool recargar = false)
        {
            new Task(() =>
            {
                if (tableroIndex > -1)
                {
                    if (recargar || TableroSeleccionado != tableroIndex)
                    {
                        TableroSeleccionado = tableroIndex;
                        var productos = TableroProducto.todos(Tableros[tableroIndex].Id, Venta.ListaPreciosId);

                        if (productos.Count > 0)
                        {
                            MensajeNoProductos = Visibility.Collapsed;
                        }
                        else
                        {
                            MensajeNoProductos = Visibility.Visible;
                        }


                        var listado = ResetProductos();

                        foreach (var producto in productos)
                        {
                            int x = (int)producto.PosX - 1;
                            int y = (int)producto.PosY - 1;
                            int row = y * 8;
                            int index = row + x;
                            listado[index] = producto;
                        }

                        Productos = listado;
                    }
                }
                else
                {
                    Productos = ResetProductos();
                    MensajeNoProductos = Visibility.Collapsed;
                }

            }).Start();
        }


        /*---------------- FUNCIONES VENTAS -------------*/

        /**
         * Calcula el total de la venta
         */
        public void CalcularVentaTotal()
        {
            if (Venta == null)
            {
                return;
            }
            float? total = VentaProductos.Where(item => item.DeletedAt == null).Sum(item => item.Importe);
            Venta.SubTotal = total;
            Venta.DescuentoProductos = VentaProductos.Where(item => item.DeletedAt == null).Sum(item => item.Descuento);
            if (Venta.DescuentoPorcentaje > 0)
            {
                Venta.Descuento = Venta.SubTotal * (Venta.DescuentoPorcentaje / 100);
            }
            Venta.SubTotal += Venta.DescuentoProductos;
            Venta.Total = Venta.SubTotal - Venta.Descuento - Venta.DescuentoProductos;
            Venta = Venta;
            ActualizarTotalNotas();
        }

        /**
         * Obtiene los productos enviados previamente
         */
        public ObservableCollection<VentaProducto> GetProductosEnviados()
        {
            return new ObservableCollection<VentaProducto>(VentaProductos.Where(item => item.TerminalId != null).ToList());
        }

        /**
         * Borra los productos no enviados de la compra
         */
        public void RestablecerProductosVenta()
        {
            if (getProductosAgregados().Count > 0)
            {
                VentaProductos = GetProductosEnviados();
            }
        }

        /**
         * Obtiene los complementos de un producto
         */
        public ObservableCollection<VentaProducto> getComplementos(VentaProducto ventaProducto)
        {
            return new ObservableCollection<VentaProducto>(VentaProductos.Where(item => item.VentaProductoPadreId == ventaProducto.Id).ToList());
        }

        /**
         * Borra un complemento de producto
         */
        public void RemoverComplementos(int index)
        {
            var productoPadre = VentaProductos[index];
            var complementos = getComplementos(productoPadre);
            if (complementos.Count > 0)
            {
                foreach (var complemento in complementos)
                {
                    VentaProductos.Remove(complemento);
                }
            }
        }

        /**
         * Agregar un producto a una posicion especifica si se asigna
         */
        public void AgregarProducto(VentaProducto producto, int? index = null, bool extra = false)
        {
            if (producto.NumeroNota == null)
            {
                producto.NumeroNota = 1;
            }
            if (producto.Id == null)
            {
                producto.Id = Guid.NewGuid().ToString();
            }

            if (extra)
            {
                if (producto.VentaProductoPadreId == null)
                {
                    producto.VentaProductoPadreId = VentaProductos[(int)index].Id;
                    producto.NumeroNota = VentaProductos[(int)index].NumeroNota;
                }
                if (index == VentaProductos.Count - 1)
                {
                    index = null;
                }
                else
                {
                    for (int i = (int)index + 1; i < VentaProductos.Count; i++)
                    {
                        if (VentaProductos[i].Producto.CategoriaId != null)
                        {
                            break;
                        }
                        if (VentaProductos[i].Nombre == null)
                        {
                            break;
                        }
                        index = i;
                    }
                    index += 1;
                }
            }
            if (index == null)
            {
                VentaProductos.Add(producto);
            }
            else
            {
                VentaProductos.Insert((int)index, producto);
            }
            CalcularVentaTotal();
        }


        public void ActualizarVentaDetalle()
        {
            OnPropertyChanged(nameof(Venta));
        }

        /**
         * Actualiza la informacion de la venta, incluyendo productos y notas
         */
        public Venta ActualizarVentaDetalle(JObject ventaJson)
        {

            Venta = new Venta((JObject?)ventaJson["venta"]);
            if (Venta.Productos.Count > 0)
            {
                VentaProductos = new ObservableCollection<VentaProducto>(Venta.Productos.Union(getProductosAgregados()).ToList());
            }
            VentaNotas = new ObservableCollection<VentaNota>(Venta.Notas);
            CalcularVentaTotal();
            return Venta;
        }

        /**
         * Actualiza los productos agregando los que se van a enviar
         */
        public void SetVentaProductos(ObservableCollection<VentaProducto> productos)
        {

            VentaProductos = new ObservableCollection<VentaProducto>(productos.Union(getProductosAgregados()).ToList());
            CalcularVentaTotal();
        }

        /**
         * Actualiza la venta y calcula totales
         */
        public void SetVenta(Venta venta)
        {
            Venta = venta;
            CalcularVentaTotal();
        }

        /**
         * Obtiene el listado de productos agregados
         */
        public List<VentaProducto> getProductosAgregados()
        {
            return new List<VentaProducto>(VentaProductos.Where(item => item.TerminalId == null && item.ProductoId != null).ToList());
        }

        /**
         * Agrega modificadores a un producto sin enviar
         */
        public void SetModificadores(int index, string? modificadores)
        {
            VentaProductos[index].Modificadores = modificadores;
            NotificarActualizarProducto(index);
        }

        /**
         * Actualiza el listado de productos visualmente
         */
        public void NotificarActualizarProducto(int index)
        {
            var producto = VentaProductos[index];
            VentaProductos.RemoveAt(index);
            VentaProductos.Insert(index, producto);
        }

        /**
         * Obtiene el index de un producto 
         */
        public int GetIndexVentaProductos(string id)
        {
            return VentaProductos.Select((Producto, Index) => new { Producto, Index })
                        .First(item => item.Producto.Id == id)
                        .Index;
        }

        /**
         * Actualiza un producto agregado previamente
         */
        public void ReemplazarProducto(VentaProducto producto)
        {
            var Index = GetIndexVentaProductos($"{producto.Id}");
            VentaProductos.RemoveAt(Index);
            VentaProductos.Insert(Index, producto);
            OnPropertyChanged(nameof(VentaProductos));
        }

        public void ActualizarTotalNotas()
        {
            int index = 0;
            foreach (var nota in VentaNotas)
            {
                VentaNotas[index].Total = getTotalNota(nota);
                index++;
            }
            OnPropertyChanged(nameof(VentaNotas));
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

    }
}
