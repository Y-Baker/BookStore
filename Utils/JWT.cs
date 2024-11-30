using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookStore.Utils;

public class JWT
{
    private readonly string _secretKey = "Arsenal -> North London Forever -> JOO";
    private readonly SymmetricSecurityKey _symmetricSecurityKey;
    private readonly SigningCredentials signingCredentials;
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

    public TimeSpan DefaultExpireAfter { get; set; } = new TimeSpan(3, 0, 0, 0);

    public JWT()
    {
        _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_secretKey));
        signingCredentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
    }

    public string GenerateToken(List<Claim> data, DateTime? expire = null)
    {
        expire ??= DateTime.UtcNow + DefaultExpireAfter;

        JwtSecurityToken token = new JwtSecurityToken(
            claims: data,
            expires: expire,
            signingCredentials: signingCredentials
            );

        string tokenstring = _jwtSecurityTokenHandler.WriteToken(token);

        return tokenstring;
    }
}
