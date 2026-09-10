using Turpinverse.Core.Models;

namespace Turpinverse.Core.Export;

public static class ExportMapper
{
    public static IReadOnlyList<ContactExport> MapContacts(Canon canon)
    {
        var rows = new List<ContactExport>();
        foreach (var persona in canon.Personas)
        {
            foreach (var accountId in persona.OrganisationIds)
            {
                rows.Add(MapContactForMembership(persona, accountId));
            }
        }

        return rows;
    }

    public static IReadOnlyList<AccountExport> MapAccounts(Canon canon) =>
        canon.Organisations.Select(MapAccount).ToList();

    public static IReadOnlyList<DealExport> MapDeals(Canon canon) =>
        canon.Deals.Select(MapDeal).ToList();

    public static IReadOnlyList<CaseExport> MapCases(Canon canon) =>
        canon.Cases.Select(MapCase).ToList();

    public static IReadOnlyList<ProjectExport> MapProjects(Canon canon) =>
        canon.Projects.Select(MapProject).ToList();

    public static IReadOnlyList<ProductExport> MapProducts(Canon canon) =>
        canon.Products.Select(MapProduct).ToList();

    public static IReadOnlyList<LeadExport> MapLeads(Canon canon) =>
        canon.Leads.Select(MapLead).ToList();

    public static IReadOnlyList<QuoteExport> MapQuotes(Canon canon) =>
        canon.Quotes.Select(MapQuote).ToList();

    public static IReadOnlyList<SalesOrderExport> MapSalesOrders(Canon canon) =>
        canon.SalesOrders.Select(MapSalesOrder).ToList();

    public static IReadOnlyList<InvoiceExport> MapInvoices(Canon canon) =>
        canon.Invoices.Select(MapInvoice).ToList();

    public static IReadOnlyList<BillExport> MapBills(Canon canon) =>
        canon.Bills.Select(MapBill).ToList();

    public static IReadOnlyList<PaymentExport> MapPayments(Canon canon)
    {
        var invoicesById = canon.Invoices.ToDictionary(i => i.InvoiceId);
        var billsById = canon.Bills.ToDictionary(b => b.BillId);
        return canon.Payments.Select(payment => MapPayment(payment, invoicesById, billsById)).ToList();
    }

    public static IReadOnlyList<CreditNoteExport> MapCreditNotes(Canon canon) =>
        canon.CreditNotes.Select(MapCreditNote).ToList();

    public static ContactExport MapContactForMembership(Persona persona, string accountId)
    {
        var (firstName, lastName) = SplitName(persona.DisplayName);
        return new ContactExport
        {
            ContactId = persona.Id,
            FirstName = firstName,
            LastName = lastName,
            Title = persona.Title,
            Email = persona.Email,
            Phone = persona.Phone ?? string.Empty,
            AccountId = accountId,
            Status = persona.Status,
            Notes = persona.Notes ?? string.Empty,
            MailingAddress1 = persona.Address?.Address1 ?? string.Empty,
            MailingAddress2 = persona.Address?.Address2 ?? string.Empty,
            MailingAddress3 = persona.Address?.Address3 ?? string.Empty,
            MailingTown = persona.Address?.Town ?? string.Empty,
            MailingRegion = persona.Address?.Region ?? string.Empty,
            MailingPostcode = persona.Address?.Postcode ?? string.Empty,
            MailingCountry = persona.Address?.Country ?? string.Empty
        };
    }

    public static AccountExport MapAccount(Organisation organisation) =>
        new()
        {
            AccountId = organisation.Id,
            AccountName = organisation.TradingName,
            LegalName = organisation.LegalName ?? string.Empty,
            Industry = organisation.Industry,
            ParentAccountId = organisation.ParentOrganisationId ?? string.Empty,
            PrimaryContactId = organisation.PrimaryContactId ?? string.Empty,
            Description = organisation.Description,
            Website = organisation.Website ?? string.Empty,
            Status = organisation.Status,
            Roles = JoinContactIds(organisation.Roles),
            RegisteredOfficeAddress1 = organisation.RegisteredOffice.Address1,
            RegisteredOfficeAddress2 = organisation.RegisteredOffice.Address2 ?? string.Empty,
            RegisteredOfficeAddress3 = organisation.RegisteredOffice.Address3 ?? string.Empty,
            RegisteredOfficeTown = organisation.RegisteredOffice.Town,
            RegisteredOfficeRegion = organisation.RegisteredOffice.Region ?? string.Empty,
            RegisteredOfficePostcode = organisation.RegisteredOffice.Postcode,
            RegisteredOfficeCountry = organisation.RegisteredOffice.Country
        };

    public static DealExport MapDeal(Deal deal) =>
        new()
        {
            DealId = deal.DealId,
            DealName = deal.DealName,
            AccountId = deal.AccountId,
            ContactId = deal.ContactId ?? string.Empty,
            StakeholderContactIds = JoinContactIds(deal.StakeholderContactIds),
            Stage = deal.Stage,
            Amount = deal.Amount,
            CloseDate = deal.CloseDate,
            Description = deal.Description
        };

    public static CaseExport MapCase(Case caseRecord) =>
        new()
        {
            CaseId = caseRecord.CaseId,
            Subject = caseRecord.Subject,
            Description = caseRecord.Description,
            Status = caseRecord.Status,
            Priority = caseRecord.Priority,
            ContactId = caseRecord.ContactId ?? string.Empty,
            AccountId = caseRecord.AccountId,
            StakeholderContactIds = JoinContactIds(caseRecord.StakeholderContactIds),
            RelatedEventId = caseRecord.RelatedEventId ?? string.Empty
        };

    public static ProjectExport MapProject(Project project) =>
        new()
        {
            ProjectId = project.Id,
            Title = project.Title,
            Summary = project.Summary,
            AccountId = project.OrganisationId,
            ContactId = project.ContactId ?? string.Empty,
            StakeholderContactIds = JoinContactIds(project.StakeholderContactIds),
            DealId = project.DealId ?? string.Empty,
            CaseIds = JoinContactIds(project.CaseIds),
            Tags = string.Join("; ", project.Tags),
            Featured = project.Featured == true ? "true" : "false"
        };

    public static ProductExport MapProduct(Product product) =>
        new()
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Description = product.Description,
            UnitPrice = product.UnitPrice,
            TaxRateId = product.TaxRateId,
            UnitOfMeasure = product.UnitOfMeasure,
            Status = product.Status,
            Sku = product.Sku ?? string.Empty
        };

    public static LeadExport MapLead(Lead lead) =>
        new()
        {
            LeadId = lead.LeadId,
            CompanyName = lead.CompanyName,
            ContactName = lead.ContactName,
            Title = lead.Title ?? string.Empty,
            Email = lead.Email ?? string.Empty,
            Phone = lead.Phone ?? string.Empty,
            Status = lead.Status,
            Source = lead.Source,
            Rating = lead.Rating ?? string.Empty,
            Description = lead.Description,
            AccountId = lead.AccountId ?? string.Empty,
            ConvertedContactId = lead.ConvertedContactId ?? string.Empty
        };

    public static QuoteExport MapQuote(Quote quote) =>
        new()
        {
            QuoteId = quote.QuoteId,
            QuoteNumber = quote.QuoteNumber,
            AccountId = quote.AccountId,
            ContactId = quote.ContactId ?? string.Empty,
            DealId = quote.DealId ?? string.Empty,
            Status = quote.Status,
            IssueDate = quote.IssueDate,
            ExpiryDate = quote.ExpiryDate,
            Currency = quote.Currency,
            Subtotal = quote.Subtotal,
            TaxTotal = quote.TaxTotal,
            Total = quote.Total,
            Notes = quote.Notes ?? string.Empty,
            Terms = quote.Terms ?? string.Empty
        };

    public static SalesOrderExport MapSalesOrder(SalesOrder salesOrder) =>
        new()
        {
            SalesOrderId = salesOrder.SalesOrderId,
            OrderNumber = salesOrder.OrderNumber,
            AccountId = salesOrder.AccountId,
            ContactId = salesOrder.ContactId ?? string.Empty,
            DealId = salesOrder.DealId ?? string.Empty,
            Status = salesOrder.Status,
            OrderDate = salesOrder.OrderDate,
            RequestedDeliveryDate = salesOrder.RequestedDeliveryDate ?? string.Empty,
            Currency = salesOrder.Currency,
            Subtotal = salesOrder.Subtotal,
            TaxTotal = salesOrder.TaxTotal,
            Total = salesOrder.Total,
            Notes = salesOrder.Notes ?? string.Empty,
            Terms = salesOrder.Terms ?? string.Empty
        };

    public static BillExport MapBill(Bill bill) =>
        new()
        {
            BillId = bill.BillId,
            BillNumber = bill.BillNumber,
            SupplierAccountId = bill.SupplierAccountId,
            ContactId = bill.ContactId ?? string.Empty,
            DealId = bill.DealId ?? string.Empty,
            CaseId = bill.CaseId ?? string.Empty,
            Status = bill.Status,
            IssueDate = bill.IssueDate,
            DueDate = bill.DueDate,
            Currency = bill.Currency,
            Subtotal = bill.Subtotal,
            TaxTotal = bill.TaxTotal,
            Total = bill.Total,
            AmountDue = bill.AmountDue,
            Notes = bill.Notes ?? string.Empty
        };

    public static InvoiceExport MapInvoice(Invoice invoice) =>
        new()
        {
            InvoiceId = invoice.InvoiceId,
            InvoiceNumber = invoice.InvoiceNumber,
            AccountId = invoice.AccountId,
            ContactId = invoice.ContactId ?? string.Empty,
            DealId = invoice.DealId ?? string.Empty,
            CaseId = invoice.CaseId ?? string.Empty,
            Status = invoice.Status,
            IssueDate = invoice.IssueDate,
            DueDate = invoice.DueDate,
            Currency = invoice.Currency,
            Subtotal = invoice.Subtotal,
            TaxTotal = invoice.TaxTotal,
            Total = invoice.Total,
            AmountDue = invoice.AmountDue,
            Notes = invoice.Notes ?? string.Empty,
            Terms = invoice.Terms ?? string.Empty
        };

    public static PaymentExport MapPayment(Payment payment, Canon canon) =>
        MapPayment(
            payment,
            canon.Invoices.ToDictionary(i => i.InvoiceId),
            canon.Bills.ToDictionary(b => b.BillId));

    public static PaymentExport MapPayment(
        Payment payment,
        IReadOnlyDictionary<string, Invoice> invoicesById,
        IReadOnlyDictionary<string, Bill> billsById)
    {
        var accountId = string.Empty;
        if (!string.IsNullOrWhiteSpace(payment.InvoiceId)
            && invoicesById.TryGetValue(payment.InvoiceId, out var invoice))
        {
            accountId = invoice.AccountId;
        }
        else if (!string.IsNullOrWhiteSpace(payment.BillId)
                 && billsById.TryGetValue(payment.BillId, out var bill))
        {
            accountId = bill.SupplierAccountId;
        }

        return new PaymentExport
        {
            PaymentId = payment.PaymentId,
            PaymentDate = payment.PaymentDate,
            Amount = payment.Amount,
            Method = payment.Method,
            InvoiceId = payment.InvoiceId ?? string.Empty,
            BillId = payment.BillId ?? string.Empty,
            AccountId = accountId,
            Reference = payment.Reference ?? string.Empty
        };
    }

    public static CreditNoteExport MapCreditNote(CreditNote creditNote) =>
        new()
        {
            CreditNoteId = creditNote.CreditNoteId,
            CreditNoteNumber = creditNote.CreditNoteNumber,
            AccountId = creditNote.AccountId,
            ContactId = creditNote.ContactId ?? string.Empty,
            InvoiceId = creditNote.InvoiceId ?? string.Empty,
            IssueDate = creditNote.IssueDate,
            Currency = creditNote.Currency,
            Subtotal = creditNote.Subtotal,
            TaxTotal = creditNote.TaxTotal,
            Total = creditNote.Total,
            Notes = creditNote.Notes ?? string.Empty
        };

    internal static string JoinContactIds(IReadOnlyList<string> contactIds) =>
        contactIds.Count == 0 ? string.Empty : string.Join("; ", contactIds);

    private static (string FirstName, string LastName) SplitName(string displayName)
    {
        var parts = displayName.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length switch
        {
            0 => (string.Empty, string.Empty),
            1 => (parts[0], string.Empty),
            _ => (parts[0], parts[1])
        };
    }
}
