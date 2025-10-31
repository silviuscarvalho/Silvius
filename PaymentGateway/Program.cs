using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PaymentGateway.Data;
using PaymentGateway.Services;
using PaymentGateway.Services.Background;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection(MongoSettings.ConfigurationSectionName));
builder.Services.AddSingleton<MongoContext>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddSingleton<TransactionProcessingQueue>();
builder.Services.AddSingleton<ISolanaTransactionService, SolanaTransactionService>();
builder.Services.AddSingleton<IFoxBitService, FoxBitServiceStub>();
builder.Services.AddSingleton<IBankPaymentService, BankPaymentServiceStub>();
builder.Services.AddSingleton<IReceiptGenerator, ReceiptGenerator>();
builder.Services.AddScoped<PaymentQuoteService>();
builder.Services.AddScoped<PaymentOrchestrator>();
builder.Services.AddHostedService<UsdtSettlementBackgroundService>();

builder.Services.AddRazorPages();

builder.Services.AddCors(options =>
{
    options.AddPolicy("WalletPolicy", policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowed(_ => true)
              .AllowCredentials();
    });
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("WalletPolicy");

app.MapRazorPages();

app.MapGet("/api/transactions", async (string wallet, ITransactionRepository repository) =>
{
    var result = await repository.GetByWalletAsync(wallet);
    return Results.Ok(result);
});

app.MapGet("/api/transactions/{id}/receipt", async (string id, ITransactionRepository repository) =>
{
    var transaction = await repository.GetByIdAsync(id);
    if (transaction is null || transaction.Receipt is null)
    {
        return Results.NotFound();
    }

    return Results.File(transaction.Receipt.Data, transaction.Receipt.ContentType, transaction.Receipt.FileName);
});

app.Run();
