using Common;
using UserService.Models;

namespace UserService.Repository
{
    public interface IUserRepository :IRepository<Models.User>
    {
        Task<byte[]> GetUserImage(long id);
        Task ChangePassword(long id, string password);
    }
}
