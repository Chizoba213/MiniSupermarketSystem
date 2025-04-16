namespace MiniSupermarketSystem.ConsoleApp;
using System;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;


public class ConsoleAuthHandler
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;

    public ConsoleAuthHandler(HttpClient httpClient, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _cache = cache;
    }

    public async Task<bool> AuthenticateUser()
    {
        Console.WriteLine("MiniSupermarket System Login");
        Console.Write("Username: ");
        var username = Console.ReadLine();
        Console.Write("Password: ");
        var password = ReadPassword();

        var response = await _httpClient.PostAsJsonAsync("auth/login", new { username, password });

        if (response.IsSuccessStatusCode)
        {
            // PROPER TOKEN EXTRACTION
            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
            var token = authResponse.Token; // Just the raw token string

            // Store in cache
            _cache.Set("auth_token", token, TimeSpan.FromMinutes(20)); // Match token expiry

            return true;
        }

        Console.WriteLine("Authentication failed");
        return false;
    }

    private string ReadPassword()
    {
        var pass = string.Empty;
        ConsoleKey key;

        do
        {
            var keyInfo = Console.ReadKey(intercept: true);
            key = keyInfo.Key;

            if (key == ConsoleKey.Backspace && pass.Length > 0)
            {
                Console.Write("\b \b");
                pass = pass[0..^1];
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                Console.Write("*");
                pass += keyInfo.KeyChar;
            }
        } while (key != ConsoleKey.Enter);

        Console.WriteLine();
        return pass;
    }

    public class AuthResponse
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
