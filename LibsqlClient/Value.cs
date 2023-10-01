using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace LibsqlClient;

public abstract record Value;

public record Integer(int Value) : Value
{
    public override string ToString() => Value.ToString();
}

public record Real(double Value) : Value
{
    public override string ToString() => Value.ToString();
}

public record Text(string Value) : Value
{
    public override string ToString() => Value;
}

public record Blob(byte[] Value) : Value
{
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
}

public record Null : Value
{
    public override string ToString() => "NULL";
}