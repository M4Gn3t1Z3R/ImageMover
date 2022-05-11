/*
This file was originally Created by M4Gn3t1Z3R. Original Repository at https://github.com/M4Gn3t1Z3R/ImageMover/tree/master See the license file for further information on how you may use this file and the entire work
*/
using System.Windows;

using ImageMover.ViewModels;

namespace ImageMover
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.DataContext = new MainViewModel();
            InitializeComponent();
        }
    }
}
