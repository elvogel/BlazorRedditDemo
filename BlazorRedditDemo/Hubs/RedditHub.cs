using System.Timers;
using BRD.Common;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using SerilogTimings.Extensions;
using ILogger = Serilog.ILogger;

namespace BlazorRedditDemo.Hubs;


//https://learn.microsoft.com/en-us/aspnet/core/signalr/streaming?view=aspnetcore-8.0
public class RedditHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        log.Debug("running stats from connection");
        await runStats();
        await Clients.All.SendAsync("ReceiveMessage", stats);
        timer.Start();
    }

    private readonly System.Timers.Timer timer;
    private readonly ILogger log;
    private readonly RedditStats stats;
    private readonly HttpClient client;
    private readonly IHubContext<RedditHub> context;
    public RedditHub(ILogger logger, IHttpClientFactory clientFactory, IHubContext<RedditHub> hubContext)
    {
        context = hubContext;
        timer = new System.Timers.Timer(8000);
        timer.Elapsed += TimerOnElapsed;
        log = logger;
        stats = new RedditStats();
        client = clientFactory.CreateClient("Reddit");
    }

    private void TimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        _ = runStats().ConfigureAwait(true);
        try
        {
            context.Clients.All.SendAsync("ReceiveMessage", stats);
        }
        catch (Exception ex)
        {
            log.Debug("{Ex} in timer event: {Msg}",
                ex.GetType(),ex.Message);
        }
    }

    private async Task runStats()
    {
        var op = log.BeginOperation("runStats");
        var req = new HttpRequestMessage(HttpMethod.Get, "https://www.reddit.com/r/worldnews/hot.json?limit=100");
        var response = await client.SendAsync(req);
        var data = await response.Content.ReadAsStringAsync();
        //log.Debug("{Data}",data);
        var content = JsonConvert.DeserializeObject<Listing>(data);
        if (content == null) return;
        var list = content.data.children.ToList();

        var top10 = list.OrderByDescending(x => x.data.ups).Take(10).ToList();
        stats.Posts.Clear();
        stats.Users.Clear();
        foreach (var post in top10)
        {
            stats.Posts.Add(post.data.title, post.data.ups);
        }

        foreach (var post in list)
        {
            if (!stats.Users.TryAdd(post.data.author, 1))
                stats.Users[post.data.author]++;
        }
        op.Complete();
    }
}
