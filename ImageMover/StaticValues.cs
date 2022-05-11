/*
This file was originally Created by M4Gn3t1Z3R. Original Repository at https://github.com/M4Gn3t1Z3R/ImageMover/tree/master See the license file for further information on how you may use this file and the entire work
*/
using System;
using System.IO;

namespace ImageMover
{
    /// <summary>
    /// This class contains static values that can be reused throughout the application
    /// </summary>
    public static class StaticValues
    {
        public static readonly string ApplicationDataLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "M4Works", "ImageMover");
        public static readonly string ApplicationDataFile = Path.Combine(ApplicationDataLocation, "AppData.config");
        public static string LastSelectedSourcePath;
        public static string LastSelectedTargetPath;
        public static string[] LastTransferredFiles;
    }
}
