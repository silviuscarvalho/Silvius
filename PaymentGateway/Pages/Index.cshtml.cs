using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PaymentGateway.Models;
using PaymentGateway.Models.ViewModels;
using PaymentGateway.Services;

namespace PaymentGateway.Pages;

public class IndexModel : PageModel
{
    private readonly PaymentQuoteService _quoteService;
    private readonly PaymentOrchestrator _orchestrator;
    private readonly ITransactionRepository _repository;

    [BindProperty]
    public PaymentRequestModel Input { get; set; } = new();

    public IReadOnlyList<PaymentTransaction> RecentTransactions { get; private set; } = Array.Empty<PaymentTransaction>();

    public IndexModel(PaymentQuoteService quoteService, PaymentOrchestrator orchestrator, ITransactionRepository repository)
    {
        _quoteService = quoteService;
        _orchestrator = orchestrator;
        _repository = repository;
    }

    public async Task OnGetAsync(string? wallet)
    {
        if (!string.IsNullOrWhiteSpace(wallet))
        {
            RecentTransactions = await _repository.GetByWalletAsync(wallet);
        }
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var amountReference = Input.Direction == PaymentDirection.UsdtToBrl ? Input.UsdtAmount : Input.FiatAmount;
        if (amountReference <= 0)
        {
            return BadRequest("Informe um valor válido para a transação.");
        }

        try
        {
            var quote = await _quoteService.BuildQuoteAsync(Input.Direction, amountReference, Input.WalletAddress, Input.DestinationBankAccount);
            Input = quote;

            var transaction = await _orchestrator.CreateAsync(Input);
            return new JsonResult(transaction);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    public async Task<IActionResult> OnPostQuoteAsync()
    {
        if (string.IsNullOrWhiteSpace(Input.WalletAddress))
        {
            return BadRequest("Carteira obrigatória");
        }

        var amountReference = Input.Direction == PaymentDirection.UsdtToBrl ? Input.UsdtAmount : Input.FiatAmount;
        if (amountReference <= 0)
        {
            return BadRequest("Informe um valor maior que zero.");
        }

        try
        {
            var quote = await _quoteService.BuildQuoteAsync(Input.Direction, amountReference, Input.WalletAddress, Input.DestinationBankAccount);
            return new JsonResult(quote);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
