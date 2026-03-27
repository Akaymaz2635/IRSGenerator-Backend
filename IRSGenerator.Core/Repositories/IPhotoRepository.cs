using IRSGenerator.Core.Entities;

namespace IRSGenerator.Core.Repositories;

public interface IPhotoRepository
{
    ValueTask<Photo?> GetByIdAsync(long id);
    Task<IEnumerable<Photo>> GetAllAsync();
    Task<Photo> AddAsync(Photo entity);
    Task DeleteAsync(Photo entity);
    Task<IEnumerable<Photo>> GetByInspectionAsync(long inspectionId);
    Task LinkDefectAsync(long photoId, long defectId);
    Task UnlinkDefectAsync(long photoId, long defectId);
}
