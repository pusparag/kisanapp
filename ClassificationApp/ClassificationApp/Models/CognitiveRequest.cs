using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class CognitiveUrlRequest
    {
        public string ImageUrl { get; set; }
        public LocationDetail Location { get; set; }
    }

    public class CognitiveImageRequest
    {
        public string Image { get; set; }
        public LocationDetail Location { get; set; }
    }
}