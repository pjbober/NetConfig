using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASK.Model.NetsList;
using System.ComponentModel;

namespace ASK.ViewModels.OptionsControl
{
    class DummyOptionsControlViewModel : INotifyPropertyChanged
    {
        private string profileName;

        public string ProfileName
        {
            get { return profileName; }
            set
            {
                profileName = value;
                NotifyPropertyChanged("ProfileName");
            }
        }

        public void OnProfileChanged(Profile newProfile)
        {
            ProfileName = newProfile.Name;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
