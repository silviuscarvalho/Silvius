using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PaymentGateway.Models;

namespace PaymentGateway.Data;

public class MongoContext
{
    private readonly IMongoDatabase _database;
    private readonly MongoSettings _settings;

    public MongoContext(IOptions<MongoSettings> options)
    {
        _settings = options.Value;
        var client = new MongoClient(_settings.ConnectionString);
        _database = client.GetDatabase(_settings.Database);
    }

    public IMongoCollection<PaymentTransaction> Transactions =>
        _database.GetCollection<PaymentTransaction>(_settings.TransactionsCollection);
}
