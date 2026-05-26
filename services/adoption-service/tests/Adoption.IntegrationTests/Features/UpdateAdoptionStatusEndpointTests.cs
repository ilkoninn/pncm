[Collection("Adoption")]
public sealed class UpdateAdoptionStatusEndpointTests(AdoptionApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task UpdateAdoptionStatus_ExistingId_Returns200()
    {
        var createResponse = await _client.PostAsJsonAsync("/adoptions", new
        {
            petId = Guid.NewGuid(),
            adopterId = Guid.NewGuid(),
            message = "Test müraciəti.",
            contactPhone = "+994501234567"
        });

        var created = await createResponse.Content.ReadFromJsonAsync<AdoptionResponseDto>();

        var response = await _client.PatchAsJsonAsync($"/adoptions/{created!.Id}/status", new
        {
            status = EAdoptionStatus.Approved
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateAdoptionStatus_NonExistingId_Returns404()
    {
        var response = await _client.PatchAsJsonAsync($"/adoptions/{Guid.NewGuid()}/status", new
        {
            status = EAdoptionStatus.Approved
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
