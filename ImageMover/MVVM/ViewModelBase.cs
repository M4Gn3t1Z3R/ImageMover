/*
This file was originally Created by M4Gn3t1Z3R. Original Repository at https://github.com/M4Gn3t1Z3R/ImageMover/tree/master See the license file for further information on how you may use this file and the entire work
*/
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ImageMover.MVVM
{
    class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected virtual bool SetProperty<T>(ref T oldValue, T newValue, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
            {
                return false;
            }
            else
            {
                oldValue = newValue;
                this.OnPropertyChanged(propertyName);
                return true;
            }
        }
    }
}
