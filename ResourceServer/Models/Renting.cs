using System;
namespace ResourceServer.Models
{
    public class Renting
    {
        public int ID_Renting { get; set; }
        public int IDAp { get; set; }
        public string IDUser { get; set; }
        public DateTime date_from { get; set; }
        public DateTime? date_to { get; set; }
    }
}