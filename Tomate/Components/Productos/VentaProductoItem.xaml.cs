using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tomate.Utils;

namespace Tomate.Components.Productos
{
    /// <summary>
    /// Interaction logic for VentaProductoItem.xaml
    /// </summary>
    public partial class VentaProductoItem : UserControl
    {

        public static readonly DependencyProperty IndexProperty =
          DependencyProperty.Register("Index", typeof(string), typeof(VentaProductoItem),
              new PropertyMetadata(null));

        public string Index
        {
            get { return (string)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }

        public static readonly DependencyProperty IdProperty =
           DependencyProperty.Register("Id", typeof(string), typeof(VentaProductoItem),
               new PropertyMetadata(null));

        public string Id
        {
            get { return (string)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        public static readonly DependencyProperty TerminalIdProperty =
           DependencyProperty.Register("TerminalId", typeof(string), typeof(VentaProductoItem),
               new PropertyMetadata(null));

        public string TerminalId
        {
            get { return (string)GetValue(TerminalIdProperty); }
            set { SetValue(TerminalIdProperty, value); }
        }

        public static readonly DependencyProperty VentaProductoPadreIdProperty =
           DependencyProperty.Register("VentaProductoPadreId", typeof(string), typeof(VentaProductoItem),
               new PropertyMetadata(null));

        public string VentaProductoPadreId
        {
            get { return (string)GetValue(VentaProductoPadreIdProperty); }
            set { SetValue(VentaProductoPadreIdProperty, value); }
        }

        public static readonly DependencyProperty ImpimirProperty =
           DependencyProperty.Register("Impimir", typeof(string), typeof(VentaProductoItem),
               new PropertyMetadata(null));

        public string Impimir
        {
            get { return (string)GetValue(ImpimirProperty); }
            set { SetValue(ImpimirProperty, value); }
        }



        public static readonly DependencyProperty CantidadProperty =
          DependencyProperty.Register("Cantidad", typeof(string), typeof(VentaProductoItem),
              new PropertyMetadata(string.Empty));

        public string Cantidad
        {
            get { return (string)GetValue(CantidadProperty); }
            set { SetValue(CantidadProperty, value); }
        }

        public static readonly DependencyProperty ImporteProperty =
          DependencyProperty.Register("Importe", typeof(string), typeof(VentaProductoItem),
              new PropertyMetadata(string.Empty));

        public string Importe
        {
            get { return (string)GetValue(ImporteProperty); }
            set { SetValue(ImporteProperty, value); }
        }

        public static readonly DependencyProperty SubTotalProperty =
          DependencyProperty.Register("SubTotal", typeof(string), typeof(VentaProductoItem),
              new PropertyMetadata(string.Empty));

        public string SubTotal
        {
            get { return (string)GetValue(SubTotalProperty); }
            set { SetValue(SubTotalProperty, value); }
        }

        public static readonly DependencyProperty NombreProperty =
          DependencyProperty.Register("Nombre", typeof(string), typeof(VentaProductoItem),
              new PropertyMetadata(string.Empty));

        public string Nombre
        {
            get { return (string)GetValue(NombreProperty); }
            set { SetValue(NombreProperty, value); }
        }


        public static readonly DependencyProperty NumeroOrdenProperty =
          DependencyProperty.Register("NumeroOrden", typeof(string), typeof(VentaProductoItem),
              new PropertyMetadata(string.Empty));

        public string NumeroOrden
        {
            get { return (string)GetValue(NumeroOrdenProperty); }
            set { SetValue(NumeroOrdenProperty, value); }
        }

        public static readonly DependencyProperty NumeroCuentaOrigenProperty =
          DependencyProperty.Register("NumeroCuentaOrigen", typeof(string), typeof(VentaProductoItem),
              new PropertyMetadata(null));

        public string NumeroCuentaOrigen
        {
            get { return (string)GetValue(NumeroCuentaOrigenProperty); }
            set { SetValue(NumeroCuentaOrigenProperty, value); }
        }
        

        public static readonly DependencyProperty NumeroNotaProperty =
          DependencyProperty.Register("NumeroNota", typeof(string), typeof(VentaProductoItem),
              new PropertyMetadata(null));

        public string NumeroNota
        {
            get { return (string)GetValue(NumeroNotaProperty); }
            set { SetValue(NumeroNotaProperty, value); }
        }

        public static readonly DependencyProperty ModificadoresProperty =
          DependencyProperty.Register("Modificadores", typeof(string), typeof(VentaProductoItem),
              new PropertyMetadata(null));

        public string? Modificadores
        {
            get
            {
                return (string)GetValue(ModificadoresProperty);
            }
            set
            {
                SetValue(ModificadoresProperty, value);
            }
        }

        public static readonly DependencyProperty HoraUsuarioProperty =
          DependencyProperty.Register("HoraUsuario", typeof(string), typeof(VentaProductoItem),
              new PropertyMetadata(null));

        public string? HoraUsuario
        {
            get
            {
                return (string)GetValue(HoraUsuarioProperty);
            }
            set
            {
                SetValue(HoraUsuarioProperty, value);
            }
        }

        public static new readonly DependencyProperty WidthProperty =
           DependencyProperty.Register("Width", typeof(string), typeof(VentaProductoItem),
               new PropertyMetadata(string.Empty));

        public new string Width
        {
            get { return (string)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }

        }

        public static new readonly DependencyProperty ForegroundProperty =
           DependencyProperty.Register("Foreground", typeof(string), typeof(VentaProductoItem),
               new PropertyMetadata("#333333"));

        public new string Foreground
        {
            get { return (string)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }

        }

        public static new readonly DependencyProperty BorderBrushProperty =
           DependencyProperty.Register("BorderBrush", typeof(string), typeof(VentaProductoItem),
               new PropertyMetadata("#999999"));

        public new string BorderBrush
        {
            get { return (string)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }

        }

        public static new readonly DependencyProperty OpacityProperty =
           DependencyProperty.Register("Opacity", typeof(string), typeof(VentaProductoItem),
               new PropertyMetadata("1"));

        public new string Opacity
        {
            get { return (string)GetValue(OpacityProperty); }
            set { SetValue(OpacityProperty, value); }

        }

        public static readonly RoutedEvent OnEditarCantidadEvent = EventManager.RegisterRoutedEvent("OnEditarCantidad", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(VentaProductoItem));

        public event RoutedEventHandler OnEditarCantidad
        {
            add { AddHandler(OnEditarCantidadEvent, value); }
            remove { RemoveHandler(OnEditarCantidadEvent, value); }
        }

        public static readonly RoutedEvent OnEliminarEvent = EventManager.RegisterRoutedEvent("OnEliminar", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(VentaProductoItem));

        public event RoutedEventHandler OnEliminar
        {
            add { AddHandler(OnEliminarEvent, value); }
            remove { RemoveHandler(OnEliminarEvent, value); }
        }

        public static readonly RoutedEvent OnClickEvent = EventManager.RegisterRoutedEvent("OnClick", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(VentaProductoItem));

        public event RoutedEventHandler OnClick
        {
            add { AddHandler(OnClickEvent, value); }
            remove { RemoveHandler(OnClickEvent, value); }
        }


        private bool EditarCantidad { get; set; } = false;

        public VentaProductoItem()
        {
            InitializeComponent();
        }

        private void EditarCantidad_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Index != null)
            {
                int index = int.Parse(Index);
                if (TerminalId == null && index > -1)
                {
                    EditarCantidad = true;
                    var args = new EventClickArgs(OnEditarCantidadEvent);
                    var extras = new Dictionary<string, object?>();
                    extras["Index"] = index;
                    extras["Id"] = Id;
                    args.Extras = extras;

                    RaiseEvent(args);
                }
                else
                {
                    EditarCantidad = false;
                    EditarEvento();
                }

            }
        }


        private void Item_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            EditarCantidad = false;
            EditarEvento();
        }

        Point? SwipeEliminar = null;
        private void RevisarSwipe(Point actual, Point prevPunto)
        {
            int index = int.Parse(Index);
            var extras = new Dictionary<string, object?>();
            extras["Index"] = index;
            extras["Id"] = Id;
            double diferenciaX = prevPunto.X - actual.X;
            double diferenciaY = prevPunto.Y - actual.Y;
            if ((diferenciaX > 1.5 || diferenciaX < -1.5) && (diferenciaY < 1.5 && diferenciaY > -1.5))
            {
                var args = new EventClickArgs(OnEliminarEvent);
                args.Extras = extras;
                RaiseEvent(args);
                EditarCantidad = false;
            }
            SwipeEliminar = null;
        }

        private void EditarEvento()
        {
            int index = int.Parse(Index);
            var extras = new Dictionary<string, object?>();
            extras["Index"] = index;
            extras["Id"] = Id;
            var args = new EventClickArgs(OnClickEvent);
            args.Extras = extras;
            if (!EditarCantidad)
            {
                RaiseEvent(args);
            }

        }

        private void Item_StylusSystemGesture(object sender, StylusSystemGestureEventArgs e)
        {
            EditarCantidad = false;
            if (e.SystemGesture == SystemGesture.Drag)
            {
                var points = e.GetStylusPoints(this);
                if (points.Count > 1)
                {
                    RevisarSwipe(points[points.Count - 1].ToPoint(), points[0].ToPoint());
                }
            }
        }

        /*private void BorderBackground_TouchUp(object sender, TouchEventArgs e)
        {
            Debug.WriteLine("Ssdsd");
        }

        

        private void BorderBackground_TouchDown(object sender, TouchEventArgs e)
        {
            Debug.WriteLine("Ssdsd");
        }*/


    }
}
