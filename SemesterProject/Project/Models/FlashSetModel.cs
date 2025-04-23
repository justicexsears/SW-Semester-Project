using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemesterProject.Models
{
    public class FlashSetModel
    {
        public int _setID { get; set;} = 0;

        public string set_name {get; set;} = "";
        public string set_auth {get; set;} = "";
        public string set_date {get; set;} = "";

        public bool _isHighlighted = false;



        public FlashSetModel()
        {
           
        }

    }
}
