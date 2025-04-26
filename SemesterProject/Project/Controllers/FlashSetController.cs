using System.Collections.ObjectModel;
using System.Security.AccessControl;
using SemesterProject.Models;
using System.Diagnostics;

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

        public int SetID {get {return FlashSetModel._setID;} set{FlashSetModel._setID = value; OnPropertyChanged(nameof(SetID));}}

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
                OnPropertyChanged(nameof(SetAuth));
            }
        }

        public string SetDate
        {
            get{
                return $"{FlashSetModel.set_date}";
            }
            set{
                FlashSetModel.set_date = value;
                OnPropertyChanged(nameof(SetDate));
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

        public bool RemoveSetID(int _id)
        {
            int targetID = -1;
            for (int i = 0; i < FlashCardSets.Count; i++)
            {
                if (FlashCardSets[i].SetID == _id)
                {
                    targetID = i;
                }
            }

            if (targetID == -1) return false;

            FlashCardSets.RemoveAt(targetID);
            Debug.WriteLine("URGENT reindex");
            ReindexSets();
            return true;
        }

        public void AddNewFlashCardSet(string name, string edited, string author)
        {
            var flashcardset = new Models.FlashSetModel
            {
                set_name = name,
                _setID = FlashCardSets.Count,
                set_date = edited,
                set_auth = author
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

        public void DisplaySet(string name, string auth, string date, int id)
        {
            var flashcardset = new Models.FlashSetModel
            {
                set_name = name,
                _setID = id
            };

            FlashCardSets.Add(new FlashSetConverter(flashcardset));

            FlashCardSets[FlashCardSets.Count - 1].SetAuth = auth;
            FlashCardSets[FlashCardSets.Count - 1].SetDate = date;
        }

        public FlashSetConverter GetByID(int id)
        {
            var flashcardset = new Models.FlashSetModel
            {
                set_name = "err",
                _setID = -1
            };

            int targetID = -1;
            for (int i = 0; i < FlashCardSets.Count; i++)
            {
                if (FlashCardSets[i].SetID == id)
                {
                    targetID = i;
                }
            }

            if (targetID == -1) return new FlashSetConverter(flashcardset);

            return FlashCardSets[targetID];
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

            view.ItemsSource = FlashCardSets;
        }

        public void Clear()
        {
            FlashCardSets.Clear();
        }
    }
}