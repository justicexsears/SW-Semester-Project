using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemesterProject.Models
{
    public class FlashCardModel
    {
        public int _cardID { get; set;} = 0;

        public string card_q {get; set;} = "";

        public bool _isHighlighted = false;



        public FlashCardModel()
        {
           
        }

    }
}
