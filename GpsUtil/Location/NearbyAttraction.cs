using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GpsUtil.Location
{
    public class NearbyAttraction
    {
        public Attraction Attraction { get; set; }
        public double Distance { get; set; }
        public int RewardPoints { get; set; }
    }
}
