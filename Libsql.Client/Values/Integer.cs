using System;

namespace Libsql.Client
{
    /// <summary>
    /// Represents an INTEGER value in a database row.
    /// </summary>
    public class Integer : Value, IEquatable<Integer>
    {
        /// <summary>
        /// The integer value.
        /// </summary>
        public int Value { get; }

        internal Integer(int value)
        {
            Value = value;
        }

        public override string ToString() => Value.ToString();
        
        public static bool operator ==(Integer left, Integer right) => left?.Value == right?.Value;

        public static bool operator !=(Integer left, Integer right) => !(left == right);
        
        public static implicit operator Integer(int value) => new Integer(value);
        
        public static implicit operator int(Integer integer) => integer.Value;
        
        public override bool Equals(Value other)
        {
            if (other is Integer otherInteger)
            {
                return Equals(otherInteger);
            }
            
            return false;
        }
        
        public bool Equals(Integer other) => this == other;
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is Integer other) return Equals(other);
            return false;
        }

        public override int GetHashCode() => Value.GetHashCode();
        
    }
}