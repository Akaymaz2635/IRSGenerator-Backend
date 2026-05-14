using AutoMapper;
using MES.Application.Interfaces;
using MES.Application.Interfaces;
using MES.Domain.Dtos.Photo;
using MES.Domain.Entities;
using MES.Domain.Exceptions;
using MES.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MES.API.Services;

public class PhotoService : IPhotoService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;

    public PhotoService(IUnitOfWork uow, IMapper mapper, ApplicationDbContext context)
    {
        _uow     = uow;
        _mapper  = mapper;
        _context = context;
    }

    public async Task<IEnumerable<PhotoReadDto>> GetAllAsync(long? inspectionId = null, long? defectId = null)
    {
        IEnumerable<Photo> items;
        if (inspectionId.HasValue)
        {
            items = await _context.Photos
                .Include(p => p.PhotoDefects)
                .Where(p => p.InspectionId == inspectionId.Value)
                .ToListAsync();
        }
        else if (defectId.HasValue)
        {
            items = await _context.Photos
                .Include(p => p.PhotoDefects)
                .Where(p => p.PhotoDefects.Any(pd => pd.DefectId == defectId.Value))
                .ToListAsync();
        }
        else
        {
            items = await _uow.Photos.GetAllAsync();
        }
        return _mapper.Map<IEnumerable<PhotoReadDto>>(items);
    }

    public async Task<PhotoReadDto?> GetByIdAsync(long id)
    {
        var entity = await _uow.Photos.GetByIdAsync(id);
        return entity is null ? null : _mapper.Map<PhotoReadDto>(entity);
    }

    public async Task<(byte[] Content, string ContentType, string FileName)> GetFileAsync(long id, string uploadPath)
    {
        var entity = await _uow.Photos.GetByIdAsync(id)
            ?? throw new EntityNotFoundException<Photo>(id);

        var fullPath = Path.Combine(uploadPath, entity.Filepath);
        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"Photo file not found: {entity.Filepath}");

        var bytes = await File.ReadAllBytesAsync(fullPath);
        var ext = Path.GetExtension(entity.Filename).ToLower();
        var contentType = ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png"            => "image/png",
            ".gif"            => "image/gif",
            _                 => "application/octet-stream"
        };
        return (bytes, contentType, entity.Filename);
    }

    public async Task<PhotoReadDto> UploadAsync(Stream fileStream, string fileName, string contentType, long inspectionId, string uploadPath)
    {
        var subfolder = Path.Combine("photos", inspectionId.ToString());
        var saveDir   = Path.Combine(uploadPath, subfolder);
        Directory.CreateDirectory(saveDir);

        var uniqueName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        var savePath   = Path.Combine(saveDir, uniqueName);

        await using (var fs = File.Create(savePath))
            await fileStream.CopyToAsync(fs);

        var entity = new Photo
        {
            InspectionId = inspectionId,
            Filename     = fileName,
            Filepath     = Path.Combine(subfolder, uniqueName),
        };

        var created = await _uow.Photos.AddAsync(entity);
        await _uow.CommitAsync();
        return _mapper.Map<PhotoReadDto>(created);
    }

    public async Task SetDefectsAsync(long photoId, IEnumerable<long> defectIds)
    {
        var existing = await _context.PhotoDefects
            .Where(pd => pd.PhotoId == photoId)
            .ToListAsync();

        _context.PhotoDefects.RemoveRange(existing);

        var newLinks = defectIds
            .Distinct()
            .Select(did => new PhotoDefect { PhotoId = photoId, DefectId = did });

        await _context.PhotoDefects.AddRangeAsync(newLinks);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id, string uploadPath)
    {
        var entity = await _uow.Photos.GetByIdAsync(id)
            ?? throw new EntityNotFoundException<Photo>(id);

        var fullPath = Path.Combine(uploadPath, entity.Filepath);
        if (File.Exists(fullPath))
            File.Delete(fullPath);

        await _uow.Photos.RemoveAsync(entity.Id);
        await _uow.CommitAsync();
    }
}
