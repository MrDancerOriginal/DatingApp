using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminController : BaseApiController
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IUnitOfWork _uow;
    private readonly IPhotoService _photoService;

    public AdminController(IUnitOfWork uow, UserManager<AppUser> userManager, IPhotoService photoService)
    {
        _photoService = photoService;
        _uow = uow;
        _userManager = userManager;
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        var users = await _userManager.Users
            .OrderBy(u => u.UserName)
            .Select(u => new
            {
                u.Id,
                Username = u.UserName,
                Roles = u.UserRoles.Select(r => r.Role.Name).ToList(),

            })
            .ToListAsync();

        return Ok(users);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPut("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
    {

        if (string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role.");

        var selectedRoles = roles.Split(",").ToArray();

        var user = await _userManager.FindByNameAsync(username);

        if (user is null) return NotFound();

        var userRoles = await _userManager.GetRolesAsync(user);

        var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

        if (!result.Succeeded) return BadRequest("Failed to add to roles");

        result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

        if (!result.Succeeded) return BadRequest("Failed to remove from roles.");

        return Ok(await _userManager.GetRolesAsync(user));
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")]
    public async Task<ActionResult> GetPhotosForModeration()
    {
        return Ok(await _uow.PhotoRepository.GetUnapprovedPhotos());
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPut("approve-photo/{photoId}")]
    public async Task<ActionResult> ApprovePhoto(int photoId)
    {
        var photo = await _uow.PhotoRepository.GetPhotoById(photoId);

        if (photo is null) return BadRequest("Failed to get photo");

        photo.IsApproval = true;

        var user = await _userManager.FindByIdAsync(photo.AppUserId.ToString());

        if (!user.Photos.Any(x => x.IsMain)) photo.IsMain = true;

        if (await _uow.Complete()) return Ok();

        return BadRequest("Failed to approve photo");
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpDelete("reject-photo/{photoId}")]
    public async Task<ActionResult> RejectPhoto(int photoId)
    {
        var photo = await _uow.PhotoRepository.GetPhotoById(photoId);

        if (photo is null) BadRequest("Failed to get photo");

        if (photo.PublicId is not null)
            await _photoService.DeletePhotoAsync(photo.PublicId);

        _uow.PhotoRepository.RemovePhoto(photo);

        if (await _uow.Complete()) return Ok();

        return BadRequest("Failed to approve photo");
    }


}
