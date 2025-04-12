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

        public int ProfileID { get {return ProfileModel.Id;}}

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

       public bool RemoveProfile(string _name)
        {
            for(int i = 0; i < Profiles.Count; i++)
            {
                if (Profiles[i].ProfileName == _name)
                {
                    Profiles.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public void AddNewProfile(string name)
        {
            var profile = new Models.ProfileModel
            {
                profile_name = name
            };

            Profiles.Add(new ProfileConverter(profile));
        }

        public ProfileController(CollectionView view)
        {
            Profiles = new ObservableCollection<ProfileConverter>();

            var profile = new Models.ProfileModel
            {
                profile_name = "Guest Profile"
            };
            Profiles.Add(new ProfileConverter(profile));

            view.ItemsSource = Profiles;
        }
    }

}