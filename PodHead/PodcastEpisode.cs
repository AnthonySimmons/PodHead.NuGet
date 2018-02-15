
using System;
using System.IO;
using System.Net;

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

        public void Download(string filename)
        {
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(Link, filename);
            }
        }

        public bool TryDownload(string filename, out string errorMessage)
        {
            errorMessage = null;
            try
            {
                Download(filename);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }
    }
}