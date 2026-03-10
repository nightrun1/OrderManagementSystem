using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.DTOs.Payments;
using OrderManagementSystem.Interfaces;
using OrderManagementSystem.Lab.Lab2.AbstractFactory;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Controllers;

[ApiController]
[Route("api/payments")]
[Authorize]
public class PaymentsController(
    PaymentService paymentService,
    IOrderRepository orderRepository,
    IPaymentRepository paymentRepository) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<PaymentResponse>> Create(CreatePaymentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CardToken))
        {
            return BadRequest(new { message = "Card token is required." });
        }

        var order = await orderRepository.GetByIdAsync(request.OrderId);
        if (order is null)
        {
            return NotFound(new { message = "Order was not found." });
        }

        var currentUserId = GetCurrentUserId();
        var isAdmin = User.IsInRole(UserRole.Admin.ToString());

        if (!isAdmin && order.UserId != currentUserId)
        {
            return StatusCode(403, new { message = "You do not have access to this order." });
        }

        var result = await paymentService.ProcessPaymentAsync(order.Id, order.TotalAmount, request.CardToken.Trim());

        return Ok(new PaymentResponse(
            result.Payment.Success,
            result.Payment.TransactionId,
            result.Payment.Amount,
            result.Payment.Currency,
            result.Payment.ErrorMessage,
            result.ReceiptText,
            result.ProviderName));
    }

    [HttpGet("{orderId:int}")]
    public async Task<ActionResult<PaymentDetailsDto>> GetByOrderId(int orderId)
    {
        var order = await orderRepository.GetByIdAsync(orderId);
        if (order is null)
        {
            return NotFound(new { message = "Order was not found." });
        }

        var currentUserId = GetCurrentUserId();
        var isAdmin = User.IsInRole(UserRole.Admin.ToString());

        if (!isAdmin && order.UserId != currentUserId)
        {
            return StatusCode(403, new { message = "You do not have access to this order." });
        }

        var payment = await paymentRepository.GetByOrderIdAsync(orderId);
        if (payment is null)
        {
            return NotFound(new { message = "Payment was not found for this order." });
        }

        return Ok(new PaymentDetailsDto(
            payment.Id,
            payment.OrderId,
            payment.Amount,
            payment.Provider,
            payment.Status,
            payment.TransactionId,
            payment.CreatedAt));
    }

    private int GetCurrentUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdValue, out var userId) ? userId : 0;
    }
}
