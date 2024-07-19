using System;
using Tomate.Utils;

namespace Tomate.Components.ViewModels.Teclado
{
    public class TableroViewModel : ObservableObject
    {


        private string _MensajeTeclado;
        public string MensajeTeclado
        {
            get { return _MensajeTeclado; }
            set
            {
                _MensajeTeclado = value;
                OnPropertyChanged();
            }
        }

        private string _TituloTeclado = "Ingresa un texto";
        public string TituloTeclado
        {
            get { return _TituloTeclado; }
            set
            {
                _TituloTeclado = value;
                OnPropertyChanged();
            }
        }

        private string[] _Letras;
        public string[] Letras
        {
            get { return _Letras; }
            set
            {
                _Letras = value;
                OnPropertyChanged();
            }
        }

        public bool Mayusculas { get; set; } = true;

        public TableroViewModel()
        {
            SetLetras(Mayusculas);
        }

        public void SetLetras(bool mayusculas)
        {
            Mayusculas = mayusculas;
            string[] letras = new string[] {
                "q", "w", "e", "r", "t", "y", "u", "i", "o", "p",
                "a", "s", "d", "f", "g", "h", "j", "k", "l", "ñ",
                "z", "x", "c", "v", "b", "n", "m"
            };

            if (Mayusculas)
            {
                letras = Array.ConvertAll(letras, item => item.ToUpper());
            }

            Letras = letras;

        }

    }
}
