using OrderManagementSystem.API.Interfaces;

namespace OrderManagementSystem.API.Models;

public enum PaymentMethod
{
    Card = 0,
    Cash = 1,
    PayPal = 2
}

public enum PaymentStatus
{
    Pending = 0,
    Paid = 1,
    Failed = 2,
    Refunded = 3
}

public sealed class Payment : IPayment
{
    private Payment()
    {
    }

    public Payment(int orderId, decimal amount, PaymentMethod method)
    {
        if (orderId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(orderId), "OrderId must be greater than zero.");
        }

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        }

        OrderId = orderId;
        Amount = amount;
        Method = method;
        Status = PaymentStatus.Pending;
    }

    public int Id { get; private set; }

    public int OrderId { get; private set; }

    public decimal Amount { get; private set; }

    public PaymentMethod Method { get; private set; }

    public PaymentStatus Status { get; private set; }

    public DateTime? PaidAt { get; private set; }

    public void MarkAsPaid(DateTime? paidAt = null)
    {
        if (Status == PaymentStatus.Paid)
        {
            return;
        }

        if (Status == PaymentStatus.Refunded)
        {
            throw new InvalidOperationException("A refunded payment cannot be marked as paid.");
        }

        Status = PaymentStatus.Paid;
        PaidAt = paidAt ?? DateTime.UtcNow;
    }

    public void MarkAsFailed()
    {
        if (Status == PaymentStatus.Refunded)
        {
            throw new InvalidOperationException("A refunded payment cannot be marked as failed.");
        }

        Status = PaymentStatus.Failed;
        PaidAt = null;
    }

    public void MarkAsRefunded()
    {
        if (Status != PaymentStatus.Paid)
        {
            throw new InvalidOperationException("Only paid payments can be refunded.");
        }

        Status = PaymentStatus.Refunded;
    }
}