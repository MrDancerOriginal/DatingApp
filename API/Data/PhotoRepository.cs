using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class PhotoRepository : IPhotoRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public PhotoRepository(DataContext context, IMapper mapper)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<Photo> GetPhotoById(int id)
    {
        return await _context.Photos.IgnoreQueryFilters().SingleOrDefaultAsync(photo => photo.Id == id);
    }

    public async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos()
    {
        var query = _context.Photos
                .IgnoreQueryFilters()
                .Where(p => p.IsApproval == false)
                .ProjectTo<PhotoForApprovalDto>(_mapper.ConfigurationProvider);


        return await query.ToListAsync();
    }

    public void RemovePhoto(Photo photo)
    {
        _context.Photos.Remove(photo);
    }
}
