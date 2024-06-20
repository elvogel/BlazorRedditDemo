namespace BRD.Common;

/// <summary>
/// The Reddit statistics we're tracking for a single subreddit.
/// </summary>
public class RedditStats
{
    /// <summary>
    /// User statistics: username, posts
    /// </summary>
    public Dictionary<string, int> Users { get; set; } = new();
    /// <summary>
    /// Post statistics: Title, votes
    /// </summary>
    public Dictionary<string, int> Posts { get; set; } = new();
}
