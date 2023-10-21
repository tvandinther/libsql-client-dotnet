using System;
using System.Globalization;

namespace Libsql.Client
{
    public class Real : Value, IEquatable<Real>
    {
        public double Value { get; }

        internal Real(double value)
        {
            Value = value;
        }

        public override string ToString() => Value.ToString(CultureInfo.CurrentCulture);
        
        public static bool operator ==(Real left, Real right) => left?.Value == right?.Value;

        public static bool operator !=(Real left, Real right) => !(left == right);

        public override bool Equals(Value other)
        {
            if (other is Real otherReal)
            {
                return this == otherReal;
            }
            
            return false;
        }
        
        public bool Equals(Real other) => this == other;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is Real other) return Equals(other);
            return false;
        }

        public override int GetHashCode() => Value.GetHashCode();
    }
}