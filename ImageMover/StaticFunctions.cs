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
    public static class StaticFunctions
    {
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

        public static byte[] BitmapToByteArray(Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, bitmap.RawFormat);
                return ms.ToArray();
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

        public static void InitializeStaticValues()
        {
            if (!File.Exists(StaticValues.ApplicationDataFile))
            {
                if (!Directory.Exists(StaticValues.ApplicationDataLocation))
                {
                    Directory.CreateDirectory(StaticValues.ApplicationDataLocation);
                }
                string[] emptyContent = { "Desktop", "Desktop", "", "", "", "", "" };
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
