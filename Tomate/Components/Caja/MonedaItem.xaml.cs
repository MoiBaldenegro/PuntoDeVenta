using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tomate.Components.Productos;
using Tomate.Models;
using Tomate.Utils;

namespace Tomate.Components.Caja
{
    /// <summary>
    /// Interaction logic for MonedaItem.xaml
    /// </summary>
    public partial class MonedaItem : UserControl
    {
        public static readonly DependencyProperty IndexProperty =
          DependencyProperty.Register("Index", typeof(string), typeof(MonedaItem),
              new PropertyMetadata(null));

        public string Index
        {
            get { return (string)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }

        public static readonly DependencyProperty CantidadProperty = DependencyProperty.Register(nameof(Cantidad), typeof(string), typeof(MonedaItem),
        new PropertyMetadata("0"));


        public string? Cantidad
        {
            get { return (string)GetValue(CantidadProperty); }
            set { SetValue(CantidadProperty, value); }
        }

        public static readonly DependencyProperty ValorProperty = DependencyProperty.Register(nameof(Valor), typeof(string), typeof(MonedaItem),
        new PropertyMetadata("0"));


        public string Valor
        {
            get
            {
                return (string)GetValue(ValorProperty);
            }
            set
            {
                SetValue(ValorProperty, value);
            }

        }

        public static readonly DependencyProperty ImporteProperty = DependencyProperty.Register(nameof(Importe), typeof(string), typeof(MonedaItem),
        new PropertyMetadata(string.Empty));


        public string Importe
        {
            get
            {
                return (string)GetValue(ImporteProperty);
            }
            set
            {
                SetValue(ImporteProperty, value);
            }

        }

        public static new readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(string), typeof(MonedaItem),
                new PropertyMetadata("45"));

        public new string Height
        {
            get { return (string)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }

        }

        public static new readonly DependencyProperty ForegroundProperty =
           DependencyProperty.Register("Foreground", typeof(string), typeof(MonedaItem),
               new PropertyMetadata("#999"));

        public new string Foreground
        {
            get { return (string)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }

        }

        public static new readonly DependencyProperty BorderBrushProperty =
           DependencyProperty.Register("BorderBrush", typeof(string), typeof(MonedaItem),
               new PropertyMetadata("#777"));

        public new string BorderBrush
        {
            get { return (string)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }

        }

        public static new readonly DependencyProperty BorderThicknessProperty =
           DependencyProperty.Register("BorderThickness", typeof(string), typeof(MonedaItem),
               new PropertyMetadata("0,0,0,1"));

        public new string BorderThickness
        {
            get { return (string)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }

        }

        

        public static readonly RoutedEvent OnActualizarEvent = EventManager.RegisterRoutedEvent("OnActualizar", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(MonedaItem));

        public event RoutedEventHandler OnActualizar
        {
            add { AddHandler(OnActualizarEvent, value); }
            remove { RemoveHandler(OnActualizarEvent, value); }
        }

        public MonedaItem()
        {
            InitializeComponent();
        }

    }
}
