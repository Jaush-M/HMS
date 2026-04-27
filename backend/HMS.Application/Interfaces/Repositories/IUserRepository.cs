// Author: Salaams
using HMS.Domain.Entities;

namespace HMS.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<GuestUser?> GetGuestByIdAsync(int id);
    Task<StaffUser?> GetStaffByIdAsync(int id);
    Task<IEnumerable<StaffUser>> GetAllStaffAsync();
    Task UpdateAsync(User user);
}
