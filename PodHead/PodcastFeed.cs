using PodHead.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        private bool isLoaded;
        public bool IsLoaded
        {
            get { return isLoaded; }
            internal set
            {
                isLoaded = value;
                if(isLoaded)
                {
                    CheckImageDownload();
                }
            }
        }

        public bool ItemsLoaded { get; internal set; }

        public uint MaxItems { get; internal set; }

        public bool ImageLoaded { get; internal set; }

        public string ImageFilePath
        {
            get
            {
                return Path.Combine(_config.AppDataImageFolder, string.Format("{0}.{1}", Title, GetImageFileType()));
            }
        }

        private string GetImageFileType()
        {
            var fileType = string.Empty;
            if(!string.IsNullOrEmpty(ImageUrl))
            {
                var vals = ImageUrl.Split('.');
                if(vals.Length > 0)
                {
                    fileType = vals[vals.Length - 1];
                }
            }
            return fileType;
        }

        private void CheckImageDownload()
        {
            if(File.Exists(ImageFilePath))
            {
                ImageLoaded = true;                
                ImageUrl = ImageFilePath;
            }
        }

        private readonly IConfig _config;

        public const int DefaultMaxItems = 10;

        public PodcastFeed(IConfig config)
        {
            _config = config;

            Feed = string.Empty;
            Version = string.Empty;
            Title = string.Empty;
            Description = string.Empty;
            RssLink = string.Empty;
            LastBuildDate = string.Empty;
            PubDate = string.Empty;
            Ttl = string.Empty;

            PodcastEpisodes = new ConcurrentList<PodcastEpisode>();
            SiteLink = string.Empty;
            ImageUrl = string.Empty;
            Category = string.Empty;
            MaxItems = DefaultMaxItems;

            CheckImageDownload();
        }

        public IEnumerable<PodcastEpisode> GetDownloads()
        {
            return PodcastEpisodes.Where(it => it.IsDownloaded);
        }
         
        public IEnumerable<PodcastEpisode> GetPlayed()
        {
            return PodcastEpisodes.Where(it => it.PercentPlayed > double.Epsilon);
        }       
    }
}
