using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResourceServer.Models;

namespace ResourceServer.JSONModels
{
    public class RatingJSON
    {
        public IList<Rating> ratingsList { get; set; }
        public bool hasMore { get; set; }
    }
}
