using System.Threading.Tasks;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    public interface IAuthRepository
    {
        //  Registering the user
        Task<User> Register(User user, string password);

        //  User to login to API
        Task<User> Login(string username, string password);

        //  To check if user actually exists
        Task<bool> UserExists(string username);

    }
        
}