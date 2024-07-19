using System;
using System.Diagnostics;
using System.Globalization;

namespace Tomate.Models.Caja
{
    public class ArqueoFormaPago
    {

        public string? Nombre { get; set; }

        public string? _Cantidad;
        public string? Cantidad
        {
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
                string totalFormato = "0";
                if (Cantidad != null && Cantidad.Length > 0)
                {
                    try
                    {
                        var descimales = Cantidad.Split('.');
                        float cantidadTotal = float.Parse(descimales[0]);
                        totalFormato = cantidadTotal.ToString("#,##0", CultureInfo.CurrentCulture);
                        if (descimales.Length > 1)
                        {
                            totalFormato = $"{totalFormato}.{descimales[1]}";
                        }
                    }
                    catch (Exception)
                    {
                        totalFormato = $"{Cantidad}";
                    }
                }

                return totalFormato;
            }
        }

        public float Pagos { get; set; } = 0;

        public string PagosFormato
        {
            get
            {
                return $"{Pagos.ToString("C", CultureInfo.CurrentCulture)}";
            }
        }

        public string DiferenciaFormato
        {
            get
            {
                float cantidad = 0;
                if (Cantidad != null && Cantidad.Length > 0)
                {
                    cantidad = float.Parse(Cantidad);
                }
                float importe = cantidad - Pagos;
                if (importe < 0)
                {
                    ForegroundDiferencia = "#DE0300";
                }
                /*else if (importe > 0)
                {
                    ForegroundDiferencia = "#05E513";
                }*/
                else
                {
                    ForegroundDiferencia = "#555555";
                }
                return $"{importe.ToString("C", CultureInfo.CurrentCulture)}";
            }
        }

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

        public string BorderBrush { get; set; } = "#777";
        public string Foreground { get; set; } = "#999";
        public string ForegroundDiferencia { get; set; } = "#999";
        public string BorderThickness { get; set; } = "0,0,0,1";

        public ArqueoFormaPago() { }

        public ArqueoFormaPago(string nombre, float pagos)
        {
            Nombre = nombre;
            Pagos = pagos;
        }

    }
}
