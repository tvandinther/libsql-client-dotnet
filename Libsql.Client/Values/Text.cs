using System;

namespace Libsql.Client
{
    public class Text : Value, IEquatable<Text>
    {
        public string Value { get; }

        public Text(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;
        
        public static bool operator ==(Text left, Text right) => left?.Value == right?.Value;
        
        public static bool operator !=(Text left, Text right) => !(left == right);
        
        public override bool Equals(Value other)
        {
            if (other is Text otherText)
            {
                return this == otherText;
            }

            return false;
        }
        
        public bool Equals(Text other) => this == other;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is Text other) return Equals(other);
            return false;
        }

        public override int GetHashCode() => Value.GetHashCode();
    }
}