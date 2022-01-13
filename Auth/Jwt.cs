using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MinimalAPI.Auth;

public static class Jwt
{
	private static readonly string JWT_SECRET = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "689F0518-627D-41C3-A417-BAA9FC31D537";
	private static readonly SymmetricSecurityKey SecurityKey = new(Encoding.ASCII.GetBytes(JWT_SECRET));
	private static readonly JwtSecurityTokenHandler SecurityHandler = new();
	public static readonly TokenValidationParameters ValidationParams = new()
	{
		ValidateIssuerSigningKey = true,
		ValidateIssuer = false,
		ValidateAudience = false,
		//ValidIssuer = "http://issuer.site",
		//ValidAudience =  "http://someaudience.site",
		IssuerSigningKey = SecurityKey,
		ValidateLifetime = true
	};

	/// <summary>
	/// Generated token by default will be valid for one month
	/// </summary>
	/// <param name="user"></param>
	/// <param name="monthsValidity"></param>
	/// <returns></returns>
	public static string GenerateToken(ApiUser user, int monthsValidity = 1)
	{
		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(new Claim[]
			{
					new (ClaimTypes.UserData, JsonSerializer.Serialize(user)),
			}),
			Expires = DateTime.UtcNow.AddMonths(monthsValidity),
			// Audience = "http://someaudience.site",
			// Issuer = "http://issuer.site",
			SigningCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256Signature)
		};

		var token = SecurityHandler.CreateToken(tokenDescriptor);

		return SecurityHandler.WriteToken(token);
	}

	private static IEnumerable<Claim> GetClaims(string token)
	{
		const string bearer = "Bearer ";
		if (token.StartsWith(bearer))
			token = token[bearer.Length..];

		var claimsPrincipal = SecurityHandler.ValidateToken(token, ValidationParams, out _);

		return claimsPrincipal.Claims;
	}

	private static ApiUser? GetUserData(Claim? userDataClaim)
	{
		return userDataClaim is null ? null : JsonSerializer.Deserialize<ApiUser>(userDataClaim.Value);
	}

	public static ApiUser? GetUserData(IEnumerable<Claim> claims)
	{
		var userData = claims.Where(c => c.Type == ClaimTypes.UserData).SingleOrDefault();

		return GetUserData(userData);
	}

	public static ApiUser? GetUserData(string token)
	{
		var claims = GetClaims(token);

		return GetUserData(claims);
	}

	public static ApiUser? GetUserData(HttpContext context)
	{
		var auth = context.Request.Headers["Authorization"];

		return GetUserData(auth);
	}
}

public class ApiUser
{
	public int Id { get; set; }

	public string? Email { get; set; }

	public string? Name { get; set; }
}