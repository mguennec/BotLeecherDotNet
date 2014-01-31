// Type: NetIrc2.Details.ThrowExtensions
// Assembly: NetIrc2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1FC96D18-89A1-4E53-A98F-EFCAE44F24F1
// Assembly location: C:\Users\maguenne\Documents\Visual Studio 2013\Projects\TestApplication\packages\NetIrc2.1.0.0.0\lib\NetIrc2.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace BotLeecher.NetIrc.Details
{
    static class ThrowExtensions
    {
        public static Throw True(this Throw self, bool condition, string paramName = null)
        {
            if (condition)
                throw new ArgumentException(paramName);
            else
                return (Throw)null;
        }

        public static Throw False(this Throw self, bool condition, string paramName = null)
        {
            if (!condition)
                throw new ArgumentException(paramName);
            else
                return (Throw)null;
        }

        public static Throw Negative(this Throw self, int value, string paramName = null)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(paramName);
            else
                return (Throw)null;
        }

        public static Throw Null<T>(this Throw self, T value, string paramName = null)
        {
            if ((object)value == null)
                throw new ArgumentNullException(paramName);
            else
                return (Throw)null;
        }

        public static Throw NullElements<T>(this Throw self, IEnumerable<T> values, string paramName = null)
        {
            ThrowExtensions.Null<IEnumerable<T>>(Throw.If, values, paramName);
            ThrowExtensions.True(Throw.If, Enumerable.Any<T>(values, (Func<T, bool>)(value => (object)value == null)), paramName);
            return (Throw)null;
        }

        public static Throw OutOfRange<T>(this Throw self, IList<T> buffer, int offset, int count, string bufferName = "buffer", string offsetName = "offset", string countName = "count")
        {
            ThrowExtensions.Null<IList<T>>(Throw.If, buffer, bufferName);
            if (offset < 0 || offset > buffer.Count)
                throw new ArgumentOutOfRangeException(offsetName);
            if (count < 0 || count > buffer.Count - offset)
                throw new ArgumentOutOfRangeException(countName);
            else
                return (Throw)null;
        }
    }
}
