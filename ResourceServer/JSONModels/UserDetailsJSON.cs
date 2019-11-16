using ResourceServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResourceServer.JSONModels
{
    public class UserDetailsJSON
    {
        public User user { get; set; }
        public PersonalData personalData { get; set; }
        public IList<Apartment> apartmentList { get; set; }
    }
}