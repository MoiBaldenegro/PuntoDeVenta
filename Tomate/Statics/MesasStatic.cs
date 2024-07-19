using System;
using System.Collections.Generic;
using Tomate.Models.Usuarios;
using Tomate.Models.Ventas;

namespace Tomate.Statics
{
    public class MesasStatic
    {
        private static Dictionary<string, Usuario?> MesasUsuarios = new Dictionary<string, Usuario?>();

        public static void CargarMesas()
        {
            MesasUsuarios = new Dictionary<string, Usuario?>();
            foreach (var mesa in Mesa.todas())
            {
                var usuario = new Usuario();
                usuario.Id = mesa.UsuarioId;
                usuario.Ausente = mesa.UsuarioAusente;
                MesasUsuarios[$"{mesa.NumeroMesa}"] = usuario;
            }
        }

        public static string? getMesaUsuarioId(string numeroMesa)
        {
            try
            {
                if (MesasUsuarios.ContainsKey($"{numeroMesa}"))
                {
                    return MesasUsuarios[$"{numeroMesa}"].Id;
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        public static bool RevisarMesaPermiso(string numeroMesa, string usuarioId)
        {
            try
            {
                return(MesasUsuarios.ContainsKey($"{numeroMesa}")
                                && MesasUsuarios[$"{numeroMesa}"].Id == usuarioId) || MesasUsuarios[$"{numeroMesa}"].Ausente;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsMesaValida(string numeroMesa)
        {
            try
            {
                return MesasUsuarios.ContainsKey($"{numeroMesa}");
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
