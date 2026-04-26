// Author: Salaams (smoke-test entity, to be removed in Phase 2)
namespace HMS.Domain.Entities;

public class PingEntity
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}