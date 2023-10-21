using System;

namespace Libsql.Client
{
    public class Null : Value, IEquatable<Null>
    {
        public override string ToString() => "NULL";
        
        public static bool operator ==(Null left, Null right) => true;

        public static bool operator !=(Null left, Null right) => !(left == right);

        public override bool Equals(Value other) => other is Null;
        
        public bool Equals(Null other) => this == other;

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