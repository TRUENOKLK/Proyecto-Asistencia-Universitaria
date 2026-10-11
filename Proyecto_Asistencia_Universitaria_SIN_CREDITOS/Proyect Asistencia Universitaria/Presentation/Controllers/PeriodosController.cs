using Aplication.DTos;
using Aplication.Interface;
using Aplication.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace Presentation.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class PeriodosController : Controller
    {
        private readonly PeriodoServices _service;
        private readonly AuditoriaServices _auditoria;

        public PeriodosController(PeriodoServices service, AuditoriaServices auditoria)
        {
            _service = service;
            _auditoria = auditoria;
        }

        [HttpGet]
        public async Task<ActionResult> ListarPeriodos() => View(await _service.ListarPeriodos());

        [HttpGet]
        public IActionResult NuevoPeriodo() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NuevoPeriodo(PeriodoDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            try
            {
                await _service.NuevoPeriodo(dto);
                await _auditoria.Registrar(User.Identity?.Name, AccionAuditoria.Insertar, "Periodos", "SP_InsertarPeriodo", $"Creó el periodo {dto.NombrePeriodo}");
                return RedirectToAction(nameof(ListarPeriodos));
            }
            catch (Exception ex) { ModelState.AddModelError("", ex.Message); return View(dto); }
        }

        [HttpGet]
        public async Task<IActionResult> EditarPeriodo(int id)
        {
            var periodo = await _service.ObtenerPorId(id);
            if (periodo == null) return NotFound();
            return View(periodo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarPeriodo(PeriodoDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            try
            {
                await _service.EditarPeriodo(dto);
                await _auditoria.Registrar(User.Identity?.Name, AccionAuditoria.Editar, "Periodos", "SP_EditarPeriodo", $"Editó el periodo {dto.NombrePeriodo} (Id {dto.IdPeriodo})");
                TempData["MensajeExito"] = "Periodo actualizado correctamente.";
                return RedirectToAction(nameof(ListarPeriodos));
            }
            catch (Exception ex) { ModelState.AddModelError("", ex.Message); return View(dto); }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarPeriodo(int id)
        {
            try
            {
                await _service.EliminarPeriodo(id);
                await _auditoria.Registrar(User.Identity?.Name, AccionAuditoria.Eliminar, "Periodos", "SP_EliminarPeriodo", $"Eliminó el periodo Id {id}");
                TempData["MensajeExito"] = "Periodo eliminado correctamente.";
            }
            catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction(nameof(ListarPeriodos));
        }
    }
}