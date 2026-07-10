using Wevo_Pay_Project.Services.Interfaces;

namespace Wevo_Pay_Project.Services
{
    public class TransferExpiryBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<TransferExpiryBackgroundService> _logger;

        public TransferExpiryBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<TransferExpiryBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var transferService = scope.ServiceProvider.GetRequiredService<ITransferService>();
                    var count = await transferService.CancelExpiredPendingTransfersAsync();

                    if (count > 0)
                    {
                        _logger.LogInformation(
                            "Auto-cancelled {Count} pending transfer(s) after timeout.",
                            count);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while auto-cancelling expired transfers.");
                }

                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }
}
