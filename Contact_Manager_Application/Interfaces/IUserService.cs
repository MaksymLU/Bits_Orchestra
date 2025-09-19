using Contact_Manager_Application.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contact_Manager_Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<bool> UpdateUserAsync(int id, User updatedUser);
        Task<bool> DeleteUserAsync(int id);
    }
}
