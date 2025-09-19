using Contact_Manager_Application.Db;
using Contact_Manager_Application.Interfaces;
using Contact_Manager_Application.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contact_Manager_Application.Services
{
    public class UserService : IUserService
    {
        private readonly AdmissionsDbContext _context;

        public UserService(AdmissionsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<bool> UpdateUserAsync(int id, User updatedUser)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            user.Name = updatedUser.Name;
            user.DateOfBirth = updatedUser.DateOfBirth;
            user.Married = updatedUser.Married;
            user.Phone = updatedUser.Phone;
            user.Salary = updatedUser.Salary;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
