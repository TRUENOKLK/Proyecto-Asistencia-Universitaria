using Aplication.DTos;
using Aplication.Interface;
using Aplication.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace Presentation.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CarrerasController : Controller
    {
        private readonly CarreraServices _carreraService;
        private readonly FacultadServices _facultadService;
        private readonly AuditoriaServices _auditoria;

        public CarrerasController(CarreraServices carreraService, FacultadServices facultadService, AuditoriaServices auditoria)
        {
            _carreraService = carreraService;
            _facultadService = facultadService;
            _auditoria = auditoria;
        }

        [HttpGet]
        public async Task<ActionResult> ListarCarreras()
        {
            return View(await _carreraService.ListarCarreras());
        }

        [HttpGet]
        public async Task<IActionResult> NuevaCarrera()
        {
            var facultades = await _facultadService.ListarFacultades();
            ViewBag.ListaFacultades = new SelectList(facultades, "IdFacultad", "NombreFacultad");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NuevaCarrera(CarreraDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var facultades = await _facultadService.ListarFacultades();
                    ViewBag.ListaFacultades = new SelectList(facultades, "IdFacultad", "NombreFacultad");
                    return View(dto);
                }
                await _carreraService.NuevaCarrera(dto);
                await _auditoria.Registrar(User.Identity?.Name, AccionAuditoria.Insertar, "Carreras", "SP_InsertarCarrera", $"Creó la carrera {dto.NombreCarrera}");
                return RedirectToAction(nameof(ListarCarreras));
            }
            catch (Exception ex)
            {
                var facultades = await _facultadService.ListarFacultades();
                ViewBag.ListaFacultades = new SelectList(facultades, "IdFacultad", "NombreFacultad");
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditarCarrera(int id)
        {
            var carrera = await _carreraService.ObtenerPorId(id);
            if (carrera == null) return NotFound();
            var facultades = await _facultadService.ListarFacultades();
            ViewBag.ListaFacultades = new SelectList(facultades, "IdFacultad", "NombreFacultad");
            return View(carrera);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarCarrera(CarreraDto dto)
        {
            if (!ModelState.IsValid)
            {
                var facultades = await _facultadService.ListarFacultades();
                ViewBag.ListaFacultades = new SelectList(facultades, "IdFacultad", "NombreFacultad");
                return View(dto);
            }
            try
            {
                await _carreraService.EditarCarrera(dto);
                await _auditoria.Registrar(User.Identity?.Name, AccionAuditoria.Editar, "Carreras", "SP_EditarCarrera", $"Editó la carrera {dto.NombreCarrera} (Id {dto.IdCarrera})");
                TempData["MensajeExito"] = "Carrera actualizada correctamente.";
                return RedirectToAction(nameof(ListarCarreras));
            }
            catch (Exception ex)
            {
                var facultades = await _facultadService.ListarFacultades();
                ViewBag.ListaFacultades = new SelectList(facultades, "IdFacultad", "NombreFacultad");
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarCarrera(int id)
        {
            try
            {
                await _carreraService.EliminarCarrera(id);
                await _auditoria.Registrar(User.Identity?.Name, AccionAuditoria.Eliminar, "Carreras", "SP_EliminarCarrera", $"Eliminó la carrera Id {id}");
                TempData["MensajeExito"] = "Carrera eliminada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(ListarCarreras));
        }
    }
}