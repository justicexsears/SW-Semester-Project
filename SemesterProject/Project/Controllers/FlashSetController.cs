using System.Collections.ObjectModel;
using System.Security.AccessControl;
using SemesterProject.Models;

namespace SemesterProject.Controllers
{
    public class FlashSetConverter : BindableObject
    {
        public Models.FlashSetModel FlashSetModel {get; set;}

        private bool _editing = false;

        public FlashSetConverter(Models.FlashSetModel data)
        {
            FlashSetModel = data;
        }

        public bool IsHighlighted { 
            get {return FlashSetModel._isHighlighted;} 
            set {
                FlashSetModel._isHighlighted = value;
                OnPropertyChanged(nameof(IsHighlighted));
            }
        }

        public int SetID {get {return FlashSetModel._setID;} set{FlashSetModel._setID = value;}}

        public string SetName
        {
            get{
                return $"{FlashSetModel.set_name}";
            }
        }

        public string SetAuth
        {
            get{
                return $"{FlashSetModel.set_auth}";
            }
            set{
                FlashSetModel.set_auth = value;
            }
        }

        public string SetDate
        {
            get{
                return $"{FlashSetModel.set_date}";
            }
            set{
                FlashSetModel.set_date = value;
            }
        }

        private void NotifyView()
        {
            OnPropertyChanged(nameof(SetID));
            OnPropertyChanged(nameof(SetName));
            OnPropertyChanged(nameof(SetAuth));
            OnPropertyChanged(nameof(SetDate));
            OnPropertyChanged(nameof(IsHighlighted));
        }
  
    }

    public class FlashSetController
    {
        public ObservableCollection<FlashSetConverter> FlashCardSets {get; set;}

        public bool RemoveSet(int _ind)
        {
            if (FlashCardSets.Count > _ind)
            {
                FlashCardSets.RemoveAt(_ind);
                ReindexSets();
                return true;
            }

            return false;
        }

        public void AddNewFlashCardSet(string name)
        {
            var flashcardset = new Models.FlashSetModel
            {
                set_name = name,
                _setID = FlashCardSets.Count
            };

            FlashCardSets.Add(new FlashSetConverter(flashcardset));
        }

        public void DisplaySet(string name, string auth, string date)
        {
            var flashcardset = new Models.FlashSetModel
            {
                set_name = name,
                _setID = FlashCardSets.Count
            };

            FlashCardSets.Add(new FlashSetConverter(flashcardset));

            FlashCardSets[FlashCardSets.Count - 1].SetAuth = auth;
            FlashCardSets[FlashCardSets.Count - 1].SetDate = date;
        }

        public void ReindexSets()
        {
            //begin from 1, and step index nums back by 1, this accounts for ever present guest profile
            for(int i = 0; i < FlashCardSets.Count; i++)
            {
                FlashCardSets[i].SetID = (i);
            }
        }

        public FlashSetController(CollectionView view)
        {
            FlashCardSets = new ObservableCollection<FlashSetConverter>();

            var flashcardset = new Models.FlashSetModel
            {
                set_name = "Example Set1",
                set_auth = "Author Name",
                set_date = "01/01/1970"
            };
            FlashCardSets.Add(new FlashSetConverter(flashcardset));

            flashcardset = new Models.FlashSetModel
            {
                set_name = "Example Set2",
                set_auth = "Author Name",
                set_date = "01/01/1970"
            };
            FlashCardSets.Add(new FlashSetConverter(flashcardset));

            flashcardset = new Models.FlashSetModel
            {
                set_name = "Example Set3",
                set_auth = "Author Name",
                set_date = "01/01/1970"
            };
            FlashCardSets.Add(new FlashSetConverter(flashcardset));

            view.ItemsSource = FlashCardSets;
        }
    }
}