using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Database.model
{
    class RockType
    {
        private String code;
        private String description;
        private String colorCode;
        private Color color;

        public RockType(string code, string description, String colorCode)
        {
            this.code = code;
            this.description = description;
            this.colorCode = colorCode;
        }

        public string Code { get { return code; } set { code = value; } }
        public string Description { get { return description; } set { description = value; } }
        public string ColorCode { get { return colorCode; } set { colorCode = value; } }
        public Color Color { get { return color; } set { color = value; } }
    }
}
