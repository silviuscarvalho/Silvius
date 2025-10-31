namespace PaymentGateway.Services;

public interface IFoxBitService
{
    Task<decimal> GetExchangeRateAsync(CancellationToken cancellationToken = default);
}
