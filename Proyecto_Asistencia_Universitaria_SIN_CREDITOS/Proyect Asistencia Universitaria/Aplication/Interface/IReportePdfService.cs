using Aplication.DTos;

namespace Aplication.Interface
{
 
    public interface IReportePdfService
    {
        byte[] GenerarReporteAsistencia(GrupoDto grupo, IEnumerable<ReporteAsistenciaDto> registros);
    }
}
