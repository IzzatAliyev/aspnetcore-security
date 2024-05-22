// Copyright (c) IUA. All rights reserved.

namespace jwt.Controllers;

using jwt.Data;
using jwt.Enums;
using jwt.Models;
using jwt.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Users controller.
/// </summary>
[ApiController]
[Route("/api/users")]
public class UsersController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, TokenService tokenService, ILogger<UsersController> logger)
: ControllerBase
{
    private readonly UserManager<ApplicationUser> userManager = userManager;
    private readonly ApplicationDbContext context = context;
    private readonly TokenService tokenService = tokenService;
    private readonly ILogger logger = logger;

    /// <summary>
    /// Register.
    /// </summary>
    /// <param name="request">the body request.</param>
    /// <returns>the response.</returns>
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(RegistrationRequest request)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var result = await this.userManager.CreateAsync(
            new ApplicationUser { UserName = request.Username, Email = request.Email, Role = Role.User },
            request.Password!);

        if (result.Succeeded)
        {
            request.Password = string.Empty;
            return this.CreatedAtAction(nameof(this.Register), new { email = request.Email, role = request.Role }, request);
        }

        foreach (var error in result.Errors)
        {
            this.ModelState.AddModelError(error.Code, error.Description);
        }

        return this.BadRequest(this.ModelState);
    }

    /// <summary>
    /// Authenticate.
    /// </summary>
    /// <param name="request">the body request.</param>
    /// <returns>the authentication response.</returns>
    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<AuthResponse>> Authenticate([FromBody] AuthRequest request)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var managedUser = await this.userManager.FindByEmailAsync(request.Email!);
        if (managedUser == null)
        {
            return this.BadRequest("Bad credentials");
        }

        var isPasswordValid = await this.userManager.CheckPasswordAsync(managedUser, request.Password!);
        if (!isPasswordValid)
        {
            return this.BadRequest("Bad credentials");
        }

        var userInDb = this.context.Users.FirstOrDefault(u => u.Email == request.Email);

        if (userInDb is null)
        {
            return this.Unauthorized();
        }

        var accessToken = this.tokenService.CreateToken(userInDb);
        await this.context.SaveChangesAsync();

        return this.Ok(new AuthResponse
        {
            Username = userInDb.UserName,
            Email = userInDb.Email,
            Token = accessToken,
        });
    }
}
