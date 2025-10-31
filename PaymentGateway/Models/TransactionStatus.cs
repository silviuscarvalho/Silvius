namespace PaymentGateway.Models;

public enum TransactionStatus
{
    Draft,
    AwaitingWalletSettlement,
    AwaitingFiatSettlement,
    AwaitingFiatPayment,
    Completed,
    Failed
}
