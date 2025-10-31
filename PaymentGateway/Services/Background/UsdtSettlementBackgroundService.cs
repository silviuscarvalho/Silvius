using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PaymentGateway.Models;

namespace PaymentGateway.Services.Background;

public class UsdtSettlementBackgroundService : BackgroundService
{
    private readonly TransactionProcessingQueue _queue;
    private readonly ISolanaTransactionService _solanaService;
    private readonly IBankPaymentService _bankPaymentService;
    private readonly ITransactionRepository _repository;
    private readonly IReceiptGenerator _receiptGenerator;
    private readonly ILogger<UsdtSettlementBackgroundService> _logger;

    public UsdtSettlementBackgroundService(TransactionProcessingQueue queue,
        ISolanaTransactionService solanaService,
        IBankPaymentService bankPaymentService,
        ITransactionRepository repository,
        IReceiptGenerator receiptGenerator,
        ILogger<UsdtSettlementBackgroundService> logger)
    {
        _queue = queue;
        _solanaService = solanaService;
        _bankPaymentService = bankPaymentService;
        _repository = repository;
        _receiptGenerator = receiptGenerator;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var pending = await _repository.GetPendingSettlementAsync(stoppingToken);
        foreach (var transaction in pending)
        {
            await SafeHandleAsync(transaction, stoppingToken);
        }

        await foreach (var transaction in _queue.ReadAllAsync(stoppingToken))
        {
            await SafeHandleAsync(transaction, stoppingToken);
        }
    }

    private async Task SafeHandleAsync(PaymentTransaction transaction, CancellationToken stoppingToken)
    {
        try
        {
            await HandleTransactionAsync(transaction, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar transação {TransactionId}", transaction.Id);
            transaction.Status = TransactionStatus.Failed;
            await _repository.UpdateAsync(transaction, stoppingToken);
        }
    }

    private async Task HandleTransactionAsync(PaymentTransaction transaction, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processando fila para transação {TransactionId}", transaction.Id);
        transaction.Status = TransactionStatus.AwaitingWalletSettlement;
        await _repository.UpdateAsync(transaction, cancellationToken);

        await _solanaService.WaitForConfirmationAsync(transaction, cancellationToken);

        transaction.Status = TransactionStatus.AwaitingFiatPayment;
        await _repository.UpdateAsync(transaction, cancellationToken);

        await _bankPaymentService.ExecuteFiatPaymentAsync(transaction, cancellationToken);

        transaction.Status = TransactionStatus.AwaitingFiatSettlement;
        await _repository.UpdateAsync(transaction, cancellationToken);

        transaction.Receipt = _receiptGenerator.Generate(transaction);
        transaction.Status = TransactionStatus.Completed;
        transaction.CompletedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(transaction, cancellationToken);
        _logger.LogInformation("Transação {TransactionId} concluída", transaction.Id);
    }
}
