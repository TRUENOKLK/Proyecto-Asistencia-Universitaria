using Aplication.DTos;
using Aplication.Interface;
using Aplication.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Presentation.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class FacultadesController : Controller
    {
        private readonly FacultadServices _service;
        private readonly AuditoriaServices _auditoria;

        public FacultadesController(FacultadServices service, AuditoriaServices auditoria)
        {
            _service = service;
            _auditoria = auditoria;
        }

        [HttpGet]
        public async Task<ActionResult> ListarFacultades()
        {
            return View(await _service.ListarFacultades());
        }

        [HttpGet]
        public IActionResult NuevaFacultad()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NuevaFacultad(FacultadDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return View(dto);
                await _service.NuevaFacultad(dto);
                await _auditoria.Registrar(User.Identity?.Name, AccionAuditoria.Insertar, "Facultades", "SP_InsertarFacultad", $"Creó la facultad {dto.NombreFacultad}");
                return RedirectToAction(nameof(ListarFacultades));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditarFacultad(int id)
        {
            var facultad = await _service.ObtenerPorId(id);
            if (facultad == null) return NotFound();
            return View(facultad);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarFacultad(FacultadDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            try
            {
                await _service.EditarFacultad(dto);
                await _auditoria.Registrar(User.Identity?.Name, AccionAuditoria.Editar, "Facultades", "SP_EditarFacultad", $"Editó la facultad {dto.NombreFacultad} (Id {dto.IdFacultad})");
                TempData["MensajeExito"] = "Facultad actualizada correctamente.";
                return RedirectToAction(nameof(ListarFacultades));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarFacultad(int id)
        {
            try
            {
                await _service.EliminarFacultad(id);
                await _auditoria.Registrar(User.Identity?.Name, AccionAuditoria.Eliminar, "Facultades", "SP_EliminarFacultad", $"Eliminó la facultad Id {id}");
                TempData["MensajeExito"] = "Facultad eliminada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(ListarFacultades));
        }
    }
}