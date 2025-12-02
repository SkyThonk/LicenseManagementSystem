using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Common.Application.Authentication.Dto;
using Common.Application.Interfaces.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Common.Infrastructure.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;
    
    public JwtTokenGenerator(IOptions<JwtSettings> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value;
    }

    public string GenerateToken(Guid userId, string email, string userName, IEnumerable<UserRoleContext> roles)
    {
        // Retrieve the JWT secret from environment variables and encode it
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

        // Create claims for user identity and role
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, userName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

        if (roles.Any())
        {
            var rolesToSerialize = roles.Select(r => new { r.Id, r.Code });
            var rolesJson = JsonSerializer.Serialize(rolesToSerialize);
            claims.Add(new Claim("roles", rolesJson, JsonClaimValueTypes.JsonArray));
        }

        // Create an identity from the claims
        var identity = new ClaimsIdentity(claims);

        // Describe the token settings
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            Subject = identity,
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        // Create a JWT security token
        var token = new JwtSecurityTokenHandler().CreateJwtSecurityToken(tokenDescriptor);

        // Write the token as a string and return it
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString() + Guid.NewGuid().ToString();
    }

    public ClaimsPrincipal ValidateAndDecryptToken(string token)
    {
        // Retrieve the JWT secret from environment variables and encode it
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

        try
        {
            // Create a token handler and validate the token
            var tokenHandler = new JwtSecurityTokenHandler();
            var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            }, out SecurityToken validatedToken);

            // Return the claims principal
            return claimsPrincipal;
        }
        catch (Exception ex)
        {
            // Token validation failed
            throw new SecurityTokenException("Token validation failed", ex);
        }
    }
}
