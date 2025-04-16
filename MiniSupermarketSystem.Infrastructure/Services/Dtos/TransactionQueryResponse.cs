namespace MiniSupermarketSystem.Infrastructure.Services.Dtos;
using System;
using System.Text.Json.Serialization;

public class TransactionQueryResponse
{
    [JsonPropertyName("responseHeader")]
    public ResponseHeader ResponseHeader { get; set; } = new ResponseHeader();

    [JsonPropertyName("transactionResponseCode")]
    public string? TransactionResponseCode { get; set; }

    [JsonPropertyName("transactionResponseMessage")]
    public string? TransactionResponseMessage { get; set; }

    [JsonPropertyName("sourceBankCode")]
    public string? SourceBankCode { get; set; }

    [JsonPropertyName("sourceAccountName")]
    public string? SourceAccountName { get; set; }

    [JsonPropertyName("sourceBankName")]
    public string? SourceBankName { get; set; }

    [JsonPropertyName("sourceBankAccountNumber")]
    public string? SourceBankAccountNumber { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("destinationBankCode")]
    public string? DestinationBankCode { get; set; }

    [JsonPropertyName("destinationBankName")]
    public string? DestinationBankName { get; set; }

    [JsonPropertyName("destinationAccountName")]
    public string? DestinationAccountName { get; set; }

    [JsonPropertyName("destinationAccountNumber")]
    public string? DestinationAccountNumber { get; set; }

    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }

    [JsonPropertyName("transactionId")]
    public string? TransactionId { get; set; }

    [JsonPropertyName("sessionId")]
    public string? SessionId { get; set; }

    [JsonPropertyName("terminal")]
    public string? Terminal { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("paidAmount")]
    public decimal PaidAmount { get; set; }

    [JsonPropertyName("paymentDate")]
    public DateTime? PaymentDate { get; set; }
}