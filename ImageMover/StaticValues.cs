using System;
using System.IO;

namespace ImageMover
{
    public static class StaticValues
    {
        public static readonly string ApplicationDataLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "M4Works", "ImageMover");
        public static readonly string ApplicationDataFile = Path.Combine(ApplicationDataLocation, "AppData.config");
        public static string LastSelectedSourcePath;
        public static string LastSelectedTargetPath;
        public static string[] LastTransferredFiles;
    }
}
