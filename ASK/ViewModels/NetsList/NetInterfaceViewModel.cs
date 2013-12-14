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

namespace ASK.ViewModels.NetsList
{
    public delegate void ProfileChangedHandlerEvent(ProfileModel newProfile);

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
            
            BtnClicked = new CommandHandler(x => Clicked(x), true);

            Profiles = new ObservableCollection<ProfileButtonViewModel>();
            foreach (ProfileModel profile in netInterface.Profiles)
            {
                Profiles.Add(new ProfileButtonViewModel(profile));
            }

            IsExpanded = true;
        }

        public ObservableCollection<ProfileButtonViewModel> Profiles { get; set; }
        public NetInterfaceModel NetInterfaceModel { get; set; }

        public event ProfileChangedHandlerEvent ProfileChangedEvent;

        public void Clicked(object obj)
        {
            ProfileModel profile = obj as ProfileModel;
            if (ProfileChangedEvent != null)
            {
                ProfileChangedEvent(profile);
            }
            profile.ToggleState();
        }

        public ICommand BtnClicked
        {
            get;
            set;
        }

        public ICommand InterfaceBtnClicked
        {
            get;
            set;
        }

        // własności użyte w widoku

        public String Name
        {
            get { return NetInterfaceModel.InterfaceName; }
        }


    }
}


