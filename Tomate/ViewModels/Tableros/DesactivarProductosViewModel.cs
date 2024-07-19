using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Tomate.Models.Usuarios;
using Tomate.Models.Catalogo;
using Tomate.Utils;

namespace Tomate.ViewModels.Tableros
{
    public class DesactivarProductosViewModel : ObservableObject
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

        private ObservableCollection<Producto> _ProductosDesactivados;

        public ObservableCollection<Producto> ProductosDesactivados
        {
            get { return _ProductosDesactivados; }
            set
            {
                _ProductosDesactivados = value;
                OnPropertyChanged();
            }
        }


        //id producto || desactivado
        private Dictionary<string, bool> _ProductosEstatus;
        public Dictionary<string, bool> ProductosEstatus
        {
            get { return _ProductosEstatus; }
            set
            {
                _ProductosEstatus = value;
            }
        }

        //id producto || activo
        private Dictionary<string, bool> _ProductosModificados;

        public Dictionary<string, bool> ProductosModificados
        {
            get { return _ProductosModificados; }
            set
            {
                _ProductosModificados = value;
            }
        }

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

        public int TableroSeleccionado = -1;


        public DesactivarProductosViewModel()
        {
            Tableros = new ObservableCollection<Tablero>();
            Productos = new ObservableCollection<Producto>();
            ProductosModificados = new Dictionary<string, bool>();
            ProductosEstatus = new Dictionary<string, bool>();
            ProductosDesactivados = new ObservableCollection<Producto>();
        }

        public ObservableCollection<Tablero> ResetTableros(int filas = 2)
        {
            var tablerosListados = new ObservableCollection<Tablero>();
            for (int i = 0; i < (8 * filas); i++)
            {
                tablerosListados.Add(new Tablero());
            }
            return tablerosListados;
        }

        public ObservableCollection<Producto> ResetProductos()
        {
            var productosListados = new ObservableCollection<Producto>();
            for (int i = 0; i < (8 * 8); i++)
            {
                productosListados.Add(new Producto());
            }
            return productosListados;
        }

        public void CargarProductosEstatus()
        {
            var productosTablero = TableroProducto.todos();
            foreach (var producto in productosTablero)
            {
                ProductosEstatus[$"{producto.Id}"] = producto.TableroActivo;
            }
        }

        public void CargarProductosDesactivados()
        {
            ProductosDesactivados = TableroProducto.desactivados();
        }

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
       
        public void  setTableroSeleccionado(int tableroIndex, bool recargar = false)
        {
            new Task(() =>
            {
                if (tableroIndex > -1)
                {
                    if (recargar || TableroSeleccionado != tableroIndex)
                    {
                        TableroSeleccionado = tableroIndex;
                        var productos = TableroProducto.todos(Tableros[tableroIndex].Id);

                        if (productos.Count > 0)
                        {
                            MensajeNoProductos = Visibility.Collapsed;
                        }
                        else
                        {
                            MensajeNoProductos = Visibility.Visible;
                        }

                        foreach (var producto in productos)
                        {
                            if (ProductosModificados.ContainsKey($"{producto.TableroProductoId}"))
                            {
                                producto.TableroActivo = ProductosModificados[$"{producto.TableroProductoId}"];
                            }
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


        public bool GetProductoActivo(string tableroProductoId)
        {
            if (!ProductosEstatus.ContainsKey(tableroProductoId))
            {
                var producto = TableroProducto.buscarId(tableroProductoId);
                if (producto == null)
                {
                    return false;
                }
                ProductosEstatus.Add(tableroProductoId, producto.TableroActivo);
            }
            
            return ProductosEstatus[tableroProductoId];
        }

    }
}
