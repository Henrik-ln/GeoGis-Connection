using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.model
{
    class LocationMethod
    {
        private String code;
        private String description;

        public LocationMethod()
        {
            this.code = null;
            this.description = null;
        }

        public string Code { get { return code; } set { code = value; } }
        public string Description { get { return description; } set { description = value; } }

        public override string ToString()
        {
            return "LocationMethod [code=" + code + ", description=" + description + "]";
        }
    }
}
