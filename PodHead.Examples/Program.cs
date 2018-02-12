using System;
using System.Collections.Generic;
using System.Linq;

namespace PodHead.Examples
{
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

                podHead.ErrorOccurred -= PodHead_ErrorOccurred;
            }
        }
        
        private static void PodHead_ErrorOccurred(string errorMessage)
        {
            Console.Error.WriteLine(errorMessage);
        }
    }
}
