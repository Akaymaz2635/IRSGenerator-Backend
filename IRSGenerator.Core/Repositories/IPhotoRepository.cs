using IRSGenerator.Core.Entities;

namespace IRSGenerator.Core.Repositories;

public interface IPhotoRepository : IBaseRepository<Photo>
{
    Task<IEnumerable<Photo>> GetByInspectionAsync(long inspectionId);
    Task LinkDefectAsync(long photoId, long defectId);
    Task UnlinkDefectAsync(long photoId, long defectId);
}
