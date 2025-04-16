namespace MiniSupermarketSystem.Infrastructure.Services.Dtos;
using System;

public class AuthResponse
{
    public ResponseHeader ResponseHeader { get; set; }
    public string Key { get; set; }
    public string Token { get; set; }
    public DateTime ExpiryDate { get; set; }
}

public class ResponseHeader
{
    public string? ResponseCode { get; set; }
    public string? ResponseMessage { get; set; }
    public string? TraceId { get; init; }
}