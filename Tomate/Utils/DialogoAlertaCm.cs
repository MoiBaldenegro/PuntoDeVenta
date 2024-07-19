using System;

namespace Tomate.Utils
{
    public class DialogoAlertaCm
    {


        public static void show(string mensaje)
        {
            WindowUtils.getMainWindow().MostrarDialogoAlerta(null, mensaje, false, null, null);
        }

        public static void show(string? titulo, string mensaje)
        {
            WindowUtils.getMainWindow().MostrarDialogoAlerta(titulo, mensaje, false, null, null);
        }

        public static void show(string? titulo, string mensaje, bool cancelarBoton)
        {
            WindowUtils.getMainWindow().MostrarDialogoAlerta(titulo, mensaje, cancelarBoton, null, null);
        }

        public static void show(string mensaje, bool cancelarBoton,
            Action? aceptarAction)
        {
            WindowUtils.getMainWindow().MostrarDialogoAlerta(null, mensaje, cancelarBoton, aceptarAction, null);
        }

        public static void show(string? titulo, string mensaje, bool cancelarBoton,
            Action? aceptarAction)
        {
            WindowUtils.getMainWindow().MostrarDialogoAlerta(titulo, mensaje, cancelarBoton, aceptarAction, null);
        }

        public static void show(string mensaje, bool cancelarBoton,
            Action? aceptarAction, Action? cancelarAction)
        {
            WindowUtils.getMainWindow().MostrarDialogoAlerta(null, mensaje, cancelarBoton, aceptarAction, cancelarAction);
        }

        public static void show(string? titulo, string mensaje, bool cancelarBoton,
            Action? aceptarAction, Action? cancelarAction)
        {
            WindowUtils.getMainWindow().MostrarDialogoAlerta(titulo, mensaje, cancelarBoton, aceptarAction, cancelarAction);
        }

       
    }
}
