using System;
using System.Globalization;
using Tomate.Utils;

namespace Tomate.Components.ViewModels.Teclado
{
    public class TableroCobroViewModel : ObservableObject
    {

        private string _MensajeTeclado;
        public string MensajeTeclado
        {
            get
            {
                return _MensajeTeclado;
            }
            set
            {
                _MensajeTeclado = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(MensajeTecladoFormato));
            }
        }

        public string MensajeTecladoFormato
        {
            get
            {
                string mensajeCustom = "0";
                if (_MensajeTeclado != null && _MensajeTeclado.Length > 0)
                {

                    try
                    {
                        
                        var descimales = _MensajeTeclado.Split('.');
                        float cantidad = float.Parse(descimales[0]);
                        mensajeCustom = cantidad.ToString("#,##0", CultureInfo.CurrentCulture);
                        if (descimales.Length > 1)
                        {
                            mensajeCustom = $"{mensajeCustom}.{descimales[1]}";
                        }
                    }
                    catch (Exception)
                    {

                    }

                }
                mensajeCustom = $"${mensajeCustom}";
                return mensajeCustom;
            }
        }


        private string _TituloTeclado = "Ingresa un número";
        public string? TituloTeclado
        {
            get { return _TituloTeclado; }
            set
            {
                _TituloTeclado = value;
                OnPropertyChanged();
            }
        }

       
        public TableroCobroViewModel()
        {

        }

    }
}
