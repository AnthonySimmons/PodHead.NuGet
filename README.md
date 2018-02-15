# PodHead.NuGet

This package provides an API to retrieve Podcast data from the iTunes Podcast REST API. 
Podcast data can be retrieved using a search term, or by requesting the top charts for a specific genre.
This API also provides the ability to load the data for the available podcast episodes from each feed.

The iTunes podcast REST API provides the RSS feed associated with each podcast.
The podcast data can then be loaded for each feed by retrieving the data from the RSS feed.

The maximum number of podcast feeds can be specified when searching for podcast either by a search term or by genre.
Similarly, a maximum number of episodes can be specified when loading the data for each podcast feed.

**Retrieve Podcast Feeds by Search Term**
```csharp
IEnumerable<PodcastFeed> podcastFeeds = podHead.Search(podcastTitle, maxNumberOfFeeds: 5);
```


**Retrieving Podcast Feeds by Genre**
```csharp
IEnumerable<PodcastFeed> podcastFeeds = podHead.GetTopCharts(genre, maxPodcastLimit: 5);
```

**Loading Podcast Data**
```csharp
/// <summary>
/// Downloads the podcast episodes, and prints off the data.
/// </summary>
private static void LoadPodcastEpisodes(PodcastFeed podcastFeed)
{
    if (podcastFeed.Load(maxEpisodeLimit: 5))
    {
        Console.WriteLine(podcastFeed.Title);
        Console.WriteLine(podcastFeed.Description);
        Console.WriteLine(podcastFeed.RssLink);

        foreach (PodcastEpisode podcastEpisode in podcastFeed.PodcastEpisodes)
        {
            Console.WriteLine(podcastEpisode.Title);
            Console.WriteLine(podcastEpisode.Description);
            Console.WriteLine(podcastEpisode.PubDate);
            Console.WriteLine(podcastEpisode.Link);
        }
    }
}
```

**Downloading Podcast Episodes**
```csharp
/// <summary>
/// Downloads the given episode the the Music folder.
/// </summary>
private static void DownloadEpisode(PodcastEpisode podcastEpisode)
{
    string musicFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
    string filePath = Path.Combine(musicFolder, $"LatestEpisode.mp3");
    podcastEpisode.Download(filePath);
}
```

For more examples please refere to [PodHead.Examples](PodHead.Examples/Program.cs)