using Aplication.DTos;
using Aplication.Interface;
using Aplication.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Presentation.Controllers
{
    
    [Authorize(Roles = "Administrador,Profesor")]
    public class GruposController : Controller
    {
        private readonly GrupoServices _grupoService;
        private readonly AsignaturaServices _asignaturaService;
        private readonly ProfesorServices _profesorService;
        private readonly PeriodoServices _periodoService;
        private readonly AuditoriaServices _auditoria;

        public GruposController(GrupoServices grupoService, AsignaturaServices asignaturaService, ProfesorServices profesorService, PeriodoServices periodoService, AuditoriaServices auditoria)
        {
            _grupoService = grupoService;
            _asignaturaService = asignaturaService;
            _profesorService = profesorService;
            _periodoService = periodoService;
            _auditoria = auditoria;
        }

        [HttpGet]
        public async Task<ActionResult> ListarGrupos()
        {
            if (User.IsInRole("Profesor") && !User.IsInRole("Administrador"))
            {
                var email = User.Identity?.Name ?? string.Empty;
                var profesores = await _profesorService.ListarProfesores();
                var yo = profesores.FirstOrDefault(p => string.Equals(p.Email, email, StringComparison.OrdinalIgnoreCase));

                if (yo == null)
                    return View(Enumerable.Empty<GrupoDto>());

                return View(await _grupoService.ListarGruposPorProfesor(yo.IdProfesor));
            }

            return View(await _grupoService.ListarGrupos());
        }

        [HttpGet]
        public async Task<IActionResult> Horario()
        {
            IEnumerable<GrupoDto> grupos;

            if (User.IsInRole("Profesor") && !User.IsInRole("Administrador"))
            {
                var email = User.Identity?.Name ?? string.Empty;
                var profesores = await _profesorService.ListarProfesores();
                var yo = profesores.FirstOrDefault(p => string.Equals(p.Email, email, StringComparison.OrdinalIgnoreCase));

                grupos = yo == null ? Enumerable.Empty<GrupoDto>() : await _grupoService.ListarGruposPorProfesor(yo.IdProfesor);
                ViewBag.Titulo = "Mi Horario Semanal";
            }
            else
            {
                grupos = await _grupoService.ListarGrupos();
                ViewBag.Titulo = "Horario Semanal de Grupos";
            }

            return View(grupos);
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> EditarGrupo(int id)
        {
            var grupo = await _grupoService.ObtenerPorId(id);
            if (grupo == null) return NotFound();
            await CargarCombos();
            return View(grupo);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarGrupo(GrupoDto dto)
        {
            if (!ModelState.IsValid)
            {
                await CargarCombos();
                return View(dto);
            }
            try
            {
                await _grupoService.EditarGrupo(dto);
                await _auditoria.Registrar(User.Identity?.Name, AccionAuditoria.Editar, "Grupos", "SP_EditarGrupo", $"Editó el grupo {dto.NombreGrupo} (Id {dto.IdGrupo})");
                TempData["MensajeExito"] = "Grupo actualizado correctamente.";
                return RedirectToAction(nameof(ListarGrupos));
            }
            catch (Exception ex)
            {
                await CargarCombos();
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarGrupo(int id)
        {
            try
            {
                await _grupoService.EliminarGrupo(id);
                await _auditoria.Registrar(User.Identity?.Name, AccionAuditoria.Eliminar, "Grupos", "SP_EliminarGrupo", $"Eliminó el grupo Id {id}");
                TempData["MensajeExito"] = "Grupo eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(ListarGrupos));
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> NuevoGrupo()
        {
            await CargarCombos();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NuevoGrupo(GrupoDto dto)
        {
            if (!ModelState.IsValid)
            {
                await CargarCombos();
                return View(dto);
            }
            try
            {
                await _grupoService.NuevoGrupo(dto);
                await _auditoria.Registrar(User.Identity?.Name, AccionAuditoria.Insertar, "Grupos", "SP_InsertarGrupo", $"Creó el grupo {dto.NombreGrupo}");
                return RedirectToAction(nameof(ListarGrupos));
            }
            catch (Exception ex)
            {
                await CargarCombos();
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> GestionarEstudiantes(int idGrupo)
        {
            var grupo = await _grupoService.ObtenerPorId(idGrupo);
            if (grupo == null) return NotFound();

            ViewBag.Grupo = grupo;
            ViewBag.Inscritos = await _grupoService.ListarEstudiantesInscritos(idGrupo);
            ViewBag.Disponibles = await _grupoService.ListarEstudiantesDisponibles(idGrupo);
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InscribirEstudiante(int idGrupo, int idEstudiante)
        {
            try
            {
                await _grupoService.InscribirEstudiante(idGrupo, idEstudiante);
                await _auditoria.Registrar(User.Identity?.Name, AccionAuditoria.Insertar, "Inscripciones", "Inscripciones", $"Inscribió al estudiante Id {idEstudiante} en el grupo Id {idGrupo}");
                TempData["MensajeExito"] = "Estudiante inscrito correctamente en el grupo.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(GestionarEstudiantes), new { idGrupo });
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DesinscribirEstudiante(int idGrupo, int idEstudiante)
        {
            try
            {
                await _grupoService.DesinscribirEstudiante(idGrupo, idEstudiante);
                await _auditoria.Registrar(User.Identity?.Name, AccionAuditoria.Eliminar, "Inscripciones", "Inscripciones", $"Retiró al estudiante Id {idEstudiante} del grupo Id {idGrupo}");
                TempData["MensajeExito"] = "Estudiante retirado del grupo.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(GestionarEstudiantes), new { idGrupo });
        }

        private async Task CargarCombos()
        {
            ViewBag.ListaAsignaturas = new SelectList(await _asignaturaService.ListarAsignaturas(), "IdAsignatura", "NombreAsignatura");
            ViewBag.ListaProfesores = new SelectList(await _profesorService.ListarProfesores(), "IdProfesor", "Apellidos"); // Puedes concatenar Nombre y Apellido luego
            ViewBag.ListaPeriodos = new SelectList(await _periodoService.ListarPeriodos(), "IdPeriodo", "NombrePeriodo");
        }
    }
}