using System;
using System.Windows;
using System.Windows.Controls;
using Tomate.Components.Botones.Teclado;

namespace Tomate.Components.Botones.Menu
{
    /// <summary>
    /// Interaction logic for BotonMenuImagen.xaml
    /// </summary>
    public partial class BotonMenuImagen : UserControl
    {

        public static readonly DependencyProperty ImagenProperty =
            DependencyProperty.Register("Imagen", typeof(string), typeof(BotonMenuImagen),
                new PropertyMetadata(string.Empty));

        public string Imagen
        {
            get { return (string)GetValue(ImagenProperty); }
            set { SetValue(ImagenProperty, value); }
        }

        public static readonly DependencyProperty WidthImagenProperty =
          DependencyProperty.Register("WidthImagen", typeof(Double), typeof(BotonMenuImagen),
              new PropertyMetadata(Double.MinValue));

        public Double WidthImagen
        {
            get { return (Double)GetValue(WidthImagenProperty); }
            set { SetValue(WidthImagenProperty, value); }
        }

        public static readonly DependencyProperty HeightImagenProperty =
         DependencyProperty.Register("HeightImagen", typeof(Double), typeof(BotonMenuImagen),
             new PropertyMetadata(Double.MinValue));

        public Double HeightImagen
        {
            get { return (Double)GetValue(HeightImagenProperty); }
            set { SetValue(HeightImagenProperty, value); }
        }

        public static readonly DependencyProperty TituloProperty =
            DependencyProperty.Register("Titulo", typeof(string), typeof(BotonMenuImagen),
                new PropertyMetadata(string.Empty));

        public string Titulo
        {
            get { return (string)GetValue(TituloProperty); }
            set { SetValue(TituloProperty, value); }
        }

        public static readonly DependencyProperty IconoProperty =
            DependencyProperty.Register("Icono", typeof(string), typeof(BotonMenuImagen),
                new PropertyMetadata(string.Empty));

        public string Icono
        {
            get { return (string)GetValue(IconoProperty); }
            set { SetValue(IconoProperty, value); }
        }

        public static new readonly DependencyProperty BackgroundProperty =
           DependencyProperty.Register("Background", typeof(string), typeof(BotonMenuImagen),
               new PropertyMetadata("#FFFFFF"));

        public new string Background
        {
            get { return (string)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }

        }

        public static new readonly DependencyProperty ForegroundProperty =
           DependencyProperty.Register("Foreground", typeof(string), typeof(BotonMenuImagen),
               new PropertyMetadata("#333333"));

        public new string Foreground
        {
            get { return (string)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }

        }

        public static readonly RoutedEvent OnClickEvent = EventManager.RegisterRoutedEvent("OnClick", RoutingStrategy.Bubble, 
            typeof(RoutedEventHandler), typeof(BotonMenuImagen));

        public event RoutedEventHandler OnClick
        {
            add { AddHandler(OnClickEvent, value); }
            remove { RemoveHandler(OnClickEvent, value); }
        }

        public BotonMenuImagen()
        {
            InitializeComponent();
        }

        private void On_Click_Button(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(OnClickEvent));
        }

    }
}
