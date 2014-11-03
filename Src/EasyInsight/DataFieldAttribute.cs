using System;

namespace EasyInsight
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DataFieldAttribute : Attribute
    {
        internal string key;
        internal string name;
        internal DataType type;

        public DataFieldAttribute(string key, string name, DataType type)
        {
            this.key = key;
            this.name = name;
            this.type = type;
        }
    }
}