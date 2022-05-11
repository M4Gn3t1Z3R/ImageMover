using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using ImageMover;
using ImageMover.Enums;
using ImageMover.Models;
using ImageMover.MVVM;

namespace ImageMover.ViewModels
{
    class MainViewModel: ViewModelBase
    {
        private ObservableCollection<ImageDisplay> _images;
        public ObservableCollection<ImageDisplay> Images
        {
            get { return _images; }
            set { SetProperty(ref _images, value); }
        }

        private string _targetLocation;
        public string TargetLocation 
        { 
            get { return _targetLocation; }
            set { SetProperty(ref _targetLocation, value); }
        }

        private string _sourceLocation;
        public string SourceLocation 
        {
            get { return _sourceLocation; }
            set { SetProperty(ref _sourceLocation, value); }
        }

        private int _selectedIndex;
        public int SelectedIndex 
        { 
            get { return _selectedIndex; }
            set 
            {
                //maybe add validation so we don't go outside of our index range
                SetProperty(ref _selectedIndex, value); 
            }
        }

        private ImageDisplay _selectedImage;
        public ImageDisplay SelectedImage 
        {
            get { return _selectedImage; }
            set { SetProperty(ref _selectedImage, value); }
        }

        private int _currentProgress;
        public int CurrentProgress 
        {
            get { return _currentProgress; }
            set { SetProperty(ref _currentProgress, value); }
        }

        private int _maximumProgress;
        public int MaximumProgress 
        {
            get { return _maximumProgress; }
            set { SetProperty(ref _maximumProgress, value); }
        }

        public MainViewModel()
        {
            StaticFunctions.InitializeStaticValues();
            InitializeCommands();
            SourceLocation = StaticValues.LastSelectedSourcePath;
            TargetLocation = StaticValues.LastSelectedTargetPath;
        }

        private void InitializeCommands()
        {
            SelectSourceCommand = new RelayCommand<object>(c => ExecuteSelectSourceCommand(), c => !SelectSourceCommand.IsExecuting && !SelectTargetCommand.IsExecuting && !StartTransferCommand.IsExecuting);
            SelectTargetCommand = new RelayCommand<object>(c => ExecuteSelectTargetCommand(), c => !SelectSourceCommand.IsExecuting && !SelectTargetCommand.IsExecuting && !StartTransferCommand.IsExecuting);
            StartTransferCommand = new RelayCommand<object>(c => ExecuteStartTransferCommand(), c => !SelectSourceCommand.IsExecuting && !SelectTargetCommand.IsExecuting && !StartTransferCommand.IsExecuting);
            AcceptSelectedImageCommand = new RelayCommand<object>(c => ExecuteAcceptSelectedImageCommand(), c => !AcceptSelectedImageCommand.IsExecuting);
            RejectSelectedImageCommand = new RelayCommand<object>(c => ExecuteRejectSelectedImageCommand(), c => !RejectSelectedImageCommand.IsExecuting);
            GoBackOneImageCommand = new RelayCommand<object>(c => ExecuteGoBackOneImageCommand(), c => !GoBackOneImageCommand.IsExecuting && SelectedIndex > 0);
        }

        public RelayCommand<object> SelectSourceCommand { get; set; }
        public RelayCommand<object> SelectTargetCommand { get; set; }
        public RelayCommand<object> StartTransferCommand { get; set; }
        public RelayCommand<object> AcceptSelectedImageCommand { get; set; }
        public RelayCommand<object> RejectSelectedImageCommand { get; set; }
        public RelayCommand<object> GoBackOneImageCommand { get; set; }

        private void ExecuteSelectSourceCommand()
        {
            string[] files;
            //sadly we have to use the Forms FolderBrowserDialog, because WPF does not have one
            using (System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (File.Exists(StaticValues.LastSelectedSourcePath))
                    dialog.SelectedPath = StaticValues.LastSelectedSourcePath;
                else
                    dialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                dialog.ShowDialog();
                SourceLocation = dialog.SelectedPath;
                StaticFunctions.SaveApplicationData(SaveFileOption.LastSelectedExtractionDirectory, SourceLocation);
                string[] fileExtensions = { ".jpeg", ".png", ".jpg", ".bmp" };
                files = Directory.GetFiles(dialog.SelectedPath).Where(f => StaticFunctions.StringEndsWith(f.ToLower(), fileExtensions)).ToArray();
            }
            Images = new ObservableCollection<ImageDisplay>();
            foreach(string file in StaticValues.LastTransferredFiles)
            {
                //in case the user has not used this in a long time (or never) it could happen that there are too many images and the application throws an out of memory exception
                if (!string.IsNullOrWhiteSpace(file) && files.Contains(file))
                {
                    files = StaticFunctions.GetSubCollection(files, files.ToList().IndexOf(file)+1).ToArray();
                }
            }
            MaximumProgress = files.Length;
            foreach(string file in files)
            {
                Images.Add(new ImageDisplay(StaticFunctions.BitmapToImageSource(new Bitmap(Image.FromFile(file))), file));
                CurrentProgress++;
            }
            CurrentProgress = 0;
            MaximumProgress = 1;
            SelectedImage = Images[0];
        }

        private void ExecuteSelectTargetCommand()
        {
            //sadly we have to use the Forms FolderBrowserDialog, because WPF does not have one
            using (System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (File.Exists(StaticValues.LastSelectedTargetPath))
                    dialog.SelectedPath = StaticValues.LastSelectedTargetPath;
                else
                    dialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                dialog.ShowDialog();
                TargetLocation = dialog.SelectedPath;
                StaticFunctions.SaveApplicationData(SaveFileOption.LastSelectedInsertionDirectory, TargetLocation);
            }

        }

        private void ExecuteStartTransferCommand()
        {
            MaximumProgress = Images.Count(i => i.SelectedForTransfer);
            foreach(ImageDisplay image in Images.Where(i => i.SelectedForTransfer))
            {
                CurrentProgress++;
                Bitmap bmp = StaticFunctions.BitmapImageToBitmap(image.DisplayImage);
                bmp.Save(Path.Combine(TargetLocation, image.Name), bmp.RawFormat);
            }
            StaticFunctions.SaveApplicationData(SaveFileOption.LastTransferredFileName, Images.Last().FullName);
            MessageBox.Show($"{CurrentProgress} Dateien erfolgreich übertragen");
            CurrentProgress = 0;
            MaximumProgress = 0;
        }

        private void ExecuteAcceptSelectedImageCommand()
        {
            SelectedImage.SelectedForTransfer = true;
            SelectedIndex++;
            if (SelectedIndex < Images.Count)
            {
                SelectedImage = Images[SelectedIndex];
            }
            else
            {
                SelectedIndex--;
                MessageBox.Show("Alle Bilder wurden Berbeitet, bitte wähle nun den Speicherort aus und vervollständige den Vorgang");
            }
        }

        private void ExecuteRejectSelectedImageCommand()
        {
            SelectedImage.SelectedForTransfer = false;
            SelectedIndex++;
            if (SelectedIndex < Images.Count)
            {
                SelectedImage = Images[SelectedIndex];
            }
            else
            {
                SelectedIndex--;
                MessageBox.Show("Alle Bilder wurden Berbeitet, bitte wähle nun den Speicherort aus und vervollständige den Vorgang");
            }
        }

        private void ExecuteGoBackOneImageCommand()
        {
            if (SelectedIndex > 0)
            {
                SelectedIndex--;
                SelectedImage = Images[SelectedIndex];
            }
            else
            {
                MessageBox.Show("Das ist nicht möglich");
            }
        }
    }
}
