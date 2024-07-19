using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tomate.Models.Catalogo;
using Tomate.Utils;

namespace Tomate.Components.Productos
{
    /// <summary>
    /// Interaction logic for ProductoItem.xaml
    /// </summary>
    public partial class ProductoItem : UserControl
    {
        public static readonly DependencyProperty IndexProperty =
          DependencyProperty.Register("Index", typeof(string), typeof(ProductoItem),
              new PropertyMetadata(null));

        public string Index
        {
            get { return (string)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }

        public static readonly DependencyProperty NombreProperty = DependencyProperty.Register(nameof(Nombre), typeof(string), typeof(ProductoItem),
        new PropertyMetadata(string.Empty));


        public string Nombre
        {
            get { return (string)GetValue(NombreProperty); }
            set { SetValue(NombreProperty, value); }
        }


        public static readonly DependencyProperty OpacityProperty = DependencyProperty.Register(nameof(Opacity), typeof(string), typeof(ProductoItem),
        new PropertyMetadata("1"));


        public string Opacity
        {
            get { return (string)GetValue(OpacityProperty); }
            set { SetValue(OpacityProperty, value); }
        }


        public static readonly DependencyProperty PrecioProperty = DependencyProperty.Register(nameof(Precio), typeof(string), typeof(ProductoItem),
        new PropertyMetadata(string.Empty));


        public string Precio
        {
            get
            {
                return (string)GetValue(PrecioProperty);
            }
            set
            {
                SetValue(PrecioProperty, value);
            }

        }

        public static readonly DependencyProperty ProhibidoProperty = DependencyProperty.Register(nameof(Prohibido), typeof(string), typeof(ProductoItem),
        new PropertyMetadata(string.Empty));


        public string Prohibido
        {
            get
            {
                return (string)GetValue(ProhibidoProperty);
            }
            set
            {
                SetValue(ProhibidoProperty, value);
            }
        }
        

        public static new readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(string), typeof(ProductoItem),
                new PropertyMetadata(string.Empty));

        public new string Height
        {
            get { return (string)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }

        }

        public static readonly DependencyProperty ProductoProperty =
            DependencyProperty.Register("Producto", typeof(Producto), typeof(ProductoItem),
                new PropertyMetadata(null));

        public Producto Producto
        {
            get { return (Producto)GetValue(ProductoProperty); }
            set { SetValue(ProductoProperty, value); }

        }

        //Evento click
        public static readonly RoutedEvent OnClickEvent = EventManager.RegisterRoutedEvent("OnClick", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(ProductoItem));

        public event RoutedEventHandler OnClick
        {
            add { AddHandler(OnClickEvent, value); }
            remove { RemoveHandler(OnClickEvent, value); }
        }

        public ProductoItem()
        {
            InitializeComponent();
        }

        private void Agregar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            int index = int.Parse(Index);
            if (index > -1 && !string.IsNullOrEmpty(Nombre))
            {
                var args = new EventClickArgs(OnClickEvent);
                var extras = new Dictionary<string, object>();
                extras["Index"] = index;
                extras["Producto"] = Producto;
                args.Extras = extras;

                RaiseEvent(args);
            }


        }
    }
}
