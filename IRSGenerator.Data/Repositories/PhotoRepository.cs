using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IRSGenerator.Data.Repositories;

public class PhotoRepository : BaseRepository<Photo>, IPhotoRepository
{
    public PhotoRepository(IRSGeneratorDbContext context) : base(context) { }

    public async Task DeleteAsync(Photo entity)
    {
        Context.Set<Photo>().Remove(entity);
        await Context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Photo>> GetByInspectionAsync(long inspectionId)
        => await Context.Set<Photo>()
            .Include(p => p.PhotoDefects)
            .Where(p => p.InspectionId == inspectionId)
            .ToListAsync();

    public async Task LinkDefectAsync(long photoId, long defectId)
    {
        var link = new PhotoDefect { PhotoId = photoId, DefectId = defectId };
        await Context.Set<PhotoDefect>().AddAsync(link);
        await Context.SaveChangesAsync();
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
