// Copyright (c) IUA. All rights reserved.

namespace jwt.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using jwt.Models;
using Microsoft.IdentityModel.Tokens;

/// <summary>
/// Token service.
/// </summary>
public class TokenService(ILogger<TokenService> logger, IConfiguration configuration)
: ITokenService
{
    private const int ExpirationMinutes = 60;
    private readonly ILogger<TokenService> logger = logger;
    private readonly IConfiguration configuration = configuration;

    /// <inheritdoc/>
    public string CreateToken(ApplicationUser user)
    {
        var expiration = DateTime.UtcNow.AddMinutes(ExpirationMinutes);
        var token = this.CreateJwtToken(
            this.CreateClaims(user),
            this.CreateSigningCredentials(),
            expiration);
        var tokenHandler = new JwtSecurityTokenHandler();

        this.logger.LogInformation("JWT Token created");

        return tokenHandler.WriteToken(token);
    }

    private JwtSecurityToken CreateJwtToken(List<Claim> claims, SigningCredentials credentials, DateTime expiration) =>
        new JwtSecurityToken(
            new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build().GetSection("TokenSettings")["ValidIssuer"],
            new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build().GetSection("TokenSettings")["ValidAudience"],
            claims,
            expires: expiration,
            signingCredentials: credentials);

    private List<Claim> CreateClaims(ApplicationUser user)
    {
        var jwtSub = this.configuration.GetSection("TokenSettings")["JwtRegisteredClaimNamesSub"];

        try
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, jwtSub!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
            };

            return claims;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private SigningCredentials CreateSigningCredentials()
    {
        var symmetricSecurityKey = this.configuration.GetSection("TokenSettings")["SymmetricSecurityKey"] ?? "defaultKey";

        return new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(symmetricSecurityKey)),
            SecurityAlgorithms.HmacSha256);
    }
}
