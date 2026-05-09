using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using ECommerce.Infrastructure.Services;
using ECommerce.Infrastructure.Data;
using ECommerce.Domain.Entities;


[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly BogPaymentService _bogPayment;
    private readonly TbcPaymentService _tbcPayment;

    public OrdersController(
        AppDbContext db,
        BogPaymentService bogPayment,
        TbcPaymentService tbcPayment)
    {
        _db = db;
        _bogPayment = bogPayment;
        _tbcPayment = tbcPayment;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateOrder(
        [FromBody] CreateOrderDto dto,
        [FromQuery] string provider = "bog")
    {
        var userId = Guid.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var order = new Order
        {
            UserId = userId,
            ShippingAddress = dto.ShippingAddress,
            TotalAmount = dto.Items.Sum(i => i.UnitPrice * i.Quantity),
            Status = OrderStatus.Pending,
            PaymentProvider = provider.ToLower(),
            Items = dto.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        PaymentResult result;

        if (provider.ToLower() == "tbc")
        {
            result = await _tbcPayment.CreatePaymentAsync(
                order.Id.ToString(),
                order.TotalAmount,
                "E-Shop შეკვეთა");
        }
        else
        {
            var basket = dto.Items.Select(i => new BogBasketItem
            {
                product_id = i.ProductId.ToString(),
                description = i.ProductName,
                quantity = i.Quantity,
                unit_price = i.UnitPrice
            }).ToList();

            result = await _bogPayment.CreateOrderAsync(
                order.Id.ToString(),
                order.TotalAmount,
                basket);
        }

        return Ok(new
        {
            orderId = order.Id,
            redirectUrl = result.RedirectUrl,
            provider
        });
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetMyOrders()
    {
        var userId = Guid.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var orders = await _db.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new
            {
                o.Id,
                o.Status,
                o.TotalAmount,
                o.ShippingAddress,
                o.PaymentProvider,
                o.CreatedAt,
                Items = o.Items.Select(i => new
                {
                    i.ProductId,
                    ProductName = i.Product.Name,
                    i.Quantity,
                    i.UnitPrice
                })
            })
            .ToListAsync();

        return Ok(orders);
    }

    [HttpPost("/api/payment/bog-callback")]
    public async Task<IActionResult> BogCallback([FromBody] JsonElement body)
    {
        try
        {
            var externalOrderId = body.GetProperty("external_order_id").GetString();
            var eventType = body.GetProperty("event").GetString();

            if (Guid.TryParse(externalOrderId, out var orderId))
            {
                var order = await _db.Orders.FindAsync(orderId);
                if (order != null)
                {
                    order.Status = eventType == "completed"
                        ? OrderStatus.Confirmed
                        : OrderStatus.Cancelled;
                    order.UpdatedAt = DateTime.UtcNow;
                    await _db.SaveChangesAsync();
                }
            }
        }
        catch { }
        return Ok();
    }

    [HttpPost("/api/payment/tbc-callback")]
    public async Task<IActionResult> TbcCallback([FromBody] JsonElement body)
    {
        try
        {
            var paymentId = body.GetProperty("PaymentId").GetString();
            var order = await _db.Orders
                .FirstOrDefaultAsync(o => o.Id.ToString() == paymentId);

            if (order != null)
            {
                order.Status = OrderStatus.Confirmed;
                order.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
        }
        catch { }
        return Ok();
    }

    [HttpPost("/api/payment/mock-confirm")]
    public async Task<IActionResult> MockConfirm([FromBody] MockConfirmDto dto)
    {
        var order = await _db.Orders.FindAsync(dto.OrderId);
        if (order == null) return NotFound();

        order.Status = dto.Success
            ? OrderStatus.Confirmed
            : OrderStatus.Cancelled;
        order.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(new { status = order.Status });
    }
}

public record CreateOrderDto(string ShippingAddress, List<OrderItemDto> Items);
public record OrderItemDto(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice);
public record MockConfirmDto(Guid OrderId, bool Success);