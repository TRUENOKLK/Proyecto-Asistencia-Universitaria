using Aplication.DTos;
using Aplication.Interface;
using System;
using System.Threading.Tasks;

namespace Aplication.Service
{
    public class AuthServices
    {
        private readonly IAuthRepository _repository;
        private readonly IPasswordHasher _passwordHasher;

        public AuthServices(IAuthRepository repository, IPasswordHasher passwordHasher)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
        }

        public async Task<UsuarioSesionDto> ValidarAcceso(LoginDto dto)
        {
            
            const string mensajeError = "Correo o contraseña incorrectos, o usuario inactivo.";

            var usuario = await _repository.ObtenerPorEmailAsync(dto.Email!);

            if (usuario == null || !usuario.Activo)
                throw new Exception(mensajeError);

            if (!_passwordHasher.Verify(dto.Password!, usuario.PasswordHash))
                throw new Exception(mensajeError);

            return new UsuarioSesionDto
            {
                IdUsuario = usuario.IdUsuario,
                Email = usuario.Email,
                NombreRol = usuario.NombreRol,
                Activo = usuario.Activo
            };
        }
    }
}