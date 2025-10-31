using PaymentGateway.Models;

namespace PaymentGateway.Services;

public interface IBankPaymentService
{
    Task ExecuteFiatPaymentAsync(PaymentTransaction transaction, CancellationToken cancellationToken = default);
}
