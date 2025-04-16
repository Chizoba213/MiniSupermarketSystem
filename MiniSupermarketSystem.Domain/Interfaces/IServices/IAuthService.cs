using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MiniSupermarketSystem.Domain.Entities;

namespace MiniSupermarketSystem.Domain.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<string> Authenticate(string username, string password);
    }
}
