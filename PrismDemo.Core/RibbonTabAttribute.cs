using System;
using System.Collections.Generic;
using System.Text;

namespace PrismDemo.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]  //mozemo imati vise od 1 ribbon taba asociranog view-u
    public class RibbonTabAttribute : Attribute
    {
        public Type Type { get; private set; }

        public RibbonTabAttribute(Type ribbonType)
        {
            Type = ribbonType;
        }
    }
}
