namespace Wevo_Pay_Project.DTOs
{
    public class CurrencyRateItem
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Flag { get; set; } = string.Empty;
        public decimal RateToEgp { get; set; }
        public bool IsAvailable { get; set; } = true;
    }

    public class GoldPriceItem
    {
        public string Label { get; set; } = string.Empty;
        public decimal PriceEgp { get; set; }
        public string Unit { get; set; } = string.Empty;
    }

    public class ExchangeRatesViewModel
    {
        public List<CurrencyRateItem> Currencies { get; set; } = new();
        public List<GoldPriceItem> Gold { get; set; } = new();
        public DateTime UpdatedAtUtc { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public decimal? GoldUsdPerOunce { get; set; }
    }
}
