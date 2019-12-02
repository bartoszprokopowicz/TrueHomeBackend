using System;

namespace ResourceServer.Models
{
    public class PersonalData
    {
        public int ID_PData { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string PhoneNumber { get; set; }
        public string IDUser { get; set; }
    }
}