using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Tomate.Utils;

namespace Tomate.Components.Botones.Teclado
{
    /// <summary>
    /// Interaction logic for BotonNumero.xaml
    /// </summary>
    public partial class TecladoVerticalBoton : UserControl
    {

        public static readonly DependencyProperty TituloProperty =
            DependencyProperty.Register("Titulo", typeof(string), typeof(TecladoVerticalBoton),
                new PropertyMetadata(string.Empty));

        public string Titulo
        {
            get { return (string)GetValue(TituloProperty); }
            set { SetValue(TituloProperty, value); }
        }

        public static readonly RoutedEvent OnClickEvent = EventManager.RegisterRoutedEvent("OnClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TecladoVerticalBoton));

        public event RoutedEventHandler OnClick
        {
            add { AddHandler(OnClickEvent, value); }
            remove { RemoveHandler(OnClickEvent, value); }
        }

        public static readonly DependencyProperty ValorClickProperty =
            DependencyProperty.Register("ValorClick", typeof(object), typeof(TecladoVerticalBoton),
                new PropertyMetadata(null));

        public object ValorClick
        {
            get { return (object)GetValue(ValorClickProperty); }
            set { SetValue(ValorClickProperty, value); }
        }

        public TecladoVerticalBoton()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void On_Click_Button(object sender, RoutedEventArgs e)
        {
            var args = new EventClickArgs(OnClickEvent);
            var extras = new Dictionary<string, object>();
            extras["Valor"] = ValorClick;
            args.Extras = extras;

            RaiseEvent(args);
        }
    }
}
