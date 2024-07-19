using System.Globalization;

namespace Tomate.Models.Cobrar
{
    public class CobroMoneda
    {

     
        public float? _Cantidad;
        public float? Cantidad { 
            get 
            {
                return _Cantidad;
            } 
            set 
            {
                _Cantidad = value;
            } 
        }


        public string CantidadFormato
        {
            get
            {
                string formato = "$#,##0.##";
                
                if (Cantidad != null)
                {
                    var decimales = Cantidad - (int)Cantidad;
                    if (decimales > 0)
                    {
                        formato = "C";
                    }
                }
                return $"{(Cantidad ?? 0).ToString(formato, CultureInfo.CurrentCulture)}";
            }
        }

        public CobroMoneda() { }

        public CobroMoneda(float cantidad)
        {
            Cantidad = cantidad;
        }

    }
}
