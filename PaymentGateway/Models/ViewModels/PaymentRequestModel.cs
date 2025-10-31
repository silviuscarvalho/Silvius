using System.ComponentModel.DataAnnotations;
using PaymentGateway.Models;

namespace PaymentGateway.Models.ViewModels;

public class PaymentRequestModel
{
    [Required]
    public PaymentDirection Direction { get; set; }

    [Range(0, double.MaxValue)]
    public decimal FiatAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal UsdtAmount { get; set; }

    [Required]
    public decimal ExchangeRate { get; set; }

    public decimal ServiceFee { get; set; }

    public decimal NetworkFee { get; set; }

    [Required]
    public string WalletAddress { get; set; } = string.Empty;

    public string DestinationBankAccount { get; set; } = string.Empty;
}
