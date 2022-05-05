using System;
using System.Collections.Generic;
using System.Text;

namespace PrismDemo.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]  //mozemo imati vise od 1 ribbon taba asociranog view-u
    public class DependentViewAttribute : Attribute
    {
        public Type Type { get; private set; }

        public string TargetRegionName { get; private set; }

        public DependentViewAttribute(Type viewType, string targetRegionName)
        {
            Type = viewType;
            TargetRegionName = targetRegionName;
        }
    }
}
