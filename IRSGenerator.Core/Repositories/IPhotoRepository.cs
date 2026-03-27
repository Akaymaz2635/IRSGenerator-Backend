using IRSGenerator.Core.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace IRSGenerator.Core.Repositories;

public interface IPhotoRepository
{
    ValueTask<Photo?> GetByIdAsync(long id, Func<IQueryable<Photo>, IIncludableQueryable<Photo, object>>? include = null);
    Task<IEnumerable<Photo>> GetAllAsync(Func<IQueryable<Photo>, IIncludableQueryable<Photo, object>>? include = null);
    Task<Photo> AddAsync(Photo entity);
    Task DeleteAsync(Photo entity);
    Task<IEnumerable<Photo>> GetByInspectionAsync(long inspectionId);
    Task LinkDefectAsync(long photoId, long defectId);
    Task UnlinkDefectAsync(long photoId, long defectId);
}
