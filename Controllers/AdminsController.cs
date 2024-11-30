using AutoMapper;
using BookStore.DTOs.User;
using BookStore.Models;
using BookStore.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace BookStore.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class AdminsController : ControllerBase
{
    private const string AdminRoleName = "Admin";
    private readonly UserManager<IdentityUser> userManager;
    private readonly IMapper mapper;

    public AdminsController(UserManager<IdentityUser> userManager, IMapper _mapper)
    {
        this.userManager = userManager;
        mapper = _mapper;
    }

    [SwaggerOperation(
        Summary = "Register a new Admin",
        Description = "Requires Admin role to create a new Admin"
    )]
    [SwaggerResponse(201, "The Admin was created", typeof(AdminViewDTO))]
    [SwaggerResponse(400, "The Admin data is invalid")]
    [SwaggerResponse(401, "The user is not authorized to create an Admin, you must be an Admin")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Authorize(Roles = "Admin")]
    [HttpPost("register")]
    public IActionResult Register(AddAdminDTO? adminDTO)
    {
        if (adminDTO is null)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest();

        Admin admin = mapper.Map<Admin>(adminDTO);

        IdentityResult res = userManager.CreateAsync(admin, adminDTO.Password).Result;
        if (!res.Succeeded)
            return BadRequest(res.Errors);

        res = userManager.AddToRoleAsync(admin, AdminRoleName).Result;
        if (!res.Succeeded)
            return BadRequest(res.Errors);

        AdminViewDTO view = mapper.Map<AdminViewDTO>(admin);

        return CreatedAtAction(nameof(GetById), new { Id = view.Id }, view);
    }

    [SwaggerOperation(
        Summary = "Get Admin by Id",
        Description = "Get an Admin by their Id"
    )]
    [SwaggerResponse(200, "The Admin was found", typeof(AdminViewDTO))]
    [SwaggerResponse(404, "The Admin was not found")]
    [SwaggerResponse(401, "The user is not authorized to view an Admin, you must be logged in")]
    [Produces("application/json")]
    [HttpGet("{id}")]
    public IActionResult GetById(string id)
    {
        Admin? admin = userManager.GetUsersInRoleAsync(AdminRoleName).Result.OfType<Admin>().SingleOrDefault(a => a.Id == id);

        if (admin == null)
            return NotFound();

        AdminViewDTO view = mapper.Map<AdminViewDTO>(admin);

        return Ok(view);
    }
}
