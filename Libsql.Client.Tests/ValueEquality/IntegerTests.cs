namespace Libsql.Client.Tests.ValueEquality;

public class IntegerTests
{
    [Fact]
    public void SameType_SameValue_AreEqual()
    {
        var left = new Integer(1);
        var right = new Integer(1);
        
        var equal = left.Equals(right);
        
        Assert.True(equal);
    }
    
    [Fact]
    public void SameType_DifferentValue_AreNotEqual()
    {
        var left = new Integer(1);
        var right = new Integer(2);
        
        var equal = left.Equals(right);
        
        Assert.False(equal);
    }
    
    [Fact]
    public void SameType_Null_AreNotEqual()
    {
        var left = new Integer(1);
        Integer right = null!;
        
        var equal = left.Equals(right);
        
        Assert.False(equal);
    }
    
    [Fact]
    public void ValueType_SameValue_AreEqual()
    {
        var left = new Integer(1);
        Value right = new Integer(1);
        
        var equal = left.Equals(right);
        
        Assert.True(equal);
    }
    
    [Fact]
    public void ValueType_DifferentValue_AreNotEqual()
    {
        var left = new Integer(1);
        Value right = new Integer(2);
        
        var equal = left.Equals(right);
        
        Assert.False(equal);
    }
    
    [Fact]
    public void ValueType_Null_AreNotEqual()
    {
        var left = new Integer(1);
        Value right = null!;
        
        var equal = left.Equals(right);
        
        Assert.False(equal);
    }
}