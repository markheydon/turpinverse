using Turpinverse.Core.Models;

namespace Turpinverse.Core.Abstractions;

public interface ICanonRepository
{
    Task<Canon> LoadAsync(CancellationToken cancellationToken = default);
}
