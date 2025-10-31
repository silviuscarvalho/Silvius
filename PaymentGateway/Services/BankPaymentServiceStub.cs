using Microsoft.Extensions.Logging;
using PaymentGateway.Models;

namespace PaymentGateway.Services;

public class BankPaymentServiceStub : IBankPaymentService
{
    private readonly ILogger<BankPaymentServiceStub> _logger;

    public BankPaymentServiceStub(ILogger<BankPaymentServiceStub> logger)
    {
        _logger = logger;
    }

    public async Task ExecuteFiatPaymentAsync(PaymentTransaction transaction, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("Integração com o banco brasileiro não implementada. Simulando pagamento para a conta {Account}.", transaction.DestinationBankAccount);
        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
    }
}
