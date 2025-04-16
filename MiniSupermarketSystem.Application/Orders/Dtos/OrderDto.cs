using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniSupermarketSystem.Application.Order.Dtos
{
    public class OrderDto
    {
        public string TransactionReference { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public string BankName { get; set; }
        public string BankAccountName { get; set; }
        public string BankAccountNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; }
        public decimal AmountPaid { get; set; }
    }
}
