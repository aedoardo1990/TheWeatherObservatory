using System;
using System.Collections.Generic;
using System.Text;

namespace The_Weather_Observatory.Models
{
    public class SaveLocation
    {
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime LastSearched { get; set; }
    }
}
