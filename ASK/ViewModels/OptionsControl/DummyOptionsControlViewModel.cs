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
        private Profile profile;

        public Profile Profile
        {
            get { return profile; }
            set
            {
                profile = value;
                NotifyPropertyChanged("Profile");
            }
        }

        public void OnProfileChanged(Profile newProfile)
        {
            Profile = newProfile;
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
