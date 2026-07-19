namespace Turpinverse.Core.Abstractions;

public interface IHugoContentGenerator
{
    Task GenerateAsync(string siteRoot, CancellationToken cancellationToken = default);
}
