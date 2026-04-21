namespace TGPSound.Services;

public static class Http
{
    public static readonly HttpClient Client;
    static Http()
    {
        Client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        Client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
        Client.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
    }
}