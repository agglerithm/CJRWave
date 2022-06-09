namespace CJRWave.Test;

public class Tests
{
    private WAVFile _sut;
    [SetUp]
    public void Setup()
    {
        _sut = new WAVFile();
    }

    [Test]
    public void CanReadWaveFile()
    {
        _sut.Read("dance.wav");
    }
}