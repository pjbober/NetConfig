using ASK.Model.NetsList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ASK.ViewModels.NetsList
{
    public class ProfileButtonViewModel : ViewModelBase
    {
        public ProfileButtonViewModel(ProfileModel profile)
        {
            this.Profile = profile;
            this.Profile.ProfileStateChangedEvent += HandleProfileStateChange;
        }

        public String Name
        {
            get
            {
                return Profile.Name;
            }
        }

        public ProfileModel Profile { get; private set; }

        // TODO: zamiast tego, można zrobić convertery

        public Style Style
        {
            get
            {
                switch (Profile.ProfileState)
                {
                    default:
                    case ProfileModel.StateEnum.OFF:
                        return GetStyle("ProfileButtonDefault");
                    case ProfileModel.StateEnum.ON:
                        return GetStyle("ProfileButtonActive");
                    case ProfileModel.StateEnum.ACTIVATING:
                        return GetStyle("ProfileButtonActivating");
                    case ProfileModel.StateEnum.DEACTIVATING:
                        return GetStyle("ProfileButtonDeactivating");
                }
            }
        }

        public SolidColorBrush ActiveRectColor
        {
            get
            {
                switch (Profile.ProfileState)
                {
                    default:
                    case ProfileModel.StateEnum.OFF:
                        return new SolidColorBrush(System.Windows.Media.Colors.Transparent);
                    case ProfileModel.StateEnum.ON:
                        return GetBrush("LighterColor");
                    case ProfileModel.StateEnum.ACTIVATING:
                        return GetBrush("YellowColor");
                    case ProfileModel.StateEnum.DEACTIVATING:
                        return GetBrush("RedColor");
                }
            }
        }

        // -- Handlery --

        public void HandleProfileStateChange(ProfileModel.StateEnum newState)
        {
            EmitPropertyChanged("ActiveRectColor");
            EmitPropertyChanged("Style");
        }

        internal void ToggleState()
        {
            Profile.ToggleState();
        }

        internal void EditProfile()
        {
            MainWindow.OptionsPanelViewModel.SetProfile(Profile);
        }
    }
}
