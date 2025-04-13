using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemesterProject.Models
{
    public class FlashCardModel
    {
        private static int _FCset_counter = 0;

        private int _setID;

        public int SetID {get {return _setID; } }

        public string set_name {get; set;} = "";

        public FlashCardModel()
        {
            _setID = _FCset_counter++;
        }

    }
}
