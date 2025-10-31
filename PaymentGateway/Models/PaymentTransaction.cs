using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PaymentGateway.Models;

public class PaymentTransaction
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string WalletAddress { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.String)]
    public PaymentDirection Direction { get; set; }

    public decimal RequestedAmountFiat { get; set; }

    public decimal RequestedAmountUsdt { get; set; }

    public decimal ExchangeRate { get; set; }

    public decimal ServiceFeeFiat { get; set; }

    public decimal NetworkFee { get; set; }

    public decimal TotalPaidFiat { get; set; }

    public string SolanaSignature { get; set; } = string.Empty;

    public string DestinationBankAccount { get; set; } = string.Empty;

    public TransactionStatus Status { get; set; } = TransactionStatus.Draft;

    public TransactionReceipt? Receipt { get; set; }

    public DateTime? CompletedAt { get; set; }
}
