using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Tomate.Utils;

namespace Tomate.Components.Botones.Teclado
{
    /// <summary>
    /// Interaction logic for TecladoBoton.xaml
    /// </summary>
    /// 


    public partial class TecladoBoton : UserControl
    {

        public static readonly DependencyProperty TituloProperty = DependencyProperty.Register(nameof(Titulo), typeof(string), typeof(TecladoBoton),
        new PropertyMetadata(string.Empty));


        public string Titulo
        {
            get
            {
                return (string)GetValue(TituloProperty);
            }
            set
            {
                SetValue(TituloProperty, value);
            }

        }

        public static readonly DependencyProperty ValorClickProperty =
           DependencyProperty.Register("ValorClick", typeof(object), typeof(TecladoBoton),
               new PropertyMetadata(null));

        public object ValorClick
        {
            get { return (object)GetValue(ValorClickProperty); }
            set { SetValue(ValorClickProperty, value); }
        }


        public static new readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(string), typeof(TecladoBoton),
                new PropertyMetadata(string.Empty));

        public new string Width
        {
            get { return (string)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }

        }

        public static new readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(string), typeof(TecladoBoton),
                new PropertyMetadata(string.Empty));

        public new string Height
        {
            get { return (string)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }

        }

        public static new readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register("Padding", typeof(string), typeof(TecladoBoton),
                new PropertyMetadata("10"));

        public new string Padding
        {
            get { return (string)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }

        }

        public static new readonly DependencyProperty BackgroundProperty =
           DependencyProperty.Register("Background", typeof(string), typeof(TecladoBoton),
               new PropertyMetadata("#FFFFFF"));

        public new string Background
        {
            get { return (string)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }

        }

        public static new readonly DependencyProperty ForegroundProperty =
           DependencyProperty.Register("Foreground", typeof(string), typeof(TecladoBoton),
               new PropertyMetadata("#333333"));

        public new string Foreground
        {
            get { return (string)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }

        }

        public static new readonly DependencyProperty FontSizeProperty =
           DependencyProperty.Register("FontSize", typeof(string), typeof(TecladoBoton),
               new PropertyMetadata("40"));

        public new string FontSize
        {
            get { return (string)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }

        }


        //Evento click
        public static readonly RoutedEvent OnClickEvent = EventManager.RegisterRoutedEvent("OnClick", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(TecladoBoton));

        public event RoutedEventHandler OnClick
        {
            add { AddHandler(OnClickEvent, value); }
            remove { RemoveHandler(OnClickEvent, value); }
        }


        public TecladoBoton()
        {
            InitializeComponent();
        }


        private void On_Click_Button(object sender, RoutedEventArgs e)
        {
            var args = new EventClickArgs(OnClickEvent);
            var extras = new Dictionary<string, object?>();
            extras["Valor"] = ValorClick;
            args.Extras = extras;

            RaiseEvent(args);
        }

    }
}
