using Aplication.DTos;
using Aplication.Interface;
using Aplication.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace Presentation.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class EstudiantesController : Controller
    {
        private readonly EstudianteServices _estudianteService;
        private readonly CarreraServices _carreraService;
        private readonly AuditoriaServices _auditoria;

        
        public EstudiantesController(EstudianteServices estudianteService, CarreraServices carreraService, AuditoriaServices auditoria)
        {
            _estudianteService = estudianteService;
            _carreraService = carreraService;
            _auditoria = auditoria;
        }

        [HttpGet]
        public async Task<ActionResult> ListarEstudiantes()
        {
            var lista = await _estudianteService.ListarEstudiantes();
            return View(lista);
        }

        [HttpGet]
        public async Task<IActionResult> NuevoEstudiante()
        {
            
            var carreras = await _carreraService.ListarCarreras();
            ViewBag.ListaCarreras = new SelectList(carreras, "IdCarrera", "NombreCarrera");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NuevoEstudiante(EstudianteDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var carreras = await _carreraService.ListarCarreras();
                    ViewBag.ListaCarreras = new SelectList(carreras, "IdCarrera", "NombreCarrera");
                    return View(dto);
                }

                var correoGenerado = await _estudianteService.NuevoEstudiante(dto);
                await _auditoria.Registrar(User.Identity?.Name, AccionAuditoria.Insertar, "Estudiantes", "SP_InsertarEstudiante", $"Creó al estudiante {dto.Nombres} {dto.Apellidos} ({correoGenerado})");
                TempData["MensajeExito"] = $"Estudiante creado correctamente. Su correo institucional es: {correoGenerado}";
                return RedirectToAction(nameof(ListarEstudiantes));
            }
            catch (Exception ex)
            {
                var carreras = await _carreraService.ListarCarreras();
                ViewBag.ListaCarreras = new SelectList(carreras, "IdCarrera", "NombreCarrera");
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditarEstudiante(int id)
        {
            var estudiante = await _estudianteService.ObtenerPorId(id);
            if (estudiante == null) return NotFound();
            var carreras = await _carreraService.ListarCarreras();
            ViewBag.ListaCarreras = new SelectList(carreras, "IdCarrera", "NombreCarrera");
            return View(estudiante);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarEstudiante(EstudianteDto dto)
        {
            
            ModelState.Remove(nameof(EstudianteDto.PasswordHash));
            ModelState.Remove(nameof(EstudianteDto.ConfirmarPassword));

            if (!ModelState.IsValid)
            {
                var carreras = await _carreraService.ListarCarreras();
                ViewBag.ListaCarreras = new SelectList(carreras, "IdCarrera", "NombreCarrera");
                return View(dto);
            }

            try
            {
                await _estudianteService.EditarEstudiante(dto);
                await _auditoria.Registrar(User.Identity?.Name, AccionAuditoria.Editar, "Estudiantes", "SP_EditarEstudiante", $"Editó al estudiante {dto.Nombres} {dto.Apellidos} (Id {dto.IdEstudiante})");
                TempData["MensajeExito"] = "Estudiante actualizado correctamente.";
                return RedirectToAction(nameof(ListarEstudiantes));
            }
            catch (Exception ex)
            {
                var carreras = await _carreraService.ListarCarreras();
                ViewBag.ListaCarreras = new SelectList(carreras, "IdCarrera", "NombreCarrera");
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
                await _estudianteService.CambiarEstado(id, activo);
                await _auditoria.Registrar(User.Identity?.Name, activo ? AccionAuditoria.Activar : AccionAuditoria.Desactivar, "Estudiantes", "SP_CambiarEstadoEstudiante", $"{(activo ? "Activó" : "Desactivó")} al estudiante Id {id}");
                TempData["MensajeExito"] = activo ? "Estudiante activado." : "Estudiante desactivado: ya no podrá iniciar sesión.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(ListarEstudiantes));
        }

       

        [HttpGet]
        public async Task<IActionResult> RegistroMasivo()
        {
            var carreras = await _carreraService.ListarCarreras();
            ViewBag.ListaCarreras = new SelectList(carreras, "IdCarrera", "NombreCarrera");

          
            var listaInicial = new List<EstudianteDto>
            {
                new EstudianteDto(),
                new EstudianteDto(),
                new EstudianteDto()
            };

            return View(listaInicial);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistroMasivo(List<EstudianteDto> lista)
        {
            try
            {
                
                var listaValida = lista?.Where(e => !string.IsNullOrWhiteSpace(e.Carnet) && !string.IsNullOrWhiteSpace(e.Nombres)).ToList();

                if (listaValida == null || !listaValida.Any())
                {
                    var carreras = await _carreraService.ListarCarreras();
                    ViewBag.ListaCarreras = new SelectList(carreras, "IdCarrera", "NombreCarrera");
                    ModelState.AddModelError("", "Debe completar al menos un estudiante válido (Carnet y Nombres).");
                    return View(lista);
                }

                await _estudianteService.GuardarMasivo(listaValida);
                await _auditoria.Registrar(User.Identity?.Name, AccionAuditoria.Insertar, "Estudiantes", "SP_InsertarEstudiante", $"Registro masivo de {listaValida.Count} estudiante(s)");
                TempData["MensajeExito"] = $"Se registraron {listaValida.Count} estudiante(s) correctamente. Los correos institucionales se generaron automáticamente.";
                return RedirectToAction(nameof(ListarEstudiantes));
            }
            catch (Exception ex)
            {
                var carreras = await _carreraService.ListarCarreras();
                ViewBag.ListaCarreras = new SelectList(carreras, "IdCarrera", "NombreCarrera");
                ModelState.AddModelError("", ex.Message);
                return View(lista);
            }
        }
    }
}