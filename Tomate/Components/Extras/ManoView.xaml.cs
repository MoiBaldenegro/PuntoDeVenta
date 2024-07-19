using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Tomate.Utils;

namespace Tomate.Components.Extras
{
    /// <summary>
    /// Interaction logic for ManoView.xaml
    /// </summary>
    /// 

    public class ManoViewViewModel : ObservableObject
    {

        private string[] _Colores = new string[10];

        public string[] Colores
        {
            get
            {
                return _Colores;
            }
            set
            {
                _Colores = value;
                OnPropertyChanged();
            }
        }

        private string[] _ColoresHover = new string[10];

        public string[] ColoresHover
        {
            get
            {
                return _ColoresHover;
            }
            set
            {
                _ColoresHover = value;
                OnPropertyChanged();
            }
        }




    }

    public partial class ManoView : UserControl
    {

        private ManoViewViewModel ViewModel { get; set; }

        public static readonly RoutedEvent OnSeleccionarDedoEvent = EventManager.RegisterRoutedEvent("OnSeleccionarDedo", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(ManoView));

        public event RoutedEventHandler OnSeleccionarDedo
        {
            add { AddHandler(OnSeleccionarDedoEvent, value); }
            remove { RemoveHandler(OnSeleccionarDedoEvent, value); }
        }


        public ManoView()
        {
            ViewModel = new ManoViewViewModel();
            InitializeComponent();
            DataContext = ViewModel;
            RestablecerDedos();
        }

        private void RestablecerDedos()
        {
            var _Colores = new string[10];
            _Colores[0] = "#e8eae9";
            _Colores[1] = "#e8eae9";
            _Colores[2] = "#e8eae9";
            _Colores[3] = "#e8eae9";
            _Colores[4] = "#e8eae9";
            _Colores[5] = "#e8eae9";
            _Colores[6] = "#e8eae9";
            _Colores[7] = "#e8eae9";
            _Colores[8] = "#e8eae9";
            _Colores[9] = "#e8eae9";

            var _ColoresHover = new string[10];
            _ColoresHover[0] = "#fc5656";
            _ColoresHover[1] = "#fc5656";
            _ColoresHover[2] = "#fc5656";
            _ColoresHover[3] = "#fc5656";
            _ColoresHover[4] = "#fc5656";
            _ColoresHover[5] = "#fc5656";
            _ColoresHover[6] = "#fc5656";
            _ColoresHover[7] = "#fc5656";
            _ColoresHover[8] = "#fc5656";
            _ColoresHover[9] = "#fc5656";

            ViewModel.Colores = _Colores;
            ViewModel.ColoresHover = _ColoresHover;
        }

        public void SetDedosAgregados(int[] dedosAgregados)
        {
            RestablecerDedos();
            foreach (int index in dedosAgregados)
            {
                ViewModel.Colores[index - 1] = "#9597a5";
                ViewModel.ColoresHover[index - 1] = "#9597a5";
            }
            ViewModel.Colores = ViewModel.Colores;
            ViewModel.ColoresHover = ViewModel.ColoresHover;
        }

        public void SetModificarHuella(int numero)
        {
            ViewModel.Colores[numero - 1] = "#fc5656";
            ViewModel.ColoresHover[numero - 1] = "#fc5656";
            ViewModel.Colores = ViewModel.Colores;
            ViewModel.ColoresHover = ViewModel.ColoresHover;

        }

        private void EditarDedo_Click(object sender, RoutedEventArgs e)
        {
            var args = new EventClickArgs(OnSeleccionarDedoEvent);
            var Extras = new Dictionary<string, object?>();
            Extras["NumeroDedo"] = int.Parse($"{((Button)sender).Tag}");
            args.Extras = Extras;
            RaiseEvent(args);
        }
    }
}
