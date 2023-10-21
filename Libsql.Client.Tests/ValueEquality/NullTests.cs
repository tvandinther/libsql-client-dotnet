namespace Libsql.Client.Tests.ValueEquality;

public class NullTests
{
    [Fact]
    public void SameType_SameValue_AreEqual()
    {
        var left = new Null();
        var right = new Null();
        
        var equal = left.Equals(right);
        
        Assert.True(equal);
    }
    
    [Fact]
    public void SameType_Null_AreNotEqual()
    {
        var left = new Null();
        Null right = null!;
        
        var equal = left.Equals(right);
        
        Assert.False(equal);
    }
    
    [Fact]
    public void ValueType_SameValue_AreEqual()
    {
        var left = new Null();
        Value right = new Null();
        
        var equal = left.Equals(right);
        
        Assert.True(equal);
    }
    
    [Fact]
    public void ValueType_Null_AreNotEqual()
    {
        var left = new Null();
        Value right = null!;
        
        var equal = left.Equals(right);
        
        Assert.False(equal);
    }
}