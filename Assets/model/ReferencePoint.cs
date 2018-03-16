using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database.model
{
    class ReferencePoint
    {
        private String code;
        private String description;

        public ReferencePoint()
        {
            this.code = null;
            this.description = null;
        }

        public string Code { get { return code; } set { code = value; } }
        public string Description { get { return description; } set { description = value; } }

        public override string ToString()
        {
            return "ReferencePoint [code=" + code + ", description=" + description + "]";
        }
    }
}
