// Author: Salaams
using HMS.Domain.Entities;

namespace HMS.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<GuestUser?> GetGuestByIdAsync(int id);
    Task<StaffUser?> GetStaffByIdAsync(int id);
    Task<IEnumerable<StaffUser>> GetAllStaffAsync();
    Task AddAsync(GuestUser guest);
    Task UpdateAsync(User user);
}
