namespace MiniSupermarketSystem.Domain.Entities;
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string TerminalId { get; set; }
    public string MerchantId { get; set; }
}
