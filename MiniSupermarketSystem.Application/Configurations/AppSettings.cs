namespace MiniSupermarketSystem.Application.Configurations;

public class AppSettings
{
    public NovusBankSettings NovusBank { get; set; }
    public int InventoryCheckIntervalMinutes { get; set; }
    public bool IsTest { get; set; }
    public string TestAccount { get; set; }
    public JwtSettings AppJwt { get; set; }
}

public class NovusBankSettings
{
    public string BaseUrl { get; set; }

    public string ApiKey { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public string TerminalId { get; set; }

    public string MerchantId { get; set; }

    public string BankCode { get; set; }
}

public class JwtSettings
{
    public string Secret { get; set; }
    public int ExpiryMinutes { get; set; }
}
