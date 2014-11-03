using System;

namespace EasyInsight
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DataSourceAttribute : Attribute
    {
        internal string name;

        public DataSourceAttribute(string name)
        {
            this.name = name;
        }
    }
}