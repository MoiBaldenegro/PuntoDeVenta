using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Tomate.Components.ViewModels.Venta;
using Tomate.Models.Ventas;
using Tomate.Models.Catalogo;
using Tomate.Utils;
using Tomate.Views;

namespace Tomate.Components.Dialogos.Venta
{
    /// <summary>
    /// Interaction logic for ProductoModificadoresModal.xaml
    /// </summary>
    public partial class ProductoModificadoresModal : UserControl
    {

        public Dictionary<string, object?>? Extras { get; set; } = new Dictionary<string, object?>();

        public ProductoModificadoresViewModel ViewModel { get; set; }

        private Action<Dictionary<string, object?>>? OnEnterAction;
        private Action<Dictionary<string, object?>>? OnCloseAction;
        public bool DesactivarBlur { get; set; } = true;

        public ProductoModificadoresModal()
        {
            ViewModel = new ProductoModificadoresViewModel();
            InitializeComponent();
            DataContext = ViewModel;
        }

        public void mostrar(VentaProducto ventaProducto, Action<Dictionary<string, object?>>? enter,
            Action<Dictionary<string, object?>>? cerrar)
        {
            GetModificadores($"{ventaProducto.Producto.CategoriaId}");
            setOnEnter(enter);
            setOnClose(cerrar);
            ViewModel.ModificadoresAgregados = new ObservableCollection<Modificador>();
            TituloModificadores.Visibility = Visibility.Visible;


            if (ventaProducto.Modificadores != null && ventaProducto.Modificadores.Length > 0)
            {
                foreach (string modificador in ventaProducto.Modificadores.Split("|"))
                {
                    AgregarModificador(new Modificador(modificador));
                }
            }

            DesactivarBlur = true;
            DialogoModificadores.IsOpen = true;

        }

        private void GetModificadores(string categoriaId)
        {
            ViewModel.Modificadores = Modificador.todos(categoriaId);
            var modificador = new Modificador("Otro");
            modificador.Id = null;
            ViewModel.Modificadores.Add(modificador);
        }

        public void setOnEnter(Action<Dictionary<string, object?>>? accion)
        {
            OnEnterAction = accion;
        }

        public void setOnClose(Action<Dictionary<string, object?>>? accion)
        {
            OnCloseAction = accion;
        }

        public void ocultar()
        {
            DialogoModificadores.IsOpen = false;
        }

        private void BotonCerrarModal_Click(object sender, RoutedEventArgs e)
        {
            ocultar();
        }

        private void DialogoTeclado_DialogOpened(object sender, DialogOpenedEventArgs eventArgs)
        {
            BackgroundDialogo.Visibility = Visibility.Visible;
            this.Dispatcher.Invoke(() =>
            {
                getWindow().AgregarBlur();
            });
        }

        private void DialogoTeclado_DialogClosed(object sender, DialogClosedEventArgs eventArgs)
        {
            BackgroundDialogo.Visibility = Visibility.Collapsed;
            if (Extras == null)
            {
                Extras = new Dictionary<string, object?>();
            }
            if (OnCloseAction != null)
            {
                OnCloseAction.Invoke(Extras);
            }
            if (DesactivarBlur)
            {
                this.Dispatcher.Invoke(() =>
                {
                    getWindow().RemoverBlur();
                });
            }
        }


        /**
         * Desactivar Feedback Boundary de touch
         */
        private void ScrollViewer_ManipulationBoundaryFeedback(object sender, System.Windows.Input.ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        /**
         * Obtiene la ventana principal del programa
         */
        private MainWindow getWindow()
        {
            return WindowUtils.getMainWindow();
        }

        private void AgregarModificador_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ModificadoresListado.SelectedIndex > -1)
            {
                Modificador modificador = ViewModel.Modificadores[ModificadoresListado.SelectedIndex];
                if (modificador.Id != null)
                {
                    AgregarModificador(modificador);
                }
                else
                {
                    AgregarModificadorOtro();
                }

            }
        }

        private void AgregarModificador(Modificador modificador)
        {
            if ($"{ModificadoresTexto()}{modificador.Nombre}".Length < 101)
            {
                ViewModel.ModificadoresAgregados.Add(modificador);
                if (ViewModel.ModificadoresAgregados.Count > 0)
                {
                    TituloModificadores.Visibility = Visibility.Collapsed;
                }

                ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(ModificadoresAgregadosListado, 0);
                scrollViewer.ScrollToBottom();
            }

        }

        private void RemoverModificador(int index)
        {
            if (index < ViewModel.ModificadoresAgregados.Count)
            {
                ViewModel.ModificadoresAgregados.RemoveAt(index);
            }

            if (ViewModel.ModificadoresAgregados.Count == 0)
            {
                TituloModificadores.Visibility = Visibility.Visible;
            }
        }

        private void RemoverModificador_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ModificadoresAgregadosListado.SelectedIndex > -1)
            {
                RemoverModificador(ModificadoresAgregadosListado.SelectedIndex);
            }
        }

        private void NuevoModificador_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AgregarModificadorOtro()
        {
            DialogoModificadores.IsOpen = false;
            //DesactivarBlur = false;

            this.Dispatcher.Invoke(() =>
            {
                var datos = new Dictionary<string, object?>();
                datos["Titulo"] = "Ingresa modificador";
                datos["Valor"] = null;
                datos["DesactivarBlur"] = false;

                getWindow().MostrarTeclado(datos, delegate (Dictionary<string, object?> respuesta)
                {
                    string Valor = $"{(string?)respuesta["Valor"]}";
                    AgregarModificador(new Modificador(Valor));
                    DialogoModificadores.IsOpen = true;
                }, delegate (Dictionary<string, object?> respuesta)
                {
                    DialogoModificadores.IsOpen = true;
                });
            });
        }

        private void GuardarModificadores_Click(object sender, RoutedEventArgs e)
        {
            EnterModificadores();
        }

        private string ModificadoresTexto()
        {
            string modificadores = "";
            if (ViewModel.ModificadoresAgregados.Count > 0)
            {
                modificadores = String.Join("|", ViewModel.ModificadoresAgregados.Select(item => item.Nombre).ToList());
            }
            return modificadores;
        }

        private void EnterModificadores()
        {
            if (Extras == null)
            {
                Extras = new Dictionary<string, object?>();
            }

            string? modificadores = ModificadoresTexto();

            if (modificadores.Length == 0)
            {
                modificadores = null;
            }

            Extras["Valor"] = modificadores;

            ocultar();
            if (OnEnterAction != null)
            {
                OnEnterAction.Invoke(Extras);
            }
        }

        private void EliminarModificadores_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ModificadoresAgregados.Clear();
        }

        private void AgregarEspacio_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.ModificadoresAgregados.Count > 0)
            {
                if (ViewModel.ModificadoresAgregados[ViewModel.ModificadoresAgregados.Count - 1].Nombre != "\n")
                {
                    AgregarModificador(new Modificador("\n"));
                }
            }

        }
    }
}
