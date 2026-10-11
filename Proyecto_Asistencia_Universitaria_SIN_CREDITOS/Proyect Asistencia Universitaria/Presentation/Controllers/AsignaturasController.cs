using Aplication.DTos;
using Aplication.Interface;
using Aplication.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace Presentation.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AsignaturasController : Controller
    {
        private readonly AsignaturaServices _asignaturaService;
        private readonly CarreraServices _carreraService;
        private readonly AuditoriaServices _auditoria;

        public AsignaturasController(AsignaturaServices asignaturaService, CarreraServices carreraService, AuditoriaServices auditoria)
        {
            _asignaturaService = asignaturaService;
            _carreraService = carreraService;
            _auditoria = auditoria;
        }

        [HttpGet]
        public async Task<ActionResult> ListarAsignaturas() => View(await _asignaturaService.ListarAsignaturas());

        [HttpGet]
        public async Task<IActionResult> NuevaAsignatura()
        {
            ViewBag.ListaCarreras = new SelectList(await _carreraService.ListarCarreras(), "IdCarrera", "NombreCarrera");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NuevaAsignatura(AsignaturaDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ListaCarreras = new SelectList(await _carreraService.ListarCarreras(), "IdCarrera", "NombreCarrera");
                return View(dto);
            }
            try
            {
                await _asignaturaService.NuevaAsignatura(dto);
                await _auditoria.Registrar(User.Identity?.Name, AccionAuditoria.Insertar, "Asignaturas", "SP_InsertarAsignatura", $"Creó la asignatura {dto.NombreAsignatura} ({dto.Codigo})");
                return RedirectToAction(nameof(ListarAsignaturas));
            }
            catch (Exception ex)
            {
                ViewBag.ListaCarreras = new SelectList(await _carreraService.ListarCarreras(), "IdCarrera", "NombreCarrera");
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditarAsignatura(int id)
        {
            var asignatura = await _asignaturaService.ObtenerPorId(id);
            if (asignatura == null) return NotFound();
            ViewBag.ListaCarreras = new SelectList(await _carreraService.ListarCarreras(), "IdCarrera", "NombreCarrera");
            return View(asignatura);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarAsignatura(AsignaturaDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ListaCarreras = new SelectList(await _carreraService.ListarCarreras(), "IdCarrera", "NombreCarrera");
                return View(dto);
            }
            try
            {
                await _asignaturaService.EditarAsignatura(dto);
                await _auditoria.Registrar(User.Identity?.Name, AccionAuditoria.Editar, "Asignaturas", "SP_EditarAsignatura", $"Editó la asignatura {dto.NombreAsignatura} (Id {dto.IdAsignatura})");
                TempData["MensajeExito"] = "Asignatura actualizada correctamente.";
                return RedirectToAction(nameof(ListarAsignaturas));
            }
            catch (Exception ex)
            {
                ViewBag.ListaCarreras = new SelectList(await _carreraService.ListarCarreras(), "IdCarrera", "NombreCarrera");
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarAsignatura(int id)
        {
            try
            {
                await _asignaturaService.EliminarAsignatura(id);
                await _auditoria.Registrar(User.Identity?.Name, AccionAuditoria.Eliminar, "Asignaturas", "SP_EliminarAsignatura", $"Eliminó la asignatura Id {id}");
                TempData["MensajeExito"] = "Asignatura eliminada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(ListarAsignaturas));
        }
    }
}