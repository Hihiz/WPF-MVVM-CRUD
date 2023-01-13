using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WPF_MVVM_CRUD.ViewModels.Base
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected virtual bool Set<T>(ref T filed, T value, [CallerMemberName] string PropertyName = null)
        {
            if (Equals(filed, value)) return false;

            filed = value;
            OnPropertyChanged(PropertyName);
            return true;
        }
    }
}
