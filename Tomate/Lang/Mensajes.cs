using System.Collections.Generic;

namespace Tomate.Lang
{
    public class Mensajes
    {
        /**
         * Listado de mensajes del sistema 
         */
        public static Dictionary<string, string> Textos
        {
            get
            {
                var mensajes = new Dictionary<string, string>();
                //tablero de inicio
                mensajes["ingresa_codigo_id"] = "Ingresa código";
                mensajes["ingresa_codigo_huella"] = "Ingresa código o huella";
                mensajes["ingresa_codigo_huella_error"] = "Usuario incorrecto";
                mensajes["ingresa_codigo_id_error"] = "Usuario incorrecto";
                mensajes["ingresa_codigo_mensaje"] = "Ingresa tu ID";
                mensajes["ingresa_empleado_error"] = "Usuario incorrecto";
                mensajes["ingresa_empleado_password_error"] = "Contraseña incorrecta";
                

                //listado cuentas
                mensajes["ingresa_cuenta"] = "Ingresa número de cuenta";
                mensajes["ingresa_cuenta_no_disponible"] = "Cuenta no disponible";
                mensajes["cuenta_no_disponible"] = "La cuenta no está disponible";
                mensajes["cuentas_abiertas"] = "Cuentas Abiertas";


                return mensajes;
            }
        }

    }
}
