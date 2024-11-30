using BookStore.DTOs;
using BookStore.DTOs.User;
using BookStore.Models;
using BookStore.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Immutable;
using System.Security.Claims;

namespace BookStore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private const string AdminRoleName = "Admin";

    private readonly UserManager<IdentityUser> userManager;
    private readonly SignInManager<IdentityUser> signInManger;
    private readonly JWT _jwt;

    public UsersController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManger, JWT _jwt)
    {
        this.userManager = userManager;
        this.signInManger = signInManger;
        this._jwt = _jwt;
    }

    [SwaggerOperation(Summary = "Register a new User", Description = "no authentication required")]
    [SwaggerResponse(201, "The User was created and token returned", typeof(string))]
    [SwaggerResponse(400, "The User data is invalid")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [HttpPost("login")]
    public IActionResult Login(LoginDTO loginDTO)
    {
        var res = signInManger.PasswordSignInAsync(loginDTO.Username, loginDTO.Password, false, false).Result;
        if (!res.Succeeded)
            return Unauthorized("Invalid Username or Password");

        IdentityUser user = userManager.FindByNameAsync(loginDTO.Username).Result!;
        List<string>? roles = userManager.GetRolesAsync(user).Result.ToList();

        List<Claim> userData = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? "")
        };
        foreach (string role in roles)
        {
            userData.Add(new Claim(ClaimTypes.Role, role));
        }

        string token = _jwt.GenerateToken(userData);
        return Ok(token);
    }

    [SwaggerOperation(Summary = "Register a new User", Description = "Change the password of the logged in user")]
    [SwaggerResponse(204, "The password was changed")]
    [SwaggerResponse(400, "The password data is invalid")]
    [SwaggerResponse(401, "The user is not authorized to change the password")]
    [SwaggerResponse(404, "The user was not found")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Authorize]
    [HttpPut("profile/changepassword")]
    public IActionResult ChangePassword(ChangePasswordDTO changePasswordDTO)
    {
        if (changePasswordDTO == null) return BadRequest();
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (User.Identity?.Name == null) return Unauthorized();

        IdentityUser? user = userManager.FindByNameAsync(User.Identity.Name).Result;
        if (user == null) return NotFound();

        IdentityResult res = userManager.ChangePasswordAsync(user, changePasswordDTO.OldPassword, changePasswordDTO.NewPassword).Result;
        if (!res.Succeeded) return BadRequest(res.Errors);

        return NoContent();
    }

    [SwaggerOperation(Summary = "Delete a User by Id", Description = "Requires Admin role or the User Id must match the logged in user")]
    [SwaggerResponse(204, "The User was deleted")]
    [SwaggerResponse(404, "The User was not found")]
    [SwaggerResponse(401, "The user is not authorized to delete a User")]
    [Produces("application/json")]
    [Authorize]
    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        ClaimsIdentity? identity = User.Identity as ClaimsIdentity;
        if (identity == null)
            return Unauthorized();
        List<Claim>? claims = identity.Claims.ToList();

        if (!claims.Any(c => c.Type == ClaimTypes.Role && c.Value == AdminRoleName))
        {
            Claim? claimId = claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (claimId == null || claimId.Value != id)
                return Unauthorized();
        }

        IdentityUser? user = userManager.FindByIdAsync(id).Result;
        if (user == null) return NotFound();

        IdentityResult res = userManager.DeleteAsync(user).Result;
        if (!res.Succeeded) return BadRequest(res.Errors);

        return NoContent();
    }

    [SwaggerOperation(Summary = "Logout a User", Description = "Log the user out")]
    [SwaggerResponse(204, "The User was logged out")]
    [SwaggerResponse(401, "The user is not authorized to logout")]
    [Produces("application/json")]
    [Authorize]
    [HttpGet("logout")]
    public IActionResult Logout()
    {
        signInManger.SignOutAsync();
        return NoContent();
    }
}
