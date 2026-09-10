namespace Turpinverse.Core.Export;

public static class ExportCsvColumns
{
    public static readonly IReadOnlyList<string> Contacts =
    [
        "contactId", "firstName", "lastName", "title", "email",
        "phone", "accountId", "status", "notes",
        "mailingAddress1", "mailingAddress2", "mailingAddress3",
        "mailingTown", "mailingRegion", "mailingPostcode", "mailingCountry"
    ];

    public static readonly IReadOnlyList<string> Accounts =
    [
        "accountId", "accountName", "legalName", "industry",
        "parentAccountId", "primaryContactId", "description", "website", "status", "roles",
        "registeredOfficeAddress1", "registeredOfficeAddress2", "registeredOfficeAddress3",
        "registeredOfficeTown", "registeredOfficeRegion", "registeredOfficePostcode", "registeredOfficeCountry"
    ];

    public static readonly IReadOnlyList<string> Deals =
    [
        "dealId", "dealName", "accountId", "contactId", "stakeholderContactIds", "stage",
        "amount", "closeDate", "description"
    ];

    public static readonly IReadOnlyList<string> Cases =
    [
        "caseId", "subject", "description", "status", "priority",
        "contactId", "accountId", "stakeholderContactIds", "relatedEventId"
    ];

    public static readonly IReadOnlyList<string> Projects =
    [
        "projectId", "title", "summary", "accountId",
        "contactId", "stakeholderContactIds", "dealId", "caseIds",
        "tags", "featured"
    ];

    public static readonly IReadOnlyList<string> Products =
    [
        "productId", "name", "description", "unitPrice", "taxRateId",
        "unitOfMeasure", "status", "sku"
    ];

    public static readonly IReadOnlyList<string> Leads =
    [
        "leadId", "companyName", "contactName", "title", "email", "phone",
        "status", "source", "rating", "description", "accountId", "convertedContactId"
    ];

    public static readonly IReadOnlyList<string> Activities =
    [
        "activityId", "type", "subject", "description", "activityDate", "dueDate",
        "status", "regardingType", "regardingId", "ownerContactId", "durationMinutes"
    ];

    public static readonly IReadOnlyList<string> Quotes =
    [
        "quoteId", "quoteNumber", "accountId", "contactId", "dealId",
        "status", "issueDate", "expiryDate", "currency", "subtotal", "taxTotal", "total",
        "notes", "terms"
    ];

    public static readonly IReadOnlyList<string> SalesOrders =
    [
        "salesOrderId", "orderNumber", "accountId", "contactId", "dealId",
        "status", "orderDate", "requestedDeliveryDate", "currency", "subtotal", "taxTotal", "total",
        "notes", "terms"
    ];

    public static readonly IReadOnlyList<string> Invoices =
    [
        "invoiceId", "invoiceNumber", "accountId", "contactId", "dealId", "caseId",
        "status", "issueDate", "dueDate", "currency", "subtotal", "taxTotal", "total",
        "amountDue", "notes", "terms"
    ];

    public static readonly IReadOnlyList<string> Bills =
    [
        "billId", "billNumber", "supplierAccountId", "contactId", "dealId", "caseId",
        "status", "issueDate", "dueDate", "currency", "subtotal", "taxTotal", "total",
        "amountDue", "notes"
    ];

    public static readonly IReadOnlyList<string> Payments =
    [
        "paymentId", "paymentDate", "amount", "method",
        "invoiceId", "billId", "accountId", "reference"
    ];

    public static readonly IReadOnlyList<string> CreditNotes =
    [
        "creditNoteId", "creditNoteNumber", "accountId", "contactId", "invoiceId",
        "issueDate", "currency", "subtotal", "taxTotal", "total", "notes"
    ];

    public static IReadOnlyList<string> ForDataset(string dataset) =>
        dataset.ToLowerInvariant() switch
        {
            "contacts" => Contacts,
            "accounts" => Accounts,
            "deals" => Deals,
            "cases" => Cases,
            "projects" => Projects,
            "products" => Products,
            "leads" => Leads,
            "activities" => Activities,
            "quotes" => Quotes,
            "sales-orders" => SalesOrders,
            "invoices" => Invoices,
            "bills" => Bills,
            "payments" => Payments,
            "credit-notes" => CreditNotes,
            _ => throw new ArgumentException($"Dataset '{dataset}' is not supported.", nameof(dataset))
        };
}
