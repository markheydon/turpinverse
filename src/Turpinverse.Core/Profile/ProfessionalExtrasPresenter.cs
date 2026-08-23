using Turpinverse.Core.Career;
using Turpinverse.Core.Models;

namespace Turpinverse.Core.Profile;

public sealed class ProfessionalExtrasPresenter
{
    public const string PrimaryPersonaId = CareerPortfolioPresenter.PrimaryPersonaId;

    public ProfessionalExtras? GetExtrasForPersona(Canon canon, string personaId) =>
        canon.ProfessionalExtras.FirstOrDefault(e => e.PersonaId == personaId);

    public bool HasExtrasContent(ProfessionalExtras? extras) =>
        extras is not null && (
            HasIntro(extras)
            || HasAbout(extras)
            || HasSkills(extras)
            || HasContact(extras)
            || HasSocials(extras));

    public bool HasIntro(ProfessionalExtras? extras) =>
        extras?.Intro is { ShortIntro: { Length: > 0 }, Headline: { Length: > 0 }, Subtitle: { Length: > 0 } };

    public bool HasAbout(ProfessionalExtras? extras) =>
        !string.IsNullOrWhiteSpace(extras?.About);

    public bool HasSkills(ProfessionalExtras? extras) =>
        !string.IsNullOrWhiteSpace(extras?.SkillsHeading)
        && extras.Skills.Count > 0;

    public bool HasContact(ProfessionalExtras? extras) =>
        extras?.Contact is { Copy: { Length: > 0 }, Email: { Length: > 0 } };

    public bool HasSocials(ProfessionalExtras? extras) =>
        extras?.Socials.Count > 0;

    public bool ShouldSuppressTitle(ProfessionalExtras? extras) => HasIntro(extras);

    public bool ShouldSuppressBiography(ProfessionalExtras? extras) => HasAbout(extras);

    public bool ShouldSuppressHeaderEmail(ProfessionalExtras? extras) => HasContact(extras);

    public IReadOnlyList<string> GetSkills(ProfessionalExtras extras) => extras.Skills;

    public IReadOnlyList<ProfessionalSocial> GetSocials(ProfessionalExtras extras) => extras.Socials;
}
