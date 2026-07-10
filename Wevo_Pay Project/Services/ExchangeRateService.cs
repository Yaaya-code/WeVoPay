using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Wevo_Pay_Project.DTOs;
using Wevo_Pay_Project.Services.Interfaces;

namespace Wevo_Pay_Project.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private const string CacheKey = "exchange_rates_egp_v1";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

        private static readonly (string Code, string Name, string Flag)[] TrackedCurrencies =
        {
            ("USD", "US Dollar", "US"),
            ("EUR", "Euro", "EU"),
            ("GBP", "British Pound", "GB"),
            ("SAR", "Saudi Riyal", "SA"),
            ("AED", "UAE Dirham", "AE"),
            ("KWD", "Kuwaiti Dinar", "KW"),
            ("QAR", "Qatari Riyal", "QA"),
            ("BHD", "Bahraini Dinar", "BH"),
            ("OMR", "Omani Rial", "OM"),
            ("JOD", "Jordanian Dinar", "JO"),
            ("IQD", "Iraqi Dinar", "IQ"),
            ("LBP", "Lebanese Pound", "LB"),
            ("SYP", "Syrian Pound", "SY"),
            ("MAD", "Moroccan Dirham", "MA"),
            ("TND", "Tunisian Dinar", "TN"),
            ("DZD", "Algerian Dinar", "DZ"),
            ("LYD", "Libyan Dinar", "LY"),
            ("SDG", "Sudanese Pound", "SD"),
            ("YER", "Yemeni Rial", "YE"),
            ("EGP", "Egyptian Pound", "EG")
        };

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ExchangeRateService> _logger;

        public ExchangeRateService(
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache,
            ILogger<ExchangeRateService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ExchangeRatesViewModel> GetRatesAsync()
        {
            if (_cache.TryGetValue(CacheKey, out ExchangeRatesViewModel? cached) && cached != null)
            {
                return cached;
            }

            try
            {
                var model = await FetchRatesAsync();
                _cache.Set(CacheKey, model, CacheDuration);
                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load exchange rates.");
                return new ExchangeRatesViewModel
                {
                    Success = false,
                    ErrorMessage = "Could not load live rates right now. Please try again later.",
                    UpdatedAtUtc = DateTime.UtcNow
                };
            }
        }

        private async Task<ExchangeRatesViewModel> FetchRatesAsync()
        {
            var client = _httpClientFactory.CreateClient("ExchangeRates");

            // Rates as: 1 USD = X currency units
            using var response = await client.GetAsync("https://open.er-api.com/v6/latest/USD");
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);

            var root = doc.RootElement;
            if (!root.TryGetProperty("rates", out var ratesElement))
            {
                throw new Exception("Rates not found in API response.");
            }

            if (!TryGetRate(ratesElement, "EGP", out var usdToEgp) || usdToEgp <= 0)
            {
                throw new Exception("EGP rate not available.");
            }

            var currencies = new List<CurrencyRateItem>();

            foreach (var (code, name, flag) in TrackedCurrencies)
            {
                if (code == "EGP")
                {
                    currencies.Add(new CurrencyRateItem
                    {
                        Code = code,
                        Name = name,
                        Flag = flag,
                        RateToEgp = 1m,
                        IsAvailable = true
                    });
                    continue;
                }

                if (code == "USD")
                {
                    currencies.Add(new CurrencyRateItem
                    {
                        Code = code,
                        Name = name,
                        Flag = flag,
                        RateToEgp = Math.Round(usdToEgp, 4),
                        IsAvailable = true
                    });
                    continue;
                }

                if (TryGetRate(ratesElement, code, out var usdToCurrency) && usdToCurrency > 0)
                {
                    // 1 CODE = (USD per CODE) * (EGP per USD) = usdToEgp / usdToCurrency
                    var toEgp = usdToEgp / usdToCurrency;
                    currencies.Add(new CurrencyRateItem
                    {
                        Code = code,
                        Name = name,
                        Flag = flag,
                        RateToEgp = Math.Round(toEgp, 4),
                        IsAvailable = true
                    });
                }
                else
                {
                    currencies.Add(new CurrencyRateItem
                    {
                        Code = code,
                        Name = name,
                        Flag = flag,
                        RateToEgp = 0,
                        IsAvailable = false
                    });
                }
            }

            decimal? goldUsdPerOunce = await TryGetGoldUsdPerOunceAsync(client);
            var gold = new List<GoldPriceItem>();

            if (goldUsdPerOunce.HasValue && goldUsdPerOunce.Value > 0)
            {
                var ounceEgp = goldUsdPerOunce.Value * usdToEgp;
                var gramEgp = ounceEgp / 31.1034768m;
                var kgEgp = gramEgp * 1000m;

                gold.Add(new GoldPriceItem
                {
                    Label = "Gold (24K)",
                    Unit = "per gram",
                    PriceEgp = Math.Round(gramEgp, 2)
                });
                gold.Add(new GoldPriceItem
                {
                    Label = "Gold (24K)",
                    Unit = "per ounce (troy)",
                    PriceEgp = Math.Round(ounceEgp, 2)
                });
                gold.Add(new GoldPriceItem
                {
                    Label = "Gold (24K)",
                    Unit = "per kilogram",
                    PriceEgp = Math.Round(kgEgp, 2)
                });
                gold.Add(new GoldPriceItem
                {
                    Label = "Gold 21K (approx.)",
                    Unit = "per gram",
                    PriceEgp = Math.Round(gramEgp * 21m / 24m, 2)
                });
                gold.Add(new GoldPriceItem
                {
                    Label = "Gold 18K (approx.)",
                    Unit = "per gram",
                    PriceEgp = Math.Round(gramEgp * 18m / 24m, 2)
                });
            }

            return new ExchangeRatesViewModel
            {
                Success = true,
                Currencies = currencies,
                Gold = gold,
                GoldUsdPerOunce = goldUsdPerOunce.HasValue
                    ? Math.Round(goldUsdPerOunce.Value, 2)
                    : null,
                UpdatedAtUtc = DateTime.UtcNow
            };
        }

        private async Task<decimal?> TryGetGoldUsdPerOunceAsync(HttpClient client)
        {
            try
            {
                // Free gold spot endpoint (USD per troy ounce)
                using var response = await client.GetAsync("https://api.gold-api.com/price/XAU");
                if (!response.IsSuccessStatusCode)
                    return null;

                await using var stream = await response.Content.ReadAsStreamAsync();
                using var doc = await JsonDocument.ParseAsync(stream);

                if (doc.RootElement.TryGetProperty("price", out var priceEl) &&
                    priceEl.TryGetDecimal(out var price))
                {
                    return price;
                }

                if (doc.RootElement.TryGetProperty("price", out var priceStr) &&
                    priceStr.ValueKind == JsonValueKind.String &&
                    decimal.TryParse(priceStr.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed))
                {
                    return parsed;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Gold price API unavailable.");
            }

            return null;
        }

        private static bool TryGetRate(JsonElement rates, string code, out decimal value)
        {
            value = 0;
            if (!rates.TryGetProperty(code, out var el))
                return false;

            if (el.ValueKind == JsonValueKind.Number && el.TryGetDecimal(out value))
                return true;

            if (el.ValueKind == JsonValueKind.String &&
                decimal.TryParse(el.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            {
                return true;
            }

            return false;
        }
    }
}
