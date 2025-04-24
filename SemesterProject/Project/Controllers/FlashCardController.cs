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

        public bool IsHighlighted { 
            get {return FlashCardModel._isHighlighted;} 
            set {
                FlashCardModel._isHighlighted = value;
                OnPropertyChanged(nameof(IsHighlighted));
            }
        }

        public int CardID {
            get {return FlashCardModel._cardID;} 
            set{
                FlashCardModel._cardID = value;
                OnPropertyChanged(nameof(CardID));
            }
        }

        public string CardQ
        {
            get{
                return $"{FlashCardModel.card_q}";
            }
            set{
                FlashCardModel.card_q = value;
                OnPropertyChanged(nameof(CardQ));
            }
        }

        private void NotifyView()
        {
            OnPropertyChanged(nameof(CardID));
            OnPropertyChanged(nameof(CardQ));
            OnPropertyChanged(nameof(IsHighlighted));
        }
  
    }

    public class FlashCardController
    {
        public ObservableCollection<FlashCardConverter> FlashCards {get; set;}

        public bool RemoveCard(int _ind)
        {
            if (FlashCards.Count > _ind)
            {
                FlashCards.RemoveAt(_ind);
                ReindexCards();
                return true;
            }

            return false;
        }

        public void AddNewFlashCard()
        {
            var flashcard = new Models.FlashCardModel
            {
                _cardID = FlashCards.Count,
                card_q = ""
            };

            FlashCards.Add(new FlashCardConverter(flashcard));
        }

        public void DisplayCard(string q)
        {
            var flashcard = new Models.FlashCardModel
            {
                _cardID = FlashCards.Count,
                card_q = q
            };

            FlashCards.Add(new FlashCardConverter(flashcard));
        }

        public void ReindexCards()
        {
            //begin from 1, and step index nums back by 1, this accounts for ever present guest profile
            for(int i = 0; i < FlashCards.Count; i++)
            {
                FlashCards[i].CardID = (i);
            }
        }

        public FlashCardController(CollectionView view)
        {
            FlashCards = new ObservableCollection<FlashCardConverter>();

            var item = new Models.FlashCardModel
            {
                card_q = "What is the capital of Michigan?",
                _cardID = FlashCards.Count
            };
            FlashCards.Add(new FlashCardConverter(item));

            item = new Models.FlashCardModel
            {
                card_q = "How many are in a dozen?",
                _cardID = FlashCards.Count
            };
            FlashCards.Add(new FlashCardConverter(item));

            item = new Models.FlashCardModel
            {
                card_q = "What is a djungelskog?",
                _cardID = FlashCards.Count
            };
            FlashCards.Add(new FlashCardConverter(item));

            item = new Models.FlashCardModel
            {
                card_q = "3 + 4 = __?",
                _cardID = FlashCards.Count
            };
            FlashCards.Add(new FlashCardConverter(item));

            item = new Models.FlashCardModel
            {
                card_q = "Who invented the lightbulb?",
                _cardID = FlashCards.Count
            };
            FlashCards.Add(new FlashCardConverter(item));

            view.ItemsSource = FlashCards;
        }
    }
}