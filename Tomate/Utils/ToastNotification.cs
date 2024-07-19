using System;

namespace Tomate.Utils
{
    public class ToastNotification
    {
        public static int SHORT = 1500;
        public static int LONG = 3000;

        public static void show(string mensaje, int duracion)
        {
            WindowUtils.getMainWindow().showToast(mensaje, duracion);
        }

    }
}
