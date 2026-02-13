using OrderManagementSystem.Interfaces.Services;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Services;

// TODO: Implement payment service with single responsibility (SRP)
public class PaymentService : IPaymentService
{
    // TODO: External payment gateway interface (to be defined)
    // TODO: Constructor with DIP
    
    // TODO: ProcessPaymentAsync implementation
    public Task<PaymentResult> ProcessPaymentAsync(Payment payment)
    {
        throw new NotImplementedException();
    }
    
    // TODO: RefundPaymentAsync implementation
    public Task<bool> RefundPaymentAsync(int paymentId, decimal refundAmount)
    {
        throw new NotImplementedException();
    }
    
    // TODO: GetPaymentStatusAsync implementation
    public Task<PaymentStatus?> GetPaymentStatusAsync(int paymentId)
    {
        throw new NotImplementedException();
    }
    
    // TODO: VerifyPaymentAsync implementation
    public Task<bool> VerifyPaymentAsync(string transactionId)
    {
        throw new NotImplementedException();
    }
}
