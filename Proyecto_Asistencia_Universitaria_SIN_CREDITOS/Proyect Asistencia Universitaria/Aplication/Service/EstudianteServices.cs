using Aplication.DTos;
using Aplication.Interface;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aplication.Service
{
    public class EstudianteServices
    {
        private readonly IEstudianteRepository _repository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ICorreoInstitucionalGenerator _correoGenerator;

        
        private const string PasswordTemporalMasiva = "Estudiante123*";

        public EstudianteServices(
            IEstudianteRepository repository,
            IUsuarioRepository usuarioRepository,
            IPasswordHasher passwordHasher,
            ICorreoInstitucionalGenerator correoGenerator)
        {
            _repository = repository;
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;
            _correoGenerator = correoGenerator;
        }

        
        private async Task<string> GenerarCorreoUnicoAsync(string nombres, string apellidos, HashSet<string>? reservadosEnLote = null)
        {
            const int maxIntentos = 50;
            for (var intento = 1; intento <= maxIntentos; intento++)
            {
                var candidato = _correoGenerator.Generar(nombres, apellidos, "Estudiante", intento);

            
                if (reservadosEnLote != null && reservadosEnLote.Contains(candidato))
                    continue;

                if (!await _usuarioRepository.ExisteEmailAsync(candidato))
                {
                    reservadosEnLote?.Add(candidato);
                    return candidato;
                }
            }
            throw new Exception("No fue posible generar un correo institucional único. Intenta de nuevo.");
        }

        public async Task<IEnumerable<EstudianteDto>> ListarEstudiantes()
        {
            var listar = await _repository.ListarEstudiantesAsync();
            return listar.Select(e => new EstudianteDto
            {
                IdEstudiante = e.IdEstudiante,
                Carnet = e.Carnet,
                Nombres = e.Nombres,
                Apellidos = e.Apellidos,
                IdCarrera = e.IdCarrera,
                NombreCarrera = e.NombreCarrera,
                Email = e.Email,
                Activo = e.Activo
            });
        }

        public async Task<EstudianteDto?> ObtenerPorId(int idEstudiante)
        {
            var e = await _repository.ObtenerPorIdAsync(idEstudiante);
            if (e == null) return null;
            return new EstudianteDto
            {
                IdEstudiante = e.IdEstudiante,
                Carnet = e.Carnet,
                Nombres = e.Nombres,
                Apellidos = e.Apellidos,
                IdCarrera = e.IdCarrera,
                NombreCarrera = e.NombreCarrera,
                Email = e.Email,
                Activo = e.Activo
            };
        }

      
        public async Task EditarEstudiante(EstudianteDto dto)
        {
            await _repository.EditarEstudianteAsync(new Estudiante
            {
                IdEstudiante = dto.IdEstudiante,
                Nombres = dto.Nombres,
                Apellidos = dto.Apellidos,
                IdCarrera = dto.IdCarrera
            });
        }

        
        public async Task CambiarEstado(int idEstudiante, bool activo) =>
            await _repository.CambiarEstadoAsync(idEstudiante, activo);

       
        public async Task<string> NuevoEstudiante(EstudianteDto dto)
        {
            var correoGenerado = await GenerarCorreoUnicoAsync(dto.Nombres!, dto.Apellidos!);

            var oEstudiante = new Estudiante
            {
                Carnet = dto.Carnet,
                Nombres = dto.Nombres,
                Apellidos = dto.Apellidos,
                IdCarrera = dto.IdCarrera,
                Email = correoGenerado,
                PasswordHash = _passwordHasher.Hash(dto.PasswordHash!)
            };

            await _repository.InsertarEstudianteAsync(oEstudiante);
            return correoGenerado;
        }

        public async Task GuardarMasivo(List<EstudianteDto> listaDtos)
        {
            var entidades = new List<Estudiante>();
            var reservadosEnLote = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

           
            foreach (var dto in listaDtos)
            {
                var correoGenerado = await GenerarCorreoUnicoAsync(dto.Nombres!, dto.Apellidos!, reservadosEnLote);

                entidades.Add(new Estudiante
                {
                    Carnet = dto.Carnet,
                    Nombres = dto.Nombres,
                    Apellidos = dto.Apellidos,
                    IdCarrera = dto.IdCarrera,
                    Email = correoGenerado,
                   
                    PasswordHash = _passwordHasher.Hash(string.IsNullOrWhiteSpace(dto.PasswordHash) ? PasswordTemporalMasiva : dto.PasswordHash!)
                });
            }

            await _repository.InsertarMasivoAsync(entidades);
        }

        public async Task<IEnumerable<MiAsistenciaDto>> ObtenerMiAsistencia(string email)
        {
            return await _repository.ObtenerAsistenciaPorEmailAsync(email);
        }
    }
}
