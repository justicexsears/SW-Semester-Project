using System.Collections.ObjectModel;
using System.Security.AccessControl;
using SemesterProject.Models;
using System.Diagnostics;

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

        public int ProfileID { get {return ProfileModel._id;} set{ ProfileModel._id = value;} }

        public string ProfileName
        {
            get 
            {
                return $"{ProfileModel.profile_name}";
            }
        }

        public bool IsHighlighted { 
            get {return ProfileModel._isHighlighted;} 
            set {
                ProfileModel._isHighlighted = value;
                OnPropertyChanged(nameof(IsHighlighted));
            }
        }

        public int ProfileTheme { get; set; }

        public int ProfileAccent { 
            get {return ProfileModel.profile_accent;} 
            set {
                AccentResource = calcColor(ProfileTheme, value);
                ProfileModel.profile_accent = value;

                Debug.WriteLine($"profile accent set to #{value}: {AccentResource}");
            } 
        }

        private Color _accentResource = Colors.White;

        public Color AccentResource {
            get { return _accentResource; }
            set {
                _accentResource = value;
                OnPropertyChanged(nameof(AccentResource));
            }
        }

        private void NotifyView()
        {
            OnPropertyChanged(nameof(ProfileID));
            OnPropertyChanged(nameof(ProfileName));
            OnPropertyChanged(nameof(IsHighlighted));
            OnPropertyChanged(nameof(AccentResource));
        }

        private Color calcColor(int t, int a)
        {
            Color pfpColor = Colors.Transparent;

            switch (a)
            {
                case 0:
                default:
                    if (t == 0) pfpColor = retrieveResource("LightRedAccent");
                    else        pfpColor = retrieveResource("DarkRedAccent");
                    break;
                case 1:
                    if (t == 0) pfpColor = retrieveResource("LightOrangeAccent");
                    else        pfpColor = retrieveResource("DarkOrangeAccent");
                    break;
                case 2:
                    if (t == 0) pfpColor = retrieveResource("LightYellowAccent");
                    else        pfpColor = retrieveResource("DarkYellowAccent");
                    break;
                case 3:
                    if (t == 0) pfpColor = retrieveResource("LightGreenAccent");
                    else        pfpColor = retrieveResource("DarkGreenAccent");
                    break;
                case 4:
                    if (t == 0) pfpColor = retrieveResource("LightBlueAccent");
                    else        pfpColor = retrieveResource("DarkBlueAccent");
                    break;
                case 5:
                    if (t == 0) pfpColor = retrieveResource("LightPurpleAccent");
                    else        pfpColor = retrieveResource("DarkPurpleAccent");
                    break;
            }

            return pfpColor;
        }

        private Color retrieveResource(string res)
        {
            Color resourceColor = Colors.Transparent;

            if (Application.Current.Resources.TryGetValue(res, out var resObj) && resObj is Color color)
            {
                resourceColor = color;
            }

            return resourceColor;
        }
    }

    public class ProfileController
    {
        public ObservableCollection<ProfileConverter> Profiles { get; set;}

        public bool RemoveProfile(int _ind)
        {
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
                _id = Profiles.Count,
            };

            Profiles.Add(new ProfileConverter(profile));

            Profiles[Profiles.Count - 1].ProfileTheme = theme;
            Profiles[Profiles.Count - 1].ProfileAccent = accent;
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