using BRD.Common;
using BRD.Common.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Syncfusion.Blazor.Spinner.Internal;
using ILogger = Serilog.ILogger;

#pragma warning disable CS0169 // Field is never used
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace BlazorRedditDemo.Components.Pages;

public partial class Home
{
    private HubConnection? hubConnection;

    [Inject] private ILogger log { get; set; }
    [Inject] public NavigationManager Navigation { get; set; }
    private List<ListBoxItem> posts { get; set; } = new();
    private List<ListBoxItem> users { get; set; } = new();
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        if (hubConnection == null)
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(Navigation.ToAbsoluteUri("/reddithub"))
                .Build();

            hubConnection.On<RedditStats>("ReceiveMessage", (message) =>
            {
                posts.Clear();
                users.Clear();
                foreach (var post in message.Posts)
                {
                    posts.Add(new ListBoxItem(){Count = post.Value,Title = post.Key});
                }

                foreach (var user in message.Users)
                {
                    users.Add(new ListBoxItem(){Count = user.Value,Title = user.Key});
                }
                InvokeAsync(StateHasChanged);
            });

            await hubConnection.StartAsync();
        }
    }

}
