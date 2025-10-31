using System.Threading.Channels;
using PaymentGateway.Models;

namespace PaymentGateway.Services;

public class TransactionProcessingQueue
{
    private readonly Channel<PaymentTransaction> _channel = Channel.CreateUnbounded<PaymentTransaction>();

    public ValueTask EnqueueAsync(PaymentTransaction transaction, CancellationToken cancellationToken = default)
        => _channel.Writer.WriteAsync(transaction, cancellationToken);

    public IAsyncEnumerable<PaymentTransaction> ReadAllAsync(CancellationToken cancellationToken = default)
        => _channel.Reader.ReadAllAsync(cancellationToken);
}
