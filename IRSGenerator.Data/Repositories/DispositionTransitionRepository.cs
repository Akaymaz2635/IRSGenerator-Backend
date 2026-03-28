using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IRSGenerator.Data.Repositories;

public class DispositionTransitionRepository : BaseRepository<DispositionTransition>, IDispositionTransitionRepository
{
    public DispositionTransitionRepository(IRSGeneratorDbContext context) : base(context) { }

    public new async ValueTask<DispositionTransition?> GetByIdAsync(long id)
        => await Context.Set<DispositionTransition>().FirstOrDefaultAsync(e => e.Id == id);

    public new async Task<IEnumerable<DispositionTransition>> GetAllAsync()
        => await Context.Set<DispositionTransition>().ToListAsync();

    public async Task<IEnumerable<DispositionTransition>> GetByFromCodeAsync(string? fromCode)
        => await Context.Set<DispositionTransition>()
            .Where(t => t.FromCode == fromCode)
            .ToListAsync();

    public async Task<IEnumerable<string>> GetAllowedNextCodesAsync(string? currentCode)
        => await Context.Set<DispositionTransition>()
            .Where(t => t.FromCode == currentCode)
            .Select(t => t.ToCode)
            .ToListAsync();

    public async Task DeleteAsync(DispositionTransition entity)
    {
        Context.Set<DispositionTransition>().Remove(entity);
        await Context.SaveChangesAsync();
    }

    public async Task BulkSetAsync(string? fromCode, IEnumerable<string> toCodes)
    {
        // Mevcut geçişleri sil
        var existing = await Context.Set<DispositionTransition>()
            .Where(t => t.FromCode == fromCode)
            .ToListAsync();
        Context.Set<DispositionTransition>().RemoveRange(existing);

        // Yenilerini ekle
        var now = DateTime.UtcNow;
        var newRows = toCodes.Distinct().Select(code => new DispositionTransition
        {
            FromCode  = fromCode,
            ToCode    = code,
            CreatedAt = now,
            UpdatedAt = now
        });
        await Context.Set<DispositionTransition>().AddRangeAsync(newRows);
        await Context.SaveChangesAsync();
    }
}
