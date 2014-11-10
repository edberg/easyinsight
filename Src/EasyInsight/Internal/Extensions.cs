using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using EasyInsight;
using System.Reflection;
using System.Globalization;

namespace EasyInsight.Internal
{
    internal static class Extensions
    {
        public static DataSourceAttribute GetDataSource(this Type type)
        {
            return Attribute.GetCustomAttribute(type, typeof(DataSourceAttribute)) as DataSourceAttribute;
        }

        public static DataFieldAttribute GetDataField(this MemberInfo member)
        {
            return member.GetCustomAttributes(typeof(DataFieldAttribute), true).Cast<DataFieldAttribute>().FirstOrDefault();
        }

        public static List<DataFieldAttribute> GetDataFields(this Type type)
        {
            return (from p in type.GetProperties() select Attribute.GetCustomAttribute(p, typeof(DataFieldAttribute)) as DataFieldAttribute).ToList();
        }

        public static List<KeyValuePair<string, string>> GetData(this object obj)
        {
            var props = (from p in obj.GetType().GetProperties()
                         let attr = Attribute.GetCustomAttribute(p, typeof(DataFieldAttribute)) as DataFieldAttribute
                         where attr != null
                         let value = p.GetValue(obj, null)
                         let type = attr.type
                         select new KeyValuePair<string, string>(attr.key, value.FormatData())).ToList();
            return props;
        }

        public static string FormatData(this object obj)
        {
            var culture = CultureInfo.InvariantCulture;
            if (obj is DateTime) return ((DateTime)obj).ToUniversalTime().ToString("s");
            if (obj == null) return string.Empty;
            return Convert.ToString(obj, culture);
        }

        public static Response GetResponse(this string response)
        {
            var xe = XElement.Parse(response);
            var res = new Response
            {
                Code = xe.Element("code") == null ? null : xe.Element("code").Value,
                Message = xe.Element("message") == null ? null : xe.Element("message").Value,
                TransactionId = xe.Element("transactionID") == null ? null : xe.Element("transactionID").Value,
                DataSourceKey = xe.Element("dataSourceKey") == null ? null : xe.Element("dataSourceKey").Value,
            };
            return res;
        }

        public static async Task ForEachPage<T>(this IEnumerable<T> enumeration, int pagesize, Func<IEnumerable<T>, Task> action)
        {
            var counter = pagesize;
            var page = enumeration.Take(pagesize);
            while (page.Count() > 0)
            {
                await action(page);
                page = enumeration.Skip(counter).Take(pagesize);
                counter += page.Count();
            }
        }

        public static string GetSourceCardinality(this Cardinality cardinality)
        {
            if (cardinality == Cardinality.ManyToOne) return "many";
            return "one";
        }

        public static string GetTargetCardinality(this Cardinality cardinality)
        {
            if (cardinality == Cardinality.OneToMany) return "many";
            return "one";
        }

    }
}