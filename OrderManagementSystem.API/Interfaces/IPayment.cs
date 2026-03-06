using OrderManagementSystem.API.Models;

namespace OrderManagementSystem.API.Interfaces;

public interface IPayment
{
    int Id { get; }

    int OrderId { get; }

    decimal Amount { get; }

    PaymentMethod Method { get; }

    PaymentStatus Status { get; }

    DateTime? PaidAt { get; }

    void MarkAsPaid(DateTime? paidAt = null);

    void MarkAsFailed();

    void MarkAsRefunded();
}