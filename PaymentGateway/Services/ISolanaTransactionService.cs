using PaymentGateway.Models;

namespace PaymentGateway.Services;

public interface ISolanaTransactionService
{
    Task<string> RequestTransferAsync(PaymentTransaction transaction, CancellationToken cancellationToken = default);
    Task WaitForConfirmationAsync(PaymentTransaction transaction, CancellationToken cancellationToken = default);
}
