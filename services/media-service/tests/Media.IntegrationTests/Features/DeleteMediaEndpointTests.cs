[Collection("Media")]
public class DeleteMediaEndpointTests(MediaApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task DeleteMedia_ExistingId_Returns204()
    {
        var uploadContent = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent([1, 2, 3]);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        uploadContent.Add(fileContent, "file", "test.png");
        uploadContent.Add(new StringContent(Guid.NewGuid().ToString()), "OwnerId");
        uploadContent.Add(new StringContent("0"), "OwnerType");

        var uploadResponse = await _client.PostAsync("/media/upload", uploadContent);
        var uploaded = await uploadResponse.Content.ReadFromJsonAsync<MediaFileResponseDto>();

        var response = await _client.DeleteAsync($"/media/{uploaded!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteMedia_NonExistingId_Returns404()
    {
        var response = await _client.DeleteAsync($"/media/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}