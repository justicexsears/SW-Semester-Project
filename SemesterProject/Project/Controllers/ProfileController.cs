using System.Collections.ObjectModel;
using System.Security.AccessControl;
using SemesterProject.Models;

namespace SemesterProject.Controllers
{
    public class ProfileConverter : BindableObject
    {
        public Models.ProfileModel ProfileModel { get; set; }
        private bool _editing = false;

        public ProfileConverter(Models.ProfileModel data)
        {
            ProfileModel = data;
        }

        public int ProfileID { get {return ProfileModel._id;} set{ ProfileModel._id = value;}}

        public string ProfileName
        {
            get 
            {
                return $"{ProfileModel.profile_name}";
            }
        }

        private void NotifyView()
        {
            OnPropertyChanged(nameof(ProfileID));
            OnPropertyChanged(nameof(ProfileName));
        }
    }

    public class ProfileController
    {
        public ObservableCollection<ProfileConverter> Profiles { get; set;}

        public bool RemoveProfile(int _ind)
        {
            /*
            for(int i = 0; i < Profiles.Count; i++)
            {
                if (Profiles[i].ProfileName == _name)
                {
                    Profiles.RemoveAt(i);
                    return true;
                }
            }
            */

            if (Profiles.Count > _ind)
            {
                Profiles.RemoveAt(_ind);
                ReindexProfiles();
                return true;
            }

            return false;
        }

        public void AddNewProfile(string name)
        {
            var profile = new Models.ProfileModel
            {
                profile_name = name,
                _id = Profiles.Count
            };

            Profiles.Add(new ProfileConverter(profile));
        }

        public void DisplayProfile(string name, int theme, int accent)
        {
            var profile = new Models.ProfileModel
            {
                profile_name = name,
                _id = Profiles.Count
            };

            Profiles.Add(new ProfileConverter(profile));
        }

        public void ReindexProfiles()
        {
            //begin from 1, and step index nums back by 1, this accounts for ever present guest profile
            for(int i = 0; i < Profiles.Count; i++)
            {
                Profiles[i].ProfileID = (i);
            }
        }

        public ProfileController(CollectionView view)
        {
            Profiles = new ObservableCollection<ProfileConverter>();
            
            /*
            var profile = new Models.ProfileModel
            {
                profile_name = "Debug",
                _id = 0
            };
            Profiles.Add(new ProfileConverter(profile));
            */

            view.ItemsSource = Profiles;
        }
    }

}