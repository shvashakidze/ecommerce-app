using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Infrastructure.Services;

public class BogPaymentService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;

    public BogPaymentService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _config = config;
    }

    private async Task<string> GetAccessTokenAsync()
    {
        var bog = _config.GetSection("BogPayment");
        var credentials = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{bog["ClientId"]}:{bog["ClientSecret"]}"));

        var request = new HttpRequestMessage(HttpMethod.Post, bog["TokenUrl"]);
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Basic", credentials);
        request.Content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials")
        });

        var response = await _http.SendAsync(request);
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(json);
        return result.GetProperty("access_token").GetString()!;
    }

    public async Task<PaymentResult> CreateOrderAsync(
        string externalOrderId,
        decimal totalAmount,
        List<BogBasketItem> basket)
    {
        var bog = _config.GetSection("BogPayment");

        // ტესტ რეჟიმი
        if (string.IsNullOrEmpty(bog["ClientId"]))
        {
            await Task.Delay(300);
            return new PaymentResult
            {
                PaymentId = externalOrderId,
                RedirectUrl = $"http://localhost:4200/payment/mock?orderId={externalOrderId}&amount={totalAmount}&provider=bog"
            };
        }

        var token = await GetAccessTokenAsync();

        var body = new
        {
            callback_url = bog["CallbackUrl"],
            external_order_id = externalOrderId,
            redirect_urls = new
            {
                success = bog["SuccessUrl"],
                fail = bog["FailUrl"]
            },
            purchase_units = new
            {
                currency = "GEL",
                total_amount = totalAmount,
                basket
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, bog["OrderUrl"]);
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("Accept-Language", "ka");
        request.Content = new StringContent(
            JsonSerializer.Serialize(body),
            Encoding.UTF8,
            "application/json");

        var response = await _http.SendAsync(request);
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(json);

        return new PaymentResult
        {
            PaymentId = externalOrderId,
            RedirectUrl = result
                .GetProperty("_links")
                .GetProperty("redirect")
                .GetProperty("href")
                .GetString()!
        };
    }
}

public class BogBasketItem
{
    public string product_id { get; set; } = "";
    public string description { get; set; } = "";
    public int quantity { get; set; }
    public decimal unit_price { get; set; }
}

public class PaymentResult
{
    public string PaymentId { get; set; } = "";
    public string RedirectUrl { get; set; } = "";
}