using NAudio.Wave;

namespace Sound.Core.Tests;

public class Tests
{
    private WaveOutWrapper _sut;
    [SetUp]
    public void Setup()
    {
        _sut = new WaveOutWrapper();
    }
    
    [Test]
    public void CanOpenDevice()
    {
        var format = new FormatRequest()
        {
            BitsPerSample = 16,
            Channels = 1,
            SampleRate = 44500,
            Flags = WaveInterop.WaveInOutOpenFlags.CallbackFunction
        };
        _sut.Open(format);
    }
}