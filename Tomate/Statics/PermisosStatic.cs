using System;
using System.Collections.Generic;
using Tomate.Models.Usuarios;

namespace Tomate.Statics
{
    public class PermisosStatic
    {
        private static Dictionary<string, bool> PermisosPerfiles = new Dictionary<string, bool>();

        public static void CargarPermisos()
        {
            PermisosPerfiles = new Dictionary<string, bool>();
            foreach (var permiso in PerfilPermiso.todos())
            {
                PermisosPerfiles[$"{permiso.PerfilId}-{permiso.Permiso}"] = true;
            }
        }

        public static bool RevisarPermiso(string perfilId, string permiso)
        {
            try
            {
                return PermisosPerfiles.ContainsKey($"{perfilId}-{permiso}");
            }
            catch (Exception)
            {
                return false;
            }
            
        }
    }
}
