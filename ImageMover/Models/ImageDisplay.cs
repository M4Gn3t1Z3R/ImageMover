/*
This file was originally Created by M4Gn3t1Z3R. Original Repository at https://github.com/M4Gn3t1Z3R/ImageMover/tree/master See the license file for further information on how you may use this file and the entire work
*/
using System.Windows.Media.Imaging;

using ImageMover.MVVM;

namespace ImageMover.Models
{
    /// <summary>
    /// this is a display class to simply display our images in the list we have in wpf
    /// </summary>
    class ImageDisplay : ViewModelBase
    {
        private bool _selectedForTransfer;
        public bool SelectedForTransfer 
        {
            get { return _selectedForTransfer; }
            set { SetProperty(ref _selectedForTransfer, value); }
        }

        private BitmapImage _displayImage;
        public BitmapImage DisplayImage 
        { 
            get { return _displayImage; }
            set { SetProperty(ref _displayImage, value); }
        }

        private string _name;
        public string Name 
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _fullName;
        public string FullName
        {
            get { return _fullName; }
            set { SetProperty(ref _fullName, value); }
        }

        public ImageDisplay()
        {

        }

        public ImageDisplay(BitmapImage image, string name)
        {
            Name = name.Substring(name.LastIndexOf('\\')+1);
            FullName = name;
            DisplayImage = image;
            SelectedForTransfer = false;
        }
    }
}
