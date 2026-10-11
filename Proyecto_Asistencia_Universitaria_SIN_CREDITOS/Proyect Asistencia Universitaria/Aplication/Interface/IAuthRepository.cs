using Domain.Entities;
using System.Threading.Tasks;

namespace Aplication.Interface
{
    public interface IAuthRepository
    {
       
        Task<Usuario?> ObtenerPorEmailAsync(string email);
    }
}
