using System;

namespace Domain.Entities
{
    public class RegistroAuditoria
    {
        public int IdAuditoria { get; set; }
        public string? Email { get; set; }
        public string? Accion { get; set; }
        public string? Modulo { get; set; }
        public string? Procedimiento { get; set; }
        public string? Detalle { get; set; }
        public DateTime FechaHora { get; set; }
    }
}
