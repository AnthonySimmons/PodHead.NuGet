
namespace PodHead
{

    public class PodcastEpisode
    {
        public string Title { get; internal set; }

        public string Description { get; internal set; }

        public string Link { get; internal set; }

        public string Guid { get; internal set; }

        public string PubDate { get; internal set; }
             
        public PodcastFeed ParentFeed { get; internal set; }

        public int DurationMs { get; internal set; }

        public bool IsLoaded { get; internal set; }

        internal int RowNum;
        
        public PodcastEpisode() :
            this(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty)
        {
        }

        public PodcastEpisode(string title, string description, string link, string guid, string pubDate)
        {
            Title = title;
            Description = description;
            Link = link;
            Guid = guid;
            PubDate = pubDate;
        }
    }
}