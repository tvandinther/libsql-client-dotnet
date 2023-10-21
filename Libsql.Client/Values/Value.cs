using System;

namespace Libsql.Client
{
    public abstract class Value : IEquatable<Value>
    {
        public abstract override int GetHashCode();

        public static bool operator ==(Value left, Value right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(Value left, Value right) => !(left == right);
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is Value other) return Equals(other);
            return false;
        }
        
        public abstract bool Equals(Value other);
    }
}