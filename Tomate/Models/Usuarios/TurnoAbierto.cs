using Newtonsoft.Json.Linq;
using System;

namespace Tomate.Models.Usuarios
{
    public class TurnoAbierto
    {

        public string? Id { get; set; }        
        public string? UsuarioId { get; set; }
        public string? PerfilId { get; set; }
        public DateTime? HoraEntrada { get; set; }
        public DateTime? HoraSalida { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public TurnoAbierto() { }


        public TurnoAbierto(JObject data)
        {
            Id = (string?)data["id"];
            UsuarioId = (string?)data["usuario_id"];
            PerfilId = (string?)data["perfil_id"];
            HoraEntrada = (DateTime?)data["hora_entrada"];
            HoraSalida = (DateTime?)data["hora_salida"];
            CreatedAt = (DateTime?)data["created_at"];
            UpdatedAt = (DateTime?)data["updated_at"];
            DeletedAt = (DateTime?)data["deleted_at"];
        }

    }
}
