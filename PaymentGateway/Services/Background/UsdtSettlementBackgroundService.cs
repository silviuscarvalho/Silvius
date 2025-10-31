using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PaymentGateway.Services.Background
{
    /// <summary>
    /// Periodically triggers settlement of pending USDT transactions.
    /// </summary>
    public sealed class UsdtSettlementBackgroundService : BackgroundService
    {
        private static readonly TimeSpan DefaultDelay = TimeSpan.FromSeconds(5);

        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<UsdtSettlementBackgroundService> _logger;
        private readonly TimeSpan _pollingDelay;

        public UsdtSettlementBackgroundService(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<UsdtSettlementBackgroundService> logger)
            : this(serviceScopeFactory, logger, DefaultDelay)
        {
        }

        public UsdtSettlementBackgroundService(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<UsdtSettlementBackgroundService> logger,
            TimeSpan pollingDelay)
        {
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _pollingDelay = pollingDelay;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var repository = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();
                    await repository.SettlePendingTransactionsAsync(stoppingToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    // Swallow cancellation exceptions triggered by the runtime when the host is shutting down.
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while settling USDT transactions.");
                }

                try
                {
                    await Task.Delay(_pollingDelay, stoppingToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }
    }
}
