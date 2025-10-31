using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Services
{
    /// <summary>
    /// Abstraction for persisting and settling transactions that are awaiting USDT payout.
    /// </summary>
    public interface ITransactionRepository
    {
        /// <summary>
        /// Performs whatever logic is required to settle pending USDT transactions.
        /// </summary>
        /// <param name="cancellationToken">Token used to observe cancellation requests.</param>
        Task SettlePendingTransactionsAsync(CancellationToken cancellationToken);
    }
}
