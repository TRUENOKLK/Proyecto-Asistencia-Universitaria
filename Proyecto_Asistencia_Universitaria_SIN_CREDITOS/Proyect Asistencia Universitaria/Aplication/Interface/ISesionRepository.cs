using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplication.Interface
{
    public interface ISesionRepository
    {
        Task CrearSesionAsync(Sesion sesion);
        Task<IEnumerable<Sesion>> ListarSesionesPorGrupoAsync(int idGrupo);
    }
}
