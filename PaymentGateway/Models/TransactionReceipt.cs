using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PaymentGateway.Models;

public class TransactionReceipt
{
    [BsonRepresentation(BsonType.String)]
    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = "application/pdf";

    public byte[] Data { get; set; } = Array.Empty<byte>();
}
