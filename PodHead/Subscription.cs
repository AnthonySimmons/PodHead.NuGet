using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PodHead
{
    internal class Subscription
    {
        public string Feed { get; set; }

        public string Version { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string RssLink { get; set; }

        public string LastBuildDate { get; set; }

        public string PubDate { get; set; }

        public string Ttl { get; set; }

        public int Update { get; set; }
        
        public IList<Item> Items { get; set; }

        public string SiteLink { get; set; }

        public string ImageUrl { get; set; }

        public string Category { get; set; }

        public bool HasErrors { get; set; }

        private bool isLoaded;
        public bool IsLoaded
        {
            get { return isLoaded; }
            set
            {
                isLoaded = value;
                if(isLoaded)
                {
                    CheckImageDownload();
                }
            }
        }

        public bool ItemsLoaded { get; set; }

        public int MaxItems { get; set; }

        public bool ImageLoaded { get; set; }

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

        public Subscription(IConfig config)
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

            Items = new ConcurrentList<Item>();
            SiteLink = string.Empty;
            ImageUrl = string.Empty;
            Category = string.Empty;
            MaxItems = DefaultMaxItems;

            CheckImageDownload();
        }

        public IEnumerable<Item> GetDownloads()
        {
            return Items.Where(it => it.IsDownloaded);
        }
         
        public IEnumerable<Item> GetPlayed()
        {
            return Items.Where(it => it.PercentPlayed > double.Epsilon);
        }       
    }
}
