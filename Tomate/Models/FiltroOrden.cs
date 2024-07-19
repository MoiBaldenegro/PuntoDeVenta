namespace Tomate.Models
{
    public class FiltroOrden
    {
       
        public string Nombre { get; set; }
        public string Columna { get; set; }
        public string Orden { get; set; }

        public FiltroOrden(string nombre, string columna, string orden)
        {
            Nombre = nombre;
            Columna = columna;
            Orden = orden;
        }
    }
}
