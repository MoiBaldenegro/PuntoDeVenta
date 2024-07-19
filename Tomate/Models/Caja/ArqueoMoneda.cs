using System.Globalization;

namespace Tomate.Models.Caja
{
    public class ArqueoMoneda
    {

        public string? Usuario { get; set; }

        public string? Descripcion { get; set; }

        public string? Tipo { get; set; }

        public string? _Cantidad;
        public string? Cantidad { 
            get 
            {
                return _Cantidad;
            } 
            set 
            {
                _Cantidad = value;
                if (Cantidad != null && Cantidad.Length > 0)
                {
                    Foreground = "#333";
                }
                else
                {
                    Foreground = "#999";
                }
            } 
        }


        public string CantidadFormato
        {
            get
            {
                return Cantidad ?? "0";
            }
        }

        public float Valor { get; set; } = 0;

        public int Index { get; set; } = -1;

        private bool _IsFocus = false;
        public bool IsFocus
        {
            get
            {
                return _IsFocus;
            }
            set
            {
                _IsFocus = value;
                if (_IsFocus)
                {
                    BorderBrush = "#FC5656";
                    BorderThickness = "0,0,0,2";
                }
                else
                {
                    BorderBrush = "#777";
                    BorderThickness = "0,0,0,1";
                }
            }
        }

        public string ValorFormato
        {
            get
            {
                string valorText = $"{Valor.ToString("C", CultureInfo.CurrentCulture)}";
                if (Valor == 0.5)
                {
                    valorText = $"{valorText} centavos";
                }
                else if (Valor == 1)
                {
                    valorText = $"{valorText} peso";
                }
                else
                {
                    valorText = $"{valorText} pesos";
                }
                return valorText;
            }
        }


        public float Importe
        {
            get
            {
                float importe = 0;

                if (Cantidad != null && Cantidad.Length > 0)
                {
                    int cantidad = int.Parse(Cantidad);
                    importe = cantidad * Valor;
                }
                return importe;
            }
        }

        public string ImporteFormato
        {
            get
            {
                return $"{Importe.ToString("C", CultureInfo.CurrentCulture)}";
            }
        }

        public string BorderBrush { get; set; } = "#777";
        public string Foreground { get; set; } = "#999";
        public string BorderThickness { get; set; } = "0,0,0,1";
        
        public ArqueoMoneda() { }

        public ArqueoMoneda(float valor, int index, bool focus)
        {
            Valor = valor;
            Index = index;
            IsFocus = focus;
        }

    }
}
