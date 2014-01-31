// Type: NetIrc2.Details.IrcStringTypeConverter
// Assembly: NetIrc2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1FC96D18-89A1-4E53-A98F-EFCAE44F24F1
// Assembly location: C:\Users\maguenne\Documents\Visual Studio 2013\Projects\TestApplication\packages\NetIrc2.1.0.0.0\lib\NetIrc2.dll

using NetIrc2;
using System;
using System.ComponentModel;
using System.Globalization;

namespace BotLeecher.NetIrc.Details
{
    /// <summary>
    /// Converts between <see cref="T:NetIrc2.IrcString"/> and <see cref="T:System.String"/>.
    ///             This class is used by Visual Studio's various Designers.
    /// 
    /// </summary>
    public class IrcStringTypeConverter : TypeConverter
    {
        /// <inheritdoc/>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            else
                return base.CanConvertFrom(context, sourceType);
        }

        /// <inheritdoc/>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            else
                return base.CanConvertTo(context, destinationType);
        }

        /// <inheritdoc/>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
                return (object)(IrcString)((string)value);
            if (value == null)
                return (object)null;
            else
                return base.ConvertFrom(context, culture, value);
        }

        /// <inheritdoc/>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value is IrcString)
                    return (object)(string)((IrcString)value);
                if (value == null)
                    return (object)null;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
