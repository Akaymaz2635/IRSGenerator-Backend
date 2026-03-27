using Microsoft.EntityFrameworkCore;
using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;

namespace IRSGenerator.Data.Repositories;

public class PhotoRepository : BaseRepository<Photo>, IPhotoRepository
{
    public PhotoRepository(IRSGeneratorDbContext context) : base(context) { }

    public async Task<IEnumerable<Photo>> GetByInspectionAsync(long inspectionId)
        => await Context.Set<Photo>()
            .Include(p => p.PhotoDefects)
            .Where(p => p.InspectionId == inspectionId)
            .ToListAsync();

    public async Task LinkDefectAsync(long photoId, long defectId)
    {
        var exists = await Context.Set<PhotoDefect>()
            .AnyAsync(pd => pd.PhotoId == photoId && pd.DefectId == defectId);
        if (!exists)
        {
            Context.Set<PhotoDefect>().Add(new PhotoDefect { PhotoId = photoId, DefectId = defectId });
            await Context.SaveChangesAsync();
        }
    }

    public async Task UnlinkDefectAsync(long photoId, long defectId)
    {
        var link = await Context.Set<PhotoDefect>()
            .FirstOrDefaultAsync(pd => pd.PhotoId == photoId && pd.DefectId == defectId);
        if (link != null)
        {
            Context.Set<PhotoDefect>().Remove(link);
            await Context.SaveChangesAsync();
        }
    }
}
