using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniSupermarketSystem.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string TransactionReference { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime PaymentDate { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountPaid { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; } // "Pending", "Completed", "Failed"
        public string BankAccountNumber { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
