[Collection("Media")]
public class UploadMediaEndpointTests(MediaApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task UploadMedia_ValidFile_Returns201()
    {
        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent([1, 2, 3]);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        content.Add(fileContent, "file", "test.png");
        content.Add(new StringContent(Guid.NewGuid().ToString()), "OwnerId");
        content.Add(new StringContent("0"), "OwnerType");

        var response = await _client.PostAsync("/media/upload", content);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task UploadMedia_NoFile_Returns400()
    {
        var content = new MultipartFormDataContent();
        content.Add(new StringContent(Guid.NewGuid().ToString()), "OwnerId");
        content.Add(new StringContent("0"), "OwnerType");

        var response = await _client.PostAsync("/media/upload", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}