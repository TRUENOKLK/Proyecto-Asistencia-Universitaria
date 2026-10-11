using Aplication.DTos;
using Aplication.Interface;
using Aplication.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly AuthServices _authService;
        private readonly AuditoriaServices _auditoria;

        public AuthController(AuthServices authService, AuditoriaServices auditoria)
        {
            _authService = authService;
            _auditoria = auditoria;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto dto, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid) return View(dto);

            try
            {
                var usuario = await _authService.ValidarAcceso(dto);

               
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                    new(ClaimTypes.Name, usuario.Email ?? string.Empty),
                    new(ClaimTypes.Email, usuario.Email ?? string.Empty),
                    new(ClaimTypes.Role, usuario.NombreRol ?? string.Empty)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties { IsPersistent = true });

                await _auditoria.Registrar(usuario.Email, AccionAuditoria.Login, "Autenticación", "SP_ObtenerUsuarioPorEmail", $"Inicio de sesión ({usuario.NombreRol})");

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var email = User.Identity?.Name;

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await _auditoria.Registrar(email, AccionAuditoria.Logout, "Autenticación", null, "Cierre de sesión");

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();
    }
}