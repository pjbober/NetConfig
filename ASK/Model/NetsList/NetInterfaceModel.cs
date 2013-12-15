﻿using ASK.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASK.Model.NetsList
{
    public delegate void ProfileAdded(ProfileModel newProfile);

    public class NetInterfaceModel
    {
        public string InterfaceName { get; set; }
        public List<ProfileModel> Profiles { get; set; }

        public NetInterfaceModel(string name)
        {
            Profiles = new List<ProfileModel>();
            this.InterfaceName = name;
        }

        public event ProfileAdded ProfileAddedEvent;

        public void AddProfile(ProfileModel profile)
        {
            Profiles.Add(profile);
            if (ProfileAddedEvent != null)
            {
                ProfileAddedEvent(profile);
            }
        }

        public void ProfileChange(ProfileModel profile)
        {
            Console.Out.WriteLine("Changing profile to: " + profile.ProfileName);

            switch (profile.ProfileState)
            {
                case ProfileModel.StateEnum.ON:
                    // TODO: coś poszło nie tak - profil zazwyczaj domaga się włączenia
                    throw new Exception();
                //break;
                case ProfileModel.StateEnum.OFF:
                case ProfileModel.StateEnum.ACTIVATING:
                // TODO: powdójny request - niecierpliwy użytkownik?
                case ProfileModel.StateEnum.DEACTIVATING:
                default:
                    break;
            }

            // TODO: sytuacja, kiedy jakiś profil się aktywuje bądź deaktywuje
            ProfileModel currentActiveProfile = null;
            foreach (ProfileModel p in Profiles)
            {
                // sprawdź, który profil jest włączony - powinien być tylko jeden
                // TODO: wykrywanie sytuacji, gdy kilka profili jest aktywnych? niezgodne stany?
                if (p.ProfileState == ProfileModel.StateEnum.ON)
                {
                    currentActiveProfile = p;
                    break;
                }
            }

            if (currentActiveProfile != null)
            {
                currentActiveProfile.Deactivate(); // blokująca deaktywacja profilu
                if (currentActiveProfile.ProfileState != ProfileModel.StateEnum.OFF)
                {
                    // TODO: nie udało się deaktywować, ponowić próbę?
                }
            }

            // teraz żaden inny profil nie powinien być aktywny
            profile.Activate(); // blokująca aktywacja profilu z żądania
            if (profile.ProfileState != ProfileModel.StateEnum.ON)
            {
                // TODO: coś poszło nie tak
            }

            // sukces
        }
    }
}
