using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PodHead.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            SearchForPodcast("NPR News Now");
            GetPodcastCharts(PodcastGenre.Comedy);
            
            TrySearchForPodcast("NPR News Now");
            TryGetPodcastCharts(PodcastGenre.Comedy);
        }

        /// <summary>
        /// Retrieves and Loads Podcast Feeds by Search Term.
        /// In this example the search term happens to be the title of the podcast.
        /// </summary>
        private static void SearchForPodcast(string podcastTitle)
        {
            PodHead podHead = new PodHead();

            //Get a collection of podcast feeds returned by the search. (May throw exceptions).
            IEnumerable<PodcastFeed> podcastFeeds = podHead.Search(podcastTitle, maxNumberOfFeeds: 5);

            //Get the podcast feed that matches the title, and print its data.
            PodcastFeed nprNewsPodcastFeed = podcastFeeds.FirstOrDefault(podcastFeed => podcastFeed.Title == podcastTitle);
            if (nprNewsPodcastFeed != null)
            {
                LoadPodcastEpisodes(nprNewsPodcastFeed);
            }
        }
        
        /// <summary>
        /// Retrieves and Loads Podcast Feeds by Search Term.
        /// In this example the search term happens to be the title of the podcast.
        /// </summary>
        private static void TrySearchForPodcast(string podcastTitle)
        {
            PodHead podHead = new PodHead();

            //Get a collection of podcast feeds returned by the search.
            if(podHead.TrySearch(podcastTitle, out IEnumerable<PodcastFeed> podcastFeeds, out string errorMessage, maxNumberOfFeeds: 5))
            {
                //Get the podcast feed that matches the title, and print its data.
                PodcastFeed podcastFeed = podcastFeeds.FirstOrDefault(feed => feed.Title == podcastTitle);
                if (podcastFeed != null)
                {
                    LoadPodcastEpisodes(podcastFeed);
                    //Download latest episode
                    DownloadEpisode(podcastFeed.PodcastEpisodes.First());
                }
            }
            else
            {
                Console.Error.WriteLine(errorMessage);
            }
        }
        
        /// <summary>
        /// Downloads the given episode the the Music folder.
        /// </summary>
        private static void DownloadEpisode(PodcastEpisode podcastEpisode)
        {
            string musicFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            string filePath = Path.Combine(musicFolder, $"LatestEpisode.mp3");
            podcastEpisode.Download(filePath);
        }

        /// <summary>
        /// Retrieves and Loads Podcast Feeds by Genre.
        /// </summary>
        private static void GetPodcastCharts(PodcastGenre genre)
        {
            PodHead podHead = new PodHead();
            //Get the top podcasts for this genre
            IEnumerable<PodcastFeed> podcastFeeds = podHead.GetTopCharts(genre, maxPodcastLimit: 5);

            foreach (PodcastFeed podcastFeed in podcastFeeds)
            {
                LoadPodcastEpisodes(podcastFeed);
            }
        }

        /// <summary>
        /// Retrieves and Loads Podcast Feeds by Genre.
        /// </summary>
        private static void TryGetPodcastCharts(PodcastGenre genre)
        {
            PodHead podHead = new PodHead();
            //Get the top podcasts for this genre
            if(podHead.TryGetTopCharts(genre, out IEnumerable<PodcastFeed> podcastFeeds, out string errorMessage, maxPodcastLimit: 5))
            {
                foreach (PodcastFeed podcastFeed in podcastFeeds)
                {
                    LoadPodcastEpisodes(podcastFeed);
                }
            }
            else
            {
                Console.Error.WriteLine(errorMessage);
            }
        }

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
    }
}
