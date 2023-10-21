namespace Libsql.Client.Tests.ValueEquality;

public class RealTests
{
    [Fact]
    public void SameType_SameValue_AreEqual()
    {
        var left = new Real(0.177);
        var right = new Real(0.177);
        
        var equal = left.Equals(right);
        
        Assert.True(equal);
    }
    
    [Fact]
    public void SameType_DifferentValue_AreNotEqual()
    {
        var left = new Real(0.177);
        var right = new Real(0.833);
        
        var equal = left.Equals(right);
        
        Assert.False(equal);
    }
    
    [Fact]
    public void SameType_Null_AreNotEqual()
    {
        var left = new Real(0.177);
        Real right = null!;
        
        var equal = left.Equals(right);
        
        Assert.False(equal);
    }
    
    [Fact]
    public void ValueType_SameValue_AreEqual()
    {
        var left = new Real(0.177);
        Value right = new Real(0.177);
        
        var equal = left.Equals(right);
        
        Assert.True(equal);
    }
    
    [Fact]
    public void ValueType_DifferentValue_AreNotEqual()
    {
        var left = new Real(0.177);
        Value right = new Real(0.833);
        
        var equal = left.Equals(right);
        
        Assert.False(equal);
    }
    
    [Fact]
    public void ValueType_Null_AreNotEqual()
    {
        var left = new Real(0.177);
        Value right = null!;
        
        var equal = left.Equals(right);
        
        Assert.False(equal);
    }
}