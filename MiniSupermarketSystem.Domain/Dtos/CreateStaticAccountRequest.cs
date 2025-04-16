using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniSupermarketSystem.Infrastructure.Services.Dtos;

namespace MiniSupermarketSystem.Domain.Dtos
{
    public record CreateStaticAccountRequest
    {
        public RequestHeader RequestHeader { get; init; }
        public string BankCode { get; init; }
        public string Description { get; init; }
        public string AccountName { get; init; }
    }

    public record RequestHeader
    {
        public string MerchantId { get; init; }
        public string TerminalId { get; init; }
        public string TraceId { get; init; }
    }

    public record CreateStaticAccountResponse
    {
        public ResponseHeader ResponseHeader { get; init; }
        public string DestinationBankCode { get; init; }
        public string DestinationBankName { get; init; }
        public string Description { get; init; }
        public string DestinationAccountName { get; init; }
        public string DestinationAccountNumber { get; init; }
        public string TransactionId { get; init; }
        public string SessionId { get; init; }
        public string DestinationBankLogo { get; init; }
    }
}
