using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.model
{
    class BoreholeType
    {
        private String code;
        private String shortDescription;
        private String description;
        private String colorCode;

        public BoreholeType()
        {
            this.code = null;
            this.shortDescription = null;
            this.description = null;
            this.colorCode = null;
        }

        public string Code { get { return code; } set { code = value; } }
        public string Description { get { return description; } set { description = value; } }
        public string ColorCode { get { return colorCode; } set { colorCode = value; } }
        public string ShortDescription { get { return shortDescription; } set { shortDescription = value; } }

        public override string ToString()
        {
            string returnString = "BoreholeType [";
            if (code != null)
                returnString += "code = " + code;
            if (shortDescription != null)
                returnString += ", shortDescription=" + shortDescription;
            if (description != null)
                returnString += ", description=" + description;
            if (colorCode != null)
                returnString += ", colorCode=" + colorCode;
            return returnString + "]";
        }
    }
}