// Author: Salaams
using HMS.Application.Interfaces.Repositories;
using HMS.Domain.Entities;
using HMS.Infrastructure.Persistence;

namespace HMS.Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly HmsDbContext _db;
    public AuditLogRepository(HmsDbContext db) => _db = db;

    public async Task AddAsync(AuditLog log)
    {
        _db.AuditLogs.Add(log);
        await _db.SaveChangesAsync();
    }
}
