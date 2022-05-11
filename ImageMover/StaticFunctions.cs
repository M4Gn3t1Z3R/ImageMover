/*
This file was originally Created by M4Gn3t1Z3R. Original Repository at https://github.com/M4Gn3t1Z3R/ImageMover/tree/master See the license file for further information on how you may use this file and the entire work
*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

using ImageMover.Enums;

namespace ImageMover
{
    /// <summary>
    /// this class provides independent methods to alter various data and to write a savefile
    /// </summary>
    public static class StaticFunctions
    {
        /// <summary>
        /// this method converts a bitmap to a wpf-usable image source
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            BitmapImage returnerImage = new BitmapImage();

            MemoryStream stream = new MemoryStream(); //we must not dispose of the MemoryStream, because the image would be lost.
            ((Bitmap)bitmap).Save(stream, ImageFormat.Bmp);
            returnerImage.BeginInit();
            stream.Seek(0, SeekOrigin.Begin);
            returnerImage.StreamSource = stream;
            returnerImage.EndInit();
            returnerImage.Freeze();
            return returnerImage;
        }

        /// <summary>
        /// this method converts a wpf-usable image source to a bitmap
        /// </summary>
        /// <param name="bitmapImage"></param>
        /// <returns></returns>
        public static Bitmap BitmapImageToBitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(ms);
                Bitmap returner = new Bitmap(ms);
                return new Bitmap(returner);
            }
        }

        /// <summary>
        /// This method overwrites a part of the settings file
        /// </summary>
        /// <param name="option">The part that should be overwritten</param>
        /// <param name="value">The value that should be written</param>
        public static void SaveApplicationData(SaveFileOption option, string value)
        {
            string[] previousFileContent = { "", "", "", "", "", "", ""};
            if (File.Exists(StaticValues.ApplicationDataFile))
            {
                previousFileContent = File.ReadAllLines(StaticValues.ApplicationDataFile);
            }
            switch (option)
            {
                case SaveFileOption.LastSelectedExtractionDirectory:
                    //the file we take the images from is changed
                    previousFileContent[0] = value;
                    break;
                case SaveFileOption.LastSelectedInsertionDirectory:
                    //the file we save the images to is changed
                    previousFileContent[1] = value;
                    break;
                case SaveFileOption.LastTransferredFileName:
                    //the last transferred file is changed
                    //we store the last 5 transferred files in case the user has multiple directories to extract files from
                    if (previousFileContent.Contains(value))
                    {
                        break;
                    }
                    for(int i = 0; i<5; i++)
                    {
                        previousFileContent[previousFileContent.Length - 1 - i] = previousFileContent[previousFileContent.Length - 2 - i];
                    }
                    previousFileContent[2] = value;
                    UpdateLastTransferredFilesFromSaveFile();
                    break;
                default:
                    //unknown Save Request
                    break;
            }
            if (!Directory.Exists(StaticValues.ApplicationDataLocation))
            {
                Directory.CreateDirectory(StaticValues.ApplicationDataLocation);
            }
            File.WriteAllLines(StaticValues.ApplicationDataFile, previousFileContent);
        }

        /// <summary>
        /// this method initializes some of the static values that can be used
        /// </summary>
        public static void InitializeStaticValues()
        {
            if (!File.Exists(StaticValues.ApplicationDataFile))
            {
                if (!Directory.Exists(StaticValues.ApplicationDataLocation))
                {
                    Directory.CreateDirectory(StaticValues.ApplicationDataLocation);
                }
                string[] emptyContent = { Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "", "", "", "", "" };
                File.WriteAllLines(StaticValues.ApplicationDataFile, emptyContent);
            }
            string[] appData = File.ReadAllLines(StaticValues.ApplicationDataFile);
            StaticValues.LastSelectedSourcePath = appData[0];
            StaticValues.LastSelectedTargetPath = appData[1];
            UpdateLastTransferredFilesFromSaveFile();
        }

        public static void UpdateLastTransferredFilesFromSaveFile()
        {
            string[] appData = File.ReadAllLines(StaticValues.ApplicationDataFile);
            List<string> lastFiles = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                lastFiles.Add(appData[2 + i]);
            }
            StaticValues.LastTransferredFiles = lastFiles.ToArray();
        }

        /// <summary>
        /// this method is used to extract a sub collection from a collection based on a start and end index, like the string.substring() method does
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="originalCollection"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetSubCollection<T>(IEnumerable<T> originalCollection, int startIndex, int endIndex = -1)
        {
            if (endIndex < 0)
            {
                endIndex = originalCollection.Count();
            }
            List<T> returner = new List<T>();
            var origin = originalCollection.ToList();
            for(int i = startIndex; i<endIndex; i++)
            {
                returner.Add(origin[i]);
            }
            return returner;
        }

        /// <summary>
        /// this is a helper method that checks wether a string ends with any of the strings in the endings array
        /// </summary>
        /// <param name="text"></param>
        /// <param name="endings"></param>
        /// <returns></returns>
        public static bool StringEndsWith(string text, string[] endings)
        {
            foreach(string ending in endings)
            {
                if (text.EndsWith(ending))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
