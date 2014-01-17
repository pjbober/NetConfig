using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using ASK.Model.NetsList;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ASK.ViewModels.NetsList
{
    //public delegate void ProfileChangedEvent(ProfileModel profile);
    public class NetInterfaceViewModel : ViewModelBase
    {
        // TODO: model powinien zawierać stan: włączony/wyłączony

        public Style Style
        {
            get
            {
                return GetStyle("InterfaceButtonOn");
            }
        }

        public SolidColorBrush ActiveRectColor
        {
            get
            {
                return GetBrush("LighterColor");
            }
        }

        Boolean _isExpanded = true;

        public Boolean IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                _isExpanded = value;
                EmitPropertyChanged("IsExpanded");
            }
        }

        public NetInterfaceViewModel(NetInterfaceModel netInterface)
        {
            NetInterfaceModel = netInterface;
            NetInterfaceModel.ProfileAddedEvent += HandleProfileAddedEvent;

            Profiles = new ObservableCollection<ProfileButtonViewModel>();
            foreach (ProfileModel profile in netInterface.Profiles)
            {
                Profiles.Add(new ProfileButtonViewModel(profile));
            }

            IsExpanded = true;
        }

        void HandleProfileAddedEvent(ProfileModel newProfile)
        {
            Profiles.Add(new ProfileButtonViewModel(newProfile));
        }

        public ObservableCollection<ProfileButtonViewModel> Profiles { get; set; }
        public NetInterfaceModel NetInterfaceModel { get; set; }

        //public event ProfileChangedEvent ProfileChangedEvent;

        //public void Clicked(object obj)
        //{
        //    ProfileModel profile = obj as ProfileModel;
        //    if (ProfileChangedEvent != null)
        //    {
        //        ProfileChangedEvent(profile);
        //    }
        //    profile.ToggleState();
        //}


        // własności użyte w widoku

        public String Name
        {
            get { return NetInterfaceModel.InterfaceName; }
        }

        public static readonly ImageSource WIRELESS_ICON = LoadPng("network-wireless");
        public static readonly ImageSource WIRED_ICON = LoadPng("network-wired");
        public static readonly ImageSource LOOPBACK_ICON = LoadPng("network-loopback");
        public static readonly ImageSource OTHER_ICON = LoadPng("network-other");

        public static readonly ImageSource SHUTDOWN_ICON = LoadPng("interface-shutdown");
        public static readonly ImageSource START_ICON = LoadPng("interface-start");

        public static readonly ImageSource ADD_ICON = LoadPng("add");

        public ImageSource TypeIcon
        {
            get
            {
                switch (NetInterfaceModel.Type)
                {
                    case NetInterfaceType.Loopback:
                        return LOOPBACK_ICON;
                    case NetInterfaceType.Wired:
                        return WIRED_ICON;
                    case NetInterfaceType.Wireless:
                        return WIRELESS_ICON;
                    case NetInterfaceType.Other:
                    default:
                        return OTHER_ICON;
                }
            }
        }

        public ImageSource TurnOnIcon
        {
            get
            {
                switch (NetInterfaceModel.State)
                {
                    case NetInterfaceState.Connected:
                        return SHUTDOWN_ICON;
                    default:
                    case NetInterfaceState.Disconnected:
                        return START_ICON;
                }
            }
        }


        internal void AddNewProfile()
        {
            this.NetInterfaceModel.AddNewProfile();
        }
    }
}


