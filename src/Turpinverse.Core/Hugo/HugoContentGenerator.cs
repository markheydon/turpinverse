using System.Text;
using System.Text.Json;
using Turpinverse.Core.Abstractions;
using Turpinverse.Core.Career;
using Turpinverse.Core.Models;
using Turpinverse.Core.Profile;

namespace Turpinverse.Core.Hugo;

public sealed class HugoContentGenerator(ICanonRepository canonRepository) : IHugoContentGenerator
{
    private readonly CareerPortfolioPresenter _presenter = new();
    private readonly ProfessionalExtrasPresenter _extrasPresenter = new();
    private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task GenerateAsync(string siteRoot, CancellationToken cancellationToken = default)
    {
        var canon = await canonRepository.LoadAsync(cancellationToken);

        var contentDir = Path.Combine(siteRoot, "content");
        var dataDir = Path.Combine(siteRoot, "data");

        Directory.CreateDirectory(Path.Combine(contentDir, "personas"));
        Directory.CreateDirectory(Path.Combine(contentDir, "organisations"));
        Directory.CreateDirectory(Path.Combine(contentDir, "timeline"));
        Directory.CreateDirectory(Path.Combine(contentDir, "deals"));
        Directory.CreateDirectory(Path.Combine(contentDir, "cases"));
        Directory.CreateDirectory(Path.Combine(contentDir, "projects"));
        Directory.CreateDirectory(Path.Combine(contentDir, "products"));
        Directory.CreateDirectory(Path.Combine(contentDir, "leads"));
        Directory.CreateDirectory(Path.Combine(contentDir, "quotes"));
        Directory.CreateDirectory(Path.Combine(contentDir, "sales-orders"));
        Directory.CreateDirectory(Path.Combine(contentDir, "invoices"));
        Directory.CreateDirectory(Path.Combine(contentDir, "bills"));
        Directory.CreateDirectory(Path.Combine(contentDir, "payments"));
        Directory.CreateDirectory(Path.Combine(contentDir, "credit-notes"));
        Directory.CreateDirectory(Path.Combine(contentDir, "articles"));
        Directory.CreateDirectory(Path.Combine(contentDir, "galleries"));
        Directory.CreateDirectory(Path.Combine(dataDir, "career"));
        Directory.CreateDirectory(Path.Combine(dataDir, "profile"));
        Directory.CreateDirectory(dataDir);

        var personaNames = canon.Personas.ToDictionary(p => p.Id, p => p.DisplayName);

        foreach (var persona in canon.Personas)
        {
            var summary = EscapeYaml(ExtractSummary(persona.Biography));
            var addressYaml = persona.Address is not null
                ? FormatAddressYaml("address", persona.Address)
                : string.Empty;
            var content = $"""
                ---
                title: "{EscapeYaml(persona.DisplayName)}"
                type: "personas"
                jobTitle: "{EscapeYaml(persona.Title)}"
                email: "{persona.Email}"
                status: "{persona.Status}"
                summary: "{summary}"
                organisations: {JsonSerializer.Serialize(persona.OrganisationIds)}
                {addressYaml}---

                {persona.Biography}
                """;
            await File.WriteAllTextAsync(
                Path.Combine(contentDir, "personas", $"{persona.Id}.md"),
                content,
                Encoding.UTF8,
                cancellationToken);
        }

        foreach (var org in canon.Organisations)
        {
            var legalName = org.LegalName ?? string.Empty;
            var foundedLine = org.FoundedYear.HasValue ? $"foundedYear: {org.FoundedYear}\n" : string.Empty;
            var primaryContactLine = !string.IsNullOrWhiteSpace(org.PrimaryContactId)
                ? $"primaryContactId: \"{org.PrimaryContactId}\"\n"
                : string.Empty;
            var rolesLine = org.Roles.Count > 0
                ? $"roles: {JsonSerializer.Serialize(org.Roles)}\n"
                : string.Empty;
            var content = $"""
                ---
                title: "{EscapeYaml(org.TradingName)}"
                type: "organisations"
                industry: "{EscapeYaml(org.Industry)}"
                status: "{org.Status}"
                legalName: "{EscapeYaml(legalName)}"
                {foundedLine}{primaryContactLine}{rolesLine}members: {JsonSerializer.Serialize(org.MemberPersonaIds)}
                parent: "{org.ParentOrganisationId ?? ""}"
                {FormatAddressYaml("registeredOffice", org.RegisteredOffice).TrimEnd()}
                ---

                {org.Description}
                """;
            await File.WriteAllTextAsync(
                Path.Combine(contentDir, "organisations", $"{org.Id}.md"),
                content,
                Encoding.UTF8,
                cancellationToken);
        }

        var timelineContent = """
            ---
            title: Timeline
            ---

            Key events from the Turpinverse canon — historical fact, Victorian legend, and fictional extension reframed in corporate voice.

            """;
        await File.WriteAllTextAsync(
            Path.Combine(contentDir, "timeline", "_index.md"),
            timelineContent,
            Encoding.UTF8,
            cancellationToken);

        await File.WriteAllTextAsync(
            Path.Combine(dataDir, "organisations.json"),
            JsonSerializer.Serialize(canon.Organisations, JsonOptions),
            Utf8NoBom,
            cancellationToken);

        await File.WriteAllTextAsync(
            Path.Combine(dataDir, "events.json"),
            JsonSerializer.Serialize(canon.Events, JsonOptions),
            Utf8NoBom,
            cancellationToken);

        var dealsIndexContent = """
            ---
            title: Deals
            ---

            Sales opportunities from the Turpinverse canon — pipeline stages, amounts, and the relationships behind each deal.

            """;
        await File.WriteAllTextAsync(
            Path.Combine(contentDir, "deals", "_index.md"),
            dealsIndexContent,
            Encoding.UTF8,
            cancellationToken);

        foreach (var deal in canon.Deals)
        {
            var contactLine = !string.IsNullOrWhiteSpace(deal.ContactId)
                ? $"contactId: \"{deal.ContactId}\"\n"
                : string.Empty;
            var stakeholderLine = deal.StakeholderContactIds.Count > 0
                ? $"stakeholderContactIds: {JsonSerializer.Serialize(deal.StakeholderContactIds)}\n"
                : string.Empty;
            var content = $"""
                ---
                title: "{EscapeYaml(deal.DealName)}"
                type: "deals"
                dealId: "{deal.DealId}"
                accountId: "{deal.AccountId}"
                {contactLine}{stakeholderLine}stage: "{EscapeYaml(deal.Stage)}"
                amount: {deal.Amount}
                closeDate: "{deal.CloseDate}"
                ---

                {deal.Description}
                """;
            await File.WriteAllTextAsync(
                Path.Combine(contentDir, "deals", $"{deal.DealId}.md"),
                content,
                Encoding.UTF8,
                cancellationToken);
        }

        var casesIndexContent = """
            ---
            title: Cases
            ---

            Support cases from the Turpinverse canon — subjects, priorities, and the people and events they touch.

            """;
        await File.WriteAllTextAsync(
            Path.Combine(contentDir, "cases", "_index.md"),
            casesIndexContent,
            Encoding.UTF8,
            cancellationToken);

        foreach (var caseRecord in canon.Cases)
        {
            var contactLine = !string.IsNullOrWhiteSpace(caseRecord.ContactId)
                ? $"contactId: \"{caseRecord.ContactId}\"\n"
                : string.Empty;
            var stakeholderLine = caseRecord.StakeholderContactIds.Count > 0
                ? $"stakeholderContactIds: {JsonSerializer.Serialize(caseRecord.StakeholderContactIds)}\n"
                : string.Empty;
            var relatedEventLine = caseRecord.RelatedEventId is not null
                ? $"relatedEventId: \"{caseRecord.RelatedEventId}\"\n"
                : string.Empty;
            var content = $"""
                ---
                title: "{EscapeYaml(caseRecord.Subject)}"
                type: "cases"
                caseId: "{caseRecord.CaseId}"
                accountId: "{caseRecord.AccountId}"
                {contactLine}{stakeholderLine}status: "{EscapeYaml(caseRecord.Status)}"
                priority: "{EscapeYaml(caseRecord.Priority)}"
                {relatedEventLine}---

                {caseRecord.Description}
                """;
            await File.WriteAllTextAsync(
                Path.Combine(contentDir, "cases", $"{caseRecord.CaseId}.md"),
                content,
                Encoding.UTF8,
                cancellationToken);
        }

        await File.WriteAllTextAsync(
            Path.Combine(dataDir, "deals.json"),
            JsonSerializer.Serialize(canon.Deals, JsonOptions),
            Utf8NoBom,
            cancellationToken);

        await File.WriteAllTextAsync(
            Path.Combine(dataDir, "cases.json"),
            JsonSerializer.Serialize(canon.Cases, JsonOptions),
            Utf8NoBom,
            cancellationToken);

        var projectsIndexContent = """
            ---
            title: Projects
            ---

            Portfolio catalog from the Turpinverse canon — products, platforms, and programmes linked to accounts and contacts.

            """;
        await File.WriteAllTextAsync(
            Path.Combine(contentDir, "projects", "_index.md"),
            projectsIndexContent,
            Encoding.UTF8,
            cancellationToken);

        foreach (var project in canon.Projects)
        {
            var featuredLine = project.Featured == true ? "featured: true\n" : string.Empty;
            var contactLine = !string.IsNullOrWhiteSpace(project.ContactId)
                ? $"contactId: \"{project.ContactId}\"\n"
                : string.Empty;
            var stakeholderLine = project.StakeholderContactIds.Count > 0
                ? $"stakeholderContactIds: {JsonSerializer.Serialize(project.StakeholderContactIds)}\n"
                : string.Empty;
            var dealLine = !string.IsNullOrWhiteSpace(project.DealId)
                ? $"dealId: \"{project.DealId}\"\n"
                : string.Empty;
            var caseIdsLine = project.CaseIds.Count > 0
                ? $"caseIds: {JsonSerializer.Serialize(project.CaseIds)}\n"
                : string.Empty;
            var content = $"""
                ---
                title: "{EscapeYaml(project.Title)}"
                type: "projects"
                projectId: "{project.Id}"
                organisationId: "{project.OrganisationId}"
                {contactLine}{stakeholderLine}{dealLine}{caseIdsLine}tags: {JsonSerializer.Serialize(project.Tags)}
                image: "{project.Image}"
                {featuredLine}---

                {project.Summary}
                """;
            await File.WriteAllTextAsync(
                Path.Combine(contentDir, "projects", $"{project.Id}.md"),
                content,
                Encoding.UTF8,
                cancellationToken);
        }

        await File.WriteAllTextAsync(
            Path.Combine(dataDir, "projects.json"),
            JsonSerializer.Serialize(canon.Projects, JsonOptions),
            Utf8NoBom,
            cancellationToken);

        var taxRateNames = canon.TaxRates.ToDictionary(t => t.TaxRateId, t => t.Name);
        var taxRatePercentages = canon.TaxRates.ToDictionary(t => t.TaxRateId, t => t.Percentage);

        var productsIndexContent = """
            ---
            title: Products
            ---

            Product and service catalogue from the Turpinverse canon — price book items with UK VAT defaults, distinct from portfolio delivery projects.

            """;
        await File.WriteAllTextAsync(
            Path.Combine(contentDir, "products", "_index.md"),
            productsIndexContent,
            Encoding.UTF8,
            cancellationToken);

        foreach (var product in canon.Products)
        {
            var skuLine = !string.IsNullOrWhiteSpace(product.Sku)
                ? $"sku: \"{EscapeYaml(product.Sku)}\"\n"
                : string.Empty;
            var taxRateName = taxRateNames.GetValueOrDefault(product.TaxRateId, product.TaxRateId);
            var taxRatePercentage = taxRatePercentages.GetValueOrDefault(product.TaxRateId, 0m);
            var content = $"""
                ---
                title: "{EscapeYaml(product.Name)}"
                type: "products"
                productId: "{product.ProductId}"
                unitPrice: {product.UnitPrice}
                taxRateName: "{EscapeYaml(taxRateName)}"
                taxRatePercentage: {taxRatePercentage}
                unitOfMeasure: "{EscapeYaml(product.UnitOfMeasure)}"
                status: "{product.Status}"
                {skuLine}---

                {product.Description}
                """;
            await File.WriteAllTextAsync(
                Path.Combine(contentDir, "products", $"{product.ProductId}.md"),
                content,
                Encoding.UTF8,
                cancellationToken);
        }

        await File.WriteAllTextAsync(
            Path.Combine(dataDir, "products.json"),
            JsonSerializer.Serialize(canon.Products, JsonOptions),
            Utf8NoBom,
            cancellationToken);

        var leadsIndexContent = """
            ---
            title: Leads
            ---

            Pre-contact CRM prospects from the Turpinverse canon — qualification pipeline before persona conversion.

            """;
        await File.WriteAllTextAsync(
            Path.Combine(contentDir, "leads", "_index.md"),
            leadsIndexContent,
            Encoding.UTF8,
            cancellationToken);

        foreach (var lead in canon.Leads)
        {
            var title = EscapeYaml(lead.CompanyName);
            var contactNameLine = !string.IsNullOrWhiteSpace(lead.ContactName)
                ? $"contactName: \"{EscapeYaml(lead.ContactName)}\"\n"
                : string.Empty;
            var titleLine = !string.IsNullOrWhiteSpace(lead.Title)
                ? $"jobTitle: \"{EscapeYaml(lead.Title)}\"\n"
                : string.Empty;
            var emailLine = !string.IsNullOrWhiteSpace(lead.Email)
                ? $"email: \"{EscapeYaml(lead.Email)}\"\n"
                : string.Empty;
            var phoneLine = !string.IsNullOrWhiteSpace(lead.Phone)
                ? $"phone: \"{EscapeYaml(lead.Phone)}\"\n"
                : string.Empty;
            var ratingLine = !string.IsNullOrWhiteSpace(lead.Rating)
                ? $"rating: \"{EscapeYaml(lead.Rating)}\"\n"
                : string.Empty;
            var accountIdLine = !string.IsNullOrWhiteSpace(lead.AccountId)
                ? $"accountId: \"{lead.AccountId}\"\n"
                : string.Empty;
            var convertedContactIdLine = !string.IsNullOrWhiteSpace(lead.ConvertedContactId)
                ? $"convertedContactId: \"{lead.ConvertedContactId}\"\n"
                : string.Empty;
            var content = $"""
                ---
                title: "{title}"
                type: "leads"
                leadId: "{lead.LeadId}"
                {contactNameLine}{titleLine}status: "{EscapeYaml(lead.Status)}"
                source: "{EscapeYaml(lead.Source)}"
                {ratingLine}{emailLine}{phoneLine}{accountIdLine}{convertedContactIdLine}---

                {lead.Description}
                """;
            await File.WriteAllTextAsync(
                Path.Combine(contentDir, "leads", $"{lead.LeadId}.md"),
                content,
                Encoding.UTF8,
                cancellationToken);
        }

        await File.WriteAllTextAsync(
            Path.Combine(dataDir, "leads.json"),
            JsonSerializer.Serialize(canon.Leads, JsonOptions),
            Utf8NoBom,
            cancellationToken);

        var quotesIndexContent = """
            ---
            title: Quotes
            ---

            Sales quotes and estimates from the Turpinverse canon — proposal-stage commercial documents with nested line items.

            """;
        await File.WriteAllTextAsync(
            Path.Combine(contentDir, "quotes", "_index.md"),
            quotesIndexContent,
            Encoding.UTF8,
            cancellationToken);

        foreach (var quote in canon.Quotes)
        {
            var contactLine = !string.IsNullOrWhiteSpace(quote.ContactId)
                ? $"contactId: \"{quote.ContactId}\"\n"
                : string.Empty;
            var dealLine = !string.IsNullOrWhiteSpace(quote.DealId)
                ? $"dealId: \"{quote.DealId}\"\n"
                : string.Empty;
            var notesBody = quote.Notes ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(quote.Terms))
            {
                notesBody = string.IsNullOrWhiteSpace(notesBody)
                    ? quote.Terms
                    : $"{notesBody}\n\n**Terms:** {quote.Terms}";
            }

            var content = $"""
                ---
                title: "{EscapeYaml(quote.QuoteNumber)}"
                type: "quotes"
                quoteId: "{quote.QuoteId}"
                quoteNumber: "{EscapeYaml(quote.QuoteNumber)}"
                accountId: "{quote.AccountId}"
                {contactLine}{dealLine}status: "{EscapeYaml(quote.Status)}"
                issueDate: "{quote.IssueDate}"
                expiryDate: "{quote.ExpiryDate}"
                currency: "{quote.Currency}"
                subtotal: {quote.Subtotal}
                taxTotal: {quote.TaxTotal}
                total: {quote.Total}
                ---

                {notesBody}
                """;
            await File.WriteAllTextAsync(
                Path.Combine(contentDir, "quotes", $"{quote.QuoteId}.md"),
                content,
                Encoding.UTF8,
                cancellationToken);
        }

        await File.WriteAllTextAsync(
            Path.Combine(dataDir, "quotes.json"),
            JsonSerializer.Serialize(canon.Quotes, JsonOptions),
            Utf8NoBom,
            cancellationToken);

        var salesOrdersIndexContent = """
            ---
            title: Sales Orders
            ---

            Sales orders from the Turpinverse canon — fulfilment documents after quotes are accepted, with nested line items.

            """;
        await File.WriteAllTextAsync(
            Path.Combine(contentDir, "sales-orders", "_index.md"),
            salesOrdersIndexContent,
            Encoding.UTF8,
            cancellationToken);

        foreach (var salesOrder in canon.SalesOrders)
        {
            var contactLine = !string.IsNullOrWhiteSpace(salesOrder.ContactId)
                ? $"contactId: \"{salesOrder.ContactId}\"\n"
                : string.Empty;
            var dealLine = !string.IsNullOrWhiteSpace(salesOrder.DealId)
                ? $"dealId: \"{salesOrder.DealId}\"\n"
                : string.Empty;
            var deliveryLine = !string.IsNullOrWhiteSpace(salesOrder.RequestedDeliveryDate)
                ? $"requestedDeliveryDate: \"{salesOrder.RequestedDeliveryDate}\"\n"
                : string.Empty;
            var notesBody = salesOrder.Notes ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(salesOrder.Terms))
            {
                notesBody = string.IsNullOrWhiteSpace(notesBody)
                    ? salesOrder.Terms
                    : $"{notesBody}\n\n**Terms:** {salesOrder.Terms}";
            }

            var content = $"""
                ---
                title: "{EscapeYaml(salesOrder.OrderNumber)}"
                type: "sales-orders"
                salesOrderId: "{salesOrder.SalesOrderId}"
                orderNumber: "{EscapeYaml(salesOrder.OrderNumber)}"
                accountId: "{salesOrder.AccountId}"
                {contactLine}{dealLine}status: "{EscapeYaml(salesOrder.Status)}"
                orderDate: "{salesOrder.OrderDate}"
                {deliveryLine}currency: "{salesOrder.Currency}"
                subtotal: {salesOrder.Subtotal}
                taxTotal: {salesOrder.TaxTotal}
                total: {salesOrder.Total}
                ---

                {notesBody}
                """;
            await File.WriteAllTextAsync(
                Path.Combine(contentDir, "sales-orders", $"{salesOrder.SalesOrderId}.md"),
                content,
                Encoding.UTF8,
                cancellationToken);
        }

        await File.WriteAllTextAsync(
            Path.Combine(dataDir, "sales-orders.json"),
            JsonSerializer.Serialize(canon.SalesOrders, JsonOptions),
            Utf8NoBom,
            cancellationToken);

        var invoicesIndexContent = """
            ---
            title: Invoices
            ---

            Sales invoices from the Turpinverse canon — accounts receivable documents with nested line items and payment history.

            """;
        await File.WriteAllTextAsync(
            Path.Combine(contentDir, "invoices", "_index.md"),
            invoicesIndexContent,
            Encoding.UTF8,
            cancellationToken);

        foreach (var invoice in canon.Invoices)
        {
            var contactLine = !string.IsNullOrWhiteSpace(invoice.ContactId)
                ? $"contactId: \"{invoice.ContactId}\"\n"
                : string.Empty;
            var dealLine = !string.IsNullOrWhiteSpace(invoice.DealId)
                ? $"dealId: \"{invoice.DealId}\"\n"
                : string.Empty;
            var caseLine = !string.IsNullOrWhiteSpace(invoice.CaseId)
                ? $"caseId: \"{invoice.CaseId}\"\n"
                : string.Empty;
            var notesBody = invoice.Notes ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(invoice.Terms))
            {
                notesBody = string.IsNullOrWhiteSpace(notesBody)
                    ? invoice.Terms
                    : $"{notesBody}\n\n**Terms:** {invoice.Terms}";
            }

            var content = $"""
                ---
                title: "{EscapeYaml(invoice.InvoiceNumber)}"
                type: "invoices"
                invoiceId: "{invoice.InvoiceId}"
                invoiceNumber: "{EscapeYaml(invoice.InvoiceNumber)}"
                accountId: "{invoice.AccountId}"
                {contactLine}{dealLine}{caseLine}status: "{EscapeYaml(invoice.Status)}"
                issueDate: "{invoice.IssueDate}"
                dueDate: "{invoice.DueDate}"
                currency: "{invoice.Currency}"
                subtotal: {invoice.Subtotal}
                taxTotal: {invoice.TaxTotal}
                total: {invoice.Total}
                amountDue: {invoice.AmountDue}
                ---

                {notesBody}
                """;
            await File.WriteAllTextAsync(
                Path.Combine(contentDir, "invoices", $"{invoice.InvoiceId}.md"),
                content,
                Encoding.UTF8,
                cancellationToken);
        }

        await File.WriteAllTextAsync(
            Path.Combine(dataDir, "invoices.json"),
            JsonSerializer.Serialize(canon.Invoices, JsonOptions),
            Utf8NoBom,
            cancellationToken);

        var billsIndexContent = """
            ---
            title: Bills
            ---

            Supplier bills from the Turpinverse canon — accounts payable purchase invoices with nested line items.

            """;
        await File.WriteAllTextAsync(
            Path.Combine(contentDir, "bills", "_index.md"),
            billsIndexContent,
            Encoding.UTF8,
            cancellationToken);

        foreach (var bill in canon.Bills)
        {
            var contactLine = !string.IsNullOrWhiteSpace(bill.ContactId)
                ? $"contactId: \"{bill.ContactId}\"\n"
                : string.Empty;
            var dealLine = !string.IsNullOrWhiteSpace(bill.DealId)
                ? $"dealId: \"{bill.DealId}\"\n"
                : string.Empty;
            var caseLine = !string.IsNullOrWhiteSpace(bill.CaseId)
                ? $"caseId: \"{bill.CaseId}\"\n"
                : string.Empty;

            var content = $"""
                ---
                title: "{EscapeYaml(bill.BillNumber)}"
                type: "bills"
                billId: "{bill.BillId}"
                billNumber: "{EscapeYaml(bill.BillNumber)}"
                supplierAccountId: "{bill.SupplierAccountId}"
                {contactLine}{dealLine}{caseLine}status: "{EscapeYaml(bill.Status)}"
                issueDate: "{bill.IssueDate}"
                dueDate: "{bill.DueDate}"
                currency: "{bill.Currency}"
                subtotal: {bill.Subtotal}
                taxTotal: {bill.TaxTotal}
                total: {bill.Total}
                amountDue: {bill.AmountDue}
                ---

                {bill.Notes ?? string.Empty}
                """;
            await File.WriteAllTextAsync(
                Path.Combine(contentDir, "bills", $"{bill.BillId}.md"),
                content,
                Encoding.UTF8,
                cancellationToken);
        }

        await File.WriteAllTextAsync(
            Path.Combine(dataDir, "bills.json"),
            JsonSerializer.Serialize(canon.Bills, JsonOptions),
            Utf8NoBom,
            cancellationToken);

        var paymentsIndexContent = """
            ---
            title: Payments
            ---

            Payments from the Turpinverse canon — settlements against sales invoices or supplier bills.

            """;
        await File.WriteAllTextAsync(
            Path.Combine(contentDir, "payments", "_index.md"),
            paymentsIndexContent,
            Encoding.UTF8,
            cancellationToken);

        foreach (var payment in canon.Payments)
        {
            var invoiceLine = !string.IsNullOrWhiteSpace(payment.InvoiceId)
                ? $"invoiceId: \"{payment.InvoiceId}\"\n"
                : string.Empty;
            var billLine = !string.IsNullOrWhiteSpace(payment.BillId)
                ? $"billId: \"{payment.BillId}\"\n"
                : string.Empty;
            var referenceLine = !string.IsNullOrWhiteSpace(payment.Reference)
                ? $"reference: \"{EscapeYaml(payment.Reference)}\"\n"
                : string.Empty;
            var description = !string.IsNullOrWhiteSpace(payment.Reference)
                ? $"Payment reference {payment.Reference}."
                : !string.IsNullOrWhiteSpace(payment.BillId)
                    ? "Supplier payment against a bill."
                    : "Customer payment against a sales invoice.";

            var content = $"""
                ---
                title: "{EscapeYaml(payment.PaymentId)}"
                type: "payments"
                paymentId: "{payment.PaymentId}"
                paymentDate: "{payment.PaymentDate}"
                amount: {payment.Amount}
                method: "{EscapeYaml(payment.Method)}"
                {invoiceLine}{billLine}{referenceLine}---

                {description}
                """;
            await File.WriteAllTextAsync(
                Path.Combine(contentDir, "payments", $"{payment.PaymentId}.md"),
                content,
                Encoding.UTF8,
                cancellationToken);
        }

        await File.WriteAllTextAsync(
            Path.Combine(dataDir, "payments.json"),
            JsonSerializer.Serialize(canon.Payments, JsonOptions),
            Utf8NoBom,
            cancellationToken);

        var creditNotesIndexContent = """
            ---
            title: Credit Notes
            ---

            Accounts receivable credit notes from the Turpinverse canon — adjustments and dispute credits with nested line items.

            """;
        await File.WriteAllTextAsync(
            Path.Combine(contentDir, "credit-notes", "_index.md"),
            creditNotesIndexContent,
            Encoding.UTF8,
            cancellationToken);

        foreach (var creditNote in canon.CreditNotes)
        {
            var contactLine = !string.IsNullOrWhiteSpace(creditNote.ContactId)
                ? $"contactId: \"{creditNote.ContactId}\"\n"
                : string.Empty;
            var invoiceLine = !string.IsNullOrWhiteSpace(creditNote.InvoiceId)
                ? $"invoiceId: \"{creditNote.InvoiceId}\"\n"
                : string.Empty;

            var content = $"""
                ---
                title: "{EscapeYaml(creditNote.CreditNoteNumber)}"
                type: "credit-notes"
                creditNoteId: "{creditNote.CreditNoteId}"
                creditNoteNumber: "{EscapeYaml(creditNote.CreditNoteNumber)}"
                accountId: "{creditNote.AccountId}"
                {contactLine}{invoiceLine}issueDate: "{creditNote.IssueDate}"
                currency: "{creditNote.Currency}"
                subtotal: {creditNote.Subtotal}
                taxTotal: {creditNote.TaxTotal}
                total: {creditNote.Total}
                ---

                {creditNote.Notes ?? string.Empty}
                """;
            await File.WriteAllTextAsync(
                Path.Combine(contentDir, "credit-notes", $"{creditNote.CreditNoteId}.md"),
                content,
                Encoding.UTF8,
                cancellationToken);
        }

        await File.WriteAllTextAsync(
            Path.Combine(dataDir, "credit-notes.json"),
            JsonSerializer.Serialize(canon.CreditNotes, JsonOptions),
            Utf8NoBom,
            cancellationToken);

        await File.WriteAllTextAsync(
            Path.Combine(dataDir, "tax-rates.json"),
            JsonSerializer.Serialize(canon.TaxRates, JsonOptions),
            Utf8NoBom,
            cancellationToken);

        var publishedArticles = canon.Articles.Where(a => !a.Draft).ToList();

        var articlesIndexContent = """
            ---
            title: Articles
            ---

            Thought leadership and team journal pieces from the Turpinverse canon — bylines, collection labels, and in-universe copy.

            """;
        await File.WriteAllTextAsync(
            Path.Combine(contentDir, "articles", "_index.md"),
            articlesIndexContent,
            Encoding.UTF8,
            cancellationToken);

        foreach (var article in publishedArticles)
        {
            var authorName = personaNames.GetValueOrDefault(article.AuthorPersonaId, article.AuthorPersonaId);
            var tagsLine = article.Tags.Count > 0
                ? $"tags: {JsonSerializer.Serialize(article.Tags)}\n"
                : string.Empty;
            var featuredImageLine = !string.IsNullOrWhiteSpace(article.FeaturedImage)
                ? $"featuredImage: \"{EscapeYaml(article.FeaturedImage)}\"\n"
                : string.Empty;
            var excerptLine = !string.IsNullOrWhiteSpace(article.Excerpt)
                ? $"excerpt: \"{EscapeYaml(article.Excerpt)}\"\n"
                : string.Empty;
            var showTocLine = article.ShowTableOfContents == true
                ? "showToc: true\n"
                : string.Empty;
            var relatedProjectLine = !string.IsNullOrWhiteSpace(article.RelatedProjectId)
                ? $"relatedProjectId: \"{article.RelatedProjectId}\"\n"
                : string.Empty;
            var relatedCaseLine = !string.IsNullOrWhiteSpace(article.RelatedCaseId)
                ? $"relatedCaseId: \"{article.RelatedCaseId}\"\n"
                : string.Empty;

            var content = $"""
                ---
                title: "{EscapeYaml(article.Title)}"
                type: "articles"
                publishedAt: "{article.PublishedAt}"
                authorPersonaId: "{article.AuthorPersonaId}"
                authorName: "{EscapeYaml(authorName)}"
                collection: "{EscapeYaml(article.Collection)}"
                {tagsLine}{featuredImageLine}{excerptLine}{showTocLine}{relatedProjectLine}{relatedCaseLine}---

                {article.Body}
                """;
            await File.WriteAllTextAsync(
                Path.Combine(contentDir, "articles", $"{article.Id}.md"),
                content,
                Encoding.UTF8,
                cancellationToken);
        }

        await File.WriteAllTextAsync(
            Path.Combine(dataDir, "articles.json"),
            JsonSerializer.Serialize(publishedArticles, JsonOptions),
            Utf8NoBom,
            cancellationToken);

        var galleriesIndexContent = """
            ---
            title: Gallery
            ---

            Team and workplace imagery from the Turpinverse canon — captioned galleries with optional lightbox viewing.

            """;
        await File.WriteAllTextAsync(
            Path.Combine(contentDir, "galleries", "_index.md"),
            galleriesIndexContent,
            Encoding.UTF8,
            cancellationToken);

        foreach (var gallery in canon.Galleries)
        {
            var descriptionLine = !string.IsNullOrWhiteSpace(gallery.Description)
                ? $"description: \"{EscapeYaml(gallery.Description)}\"\n"
                : string.Empty;
            var viewerModeLine = !string.IsNullOrWhiteSpace(gallery.Viewer.Mode)
                ? $"viewerMode: \"{EscapeYaml(gallery.Viewer.Mode)}\"\n"
                : string.Empty;
            var body = !string.IsNullOrWhiteSpace(gallery.Description)
                ? gallery.Description
                : string.Empty;

            var content = $"""
                ---
                title: "{EscapeYaml(gallery.Title)}"
                type: "galleries"
                galleryId: "{gallery.Id}"
                subject: "{gallery.Subject}"
                viewerEnabled: {gallery.Viewer.Enabled.ToString().ToLowerInvariant()}
                {descriptionLine}{viewerModeLine}---

                {body}
                """;
            await File.WriteAllTextAsync(
                Path.Combine(contentDir, "galleries", $"{gallery.Id}.md"),
                content,
                Encoding.UTF8,
                cancellationToken);
        }

        await File.WriteAllTextAsync(
            Path.Combine(dataDir, "galleries.json"),
            JsonSerializer.Serialize(canon.Galleries, JsonOptions),
            Utf8NoBom,
            cancellationToken);

        foreach (var persona in canon.Personas)
        {
            if (!_presenter.HasCareerOrPortfolioContent(canon, persona.Id))
            {
                continue;
            }

            var careerData = new
            {
                experience = _presenter.GetExperienceForPersona(canon, persona.Id)
                    .Select(CareerLinkResolver.ResolveExperience)
                    .ToList(),
                education = _presenter.GetEducationForPersona(canon, persona.Id)
                    .Select(CareerLinkResolver.ResolveEducation)
                    .ToList(),
                projects = _presenter.GetProjectsForPersona(canon, persona.Id)
                    .Select(CareerLinkResolver.ResolveProject)
                    .ToList(),
                achievements = _presenter.GetAchievementsForPersona(canon, persona.Id)
                    .Select(CareerLinkResolver.ResolveAchievement)
                    .ToList()
            };

            await File.WriteAllTextAsync(
                Path.Combine(dataDir, "career", $"{persona.Id}.json"),
                JsonSerializer.Serialize(careerData, JsonOptions),
                Utf8NoBom,
                cancellationToken);
        }

        foreach (var extras in canon.ProfessionalExtras)
        {
            if (!_extrasPresenter.HasExtrasContent(extras))
            {
                continue;
            }

            var profileData = new
            {
                intro = extras.Intro is null ? null : new
                {
                    extras.Intro.ShortIntro,
                    extras.Intro.Headline,
                    extras.Intro.Subtitle,
                    extras.Intro.Photo,
                    cta = extras.Intro.Cta is null ? null : CareerLinkResolver.ResolveLink(extras.Intro.Cta)
                },
                about = extras.About,
                skillsHeading = extras.SkillsHeading,
                skills = _extrasPresenter.GetSkills(extras),
                contact = extras.Contact is null ? null : new
                {
                    extras.Contact.Copy,
                    extras.Contact.Email,
                    extras.Contact.Phone,
                    cta = extras.Contact.Cta is null ? null : CareerLinkResolver.ResolveLink(extras.Contact.Cta)
                },
                socials = _extrasPresenter.GetSocials(extras)
            };

            await File.WriteAllTextAsync(
                Path.Combine(dataDir, "profile", $"{extras.PersonaId}.json"),
                JsonSerializer.Serialize(profileData, JsonOptions),
                Utf8NoBom,
                cancellationToken);
        }
    }

    private static string ExtractSummary(string biography)
    {
        foreach (var line in biography.Split('\n'))
        {
            var trimmed = line.Trim();
            if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith('>'))
            {
                continue;
            }

            return trimmed;
        }

        return string.Empty;
    }

    private static string EscapeYaml(string value) =>
        value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\r\n", "\\n")
            .Replace("\n", "\\n")
            .Replace("\r", "\\n");

    private static string FormatAddressYaml(string key, Address address)
    {
        var lines = new List<string> { $"{key}:" };
        AppendAddressField(lines, "address1", address.Address1);
        AppendAddressField(lines, "address2", address.Address2);
        AppendAddressField(lines, "address3", address.Address3);
        AppendAddressField(lines, "town", address.Town);
        AppendAddressField(lines, "region", address.Region);
        AppendAddressField(lines, "postcode", address.Postcode);
        AppendAddressField(lines, "country", address.Country);
        return string.Join('\n', lines) + '\n';
    }

    private static void AppendAddressField(List<string> lines, string fieldName, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        lines.Add($"  {fieldName}: \"{EscapeYaml(value)}\"");
    }
}
