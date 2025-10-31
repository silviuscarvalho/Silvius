using PaymentGateway.Models;

namespace PaymentGateway.Services;

public interface IReceiptGenerator
{
    TransactionReceipt Generate(PaymentTransaction transaction);
    byte[] RenderReceiptDocument(PaymentTransaction transaction);
}
