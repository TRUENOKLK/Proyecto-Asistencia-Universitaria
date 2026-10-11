using Aplication.DTos;
using Aplication.Interface;
using Aplication.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Presentation.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ProfesoresController : Controller
    {
        private readonly ProfesorServices _service;
        private readonly AuditoriaServices _auditoria;

        public ProfesoresController(ProfesorServices service, AuditoriaServices auditoria)
        {
            _service = service;
            _auditoria = auditoria;
        }

        [HttpGet]
        public async Task<ActionResult> ListarProfesores()
        {
            var lista = await _service.ListarProfesores();
            return View(lista);
        }

        [HttpGet]
        public IActionResult NuevoProfesor()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NuevoProfesor(ProfesorDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(dto);
                }

                var correoGenerado = await _service.NuevoProfesor(dto);
                await _auditoria.Registrar(User.Identity?.Name, AccionAuditoria.Insertar, "Profesores", "SP_InsertarProfesor", $"Creó al profesor {dto.Nombres} {dto.Apellidos} ({correoGenerado})");
                TempData["MensajeExito"] = $"Profesor creado correctamente. Su correo institucional es: {correoGenerado}";
                return RedirectToAction(nameof(ListarProfesores));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditarProfesor(int id)
        {
            var profesor = await _service.ObtenerPorId(id);
            if (profesor == null) return NotFound();
            return View(profesor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarProfesor(ProfesorDto dto)
        {
            ModelState.Remove(nameof(ProfesorDto.PasswordHash));
            ModelState.Remove(nameof(ProfesorDto.ConfirmarPassword));

            if (!ModelState.IsValid) return View(dto);

            try
            {
                await _service.EditarProfesor(dto);
                await _auditoria.Registrar(User.Identity?.Name, AccionAuditoria.Editar, "Profesores", "SP_EditarProfesor", $"Editó al profesor {dto.Nombres} {dto.Apellidos} (Id {dto.IdProfesor})");
                TempData["MensajeExito"] = "Profesor actualizado correctamente.";
                return RedirectToAction(nameof(ListarProfesores));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id, bool activo)
        {
            try
            {
                await _service.CambiarEstado(id, activo);
                await _auditoria.Registrar(User.Identity?.Name, activo ? AccionAuditoria.Activar : AccionAuditoria.Desactivar, "Profesores", "SP_CambiarEstadoProfesor", $"{(activo ? "Activó" : "Desactivó")} al profesor Id {id}");
                TempData["MensajeExito"] = activo ? "Profesor activado." : "Profesor desactivado: ya no podrá iniciar sesión.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(ListarProfesores));
        }
    }
}