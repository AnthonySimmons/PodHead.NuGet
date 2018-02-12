
using System.Collections.Generic;

namespace PodHead
{
    public class PodcastFeed
    {
        public string Feed { get; internal set; }

        public string Version { get; internal set; }

        public string Title { get; internal set; }

        public string Description { get; internal set; }

        public string RssLink { get; internal set; }

        public string LastBuildDate { get; internal set; }

        public string PubDate { get; internal set; }

        public string Ttl { get; internal set; }

        public int Update { get; internal set; }
        
        public IList<PodcastEpisode> PodcastEpisodes { get; internal set; }

        public string SiteLink { get; internal set; }

        public string ImageUrl { get; internal set; }

        public string Category { get; internal set; }

        public bool HasErrors { get; internal set; }
        
        public bool IsLoaded { get; internal set; }

        public bool ItemsLoaded { get; internal set; }

        public uint MaxItems { get; internal set; }

        public const int DefaultMaxItems = 10;

        public PodcastFeed()
            : this(string.Empty, string.Empty, string.Empty, string.Empty,
                   string.Empty, string.Empty, string.Empty, string.Empty, 
                   string.Empty, string.Empty, string.Empty)
        {
        }

        public PodcastFeed(string feed,     string version,       string title,   string description, 
                           string rssLink,  string lastBuildDate, string pubDate, string ttl, 
                           string siteLink, string imageUrl,      string category)
        {
            Feed = feed;
            Version = version;
            Title = title;
            Description = description;
            RssLink = rssLink;
            LastBuildDate = lastBuildDate;
            PubDate = pubDate;
            Ttl = ttl;
            SiteLink = siteLink;
            ImageUrl = imageUrl;
            Category = category;
            MaxItems = DefaultMaxItems;

            PodcastEpisodes = new ConcurrentList<PodcastEpisode>();
        } 

        public bool Load(uint maxEpisodeLimit = 10)
        {
            RssParser parser = new RssParser();
            return parser.LoadPodcastFeed(this, maxEpisodeLimit);
        }
    }
}
