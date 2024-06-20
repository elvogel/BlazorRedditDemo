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
        log.Debug("Connected to RedditHub");
        await base.OnConnectedAsync();
        await runStats();
        timer.Start();
    }

    private readonly System.Timers.Timer timer;
    private readonly ILogger log;
    private readonly RedditStats stats;
    private readonly HttpClient client;
    public RedditHub(ILogger logger, IHttpClientFactory clientFactory)
    {
        timer = new System.Timers.Timer(8000);
        timer.Elapsed += TimerOnElapsed;
        log = logger;
        stats = new RedditStats();
        client = clientFactory.CreateClient("Reddit");
    }

    private void TimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        _ = runStats().ConfigureAwait(true);

        Clients.All.SendAsync("TimerEvent", stats);
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
        foreach (var post in top10)
        {
            stats.Posts.Add(post.data.name, post.data.ups);
        }

        foreach (var post in list)
        {
            if (!stats.Users.TryAdd(post.data.author, 1))
                stats.Users[post.data.author]++;
        }
        op.Complete();
    }
}
