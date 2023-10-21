namespace Libsql.Client.Tests.ValueEquality;

public class BlobTests
{
    [Fact]
    public void SameType_SameValue_AreEqual()
    {
        var byteArray = new byte[] { 1 };
        var left = new Blob(byteArray);
        var right = new Blob(byteArray);
        
        var equal = left.Equals(right);
        
        Assert.True(equal);
    }
    
    [Fact]
    public void SameType_DifferentValue_AreNotEqual()
    {
        var left = new Blob(new byte[] { 1 });
        var right = new Blob(new byte[] { 2 });
        
        var equal = left.Equals(right);
        
        Assert.False(equal);
    }
    
    [Fact]
    public void SameType_Null_AreNotEqual()
    {
        var left = new Blob(new byte[] { 1 });
        Blob right = null!;
        
        var equal = left.Equals(right);
        
        Assert.False(equal);
    }
    
    [Fact]
    public void ValueType_SameValue_AreEqual()
    {
        var byteArray = new byte[] { 1 };
        var left = new Blob(byteArray);
        Value right = new Blob(byteArray);
        
        var equal = left.Equals(right);
        
        Assert.True(equal);
    }
    
    [Fact]
    public void ValueType_DifferentValue_AreNotEqual()
    {
        var left = new Blob(new byte[] { 1 });
        Value right = new Blob(new byte[] { 2 });
        
        var equal = left.Equals(right);
        
        Assert.False(equal);
    }
    
    [Fact]
    public void ValueType_Null_AreNotEqual()
    {
        var left = new Blob(new byte[] { 1 });
        Value right = null!;
        
        var equal = left.Equals(right);
        
        Assert.False(equal);
    }
}