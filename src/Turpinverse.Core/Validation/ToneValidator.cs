using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Turpinverse.Core.Models;

namespace Turpinverse.Core.Validation;

public sealed partial class ToneValidator
{
    private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(1);
    private static readonly ConcurrentDictionary<string, Regex> RegexCache = new(StringComparer.Ordinal);

    public IReadOnlyList<ValidationViolation> ValidateCanon(Canon canon)
    {
        var violations = new List<ValidationViolation>();
        var patterns = canon.ToneGuidelines.ForbiddenPatterns;

        foreach (var persona in canon.Personas)
        {
            violations.AddRange(ValidateText(persona.Notes, patterns, "Persona", persona.Id));
        }

        foreach (var org in canon.Organisations)
        {
            violations.AddRange(ValidateText(org.Description, patterns, "Organisation", org.Id));
            violations.AddRange(ValidateAddressFields(org.RegisteredOffice, patterns, "Organisation", org.Id));
        }

        foreach (var product in canon.Products)
        {
            violations.AddRange(ValidateText(product.Name, patterns, "Product", product.ProductId));
            violations.AddRange(ValidateText(product.Description, patterns, "Product", product.ProductId));
        }

        foreach (var lead in canon.Leads)
        {
            violations.AddRange(ValidateText(lead.CompanyName, patterns, "Lead", lead.LeadId));
            violations.AddRange(ValidateText(lead.ContactName, patterns, "Lead", lead.LeadId));
            violations.AddRange(ValidateText(lead.Description, patterns, "Lead", lead.LeadId));
        }

        foreach (var quote in canon.Quotes)
        {
            violations.AddRange(ValidateText(quote.Notes, patterns, "Quote", quote.QuoteId));
            violations.AddRange(ValidateText(quote.Terms, patterns, "Quote", quote.QuoteId));
            foreach (var line in quote.Lines)
            {
                violations.AddRange(ValidateText(line.Description, patterns, "Quote", quote.QuoteId));
            }
        }

        foreach (var invoice in canon.Invoices)
        {
            violations.AddRange(ValidateText(invoice.Notes, patterns, "Invoice", invoice.InvoiceId));
            violations.AddRange(ValidateText(invoice.Terms, patterns, "Invoice", invoice.InvoiceId));
            foreach (var line in invoice.Lines)
            {
                violations.AddRange(ValidateText(line.Description, patterns, "Invoice", invoice.InvoiceId));
            }
        }

        foreach (var salesOrder in canon.SalesOrders)
        {
            violations.AddRange(ValidateText(salesOrder.Notes, patterns, "SalesOrder", salesOrder.SalesOrderId));
            violations.AddRange(ValidateText(salesOrder.Terms, patterns, "SalesOrder", salesOrder.SalesOrderId));
            foreach (var line in salesOrder.Lines)
            {
                violations.AddRange(ValidateText(line.Description, patterns, "SalesOrder", salesOrder.SalesOrderId));
            }
        }

        foreach (var bill in canon.Bills)
        {
            violations.AddRange(ValidateText(bill.Notes, patterns, "Bill", bill.BillId));
            foreach (var line in bill.Lines)
            {
                violations.AddRange(ValidateText(line.Description, patterns, "Bill", bill.BillId));
            }
        }

        foreach (var payment in canon.Payments)
        {
            violations.AddRange(ValidateText(payment.Reference, patterns, "Payment", payment.PaymentId));
        }

        foreach (var creditNote in canon.CreditNotes)
        {
            violations.AddRange(ValidateText(creditNote.Notes, patterns, "CreditNote", creditNote.CreditNoteId));
            foreach (var line in creditNote.Lines)
            {
                violations.AddRange(ValidateText(line.Description, patterns, "CreditNote", creditNote.CreditNoteId));
            }
        }

        foreach (var persona in canon.Personas)
        {
            if (persona.Address is not null)
            {
                violations.AddRange(ValidateAddressFields(persona.Address, patterns, "Persona", persona.Id));
            }
        }

        return violations;
    }

    private IReadOnlyList<ValidationViolation> ValidateAddressFields(
        Address address,
        IReadOnlyList<string> forbiddenPatterns,
        string entityType,
        string entityId)
    {
        var violations = new List<ValidationViolation>();
        violations.AddRange(ValidateText(address.Address1, forbiddenPatterns, entityType, entityId));
        violations.AddRange(ValidateText(address.Address2, forbiddenPatterns, entityType, entityId));
        violations.AddRange(ValidateText(address.Address3, forbiddenPatterns, entityType, entityId));
        violations.AddRange(ValidateText(address.Town, forbiddenPatterns, entityType, entityId));
        violations.AddRange(ValidateText(address.Region, forbiddenPatterns, entityType, entityId));
        violations.AddRange(ValidateText(address.Postcode, forbiddenPatterns, entityType, entityId));
        violations.AddRange(ValidateText(address.Country, forbiddenPatterns, entityType, entityId));
        return violations;
    }

    public IReadOnlyList<ValidationViolation> ValidateText(
        string? text,
        IReadOnlyList<string> forbiddenPatterns,
        string entityType,
        string entityId)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return [];
        }

        var violations = new List<ValidationViolation>();
        foreach (var pattern in forbiddenPatterns)
        {
            if (GetRegex(pattern).IsMatch(text))
            {
                violations.Add(new ValidationViolation(
                    "TONE-001",
                    $"Text contains forbidden pattern '{pattern}'",
                    entityType,
                    entityId));
            }
        }

        return violations;
    }

    private static Regex GetRegex(string pattern) =>
        RegexCache.GetOrAdd(pattern, static p => new Regex(
            p,
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant,
            RegexTimeout));
}
