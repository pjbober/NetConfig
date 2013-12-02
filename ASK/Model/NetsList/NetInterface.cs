using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASK.Model.NetsList
{
    public class NetInterface
    {
        public string InterfaceName { get; set; }
        public List<Profile> Profiles { get; set; }

        public NetInterface(string name)
        {
            Profiles = new List<Profile>();
            this.InterfaceName = name;
        }

        public void AddProfile(Profile profile)
        {
            Profiles.Add(profile);
        }

        public void ProfileChange(Profile profile)
        {
            Console.Out.WriteLine("Changing profile to: " + profile.ProfileName);

            switch (profile.ProfileState)
            {
                case Profile.ProfileStateEnum.ON:
                    // TODO: coś poszło nie tak - profil zazwyczaj domaga się włączenia
                    throw new Exception();
                //break;
                case Profile.ProfileStateEnum.OFF:
                case Profile.ProfileStateEnum.ACTIVATING:
                // TODO: powdójny request - niecierpliwy użytkownik?
                case Profile.ProfileStateEnum.DEACTIVATING:
                default:
                    break;
            }

            // TODO: sytuacja, kiedy jakiś profil się aktywuje bądź deaktywuje
            Profile currentActiveProfile = null;
            foreach (Profile p in Profiles)
            {
                // sprawdź, który profil jest włączony - powinien być tylko jeden
                // TODO: wykrywanie sytuacji, gdy kilka profili jest aktywnych? niezgodne stany?
                if (p.ProfileState == Profile.ProfileStateEnum.ON)
                {
                    currentActiveProfile = p;
                    break;
                }
            }

            if (currentActiveProfile != null)
            {
                currentActiveProfile.Deactivate(); // blokująca deaktywacja profilu
                if (currentActiveProfile.ProfileState != Profile.ProfileStateEnum.OFF)
                {
                    // TODO: nie udało się deaktywować, ponowić próbę?
                }
            }

            // teraz żaden inny profil nie powinien być aktywny
            profile.Activate(); // blokująca aktywacja profilu z żądania
            if (profile.ProfileState != Profile.ProfileStateEnum.ON)
            {
                // TODO: coś poszło nie tak
            }

            // sukces
        }
    }
}
