

namespace PodHead
{
    public interface IConfig
    {
        /// <summary>
        /// Folder in which to download podcasts.
        /// </summary>
        string DownloadFolder { get; }

        /// <summary>
        /// Folder in which to store application data.
        /// </summary>
        string AppDataFolder { get; }

        /// <summary>
        /// Folder in which to store image data.
        /// </summary>
        string AppDataImageFolder { get; }

        /// <summary>
        /// Path to the configuration file.
        /// </summary>
        string ConfigFileName { get; }
    }
}
