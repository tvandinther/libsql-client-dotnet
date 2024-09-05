using System;
using System.Text;

namespace Libsql.Client
{
    /// <summary>
    /// Represents a BLOB value in a database row.
    /// </summary>
    public class Blob : Value, IEquatable<Blob>
    {
        /// <summary>
        /// The binary data.
        /// </summary>
        public byte[] Value { get; }

        internal Blob(byte[] value)
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

        public static implicit operator Blob(byte[] value) => new Blob(value);

        public static implicit operator byte[](Blob blob) => blob.Value;

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