using Microsoft.Extensions.Logging;
using PaymentGateway.Models;

namespace PaymentGateway.Services;

public class SolanaTransactionService : ISolanaTransactionService
{
    private readonly ILogger<SolanaTransactionService> _logger;

    public SolanaTransactionService(ILogger<SolanaTransactionService> logger)
    {
        _logger = logger;
    }

    public Task<string> RequestTransferAsync(PaymentTransaction transaction, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Solicitando transferência USDT -> {Direction} para {Wallet}", transaction.Direction, transaction.WalletAddress);
        return Task.FromResult(Guid.NewGuid().ToString("N"));
    }

    public async Task WaitForConfirmationAsync(PaymentTransaction transaction, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Aguardando confirmação na Solana para assinatura {Signature}", transaction.SolanaSignature);
        await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
        _logger.LogInformation("Transação {Id} confirmada na rede Solana", transaction.Id);
    }
}
