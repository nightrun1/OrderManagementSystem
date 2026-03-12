using OrderManagementSystem.Interfaces;
using OrderManagementSystem.Lab.Lab2.AbstractFactory;
using OrderManagementSystem.Lab.Lab4.Adapter;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Lab.Lab4.Facade;

public class OrderPlacementFacade(
    IProductRepository productRepo,
    IOrderRepository orderRepo,
    PaymentService paymentService,
    ShippingService shippingService,
    IEmailNotificationService emailService)
{
    public async Task<PlaceOrderResult> PlaceOrderAsync(PlaceOrderRequest request)
    {
        // PASUL 1 — Verifică stocul
        foreach (var item in request.Items)
        {
            if (!await productRepo.IsInStockAsync(item.ProductId, item.Quantity))
                return Fail($"Stoc insuficient pentru produsul #{item.ProductId}", "StockCheck");
        }

        // PASUL 2 — Creare comandă în DB (status = Pending)
        var orderItems = new List<OrderItem>();
        decimal totalAmount = 0;

        foreach (var item in request.Items)
        {
            var product = await productRepo.GetByIdAsync(item.ProductId);
            if (product is null)
                return Fail($"Produsul #{item.ProductId} nu a fost gasit", "StockCheck");

            orderItems.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            });
            totalAmount += product.Price * item.Quantity;
        }

        var order = new Order
        {
            UserId = request.UserId,
            ShippingAddress = request.ShippingAddress,
            TotalAmount = totalAmount,
            Status = OrderStatus.Pending,
            Items = orderItems
        };
        await orderRepo.AddAsync(order);

        // PASUL 3 — Procesează plata
        var paymentResult = await paymentService.ProcessPaymentAsync(order.Id, order.TotalAmount, request.PaymentToken);
        if (!paymentResult.Payment.Success)
        {
            order.Status = OrderStatus.Cancelled;
            await orderRepo.UpdateAsync(order);
            return Fail(paymentResult.Payment.ErrorMessage ?? "Plata a esuat", "Payment");
        }

        // PASUL 4 — Creează expedierea
        string trackingNumber;
        try
        {
            trackingNumber = await shippingService.CreateShipmentAsync(request.DeliveryOption, order);
        }
        catch (Exception ex)
        {
            trackingNumber = $"PENDING-{order.Id}";
            Console.WriteLine($"[SHIPPING] Eroare la expediere: {ex.Message}");
        }

        // PASUL 5 — Trimite email confirmare
        await emailService.SendOrderConfirmationAsync(order);

        // PASUL 6 — Actualizează status comandă → Processing
        order.Status = OrderStatus.Processing;
        order.UpdatedAt = DateTime.UtcNow;
        await orderRepo.UpdateAsync(order);

        return new PlaceOrderResult
        {
            Success = true,
            OrderId = order.Id,
            TotalCharged = order.TotalAmount,
            TrackingNumber = trackingNumber,
            ReceiptText = paymentResult.ReceiptText
        };
    }

    private static PlaceOrderResult Fail(string message, string step) => new()
    {
        Success = false,
        ErrorMessage = message,
        ErrorStep = step
    };
}
