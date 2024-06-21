# BlazorRedditDemo
A Blazor application showing user and group analytics for select Reddit forums

## 3rd Party Dependencies
- [Syncfusion.Blazor](https://www.nuget.org/packages/Syncfusion.Blazor)

This project was built on a Linux desktop using the Rider IDE.

## Solution Architecture
This application leverages the use of a SignalR hub to push timed updates to the page using a `System.Timers.Timer` object. When the timer is triggered, a method is called (and received) on the Razor page to update the lists.

The application makes use of an HttpFactory and assigns a `DelegatingHandler` to the `HttpClient` used to pull data from Reddit. Because the only requirement was for a `User-Agent` header assignment, that's all the handler is doing - further configuration is included but commented out, should the need to go further into the Reddit API were necessary (it wasn't used in this example).

The necessary secrets were stored and accessed in environment variables. I typically prefer to use [Azure App Configuration]() for such things, as it makes life easier and allows for a vast array of configuration options and features. Some of the code that would be used for this scenario is included in the code. Typically, the connection string for AppConfig would be stored in the project secrets file locally, and added to the App Service configuration on deployment. 

## Project Structure
Currently for expediency purposes, the project is organized into a single repository containing three projects:
- BlazorRedditDemo - the Blazor/UI application
- BRD.Common - the common objects library
- BRD.Services - the service/business logic layer
- BRD.Console - test console for Reddit API

For example's sake, we're using Azure DevOps in the rest of this discussion.
In a production environment these projects would reside in their own repository, under a shared project folder. There would be separate csproj files called `BlazorRedditDemo.Dev` and `BRD.Services.Dev` that would open up under a solution called `BlazorRedditDemo.Dev.sln` where the projects would directly reference each other. Each repository would have its own pipeline that would build the 'non-dev' project in dependency order and push into Azure Artifacts. the 'production' csproj files would have a reference to their respective dependencies as such:

```csharp
<PackageReference Include="BRD.Services" Version="1.0.*" />
```
This allows the DevOps pipeline to pull the most recent version out of the Artifacts feed.

For dependency references, I have built a NuGet utility called [cspmerge](https://www.nuget.org/packages/CSPMerge) that allows for the synchronization of other, specifically-versioned NuGet dependencies between project files. This allows for a more fine-tuned control of version upgrades in projects where there may exist a gap between dev and non-dev projects, while also minimizing potential git merge conflicts on critical csproj files. 

