using Microsoft.Win32;
using System.Linq;

namespace Tomate.Utils
{
    public class SoftwareUtils
    {
        /**
         * Comprueba si un software esta instalado en el sistema
         */
        public static bool IsSoftwareInstalled(string softwareName)
        {
            var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE");

            if (key == null)
                return false;

            return key.GetSubKeyNames().Any(displayName => displayName != null && displayName.Contains(softwareName));
        }

    }
}
