using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Dorak.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class PaymentRepository : BaseRepository<Payment>
    {
        public PaymentRepository(DorakContext dbContext) : base(dbContext)
        {
        }
        public Payment GetPaymentById(int paymentId)
        {
            return DbContext.Payments
                .FirstOrDefault(p => p.PaymentId == paymentId);
        }
        public List<Payment> GetPaymentsByClientId(string clientId)
        {
            return DbContext.Payments
                .Where(p => p.ClientId == clientId)
                .OrderByDescending(p => p.TransactionDate)
                .ToList();
        }


        public string ProcessPayment(int appointmentId, string clientId, decimal amount, string paymentMethod)
        {
            var transactionId = Guid.NewGuid().ToString(); 

            var payment = new Payment
            {
                AppointmentId = appointmentId,
                ClientId = clientId,
                Amount = amount,
                PaymentMethod = paymentMethod,
                PaymentStatus = "Completed",
                RefundStatus = "None",
                TransactionDate = DateTime.Now,
                TransactionId = transactionId
            };

            DbContext.Payments.Add(payment);
            DbContext.SaveChanges();

            return $"Payment Successful - Transaction Number:: {transactionId}";
        }

        public string RefundPayment(int paymentId)
        {
            var payment = GetPaymentById(paymentId);

            if (payment == null)
                return "Payment process does not exist";

            if (payment.PaymentStatus != "Completed" || payment.RefundStatus == "Refunded")
                return "This process cannot be undone.";

            payment.PaymentStatus = "Refunded";
            payment.RefundStatus = "Refunded";

            DbContext.SaveChanges();

            return "The amount has been successfully refunded.";
        }

        public decimal GetTotalPaymentsForDate(DateTime date)
        {
            return DbContext.Payments
                .Where(p => p.TransactionDate.Date == date.Date && p.PaymentStatus == "Completed")
                .Sum(p => p.Amount);
        }
    }
}
