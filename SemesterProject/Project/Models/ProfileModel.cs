using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemesterProject.Models
{
    public class ProfileModel
    {
        public int _id { get; set;} = 0;
    
        public string profile_name { get; set;} = "";

        public bool _isHighlighted = false;

        public int profile_theme { get; set;} = 0;
        public int profile_accent { get; set;} = 0;

        public ProfileModel()
        {
            //_id = _profile_counter++;
        }


    }

}