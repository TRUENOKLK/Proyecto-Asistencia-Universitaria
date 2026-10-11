using System.Threading.Tasks;

namespace Aplication.Interface
{
   
    public interface IUsuarioRepository
    {
        Task<bool> ExisteEmailAsync(string email);
    }
}
