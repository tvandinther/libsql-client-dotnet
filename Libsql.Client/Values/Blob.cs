using System;
using System.Text;

namespace Libsql.Client
{
    public class Blob : Value, IEquatable<Blob>
    {
        public byte[] Value { get; }

        public Blob(byte[] value)
        {
            Value = value;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("[ ");
            var skipped = false;
            for (var i = 0; i < Value.Length; i++)
            {
                if (!skipped && i > 3 && Value.Length > 8)
                {
                    sb.Append("... ");
                    i = Value.Length - 5;
                    skipped = true;
                    continue;
                }

                sb.Append(Value[i].ToString("x2"));
                sb.Append(" ");
            }

            sb.Remove(sb.Length - 1, 1);
            sb.Append(" ]");

            return sb.ToString();
        }
        
        public static bool operator ==(Blob left, Blob right) => left?.Value == right?.Value;

        public static bool operator !=(Blob left, Blob right) => !(left == right);

        public override bool Equals(Value other)
        {
            if (other is Blob otherBlob)
            {
                return this == otherBlob;
            }

            return false;
        }
        
        public bool Equals(Blob other) => this == other;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is Blob other) return Equals(other);
            return false;
        }

        public override int GetHashCode() => Value.GetHashCode();
    }
}