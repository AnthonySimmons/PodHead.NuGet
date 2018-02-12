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
    using (PodHead podHead = new PodHead())
    {
        IEnumerable<PodcastFeed> podcastFeeds = podHead.Search("NPR News Now", maxNumberOfFeeds: 5);
    }
```


**Retrieving Podcast Feeds by Genre**
```csharp
    using (PodHead podHead = new PodHead())
    {
        IEnumerable<PodcastFeed> podcastFeeds = podHead.GetTopCharts(PodcastGenre.Comedy, maxPodcastLimit: 5);
    }
```

**Loading Podcast Episodes**
```csharp
    using (PodHead podHead = new PodHead())
    {
        IEnumerable<PodcastFeed> podcastFeeds = podHead.GetTopCharts(PodcastGenre.Comedy, maxPodcastLimit: 5);
        foreach (PodcastFeed podcastFeed in podcastFeeds)
        {
            podcastFeed.Load(maxEpisodeLimit: 5);
        }
    }
```

**Full Example**
```csharp
    class Program
    {
        static void Main(string[] args)
        {
            SearchForPodcast("NPR News Now");
            GetPodcastCharts(PodcastGenre.Comedy);
        }

        /// <summary>
        /// Retrieves and Loads Podcast Feeds by Search Term.
        /// In this example the search term happens to be the title of the podcast.
        /// </summary>
        private static void SearchForPodcast(string podcastTitle)
        {
            using (PodHead podHead = new PodHead())
            {
                //Subscribe to any errors that may occur.
                podHead.ErrorOccurred += PodHead_ErrorOccurred;

                //Get a collection of podcast feeds returned by the search.
                IEnumerable<PodcastFeed> podcastFeeds = podHead.Search(podcastTitle, maxNumberOfFeeds: 5);

                //Get the podcast feed that matches the title, and print its data.
                PodcastFeed nprNewsPodcastFeed = podcastFeeds.FirstOrDefault(podcastFeed => podcastFeed.Title == podcastTitle);
                if (nprNewsPodcastFeed != null)
                {
                    if (nprNewsPodcastFeed.Load(maxEpisodeLimit: 5))
                    {
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

                podHead.ErrorOccurred -= PodHead_ErrorOccurred;
            }
        }


        /// <summary>
        /// Retrieves and Loads Podcast Feeds by Genre.
        /// </summary>
        private static void GetPodcastCharts(PodcastGenre genre)
        {
            using (PodHead podHead = new PodHead())
            {
                podHead.ErrorOccurred += PodHead_ErrorOccurred;

                //Get the top podcasts for this genre
                IEnumerable<PodcastFeed> podcastFeeds = podHead.GetTopCharts(genre, maxPodcastLimit: 5);

                //Loop over each podcast feed, load the episodes, and print the data.
                foreach (PodcastFeed podcastFeed in podcastFeeds)
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

                podHead.ErrorOccurred -= PodHead_ErrorOccurred;
            }
        }
        
        private static void PodHead_ErrorOccurred(string errorMessage)
        {
            Console.Error.WriteLine(errorMessage);
        }
    }
```
