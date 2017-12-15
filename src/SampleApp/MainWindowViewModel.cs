using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace SampleApp
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _columnDefinitions;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string ColumnDefinitions
        {
            get { return _columnDefinitions; }
            set
            {
                if (value == _columnDefinitions) return;
                _columnDefinitions = value;
                RaisePropertyChanged();
            }
        }
    }
}
