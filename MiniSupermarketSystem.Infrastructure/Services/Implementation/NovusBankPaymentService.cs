namespace MiniSupermarketSystem.Infrastructure.Services.Implementation;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiniSupermarketSystem.Application.Configurations;
using MiniSupermarketSystem.Domain.Dtos;
using MiniSupermarketSystem.Domain.Interfaces.IServices;
using MiniSupermarketSystem.Infrastructure.Services.Dtos;
using Newtonsoft.Json;

public class NovusBankPaymentService : IPaymentService
{
    private readonly HttpClient _httpClient;
    private readonly AppSettings _settings;
    private readonly ILogger<NovusBankPaymentService> _logger;
    private string _authToken;
    private DateTime _tokenExpiry;
    private readonly DateTime requestTime = DateTime.UtcNow;

    public NovusBankPaymentService(
        HttpClient httpClient,
        IOptions<AppSettings> appSettings,
        ILogger<NovusBankPaymentService> logger)
    {
        _httpClient = httpClient;
        _settings = appSettings.Value;
        _logger = logger;
        _httpClient.BaseAddress = new Uri(_settings.NovusBank.BaseUrl);
    }

    public async Task<CreateStaticAccountResponse> GenerateBankAccountForPayment(decimal amount, string reference)
    {
        using (_logger.BeginScope(new { TransactionReference = reference, Amount = amount }))
        {
            try
            {
                _logger.LogInformation("Generating bank account for payment");

                await EnsureValidTokenAsync();

                var request = new CreateStaticAccountRequest
                {
                    RequestHeader = new RequestHeader
                    {
                        MerchantId = _settings.NovusBank.MerchantId,
                        TerminalId = _settings.NovusBank.TerminalId,
                        TraceId = reference,
                    },
                    Description = "Account Creation",
                    AccountName = _settings.NovusBank.Username,
                    BankCode = _settings.NovusBank.BankCode,
                };

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, "api/CreateStaticAccount")
                {
                    Content = JsonContent.Create(request),
                    Headers = { { "Authorization", $"Bearer {_authToken}" } }
                };

                var response = await _httpClient.SendAsync(requestMessage);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Account creation failed. Status: {StatusCode}, Response: {ErrorResponse}",
                        response.StatusCode, errorContent);
                    response.EnsureSuccessStatusCode();
                }

                var content = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Account creation response: {Response}", content);

                var result = await response.Content.ReadFromJsonAsync<CreateStaticAccountResponse>();

                _logger.LogInformation("Successfully generated account {AccountNumber} at {BankName}",
                    result.DestinationAccountNumber, result.DestinationBankName);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate bank account for payment");
                throw;
            }
        }
    }

    private async Task EnsureValidTokenAsync()
    {
        try
        {
            if (!string.IsNullOrEmpty(_authToken) && _tokenExpiry > DateTime.UtcNow.AddMinutes(1))
            {
                _logger.LogDebug("Using cached auth token (expires at {Expiry})", _tokenExpiry);
                return;
            }

            _logger.LogInformation("Acquiring new auth token");
            var authResponse = await GetAuthTokenAsync();
            _authToken = authResponse.Token;
            _tokenExpiry = authResponse.ExpiryDate;

            _logger.LogInformation("Auth token acquired (expires at {Expiry})", _tokenExpiry);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to obtain authentication token");
            throw;
        }
    }

    private async Task<AuthResponse> GetAuthTokenAsync()
    {
        try
        {
            var authRequest = new
            {
                Username = _settings.NovusBank.Username,
                Password = _settings.NovusBank.Password,
                TerminalId = _settings.NovusBank.TerminalId
            };

            var response = await _httpClient.PostAsJsonAsync("api/Auth", authRequest);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Authentication failed. Status: {StatusCode}, Response: {ErrorResponse}",
                    response.StatusCode, errorContent);
                response.EnsureSuccessStatusCode();
            }

            var authResult = await response.Content.ReadFromJsonAsync<AuthResponse>();

            if (authResult.ResponseHeader.ResponseCode != "00")
            {
                _logger.LogError("Authentication rejected by bank: {Code} - {Message}",
                    authResult.ResponseHeader.ResponseCode,
                    authResult.ResponseHeader.ResponseMessage);
                throw new Exception($"Authentication failed: {authResult.ResponseHeader.ResponseMessage}");
            }

            return authResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during authentication");
            throw;
        }
    }

    public async Task<bool> VerifyPayment(string referenceId)
    {
        using (_logger.BeginScope(new { TransactionReference = referenceId }))
        {
            try
            {
                _logger.LogInformation("Verifying payment status");

                await EnsureValidTokenAsync();

                var request = new TransactionQueryRequest
                {
                    RequestHeader = new RequestHeader
                    {
                        MerchantId = _settings.NovusBank.MerchantId,
                        TerminalId = _settings.NovusBank.TerminalId,
                        TraceId = referenceId
                    }
                };

                _logger.LogDebug("Sending payment verification request: {Request}",
                    JsonConvert.SerializeObject(request, Formatting.Indented));

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, "api/transactionQuery")
                {
                    Content = JsonContent.Create(request),
                    Headers = { { "Authorization", $"Bearer {_authToken}" } }
                };

                var response = await _httpClient.SendAsync(requestMessage);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Payment verification failed. Status: {StatusCode}", response.StatusCode);
                    return false;
                }

                var content = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Payment verification response: {Response}", content);

                var result = JsonConvert.DeserializeObject<TransactionQueryResponse>(content);

                bool isVerified = result?.ResponseHeader?.ResponseCode == "00" &&
                                result.TransactionResponseCode == "00";

                _logger.LogInformation("Payment verification result: {IsVerified}", isVerified);

                return isVerified;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying payment");
                return false;
            }
        }
    }

    private class NovusBankResponse
    {
        public string AccountNumber { get; set; }
    }

    private class NovusBankVerificationResponse
    {
        public bool IsVerified { get; set; }
    }
}