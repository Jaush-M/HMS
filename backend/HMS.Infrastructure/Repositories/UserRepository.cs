// Author: Salaams
using HMS.Application.Interfaces.Repositories;
using HMS.Domain.Entities;
using HMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HMS.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly HmsDbContext _db;
    public UserRepository(HmsDbContext db) => _db = db;

    public async Task<User?> GetByEmailAsync(string email) =>
        await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<GuestUser?> GetGuestByIdAsync(int id) =>
        await _db.Guests.FirstOrDefaultAsync(g => g.Id == id);

    public async Task<StaffUser?> GetStaffByIdAsync(int id) =>
        await _db.Staff.FirstOrDefaultAsync(s => s.Id == id);

    public async Task<IEnumerable<StaffUser>> GetAllStaffAsync() =>
        await _db.Staff
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToListAsync();

    public async Task UpdateAsync(User user)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
    }
}
