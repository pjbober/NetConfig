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
using NetworkManager;

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
                // TODO dorobić ew. working state
                return GetStyle(NetInterfaceModel.IsEnabled ? "InterfaceButtonOn" : "InterfaceButtonOff");
            }
        }

        public SolidColorBrush ActiveRectColor
        {
            get
            {
                return NetInterfaceModel.IsEnabled ? GetBrush("LighterColor") : GetBrush("DarkerColor");
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
                EmitPropertyChanged("CollapseIcon");
            }
        }

        public NetInterfaceViewModel(NetInterfaceModel netInterface)
        {
            NetInterfaceModel = netInterface;

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

        private NetInterfaceModel _netInterfaceModel;
        public NetInterfaceModel NetInterfaceModel { 
            get { return _netInterfaceModel; }
            set
            {
                _netInterfaceModel = value;

                _netInterfaceModel.ProfileAddedEvent += HandleProfileAddedEvent;
                _netInterfaceModel.InterfaceUp += HandleInterfaceUp;
                _netInterfaceModel.InterfaceDown += HandleInterfaceDown;
                _netInterfaceModel.ActiveProfileChanged += HandleProfileActivation;
            }
        }

        private void HandleProfileActivation(ProfileModel profile)
        {
            foreach (var p in Profiles)
            {
                // TODO
            }

        }

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
            get { return NetInterfaceModel.Name; }
        }

        public static readonly ImageSource WIRELESS_ICON = LoadPng("network-wireless");
        public static readonly ImageSource WIRED_ICON = LoadPng("network-wired");
        public static readonly ImageSource LOOPBACK_ICON = LoadPng("network-loopback");
        public static readonly ImageSource OTHER_ICON = LoadPng("network-other");

        public static readonly ImageSource SHUTDOWN_ICON = LoadPng("interface-shutdown");
        public static readonly ImageSource START_ICON = LoadPng("interface-start");

        public static readonly ImageSource ADD_ICON = LoadPng("add");

        public static readonly ImageSource COLLAPSE_DOWN_ICON = LoadPng("collapse-down");
        public static readonly ImageSource COLLAPSE_UP_ICON = LoadPng("collapse-up");


        public ImageSource TypeIcon
        {
            get
            {
                switch (NetInterfaceModel.Type)
                {
                    //case NetInterfaceType.Loopback:
                    //    return LOOPBACK_ICON;
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

        public ImageSource CollapseIcon
        {
            get
            {
                return IsExpanded ? COLLAPSE_UP_ICON : COLLAPSE_DOWN_ICON;
            }
        }


        // obsługa zdarzeń

        private void HandleInterfaceUp()
        {
            EmitPropertyChanged("Style");
            EmitPropertyChanged("ActiveRectColor");
        }

        private void HandleInterfaceDown()
        {
            EmitPropertyChanged("Style");
            EmitPropertyChanged("ActiveRectColor");
        }
        internal void AddNewProfile()
        {
            this.NetInterfaceModel.AddNewProfile();
        }

        internal void ToggleInterface()
        {
            if (NetInterfaceModel.IsEnabled) {
                NetInterfaceModel.Disable();
            } else {
                NetInterfaceModel.Enable();
            }
        }
    }
}


