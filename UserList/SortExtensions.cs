using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserList
{
    public static class SortExtension
    {
        public static readonly string[] Options =
            Array.ConvertAll(typeof(Person).GetProperties(), (property) => property.Name);

        public static List<Person> SortBy(this List<Person> persons, string property)
        {
            return Array.IndexOf(Options, property) >= 0
                ? (from p in persons orderby p.GetType().GetProperty(property)?.GetValue(p, null) ascending select p).ToList()
                : persons;
        }

        public static List<Person> FilterBy(this List<Person> persons, string property, string query)
        {
            if (Array.IndexOf(Options, property) < 0) return new List<Person>();

            query = query.ToLower();
            return (from p in persons
                    where (p.GetType().GetProperty(property)?.GetValue(p, null)).ToString().ToLower().Contains(query)
                    select p).ToList();
        }
    }
}
