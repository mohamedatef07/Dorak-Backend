using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Stripe;
using System.Threading.Tasks;
using Repositories;
using Data;
using Dorak.Models;
using Microsoft.Extensions.Configuration;

namespace Services
{
    public class PaymentServices
    {
        private readonly PaymentRepository paymentRepository;
        private readonly CommitData commitData;
        private readonly IConfiguration configuration;

        public PaymentServices(PaymentRepository _paymentRepository,CommitData _commitData , IConfiguration _configuration)
        {
            paymentRepository = _paymentRepository;
            commitData = _commitData;
            configuration = _configuration;
        }
        public async Task<Charge> ProcessPayment(string token, decimal amount,
            string clientId, int appointmentId, string currency = "egp")
        {
            try
            {
                var options = new ChargeCreateOptions
                {
                    Amount = (long)(amount * 100), 
                    Currency = currency,
                    Description = "Payment for appointment reservation",
                    Source = token // Token from Stripe.js or Elements
                };

                var service = new ChargeService();
                var charge = await service.CreateAsync(options);

                var payment = new Payment
                {
                    Amount = amount,
                    PaymentMethod = "Online Payment",
                    TransactionId = charge.Id,
                    PaymentStatus = charge.Status, 
                    RefundStatus = "not_refunded",
                    TransactionDate = DateTime.UtcNow,
                    AppointmentId = appointmentId,
                    ClientId = clientId
                };
                paymentRepository.Add(payment);
                commitData.SaveChanges();
                return charge;

            }
            catch (StripeException ex)
            {
                throw new Exception($"Payment failed: {ex.Message}");
            }
        }

        public async Task<Refund> RefundPayment(string chargeId,int appointmentId)
        {
            try
            {
                var service = new RefundService();
                var refund =  await service.CreateAsync(new RefundCreateOptions
                {
                    Charge = chargeId //////////
                });


                var payment = paymentRepository.GetById(p => p.TransactionId == chargeId && p.AppointmentId == appointmentId);
                if (payment != null) { 
                    payment.RefundStatus= "refunded";
                    paymentRepository.Edit(payment);
                    commitData.SaveChanges();
                }

                return refund;
            }
            catch (StripeException ex)
            {
                throw new Exception($"Refund failed: {ex.Message}");
            }
        }

        public async Task UpdatePendingPayments()
        {
            var pendingPayments = paymentRepository.GetList(p => p.PaymentStatus == "pending").ToList();
            foreach (var payment in pendingPayments)
            {
                var service = new ChargeService();
                var charge = await service.GetAsync(payment.TransactionId);
                payment.PaymentStatus = charge.Status;
                paymentRepository.Edit(payment);
            }
            commitData.SaveChanges();
        }

    }
}
