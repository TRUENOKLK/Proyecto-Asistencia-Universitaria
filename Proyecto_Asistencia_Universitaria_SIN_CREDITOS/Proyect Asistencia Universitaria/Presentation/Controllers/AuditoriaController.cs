using Aplication.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AuditoriaController : Controller
    {
        private readonly AuditoriaServices _service;
        public AuditoriaController(AuditoriaServices service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> Index(string? accion)
        {
            ViewBag.AccionActual = accion ?? "";
            ViewBag.SesionesActivas = await _service.ObtenerContadorSesiones();
            var registros = await _service.Listar(accion);
            return View(registros);
        }
    }
}
