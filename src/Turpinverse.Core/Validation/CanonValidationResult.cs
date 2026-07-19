namespace Turpinverse.Core.Validation;

public sealed record ValidationViolation(
    string Rule,
    string Message,
    string EntityType,
    string EntityId);

public sealed record CanonValidationResult(
    bool Valid,
    string CanonVersion,
    IReadOnlyDictionary<string, int> Counts,
    IReadOnlyList<ValidationViolation> Violations);
