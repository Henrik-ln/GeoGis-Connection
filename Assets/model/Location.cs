using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.model
{
    class Location
    {
        private float x;
        private float y;
        private LocationQuality locationQuality;
        private LocationMethod locationMethod;

        public Location(float latitude, float longitude)
        {
            this.x = latitude;
            this.y = longitude;
            this.locationQuality = null;
            this.locationMethod = null;
        }

        public float X { get { return x; } set { x = value; } }
        public float Y { get { return y; } set { y = value; } }
        public LocationQuality LocationQuality { get { return locationQuality; } set { locationQuality = value; } }
        public LocationMethod LocationMethod { get { return locationMethod; } set { locationMethod = value; } }

        public override string ToString()
        {
            string returnString = "Location [x=" + x + ", longitude=" + y;
            if(locationQuality != null)
            {
                returnString += ", locationQuality =";
                returnString += locationQuality.ToString();
            }
            
            if (locationMethod != null)
            {
                returnString += ", locationMethod=";
                returnString += locationMethod.ToString();
            }
            returnString += "]";
            return returnString;
        }
    }
}
