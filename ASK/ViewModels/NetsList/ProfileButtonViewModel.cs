using ASK.Model.NetsList;
using NetworkManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using NetworkManager.Profiles;

namespace ASK.ViewModels.NetsList
{
    public class ProfileButtonViewModel : ViewModelBase
    {
        public ProfileButtonViewModel(AbstractProfileModel profile)
        {
            this.Profile = profile;
            this.Profile.ProfileStateChangedEvent += HandleProfileStateChange;
            this.Profile.ProfileDataChangedEvent += HandleProfileDataChange;
            this.Profile.ProfileEditEndEvent += HandleProfileEditEnd;
        }

        public String Name
        {
            get { return Profile.Name; }
        }

        public AbstractProfileModel Profile { get; private set; }

        // TODO: zamiast tego, można zrobić convertery

        public Style Style
        {
            get
            {
                switch (Profile.ProfileState)
                {
                    default:
                    case AbstractProfileModel.StateEnum.OFF:
                        return GetStyle("ProfileButtonDefault");
                    case AbstractProfileModel.StateEnum.ON:
                        return GetStyle("ProfileButtonActive");
                    case AbstractProfileModel.StateEnum.ACTIVATING:
                        return GetStyle("ProfileButtonActivating");
                    case AbstractProfileModel.StateEnum.DEACTIVATING:
                        return GetStyle("ProfileButtonDeactivating");
                }
            }
        }

        public Style EditButtonStyle
        {
            get
            {
                // TODO: czy teraz jest w edycji?
                if (InEditor())
                {
                    return GetStyle("ProfileEditButtonEdited");
                } else {
                    return Style;
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
                    case AbstractProfileModel.StateEnum.OFF:
                        return new SolidColorBrush(System.Windows.Media.Colors.Transparent);
                    case AbstractProfileModel.StateEnum.ON:
                        return GetBrush("LighterColor");
                }
            }
        }

        public Boolean InEditor()
        {
            var op = MainWindow.OptionsPanelViewModel.Profile;
            var t = this.Profile;
            bool eq = Object.ReferenceEquals(t, op);
            return eq;
        }

        // -- Handlery --

        public void HandleProfileStateChange(AbstractProfileModel.StateEnum newState)
        {
            EmitPropertyChanged("ActiveRectColor");
            EmitPropertyChanged("Style");
            EmitPropertyChanged("EditButtonStyle");
        }

        public void HandleProfileDataChange(AbstractProfileModel p)
        {
            EmitPropertyChanged("Name");
            EmitPropertyChanged("EditButtonStyle");
        }

        public void HandleProfileEditEnd(AbstractProfileModel p)
        {
            EmitPropertyChanged("EditButtonStyle");
        }

        internal void ToggleState()
        {
            Profile.ToggleState();
        }

        internal void EditProfile()
        {
            MainWindow.OptionsPanelViewModel.SetProfile(Profile);
            EmitPropertyChanged("EditButtonStyle");
        }
    }
}
