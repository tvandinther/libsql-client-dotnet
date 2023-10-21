namespace Libsql.Client.Tests.ValueEquality;

public class TextTests
{
    [Fact]
    public void SameType_SameValue_AreEqual()
    {
        var left = new Text("a");
        var right = new Text("a");
        
        var equal = left.Equals(right);
        
        Assert.True(equal);
    }
    
    [Fact]
    public void SameType_DifferentValue_AreNotEqual()
    {
        var left = new Text("a");
        var right = new Text("b");
        
        var equal = left.Equals(right);
        
        Assert.False(equal);
    }
    
    [Fact]
    public void SameType_Null_AreNotEqual()
    {
        var left = new Text("a");
        Text right = null!;
        
        var equal = left.Equals(right);
        
        Assert.False(equal);
    }
    
    [Fact]
    public void ValueType_SameValue_AreEqual()
    {
        var left = new Text("a");
        Value right = new Text("a");
        
        var equal = left.Equals(right);
        
        Assert.True(equal);
    }
    
    [Fact]
    public void ValueType_DifferentValue_AreNotEqual()
    {
        var left = new Text("a");
        Value right = new Text("b");
        
        var equal = left.Equals(right);
        
        Assert.False(equal);
    }
    
    [Fact]
    public void ValueType_Null_AreNotEqual()
    {
        var left = new Text("a");
        Value right = null!;
        
        var equal = left.Equals(right);
        
        Assert.False(equal);
    }
}