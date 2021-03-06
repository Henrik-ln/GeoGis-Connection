﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.model
{
    class WaterLevel
    {
        private float? measurement;
        private ReferencePoint referencePoint;
        private DateTime? measurementDate;

        public WaterLevel()
        {
            this.measurement = null;
            this.referencePoint = null;
            this.measurementDate = null;
        }

        public float? Measurement { get { return measurement; } set { measurement = value; } }
        public DateTime? MeasurementDate { get { return measurementDate; } set { measurementDate = value; } }
        internal ReferencePoint ReferencePoint { get { return referencePoint; } set { referencePoint = value; } }

        public override string ToString()
        {
            string returnString = "WaterLevel [";
            if (measurement != null)
                returnString += "measurement = " + measurement;
            if (referencePoint != null)
                returnString += ", referencePoint=" + referencePoint;
            if (measurementDate != null)
                returnString += ", measurementDate=" + measurementDate;
            return returnString + "]";
        }
    }
}
