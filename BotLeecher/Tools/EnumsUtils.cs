using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Tools
{
    public sealed class EnumsUtils
    {
        public static IList<T> GetValues<T>()
        {
            IList<T> retVal = new List<T>();
            var vals = typeof(T).GetProperties();
            foreach (var val in vals)
            {
                if (val.GetValue(null, null) is T)
                {
                    retVal.Add((T)val.GetValue(null, null));
                }
            }
            return retVal;
        }
    }
}
