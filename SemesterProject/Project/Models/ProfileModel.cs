using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemesterProject.Models
{
    public class ProfileModel
    {
        private static int _profile_counter = 0;

        private int _id;

        public int Id {get {return _id; } }
    
        public string profile_name { get; set;} = "";

    public ProfileModel()
    {
        _id = _profile_counter++;
    }

    }

}