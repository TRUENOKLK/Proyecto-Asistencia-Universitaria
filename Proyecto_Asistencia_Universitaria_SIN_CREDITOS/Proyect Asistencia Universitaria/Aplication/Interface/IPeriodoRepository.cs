using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplication.Interface
{
    public interface IPeriodoRepository
    {
        Task<IEnumerable<Periodo>> ListarPeriodosAsync();
        Task InsertarPeriodoAsync(Periodo oPeriodo);
        Task EditarPeriodoAsync(Periodo oPeriodo);
        Task EliminarPeriodoAsync(int idPeriodo);
    }
}
