using Microsoft.Extensions.Logging;

namespace PaymentGateway.Services;

public class FoxBitServiceStub : IFoxBitService
{
    private readonly ILogger<FoxBitServiceStub> _logger;

    public FoxBitServiceStub(ILogger<FoxBitServiceStub> logger)
    {
        _logger = logger;
    }

    public Task<decimal> GetExchangeRateAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("Serviço FoxBit não implementado. Utilizando taxa de câmbio fictícia.");
        return Task.FromResult(5.20m);
    }
}
