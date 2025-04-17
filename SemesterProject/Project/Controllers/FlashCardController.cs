using System.Collections.ObjectModel;
using System.Security.AccessControl;
using SemesterProject.Models;

namespace SemesterProject.Controllers
{
    public class FlashCardConverter : BindableObject
    {
        public Models.FlashCardModel FlashCardModel {get; set;}

        private bool _editing = false;

        public FlashCardConverter(Models.FlashCardModel data)
        {
            FlashCardModel = data;
        }

        public int FlashCardID {get {return FlashCardModel.SetID;}}

        public string SetName
        {
            get{
                return $"{FlashCardModel.set_name}";
            }
        }

        private void NotifyView()
        {
            OnPropertyChanged(nameof(FlashCardID));
            OnPropertyChanged(nameof(SetName));
        }
  
    }

    public class FlashCardController
    {
        public ObservableCollection<FlashCardConverter> FlashCardSets {get; set;}

        public bool RemoveSet(string _name)
        {
            for(int i=0; i < FlashCardSets.Count; i++)
            {
                if(FlashCardSets[i].SetName == _name)
                {
                    FlashCardSets.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public void AddNewFlashCardSet(string name)
        {
            var flashcardset = new Models.FlashCardModel
            {
                set_name = name
            };

            FlashCardSets.Add(new FlashCardConverter(flashcardset));
        }

        public FlashCardController(CollectionView view)
        {
            FlashCardSets = new ObservableCollection<FlashCardConverter>();

            var flashcardset = new Models.FlashCardModel
            {
                set_name = "Example Set1"
            };
            FlashCardSets.Add(new FlashCardConverter(flashcardset));

            flashcardset = new Models.FlashCardModel
            {
                set_name = "Example Set2"
            };
            FlashCardSets.Add(new FlashCardConverter(flashcardset));

            flashcardset = new Models.FlashCardModel
            {
                set_name = "Example Set3"
            };
            FlashCardSets.Add(new FlashCardConverter(flashcardset));

            view.ItemsSource = FlashCardSets;
        }
    }
}