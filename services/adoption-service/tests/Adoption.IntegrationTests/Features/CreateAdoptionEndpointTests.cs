[Collection("Adoption")]
public sealed class CreateAdoptionEndpointTests(AdoptionApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreateAdoption_ValidRequest_Returns201()
    {
        var response = await _client.PostAsJsonAsync("/adoptions", new
        {
            petId = Guid.NewGuid(),
            adopterId = Guid.NewGuid(),
            message = "Evladlığa götürmək istəyirəm.",
            contactPhone = "+994501234567"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateAdoption_InvalidRequest_Returns400()
    {
        var response = await _client.PostAsync("/adoptions",
            new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
