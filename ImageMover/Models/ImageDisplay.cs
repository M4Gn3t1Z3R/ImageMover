using System.Windows.Media.Imaging;

using ImageMover.MVVM;

namespace ImageMover.Models
{
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
