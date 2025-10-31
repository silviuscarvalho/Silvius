using System.Text;
using PaymentGateway.Models;

namespace PaymentGateway.Services;

public class ReceiptGenerator : IReceiptGenerator
{
    public TransactionReceipt Generate(PaymentTransaction transaction)
    {
        return new TransactionReceipt
        {
            FileName = $"comprovante-{transaction.Id}.pdf",
            Data = RenderReceiptDocument(transaction)
        };
    }

    public byte[] RenderReceiptDocument(PaymentTransaction transaction)
    {
        var builder = new StringBuilder();
        builder.AppendLine("Silvius Intermediação de Pagamentos");
        builder.AppendLine("Comprovante de Operação");
        builder.AppendLine(new string('-', 40));
        builder.AppendLine($"Transação: {transaction.Id}");
        builder.AppendLine($"Carteira: {transaction.WalletAddress}");
        builder.AppendLine($"Sentido: {transaction.Direction}");
        builder.AppendLine($"Assinatura Solana: {transaction.SolanaSignature}");
        builder.AppendLine($"Valor solicitado (BRL): {transaction.RequestedAmountFiat:0.00}");
        builder.AppendLine($"Valor solicitado (USDT): {transaction.RequestedAmountUsdt:0.00}");
        builder.AppendLine($"Taxa de serviço (3,5%): {transaction.ServiceFeeFiat:0.00}");
        builder.AppendLine($"Taxas de rede: {transaction.NetworkFee:0.00}");
        builder.AppendLine($"Total pago em BRL: {transaction.TotalPaidFiat:0.00}");
        builder.AppendLine($"Status final: {transaction.Status}");
        builder.AppendLine($"Finalizado em: {transaction.CompletedAt:O}");
        builder.AppendLine($"Conta destino: {transaction.DestinationBankAccount}");
        builder.AppendLine("\nObrigado por utilizar nossos serviços.");

        var plainText = builder.ToString();
        var fakePdf = $"%PDF-1.1\n1 0 obj<<>>endobj\nstream\n{plainText}\nendstream\n%%EOF";
        return Encoding.UTF8.GetBytes(fakePdf);
    }
}
