using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.model
{
    class Borehole
    {
        private String boreholeNo;
        private float? referencePoint;
        private float? referencePointKote;
        private Location location;
        private BoreholeType boreholeType;
        private List<RockLayer> rockLayers;
        private List<Screen> screens;

        public Borehole()
        {
            this.boreholeNo = null;
            this.referencePoint = null;
            this.referencePointKote = null;
            this.location = null;
            this.boreholeType = null;
            this.rockLayers = new List<RockLayer>();
            this.screens = new List<Screen>();
        }

        public string BoreholeNo { get { return boreholeNo; } set { boreholeNo = value; } }
        public float? ReferencePoint { get { return referencePoint; } set { referencePoint = value; } }
        public float? ReferencePointKote { get { return referencePointKote; } set { referencePointKote = value; } }
        public Location Location { get { return location; } set { location = value; } }
        public BoreholeType BoreholeType { get { return boreholeType; } set { boreholeType = value; } }
        public List<RockLayer> RockLayers { get { return rockLayers; } set { rockLayers = value; } }
        public List<Screen> Screens { get { return screens; } set { screens = value; } }

        public void addRockLayer(RockLayer rockLayer)
        {
            rockLayers.Add(rockLayer);
        }

        public void addScreen(Screen screen)
        {
            screens.Add(screen);
        }

        public override string ToString()
        {
            String returnString = "Borehole [boreholeNo=" + boreholeNo + ", location=";
            if(location!= null)
            {
                returnString += location.ToString();
            }
            returnString += ", referencePoint=" + referencePoint + ", referencePointKote=" + referencePointKote + ", boreholeType=" + boreholeType + ", screens=";
            if(screens!= null)
            {
                if (screens.Count > 0)
                {
                    returnString += "{";
                    for (int i = 0; i < screens.Count; i++)
                    {

                        returnString += screens[i].ToString();
                    }
                    returnString += "}";
                }
            }
            return returnString + "]";
        }
    }
}
