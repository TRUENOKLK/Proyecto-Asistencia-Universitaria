using Aplication.DTos;
using Aplication.Interface;
using Aplication.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    
    [Authorize(Roles = "Administrador,Profesor")]
    public class OperacionesController : Controller
    {
        private readonly OperacionesServices _service;
        private readonly GrupoServices _grupoService;
        private readonly IReportePdfService _reportePdfService;
        private readonly AuditoriaServices _auditoria;

        public OperacionesController(OperacionesServices service, GrupoServices grupoService, IReportePdfService reportePdfService, AuditoriaServices auditoria)
        {
            _service = service;
            _grupoService = grupoService;
            _reportePdfService = reportePdfService;
            _auditoria = auditoria;
        }

        
        [HttpGet]
        public async Task<IActionResult> SesionesPorGrupo(int idGrupo)
        {
            ViewBag.IdGrupo = idGrupo;
            var sesiones = await _service.ListarSesiones(idGrupo);
            return View(sesiones);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearSesion(SesionDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .FirstOrDefault() ?? "Datos de la sesión inválidos.";
                return RedirectToAction("SesionesPorGrupo", new { idGrupo = dto.IdGrupo });
            }
            try
            {
                await _service.CrearSesion(dto);
                await _auditoria.Registrar(User.Identity?.Name, AccionAuditoria.Insertar, "Asistencia", "SP_CrearSesion", $"Creó una sesión ({dto.Tema}) en el grupo Id {dto.IdGrupo}");
                return RedirectToAction("SesionesPorGrupo", new { idGrupo = dto.IdGrupo });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("SesionesPorGrupo", new { idGrupo = dto.IdGrupo });
            }
        }

       
        [HttpGet]
        public async Task<IActionResult> TomarAsistencia(int idGrupo, int idSesion)
        {
            ViewBag.IdSesion = idSesion;
            ViewBag.IdGrupo = idGrupo;
            var estudiantes = await _service.ObtenerEstudiantesParaAsistencia(idGrupo);
            return View(estudiantes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarAsistenciaIndividual(RegistroAsistenciaDto dto)
        {
            try
            {
                await _service.RegistrarAsistencia(dto);
                await _auditoria.Registrar(User.Identity?.Name, AccionAuditoria.Insertar, "Asistencia", "SP_RegistrarAsistencia", $"Registró asistencia ({dto.EstadoAsistencia}) para el estudiante Id {dto.IdEstudiante} en la sesión Id {dto.IdSesion}");
                return Json(new { success = true, message = "Asistencia guardada" }); 
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ReporteGrupo(int idGrupo)
        {
            ViewBag.IdGrupo = idGrupo;
            var reporte = await _service.GenerarReporte(idGrupo);
            return View(reporte);
        }


        [HttpGet]
        public async Task<IActionResult> ExportarReportePdf(int idGrupo)
        {
            var grupos = await _grupoService.ListarGrupos();
            var grupo = grupos.FirstOrDefault(g => g.IdGrupo == idGrupo) ?? new GrupoDto { IdGrupo = idGrupo };

            var reporte = await _service.GenerarReporte(idGrupo);
            var pdfBytes = _reportePdfService.GenerarReporteAsistencia(grupo, reporte);

            var nombreArchivo = $"Reporte_Asistencia_{(grupo.NombreGrupo ?? idGrupo.ToString())}_{DateTime.Now:yyyyMMdd}.pdf"
                .Replace(' ', '_');

            return File(pdfBytes, "application/pdf", nombreArchivo);
        }
    }
}