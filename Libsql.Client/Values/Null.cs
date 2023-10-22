using System;

namespace Libsql.Client
{
    /// <summary>
    /// Represents a NULL value in a database row.
    /// </summary>
    public class Null : Value, IEquatable<Null>
    {
        internal Null() { }
        
        public override string ToString() => "NULL";
        
        public static bool operator ==(Null left, Null right) => true;

        public static bool operator !=(Null left, Null right) => !(left == right);

        public override bool Equals(Value other) => other is Null;
        
        public bool Equals(Null other)
        {
            if (ReferenceEquals(null, other)) return false;
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is Null other) return Equals(other);
            return false;
        }
        
        public override int GetHashCode() => 0;
    }
}