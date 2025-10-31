using Microsoft.Extensions.Logging;
using PaymentGateway.Models;
using PaymentGateway.Models.ViewModels;

namespace PaymentGateway.Services;

public class PaymentOrchestrator
{
    private readonly ITransactionRepository _repository;
    private readonly ISolanaTransactionService _solana;
    private readonly TransactionProcessingQueue _queue;
    private readonly ILogger<PaymentOrchestrator> _logger;

    public PaymentOrchestrator(ITransactionRepository repository, ISolanaTransactionService solana, TransactionProcessingQueue queue, ILogger<PaymentOrchestrator> logger)
    {
        _repository = repository;
        _solana = solana;
        _queue = queue;
        _logger = logger;
    }

    public async Task<PaymentTransaction> CreateAsync(PaymentRequestModel request, CancellationToken cancellationToken = default)
    {
        var transaction = new PaymentTransaction
        {
            WalletAddress = request.WalletAddress,
            Direction = request.Direction,
            RequestedAmountFiat = request.FiatAmount,
            RequestedAmountUsdt = request.UsdtAmount,
            ExchangeRate = request.ExchangeRate,
            ServiceFeeFiat = request.ServiceFee,
            NetworkFee = request.NetworkFee,
            TotalPaidFiat = request.Direction == PaymentDirection.UsdtToBrl ? request.FiatAmount - request.ServiceFee - request.NetworkFee : request.FiatAmount + request.ServiceFee + request.NetworkFee,
            DestinationBankAccount = request.DestinationBankAccount,
            Status = TransactionStatus.AwaitingWalletSettlement
        };

        transaction.SolanaSignature = await _solana.RequestTransferAsync(transaction, cancellationToken);
        await _repository.CreateAsync(transaction, cancellationToken);
        await _queue.EnqueueAsync(transaction, cancellationToken);

        _logger.LogInformation("Transação {TransactionId} criada com status {Status}", transaction.Id, transaction.Status);

        return transaction;
    }
}
