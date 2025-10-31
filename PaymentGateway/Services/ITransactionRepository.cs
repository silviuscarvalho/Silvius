using PaymentGateway.Models;

namespace PaymentGateway.Services;

public interface ITransactionRepository
{
    Task CreateAsync(PaymentTransaction transaction, CancellationToken cancellationToken = default);
    Task UpdateAsync(PaymentTransaction transaction, CancellationToken cancellationToken = default);
    Task<PaymentTransaction?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PaymentTransaction>> GetByWalletAsync(string walletAddress, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PaymentTransaction>> GetPendingSettlementAsync(CancellationToken cancellationToken = default);
}
