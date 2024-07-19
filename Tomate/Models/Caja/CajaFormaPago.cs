using Newtonsoft.Json.Linq;
using System.Globalization;

namespace Tomate.Models.Caja
{
    public class CajaFormaPago
    {
        public string? Id { get; set; }
        public string? Nombre { get; set; }
        public float? Total { get; set; } = 0;

        public string TotalFormato
        {
            get
            {
                return $"{(Total ?? 0).ToString("C", CultureInfo.CurrentCulture)}";
            }
        }

        public CajaFormaPago() { }

        public CajaFormaPago(string nombre, float total)
        {
            Nombre = nombre;
            Total = total;
        }

        public CajaFormaPago(JObject data)
        {
            Id = (string?)data["id"];
            Nombre = (string?)data["forma_pago"];
            Total = (float?)data["total"];
        }

    }
}
