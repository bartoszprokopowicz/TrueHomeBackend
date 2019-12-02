using System;

namespace ResourceServer.Models
{
    public class Apartment
    {
        public int ID_Ap { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string ApartmentNumber { get; set; }
        public string ImgThumb { get; set; }
        public string[] ImgList { get; set; }
        public double OwnerRating { get; set; }
        public double LocationRating { get; set; }
        public double StandardRating { get; set; }
        public double PriceRating { get; set; }
        public string PhoneNumber { get; set; }
        public decimal Lat { get; set; }
        public decimal Long { get; set; }
        public string Description { get; set; }
        public string IDUser { get; set; }        
    }
}
