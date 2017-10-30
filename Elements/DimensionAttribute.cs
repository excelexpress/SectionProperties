using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelExpress.ComplexShape.SectionProperties
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DimensionAttribute : Attribute
    {
        private string name;

        public DimensionAttribute(string name)
        {
            this.name = name;
        }

    }
}
