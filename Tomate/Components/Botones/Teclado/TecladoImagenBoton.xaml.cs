using System;
using System.Windows;
using System.Windows.Controls;

namespace Tomate.Components.Botones.Teclado
{
    /// <summary>
    /// Interaction logic for TecladoImagenBoton.xaml
    /// </summary>
    public partial class TecladoImagenBoton : UserControl
    {
        public static readonly DependencyProperty ImagenProperty =
            DependencyProperty.Register("Imagen", typeof(string), typeof(TecladoImagenBoton),
                new PropertyMetadata(string.Empty));

        public string Imagen
        {
            get { return (string)GetValue(ImagenProperty); }
            set { SetValue(ImagenProperty, value); }
        }

        public static readonly DependencyProperty WidthImagenProperty =
          DependencyProperty.Register("WidthImagen", typeof(Double), typeof(TecladoImagenBoton),
              new PropertyMetadata(Double.MinValue));

        public Double WidthImagen
        {
            get { return (Double)GetValue(WidthImagenProperty); }
            set { SetValue(WidthImagenProperty, value); }
        }

        public static readonly DependencyProperty HeightImagenProperty =
         DependencyProperty.Register("HeightImagen", typeof(Double), typeof(TecladoImagenBoton),
             new PropertyMetadata(Double.MinValue));

        public Double HeightImagen
        {
            get { return (Double)GetValue(HeightImagenProperty); }
            set { SetValue(HeightImagenProperty, value); }
        }

        public static new readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(string), typeof(TecladoImagenBoton),
                new PropertyMetadata(string.Empty));

        public new string Width
        {
            get { return (string)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }

        }

        public static new readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(string), typeof(TecladoImagenBoton),
                new PropertyMetadata(string.Empty));

        public new string Height
        {
            get { return (string)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }

        }

        public static new readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register("Padding", typeof(string), typeof(TecladoImagenBoton),
                new PropertyMetadata("10"));

        public new string Padding
        {
            get { return (string)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }

        }

        public static new readonly DependencyProperty BackgroundProperty =
           DependencyProperty.Register("Background", typeof(string), typeof(TecladoImagenBoton),
               new PropertyMetadata("#ffffff"));

        public new string Background
        {
            get { return (string)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }

        }

        public static readonly RoutedEvent OnClickEvent = EventManager.RegisterRoutedEvent("OnClick", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(TecladoImagenBoton));

        public event RoutedEventHandler OnClick
        {
            add { AddHandler(OnClickEvent, value); }
            remove { RemoveHandler(OnClickEvent, value); }
        }

        public TecladoImagenBoton()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void On_Click_Button(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(OnClickEvent));
        }
    }
}
