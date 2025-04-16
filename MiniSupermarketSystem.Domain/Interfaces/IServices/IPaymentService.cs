namespace MiniSupermarketSystem.Domain.Interfaces.IServices;
using System.Threading.Tasks;
using MiniSupermarketSystem.Domain.Dtos;
using MiniSupermarketSystem.Infrastructure.Services.Dtos;

public interface IPaymentService
    {
        Task<CreateStaticAccountResponse> GenerateBankAccountForPayment(decimal amount, string reference);
        Task<bool> VerifyPayment(string reference);
    }