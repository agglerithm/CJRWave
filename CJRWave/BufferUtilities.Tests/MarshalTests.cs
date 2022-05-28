namespace BufferUtilities.Tests;

public class Tests
{
    public struct TestStruct
    {
        public string Value1 { get; set; }
        public int Value2 { get; set; }
    }
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void CanReferenceAndDereferenceStruct()
    {
        var testStruct = new TestStruct
        {
            Value1 = "value1",
            Value2 = 35
        };
        var ptr = testStruct.PtrFromStruct();
        Assert.IsNotNull(ptr);
        var copyStruct = ptr.DereferenceStruct<TestStruct>();
        Assert.That(copyStruct.Value2, Is.EqualTo(35));
        Assert.That(copyStruct.Value1, Is.EqualTo("value1"));
    }
    
    [Test]
    public void CanReferenceAndDereferenceBuffer()
    {
        var buff = "This is the buffer! ".StringToByteArray();
        var ptr = buff.PtrFromByteArray();
        Assert.IsNotNull(ptr);
        var buffCopy = ptr.DereferenceByteArray().ByteArrayToString();
        Console.WriteLine(buffCopy);
        Assert.That(buffCopy, Is.EqualTo("This is the buffer! "));
    }
}