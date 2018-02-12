# PodHead.NuGet

This package provides an API to retrieve Podcast data from the iTunes Podcast REST API. 
Podcast data can be retrieved using a search term, or by requesting the top charts for a specific genre.
This API also provides the ability to load the data for the available podcast episodes from each feed.

The iTunes podcast REST API provides the RSS feed associated with each podcast.
The podcast data can then be loaded for each feed by retrieving the data from the RSS feed.

The maximum number of podcast feeds can be specified when searching for podcast either by a search term or by genre.
Similarly, a maximum number of episodes can be specified when loading the data for each podcast feed.

**Retrieving and Loading Podcast Feeds by Search Term**
```csharp
  using (PodHead podHead = new PodHead())
  {
      const string podcastTitle = @"NPR News Now";
      IEnumerable<PodcastFeed> podcastFeeds = podHead.Search(podcastTitle, maxNumberOfFeeds: 5);
      PodcastFeed nprNewsPodcastFeed = podcastFeeds.FirstOrDefault(podcastFeed => podcastFeed.Title == podcastTitle);
      if(nprNewsPodcastFeed != null)
      {
          podHead.LoadPodcastFeed(nprNewsPodcastFeed, maxEpisodeLimit: 5);

          Console.WriteLine(nprNewsPodcastFeed.Title);
          Console.WriteLine(nprNewsPodcastFeed.Description);
          Console.WriteLine(nprNewsPodcastFeed.RssLink);

          foreach (PodcastEpisode podcastEpisode in nprNewsPodcastFeed.PodcastEpisodes)
          {
              Console.WriteLine(podcastEpisode.Title);
              Console.WriteLine(podcastEpisode.Description);
              Console.WriteLine(podcastEpisode.PubDate);
              Console.WriteLine(podcastEpisode.Link);
          }
      }
  }
  ```


**Retrieving and Loading Podcast Feeds by Genre**
```csharp
  using (PodHead podHead = new PodHead())
  {
      IEnumerable<PodcastFeed> podcastFeeds = podHead.GetTopCharts(PodcastGenre.Comedy, maxPodcastLimit: 5);

      foreach (PodcastFeed podcastFeed in podcastFeeds)
      {
          podHead.LoadPodcastFeed(podcastFeed, maxEpisodeLimit: 5);
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

