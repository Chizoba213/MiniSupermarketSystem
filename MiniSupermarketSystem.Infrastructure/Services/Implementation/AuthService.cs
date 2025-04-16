namespace MiniSupermarketSystem.Infrastructure.Services.Implementation;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MiniSupermarketSystem.Application.Configurations;
using MiniSupermarketSystem.Domain.Entities;
using MiniSupermarketSystem.Domain.Interfaces.IServices;
using MiniSupermarketSystem.Infrastructure.Persistence;


public class AuthService : IAuthService
{
    private readonly SupermarketDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly AppSettings _jwtSettings;
    private readonly ILogger<AuthService> _logger;
    private readonly DateTime requestTime = DateTime.UtcNow;


    public AuthService(SupermarketDbContext context, IMemoryCache cache, IOptions<AppSettings> jwtSettings, ILogger<AuthService> logger)
    {
        _context = context;
        _cache = cache;
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    public async Task<string> Authenticate(string username, string password)
    {
        try
        {
            _logger.LogInformation($"Authentication attempt for user: {username}");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                _logger.LogWarning($"User not found: {username}");
                return null;
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!isPasswordValid)
            {
                _logger.LogWarning($"Invalid password for user: {username}");
                return null;
            }

            var token = GenerateJwtToken(user);
            _cache.Set(token, user, TimeSpan.FromMinutes(30));

            _logger.LogInformation($"User authenticated successfully: {username}. Token generated.");
            return token;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Authentication failed for user: {username}");
            throw; 
        }
    }
    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.AppJwt.Secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("TerminalId", user.TerminalId),
            new Claim("MerchantId", user.MerchantId)
        }),
            Expires = DateTime.UtcNow.AddMinutes(5),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
