using System.ComponentModel;
using System.Runtime.CompilerServices;
using PropertyChanged;
using WpfApp1.Annotations;

namespace RKMApp.Basic {
    [AddINotifyPropertyChangedInterface]
    public abstract class BaseVM : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}