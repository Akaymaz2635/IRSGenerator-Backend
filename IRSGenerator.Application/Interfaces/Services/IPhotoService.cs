using MES.Domain.Dtos.Photo;

namespace MES.Application.Interfaces;

public interface IPhotoService
{
    Task<IEnumerable<PhotoReadDto>> GetAllAsync(long? inspectionId = null, long? defectId = null);
    Task<PhotoReadDto?> GetByIdAsync(long id);
    Task<(byte[] Content, string ContentType, string FileName)> GetFileAsync(long id, string uploadPath);
    Task<PhotoReadDto> UploadAsync(Stream fileStream, string fileName, string contentType, long inspectionId, string uploadPath);
    Task SetDefectsAsync(long photoId, IEnumerable<long> defectIds);
    Task DeleteAsync(long id, string uploadPath);
}
