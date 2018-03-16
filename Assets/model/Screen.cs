using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.model
{
    class Screen
    {
        private int? screenNo;
        private float? top;
        private float? bottom;
        private float? diameter;
        private String diameterUnit;
        private List<WaterLevel> waterLevels;
        //TODO reference punkt

        public Screen()
        {
            this.screenNo = null;
            this.top = null;
            this.bottom = null;
            this.diameter = null;
            this.diameterUnit = null;
            this.waterLevels = null;
        }

        public int? ScreenNo { get { return screenNo; } set { screenNo = value; } }
        public float? Top { get { return top; } set { top = value; } }
        public float? Bottom { get { return bottom; } set { bottom = value; } }
        public float? Diameter { get { return diameter; } set { diameter = value; } }
        public string DiameterUnit { get { return diameterUnit; } set { diameterUnit = value; } }
        public List<WaterLevel> WaterLevels { get { return waterLevels; } set { waterLevels = value; } }

        public override string ToString()
        {
            String returnString = "Screen [screenNo=" + screenNo + "top=" + top + ", bottom=" + bottom + ", diameter=" + diameter + ", diameterUnit=" + diameterUnit + ", waterLevels=";
            if (waterLevels != null)
            {
                if (waterLevels.Count > 0)
                {
                    returnString += "{";
                    for (int i = 0; i < waterLevels.Count; i++)
                    {

                        returnString += waterLevels[i].ToString();
                    }
                    returnString += "}";
                }
            }
            return returnString + "]";
        }
    }
}
