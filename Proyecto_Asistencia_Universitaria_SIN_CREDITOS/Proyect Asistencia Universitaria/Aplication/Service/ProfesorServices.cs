using Aplication.DTos;
using Aplication.Interface;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aplication.Service
{
    public class ProfesorServices
    {
        private readonly IProfesorRepository _repository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ICorreoInstitucionalGenerator _correoGenerator;

        public ProfesorServices(
            IProfesorRepository repository,
            IUsuarioRepository usuarioRepository,
            IPasswordHasher passwordHasher,
            ICorreoInstitucionalGenerator correoGenerator)
        {
            _repository = repository;
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;
            _correoGenerator = correoGenerator;
        }

        private async Task<string> GenerarCorreoUnicoAsync(string nombres, string apellidos)
        {
            const int maxIntentos = 50;
            for (var intento = 1; intento <= maxIntentos; intento++)
            {
                var candidato = _correoGenerator.Generar(nombres, apellidos, "Profesor", intento);
                if (!await _usuarioRepository.ExisteEmailAsync(candidato))
                    return candidato;
            }
            throw new Exception("No fue posible generar un correo institucional único. Intenta de nuevo.");
        }

        public async Task<IEnumerable<ProfesorDto>> ListarProfesores()
        {
            var listar = await _repository.ListarProfesoresAsync();

            return listar.Select(p => new ProfesorDto
            {
                IdProfesor = p.IdProfesor,
                Nombres = p.Nombres,
                Apellidos = p.Apellidos,
                Telefono = p.Telefono,
                Email = p.Email,
                Activo = p.Activo
            });
        }

        public async Task<ProfesorDto?> ObtenerPorId(int idProfesor)
        {
            var p = await _repository.ObtenerPorIdAsync(idProfesor);
            if (p == null) return null;
            return new ProfesorDto
            {
                IdProfesor = p.IdProfesor,
                Nombres = p.Nombres,
                Apellidos = p.Apellidos,
                Telefono = p.Telefono,
                Email = p.Email,
                Activo = p.Activo
            };
        }

        public async Task EditarProfesor(ProfesorDto dto)
        {
            await _repository.EditarProfesorAsync(new Profesor
            {
                IdProfesor = dto.IdProfesor,
                Nombres = dto.Nombres,
                Apellidos = dto.Apellidos,
                Telefono = dto.Telefono
            });
        }

        public async Task CambiarEstado(int idProfesor, bool activo) =>
            await _repository.CambiarEstadoAsync(idProfesor, activo);

        public async Task<string> NuevoProfesor(ProfesorDto dto)
        {
            var correoGenerado = await GenerarCorreoUnicoAsync(dto.Nombres!, dto.Apellidos!);

            var oProfesor = new Profesor
            {
                Nombres = dto.Nombres,
                Apellidos = dto.Apellidos,
                Telefono = dto.Telefono,
                Email = correoGenerado,
                PasswordHash = _passwordHasher.Hash(dto.PasswordHash!)
            };

            await _repository.InsertarProfesorAsync(oProfesor);
            return correoGenerado;
        }
    }
}
