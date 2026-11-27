namespace Turpinverse.Core.Models;

public sealed record Expense
{
    public required string ExpenseId { get; init; }
    public required string PersonaId { get; init; }
    public required string ExpenseDate { get; init; }
    public required string Description { get; init; }
    public required decimal AmountExTax { get; init; }
    public required string TaxRateId { get; init; }
    public required decimal TaxAmount { get; init; }
    public required decimal Total { get; init; }
    public required string Currency { get; init; }
    public string? ProjectId { get; init; }
    public string? BillableToProjectId { get; init; }
    public required bool Billable { get; init; }
    public decimal? MarkupPercent { get; init; }
    public bool? HasReceipt { get; init; }
    public required string Status { get; init; }
}