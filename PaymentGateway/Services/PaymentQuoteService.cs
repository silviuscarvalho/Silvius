using PaymentGateway.Models;
using PaymentGateway.Models.ViewModels;

namespace PaymentGateway.Services;

public class PaymentQuoteService
{
    private const decimal ServiceFeeRate = 0.035m;
    private const decimal MinimumFiatAmount = 50m;
    private const decimal FixedNetworkFee = 3.00m;

    private readonly IFoxBitService _foxBitService;

    public PaymentQuoteService(IFoxBitService foxBitService)
    {
        _foxBitService = foxBitService;
    }

    public async Task<PaymentRequestModel> BuildQuoteAsync(PaymentDirection direction, decimal amount, string wallet, string bankAccount, CancellationToken cancellationToken = default)
    {
        var exchangeRate = await _foxBitService.GetExchangeRateAsync(cancellationToken);
        decimal fiatAmount = direction == PaymentDirection.UsdtToBrl ? amount * exchangeRate : amount;
        if (fiatAmount < MinimumFiatAmount)
        {
            throw new InvalidOperationException($"Valor mínimo para transações é R$ {MinimumFiatAmount:0.00}");
        }

        decimal usdtAmount = direction == PaymentDirection.UsdtToBrl ? amount : amount / exchangeRate;
        decimal serviceFee = Math.Round(fiatAmount * ServiceFeeRate, 2, MidpointRounding.AwayFromZero);

        return new PaymentRequestModel
        {
            Direction = direction,
            FiatAmount = Math.Round(fiatAmount, 2),
            UsdtAmount = Math.Round(usdtAmount, 2),
            ExchangeRate = exchangeRate,
            ServiceFee = serviceFee,
            NetworkFee = FixedNetworkFee,
            WalletAddress = wallet,
            DestinationBankAccount = bankAccount,
        };
    }
}
