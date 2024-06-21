namespace BRD.Services.Handlers;

public class RedditHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {

        request.Headers.Add("User-Agent",Environment.GetEnvironmentVariable("UserAgent"));

        /*
        if (!cache.TryGetValue("RedditToken", out string? token))
        {
            var client = new HttpClient();
            var reqMsg = new HttpRequestMessage(HttpMethod.Post, "https://www.reddit.com/api/v1/access_token");
            reqMsg.Content = new StringContent("grant_type=client_credentials",new MediaTypeHeaderValue("application/x-www-form-urlencoded"));
            var bytes = Encoding.ASCII.GetBytes(
                $"{Environment.GetEnvironmentVariable("ClientId")}:{Environment.GetEnvironmentVariable("ClientSecret")}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));
            var response = await client.SendAsync(reqMsg, cancellationToken);
            var t = JsonConvert.DeserializeObject<Token>(await response.Content.ReadAsStringAsync(cancellationToken));
            cache.Set("RedditToken", t, TimeSpan.FromMinutes(55)); // short of the 60 minutes
            token = t.access_token;
            client.Dispose();
        }
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        */

        return await base.SendAsync(request, cancellationToken);
    }
    //private readonly ILogger log;
    //private readonly IMemoryCache cache;

    /// <inheritdoc />
    public RedditHandler(/*ILogger logger, IMemoryCache memoryCache*/)
    {
        //log = logger;
        //cache = memoryCache;
    }
}
