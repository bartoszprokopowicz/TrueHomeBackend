using ResourceServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResourceServer.JSONModels
{
    public class ApartmentListJSON
    {
        public IList<Apartment> apartmentsList { get; set; }
        public bool hasMore { get; set; }
    }
}
