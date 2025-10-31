using MongoDB.Driver;
using PaymentGateway.Data;
using PaymentGateway.Models;

namespace PaymentGateway.Services;

public class TransactionRepository : ITransactionRepository
{
    private readonly MongoContext _context;

    public TransactionRepository(MongoContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(PaymentTransaction transaction, CancellationToken cancellationToken = default)
    {
        await _context.Transactions.InsertOneAsync(transaction, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(PaymentTransaction transaction, CancellationToken cancellationToken = default)
    {
        var filter = Builders<PaymentTransaction>.Filter.Eq(t => t.Id, transaction.Id);
        await _context.Transactions.ReplaceOneAsync(filter, transaction, cancellationToken: cancellationToken);
    }

    public async Task<PaymentTransaction?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<PaymentTransaction>.Filter.Eq(t => t.Id, id);
        return await _context.Transactions.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PaymentTransaction>> GetByWalletAsync(string walletAddress, CancellationToken cancellationToken = default)
    {
        var filter = Builders<PaymentTransaction>.Filter.Eq(t => t.WalletAddress, walletAddress);
        return await _context.Transactions.Find(filter)
            .SortByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PaymentTransaction>> GetPendingSettlementAsync(CancellationToken cancellationToken = default)
    {
        var filter = Builders<PaymentTransaction>.Filter.In(t => t.Status, new[]
        {
            TransactionStatus.AwaitingWalletSettlement,
            TransactionStatus.AwaitingFiatPayment,
            TransactionStatus.AwaitingFiatSettlement
        });

        return await _context.Transactions.Find(filter)
            .SortBy(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
