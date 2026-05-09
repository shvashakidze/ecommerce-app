using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Infrastructure.Services;

public class TbcPaymentService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;

    public TbcPaymentService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _config = config;
    }

    private async Task<string> GetAccessTokenAsync()
    {
        var tbc = _config.GetSection("TbcPayment");

        var request = new HttpRequestMessage(HttpMethod.Post, tbc["TokenUrl"]);
        request.Headers.Add("apikey", tbc["ApiKey"]);
        request.Content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", tbc["ClientId"]!),
            new KeyValuePair<string, string>("client_secret", tbc["ClientSecret"]!)
        });

        var response = await _http.SendAsync(request);
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(json);
        return result.GetProperty("access_token").GetString()!;
    }

    public async Task<PaymentResult> CreatePaymentAsync(
        string merchantPaymentId,
        decimal totalAmount,
        string description)
    {
        var tbc = _config.GetSection("TbcPayment");

        if (string.IsNullOrEmpty(tbc["ApiKey"]))
        {
            await Task.Delay(300);
            return new PaymentResult
            {
                PaymentId = merchantPaymentId,
                RedirectUrl = $"http://localhost:4200/payment/mock?orderId={merchantPaymentId}&amount={totalAmount}&provider=tbc"
            };
        }

        var token = await GetAccessTokenAsync();

        var body = new
        {
            amount = new { currency = "GEL", total = totalAmount },
            returnurl = tbc["ReturnUrl"],
            callbackUrl = tbc["CallbackUrl"],
            preAuth = false,
            language = "KA",
            merchantPaymentId,
            description
        };

        var request = new HttpRequestMessage(HttpMethod.Post, tbc["PaymentUrl"]);
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("apikey", tbc["ApiKey"]);
        request.Content = new StringContent(
            JsonSerializer.Serialize(body),
            Encoding.UTF8,
            "application/json");

        var response = await _http.SendAsync(request);
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(json);

        var payId = result.GetProperty("payId").GetString()!;
        var redirectUrl = result.GetProperty("links")
            .EnumerateArray()
            .First(l => l.GetProperty("rel").GetString() == "approval_url")
            .GetProperty("uri").GetString()!;

        return new PaymentResult
        {
            PaymentId = payId,
            RedirectUrl = redirectUrl
        };
    }
}