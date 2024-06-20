using BRD.Common;
using Newtonsoft.Json;
using System.Linq;

// See https://aka.ms/new-console-template for more information

Console.WriteLine("Reddit test");

var client = new HttpClient();
client.DefaultRequestHeaders.Add("User-Agent",Environment.GetEnvironmentVariable("UserAgent"));
var req = new HttpRequestMessage(HttpMethod.Get, "https://www.reddit.com/r/funny/top.json");
var response = await client.SendAsync(req);
var content = JsonConvert.DeserializeObject<Listing>(await response.Content.ReadAsStringAsync());

var list = content.data.children.ToList();

var top10 = list.OrderByDescending(x => x.data.ups).Take(10).ToList();

foreach (var post in top10)
{
    Console.WriteLine($"{post.data.author}\t\t{post.data.ups}\t\t{post.data.title}");
}
