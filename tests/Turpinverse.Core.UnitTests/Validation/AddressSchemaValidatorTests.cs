using Turpinverse.Core.Abstractions;
using Turpinverse.Core.Validation;
using Turpinverse.Data.Repositories;

namespace Turpinverse.Core.UnitTests.Validation;

[Trait("Category", "CanonValidation")]
public class AddressSchemaValidatorTests
{
    private readonly ICanonRepository _repository = new JsonCanonRepository();

    [Fact]
    public async Task Validate_LoadedCanon_EveryOrganisationHasRegisteredOffice()
    {
        var canon = await _repository.LoadAsync(TestContext.Current.CancellationToken);

        Assert.All(canon.Organisations, org =>
        {
            Assert.False(string.IsNullOrWhiteSpace(org.RegisteredOffice.Address1));
            Assert.False(string.IsNullOrWhiteSpace(org.RegisteredOffice.Town));
            Assert.False(string.IsNullOrWhiteSpace(org.RegisteredOffice.Postcode));
            Assert.False(string.IsNullOrWhiteSpace(org.RegisteredOffice.Country));
        });
    }

    [Fact]
    public async Task Validate_LoadedCanon_PassesAddressSchema()
    {
        var canon = await _repository.LoadAsync(TestContext.Current.CancellationToken);
        var violations = CanonSchemaValidator.Validate(canon);

        Assert.Empty(violations);
    }

    [Fact]
    public void TryValidateJson_OrganisationWithoutRegisteredOffice_FailsSchema()
    {
        var json = """
            {
              "version": "1.3.0",
              "personas": [],
              "organisations": [
                {
                  "id": "turpin-enterprises",
                  "tradingName": "Turpin Enterprises",
                  "description": "Consulting",
                  "industry": "Consulting",
                  "historicalAnchor": "Legend",
                  "memberPersonaIds": ["dick-turpin"],
                  "status": "active"
                }
              ],
              "events": [],
              "aliases": [],
              "toneGuidelines": {
                "version": "1.0.0",
                "principles": ["A", "B", "C"],
                "examples": [],
                "forbiddenPatterns": []
              },
              "deals": [],
              "cases": [],
              "experience": [],
              "education": [],
              "projects": [],
              "achievements": [],
              "articles": [],
              "galleries": [],
              "professionalExtras": []
            }
            """;

        var valid = CanonSchemaValidator.TryValidateJson(json, out var violations);
        Assert.False(valid);
        Assert.Contains(violations, v => v.Message.Contains("registeredOffice", StringComparison.OrdinalIgnoreCase));
    }
}
