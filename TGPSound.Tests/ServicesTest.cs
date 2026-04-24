using System.Text.Json;
using TGPSound.Services;

namespace TGPSound.Tests;

public class ServicesTest
{
    private readonly JsonSerializerOptions options = new() { WriteIndented = true };

    [Fact]
    public async Task TestPipedYouTubeSearch()
    {
        var result = await APIsServices.PipedYouTubeSearch("bruno mars");
        Console.WriteLine(JsonSerializer.Serialize(result!.Items, options));
        Assert.NotNull(result);
    }

    [Fact]
    public async Task TestPaxsenixYouTubeSearch() {
        var result = await APIsServices.PaxsenixYouTubeSearch("bruno mars");
        Console.WriteLine(JsonSerializer.Serialize(result!, options));
        Assert.NotNull(result);
    }

    [Fact]
    public async Task TestDeezerSearch() {
        var result = await APIsServices.DeezerSearchTrack("bruno mars");
        Console.WriteLine(JsonSerializer.Serialize(result!.Data, options));
        Assert.NotNull(result);
    }
}
