// Type: NetIrc2.IrcString
// Assembly: NetIrc2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1FC96D18-89A1-4E53-A98F-EFCAE44F24F1
// Assembly location: C:\Users\maguenne\Documents\Visual Studio 2013\Projects\TestApplication\packages\NetIrc2.1.0.0.0\lib\NetIrc2.dll

using BotLeecher.NetIrc.Details;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace BotLeecher.NetIrc
{
    /// <summary>
    /// Allows string-style manipulation of arrays of bytes.
    ///             IRC does not define an encoding, so this provides for encoding-agnostic parsing.
    /// 
    /// </summary>
    [TypeConverter(typeof(IrcStringTypeConverter))]
    [Serializable]
    public sealed class IrcString : IEquatable<IrcString>, IList<byte>, ICollection<byte>, IEnumerable<byte>, IEnumerable, ISerializable
    {
        /// <summary>
        /// A zero-byte string.
        /// 
        /// </summary>
        public static readonly IrcString Empty = (IrcString)"";
        private byte[] _buffer;

        /// <summary>
        /// The length of the IRC string, in bytes.
        /// 
        /// </summary>
        public int Length
        {
            get
            {
                return this._buffer.Length;
            }
        }

        /// <summary>
        /// Gets a byte from the IRC string.
        /// 
        /// </summary>
        /// <param name="index">The index into the byte array.</param>
        /// <returns>
        /// The byte at the specified index.
        /// </returns>
        public byte this[int index]
        {
            get
            {
                return this._buffer[index];
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        int ICollection<byte>.Count
        {
            get
            {
                return this.Length;
            }
        }

        bool ICollection<byte>.IsReadOnly
        {
            get
            {
                return true;
            }
        }

        static IrcString()
        {
        }

        /// <summary>
        /// Creates an IRC string by converting a .NET string using UTF-8 encoding.
        /// 
        /// </summary>
        /// <param name="string">The .NET string to convert.</param>
        public IrcString(string @string)
        {
            this.Create(@string, Encoding.UTF8);
        }

        /// <summary>
        /// Creates an IRC string by converting a .NET string using the specified encoding.
        /// 
        /// </summary>
        /// <param name="string">The .NET string to convert.</param><param name="encoding">The encoding to use.</param>
        public IrcString(string @string, Encoding encoding)
        {
            this.Create(@string, encoding);
        }

        /// <summary>
        /// Creates an IRC string from a byte array.
        /// 
        /// </summary>
        /// <param name="buffer">The array of bytes.</param>
        public IrcString(byte[] buffer)
        {
            ThrowExtensions.Null<byte[]>(Throw.If, buffer, "buffer");
            this.Create(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Creates an IRC string from part of a byte array.
        /// 
        /// </summary>
        /// <param name="buffer">The array of bytes.</param><param name="startIndex">The index of the first byte in the new string.</param><param name="length">The number of bytes in the new string.</param>
        public IrcString(byte[] buffer, int startIndex, int length)
        {
            this.Create(buffer, startIndex, length);
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        private IrcString(SerializationInfo info, StreamingContext context)
        {
            ThrowExtensions.Null<SerializationInfo>(Throw.If, info, "info");
            byte[] buffer = (byte[])info.GetValue("bytes", typeof(byte[])) ?? new byte[0];
            this.Create(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Casts the IRC string to a byte array.
        /// 
        /// </summary>
        /// <param name="string">The IRC string.</param>
        /// <returns>
        /// An array of bytes
        /// </returns>
        public static implicit operator byte[](IrcString @string)
        {
            if (!(@string != (IrcString)null))
                return (byte[])null;
            else
                return @string.ToByteArray();
        }

        /// <summary>
        /// Casts the IRC string to a .NET string using UTF-8 encoding.
        /// 
        /// </summary>
        /// <param name="string">The IRC string.</param>
        /// <returns>
        /// A .NET string.
        /// </returns>
        public static implicit operator string(IrcString @string)
        {
            if (!(@string != (IrcString)null))
                return (string)null;
            else
                return ((object)@string).ToString();
        }

        /// <summary>
        /// Casts a byte array to an IRC string.
        /// 
        /// </summary>
        /// <param name="buffer">The array of bytes.</param>
        /// <returns>
        /// An IRC string.
        /// </returns>
        public static implicit operator IrcString(byte[] buffer)
        {
            if (buffer == null)
                return (IrcString)null;
            else
                return new IrcString(buffer);
        }

        /// <summary>
        /// Casts a .NET string to an IRC string using UTF-8 encoding.
        /// 
        /// </summary>
        /// <param name="string">The .NET string.</param>
        /// <returns>
        /// An IRC string.
        /// </returns>
        public static implicit operator IrcString(string @string)
        {
            if (@string == null)
                return (IrcString)null;
            else
                return new IrcString(@string);
        }

        /// <summary>
        /// Compares two strings for equality.
        /// 
        /// </summary>
        /// <param name="string1">The first string.</param><param name="string2">The second string.</param>
        /// <returns>
        /// <c>true</c> if the strings are equal.
        /// </returns>
        public static bool operator ==(IrcString string1, IrcString string2)
        {
            return object.Equals((object)string1, (object)string2);
        }

        /// <summary>
        /// Compares two strings for inequality.
        /// 
        /// </summary>
        /// <param name="string1">The first string.</param><param name="string2">The second string.</param>
        /// <returns>
        /// <c>true</c> if the strings are not equal.
        /// </returns>
        public static bool operator !=(IrcString string1, IrcString string2)
        {
            return !object.Equals((object)string1, (object)string2);
        }

        /// <summary>
        /// Concatenates two strings.
        /// 
        /// </summary>
        /// <param name="string1">The first string.</param><param name="string2">The second string.</param>
        /// <returns>
        /// A string that is the concatentaion of the two.
        /// </returns>
        public static IrcString operator +(IrcString string1, IrcString string2)
        {
            ThrowExtensions.Null<IrcString>(ThrowExtensions.Null<IrcString>(Throw.If, string1, (string)null), string2, (string)null);
            byte[] numArray = new byte[string1.Length + string2.Length];
            string1.CopyTo(numArray, 0);
            string2.CopyTo(numArray, string1.Length);
            return new IrcString(numArray);
        }

        private void Create(string @string, Encoding encoding)
        {
            ThrowExtensions.Null<Encoding>(ThrowExtensions.Null<string>(Throw.If, @string, "string"), encoding, "encoding");
            this._buffer = encoding.GetBytes(@string);
        }

        private void Create(byte[] buffer, int startIndex, int length)
        {
            ThrowExtensions.OutOfRange<byte>(Throw.If, (IList<byte>)buffer, startIndex, length, "buffer", "offset", "count");
            this._buffer = new byte[length];
            Array.Copy((Array)buffer, startIndex, (Array)this._buffer, 0, length);
        }

        /// <summary>
        /// Checks if the string contains a particular byte.
        /// 
        /// </summary>
        /// <param name="value">The byte to look for.</param>
        /// <returns>
        /// <c>true</c> if the string contains the byte.
        /// </returns>
        public bool Contains(byte value)
        {
            return this.IndexOf(value) != -1;
        }

        /// <summary>
        /// Copies the string into a byte array.
        /// 
        /// </summary>
        /// <param name="array">The byte array to copy to.</param><param name="index">The starting index to copy to.</param>
        public void CopyTo(byte[] array, int index)
        {
            this._buffer.CopyTo((Array)array, index);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as IrcString);
        }

        /// <summary>
        /// Compares the current string with another string.
        /// 
        /// </summary>
        /// <param name="other">The string to compare with.</param>
        /// <returns>
        /// <c>true</c> if the strings are equal.
        /// </returns>
        public bool Equals(IrcString other)
        {
            if (other != (IrcString)null)
                return Enumerable.SequenceEqual<byte>((IEnumerable<byte>)this._buffer, (IEnumerable<byte>)other._buffer);
            else
                return false;
        }

        /// <inheritdoc/>
        public IEnumerator<byte> GetEnumerator()
        {
            for (int i = 0; i < this.Length; ++i)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.GetEnumerator();
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Scans the string for the first instance of a particular byte.
        /// 
        /// </summary>
        /// <param name="value">The byte to look for.</param>
        /// <returns>
        /// The index of the first matching byte, or <c>-1</c>.
        /// </returns>
        public int IndexOf(byte value)
        {
            return this.IndexOf(value, 0);
        }

        /// <summary>
        /// Scans part of the string for the first instance of a particular byte.
        /// 
        /// </summary>
        /// <param name="value">The byte to look for.</param><param name="startIndex">The first byte to begin scanning at.</param>
        /// <returns>
        /// The index of the first matching byte, or <c>-1</c>.
        /// </returns>
        public int IndexOf(byte value, int startIndex)
        {
            return this.IndexOf((Func<byte, bool>)(@byte => (int)@byte == (int)value), startIndex, this.Length - startIndex);
        }

        /// <summary>
        /// Scans part of the string for the first byte that matches the specified condition.
        /// 
        /// </summary>
        /// <param name="matchCondition">The condition to match.</param><param name="startIndex">The first byte to begin scanning at.</param><param name="length">The distance to scan.</param>
        /// <returns>
        /// The index of the first matching byte, or <c>-1</c>.
        /// </returns>
        public int IndexOf(Func<byte, bool> matchCondition, int startIndex, int length)
        {
            return IrcString.IndexOf(this._buffer, matchCondition, startIndex, length);
        }

        internal static int IndexOf(byte[] buffer, Func<byte, bool> matchCondition, int startIndex, int length)
        {
            ThrowExtensions.Null<Func<byte, bool>>(Throw.If, matchCondition, "matchCondition");
            string offsetName = "startIndex";
            string countName = "length";
            ThrowExtensions.OutOfRange<byte>(Throw.If, (IList<byte>)buffer, startIndex, length, "buffer", offsetName, countName);
            for (int index1 = 0; index1 < length; ++index1)
            {
                int index2 = index1 + startIndex;
                if (matchCondition(buffer[index2]))
                    return index2;
            }
            return -1;
        }

        /// <summary>
        /// Joins together a number of strings.
        /// 
        /// </summary>
        /// <param name="separator">The string to separate individual strings with.</param><param name="strings">The strings to join.</param>
        /// <returns>
        /// The joined string.
        /// </returns>
        public static IrcString Join(IrcString separator, IrcString[] strings)
        {
            ThrowExtensions.NullElements<IrcString>(ThrowExtensions.Null<IrcString>(Throw.If, separator, "separator"), (IEnumerable<IrcString>)strings, "strings");
            int length = 0;
            for (int index = 0; index < strings.Length; ++index)
            {
                if (index != 0)
                    length += separator.Length;
                length += strings[index].Length;
            }
            byte[] numArray = new byte[length];
            int index1 = 0;
            for (int index2 = 0; index2 < strings.Length; ++index2)
            {
                if (index2 != 0)
                {
                    separator.CopyTo(numArray, index1);
                    index1 += separator.Length;
                }
                strings[index2].CopyTo(numArray, index1);
                index1 += strings[index2].Length;
            }
            return new IrcString(numArray);
        }

        /// <summary>
        /// Splits the string into a number of substrings based on a separator.
        /// 
        /// </summary>
        /// <param name="separator">The byte to separate strings by.</param>
        /// <returns>
        /// An array of substrings.
        /// </returns>
        public IrcString[] Split(byte separator)
        {
            return this.Split(separator, int.MaxValue);
        }

        /// <summary>
        /// Splits the string into a limited number of substrings based on a separator.
        /// 
        /// </summary>
        /// <param name="separator">The byte to separate strings by.</param><param name="count">The maximum number of substrings. The last substring will contain the remaining bytes.</param>
        /// <returns>
        /// An array of substrings.
        /// </returns>
        public IrcString[] Split(byte separator, int count)
        {
            ThrowExtensions.Negative(Throw.If, count, "count");
            List<IrcString> list = new List<IrcString>();
            if (count > 0)
            {
                int startIndex = 0;
                while (startIndex < this.Length)
                {
                    int num = this.IndexOf(separator, startIndex);
                    if (num == -1 || list.Count + 1 == count)
                        num = this.Length;
                    int length = num - startIndex;
                    list.Add(this.Substring(startIndex, length));
                    startIndex += length + 1;
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// Checks if the start of the current string matches the specified string.
        /// 
        /// </summary>
        /// <param name="value">The string to match with.</param>
        /// <returns>
        /// <c>true</c> if the current string starts with <paramref name="value"/>.
        /// </returns>
        public bool StartsWith(IrcString value)
        {
            ThrowExtensions.Null<IrcString>(Throw.If, value, "value");
            if (this.Length < value.Length)
                return false;
            for (int index = 0; index < value.Length; ++index)
            {
                if ((int)this[index] != (int)value[index])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Extracts the end of a string.
        /// 
        /// </summary>
        /// <param name="startIndex">The index of the first byte in the substring.</param>
        /// <returns>
        /// The substring.
        /// </returns>
        public IrcString Substring(int startIndex)
        {
            return this.Substring(startIndex, this.Length - startIndex);
        }

        /// <summary>
        /// Extracts part of a string.
        /// 
        /// </summary>
        /// <param name="startIndex">The index of the first byte in the substring.</param><param name="length">The number of bytes to extract.</param>
        /// <returns>
        /// The substring.
        /// </returns>
        public IrcString Substring(int startIndex, int length)
        {
            string offsetName = "startIndex";
            string countName = "length";
            ThrowExtensions.OutOfRange<byte>(Throw.If, (IList<byte>)this._buffer, startIndex, length, "buffer", offsetName, countName);
            return new IrcString(this._buffer, startIndex, length);
        }

        /// <summary>
        /// Gets the bytes that make up the IRC string.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// An array of bytes.
        /// </returns>
        public byte[] ToByteArray()
        {
            return (byte[])this._buffer.Clone();
        }

        /// <summary>
        /// Converts to a .NET string using UTF-8 encoding.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// The converted string.
        /// </returns>
        public override string ToString()
        {
            return this.ToString(Encoding.UTF8);
        }

        /// <summary>
        /// Converts to a .NET string using the specified encoding.
        /// 
        /// </summary>
        /// <param name="encoding">The encoding to use in the conversion.</param>
        /// <returns>
        /// The converted string.
        /// </returns>
        public string ToString(Encoding encoding)
        {
            ThrowExtensions.Null<Encoding>(Throw.If, encoding, "encoding");
            return encoding.GetString(this._buffer);
        }

        void ICollection<byte>.Add(byte value)
        {
            throw new NotSupportedException();
        }

        void ICollection<byte>.Clear()
        {
            throw new NotSupportedException();
        }

        void IList<byte>.Insert(int index, byte value)
        {
            throw new NotSupportedException();
        }

        bool ICollection<byte>.Remove(byte value)
        {
            throw new NotSupportedException();
        }

        void IList<byte>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ThrowExtensions.Null<SerializationInfo>(Throw.If, info, "info");
            info.AddValue("bytes", (object)this.ToByteArray(), typeof(byte[]));
        }
    }
}
