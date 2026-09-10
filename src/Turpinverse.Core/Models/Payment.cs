namespace Turpinverse.Core.Models;

public sealed record Payment
{
    public required string PaymentId { get; init; }
    public required string PaymentDate { get; init; }
    public required decimal Amount { get; init; }
    public required string Method { get; init; }
    public string? InvoiceId { get; init; }
    public string? BillId { get; init; }
    public string? Reference { get; init; }
}
