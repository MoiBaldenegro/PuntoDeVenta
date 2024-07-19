
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Tomate.Models.Usuarios;
using Tomate.Models.Ventas;

namespace Tomate.Statics
{
    public class VentasStatic
    {

        public static List<Venta> TodasVentas { get; set; } = new List<Venta>(); 

        public static void RevisarVentasVisibles(Usuario Usuario)
        {
            for (int i = 0; i < TodasVentas.Count; i++)
            {
                TodasVentas[i].Visible = Usuario.RevisarPermiso("pv.todas-ventas") || Usuario.BuscarMesa($"{TodasVentas[i].NumeroCuenta}");
            }
            TodasVentas = TodasVentas;
        }

        public static Venta? getVenta(string numeroCuenta)
        {
            return TodasVentas.Where(item => item.NumeroCuenta == numeroCuenta).FirstOrDefault();
        }

    }
}
