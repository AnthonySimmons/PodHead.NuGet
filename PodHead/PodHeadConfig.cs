

using PodHead.Interfaces;
using System;

namespace PodHead
{
    internal class PodHeadConfig : IConfig
    {
        public PodHeadConfig(string downloadFolder, string appDataFolder, string appDataImageFolder, string configFileName)
        {
            DownloadFolder     = !string.IsNullOrEmpty(downloadFolder)     ? downloadFolder     : throw new ArgumentException("Must provide a value.", nameof(downloadFolder));
            AppDataFolder      = !string.IsNullOrEmpty(appDataFolder)      ? appDataFolder      : throw new ArgumentException("Must provide a value.", nameof(appDataFolder));
            AppDataImageFolder = !string.IsNullOrEmpty(appDataImageFolder) ? appDataImageFolder : throw new ArgumentException("Must provide a value.", nameof(appDataImageFolder));
            ConfigFileName     = !string.IsNullOrEmpty(configFileName)     ? configFileName     : throw new ArgumentException("Must provide a value.", nameof(configFileName));
        }

        public string DownloadFolder { get; }

        public string AppDataFolder { get; }

        public string AppDataImageFolder { get; }

        public string ConfigFileName { get; }
    }
}
