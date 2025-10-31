namespace PaymentGateway.Data;

public class MongoSettings
{
    public const string ConfigurationSectionName = "MongoSettings";

    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string Database { get; set; } = "SilviusPayments";
    public string TransactionsCollection { get; set; } = "transactions";
}
