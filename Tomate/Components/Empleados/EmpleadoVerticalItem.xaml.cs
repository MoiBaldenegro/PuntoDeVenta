using System.Windows;
using System.Windows.Controls;

namespace Tomate.Components.Empleados
{
    /// <summary>
    /// Interaction logic for EmpleadoVerticalItem.xaml
    /// </summary>
    public partial class EmpleadoVerticalItem : UserControl
    {

        public static readonly DependencyProperty IndexProperty =
          DependencyProperty.Register("Index", typeof(string), typeof(EmpleadoVerticalItem),
              new PropertyMetadata(null));

        public string Index
        {
            get { return (string)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }

        public static readonly DependencyProperty NombreProperty = DependencyProperty.Register(nameof(Nombre), typeof(string), typeof(EmpleadoVerticalItem),
        new PropertyMetadata(string.Empty));

        public string Nombre
        {
            get { return (string)GetValue(NombreProperty); }
            set { SetValue(NombreProperty, value); }

        }

        public static readonly DependencyProperty PerfilProperty = DependencyProperty.Register(nameof(Perfil), typeof(string), typeof(EmpleadoVerticalItem),
        new PropertyMetadata(string.Empty));

        public string Perfil
        {
            get { return (string)GetValue(PerfilProperty); }
            set { SetValue(PerfilProperty, value); }

        }

        public static new readonly DependencyProperty ForegroundProperty =
           DependencyProperty.Register("Foreground", typeof(string), typeof(EmpleadoVerticalItem),
               new PropertyMetadata("#333333"));

        public new string Foreground
        {
            get { return (string)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        public static readonly DependencyProperty SubForegroundProperty =
           DependencyProperty.Register("SubForeground", typeof(string), typeof(EmpleadoVerticalItem),
               new PropertyMetadata("#999999"));

        public string SubForeground
        {
            get { return (string)GetValue(SubForegroundProperty); }
            set { SetValue(SubForegroundProperty, value); }
        }

        //Evento click
        public static readonly RoutedEvent OnClickEvent = EventManager.RegisterRoutedEvent("OnClick", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(EmpleadoVerticalItem));

        public event RoutedEventHandler OnClick
        {
            add { AddHandler(OnClickEvent, value); }
            remove { RemoveHandler(OnClickEvent, value); }
        }


        public EmpleadoVerticalItem()
        {
            InitializeComponent();
        }
    }
}
