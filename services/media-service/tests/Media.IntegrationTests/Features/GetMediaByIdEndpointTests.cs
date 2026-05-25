[Collection("Media")]
public class GetMediaByIdEndpointTests(MediaApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetMediaById_ExistingId_Returns200WithPresignedUrl()
    {
        var uploadContent = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent([1, 2, 3]);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        uploadContent.Add(fileContent, "file", "test.png");
        uploadContent.Add(new StringContent(Guid.NewGuid().ToString()), "OwnerId");
        uploadContent.Add(new StringContent("0"), "OwnerType");

        var uploadResponse = await _client.PostAsync("/media/upload", uploadContent);
        var uploaded = await uploadResponse.Content.ReadFromJsonAsync<MediaFileResponseDto>();

        var response = await _client.GetAsync($"/media/{uploaded!.Id}");
        var result = await response.Content.ReadFromJsonAsync<MediaFileResponseDto>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result!.Url.Should().Contain("X-Amz-Signature");
    }

    [Fact]
    public async Task GetMediaById_NonExistingId_Returns404()
    {
        var response = await _client.GetAsync($"/media/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}