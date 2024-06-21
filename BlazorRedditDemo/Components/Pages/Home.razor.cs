using BRD.Common;
using BRD.Common.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR.Client;
using Syncfusion.Blazor.Spinner.Internal;
using ILogger = Serilog.ILogger;

#pragma warning disable CS0169 // Field is never used
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace BlazorRedditDemo.Components.Pages;

public partial class Home : IAsyncDisposable
{
    private HubConnection? hubConnection;

    [Inject] private ILogger log { get; set; }
    [Inject] public NavigationManager Navigation { get; set; }
    private List<ListItem> posts { get; set; } = new();
    private List<ListItem> users { get; set; } = new();
    private bool showSpin { get; set; }
    protected override async Task OnInitializedAsync()
    {
        log.Debug("OnInitializedAsync");
        //await base.OnInitializedAsync();
        if (hubConnection == null)
        {
            log.Debug("initializing hubConnection");
            hubConnection = new HubConnectionBuilder()
                .WithUrl(Navigation.ToAbsoluteUri("/reddithub"))
                .WithKeepAliveInterval(TimeSpan.FromMilliseconds(500))
                .WithStatefulReconnect()
                .Build();

            hubConnection.On<RedditStats>("ReceiveMessage", (message) =>
            {
                showSpin = true;
                posts.Clear();
                users.Clear();
               foreach (var post in message.Posts)
                {
                    posts.Add(new ListItem{Count = post.Value,Title = $"{post.Key} ({post.Value:N0} upvotes)"});
                }

                foreach (var user in message.Users)
                {
                    //log.Debug("{Title} : {Count}",user.Key,user.Value);
                    users.Add(new ListItem{Count = user.Value,Title = $"{user.Key} ({user.Value:N0} articles)"});
                }
                users = users.OrderByDescending(x => x.Count).Take(10).ToList();
                showSpin = false;
                InvokeAsync(StateHasChanged);
            });

            hubConnection.Closed += async (error) =>
            {
                if (error == null)
                {
                    log.Debug("hub connection closed with no error, restarting");
                }
                else
                {
                    log.Debug("HubConnection closed with {Ex}: {Msg} -- restarting",
                        error.GetType(),error.Message);
                }
                await Task.Delay(new Random().Next(0,5) * 1000);
                await hubConnection.StartAsync();
            };
            await hubConnection.StartAsync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        log.Debug("DisposeAsync called");
        if (hubConnection != null) await hubConnection.DisposeAsync();
    }
}
