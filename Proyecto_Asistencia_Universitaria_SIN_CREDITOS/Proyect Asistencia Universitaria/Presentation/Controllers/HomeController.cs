using Aplication.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using System.Diagnostics;

namespace Presentation.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly EstudianteServices _estudianteService;
        public HomeController(EstudianteServices estudianteService) => _estudianteService = estudianteService;

        public IActionResult Index()
        {
            return View();
        }

        // Portal del estudiante: su propio historial de asistencia.
        [Authorize(Roles = "Estudiante")]
        public async Task<IActionResult> MisAsistencias()
        {
            var email = User.Identity?.Name ?? string.Empty;
            var historial = await _estudianteService.ObtenerMiAsistencia(email);
            return View(historial);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
