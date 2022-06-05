using NAudio.Wave;
using NUnit.Framework;

namespace Sound.Core.Tests;

[TestFixture]
public class WaveServiceTests
{
    private WaveService _sut;
    private double _frequency;
    [Test]
    public  void CanRunService()
    {
        var config = new WaveServiceConfiguration()
        {
            Channels = 1,
            BlockCount = 8,
            BlockSamples = 256,
            Flags = WaveInterop.WaveInOutOpenFlags.CallbackFunction,
            UserFunction = PlayNote
        };
        _sut = new WaveService(config);
        _sut.Start();
        while (_sut.GetTime() < 2)
        {
            _frequency = 440;
        }

        while (_sut.GetTime() < 4)
            _frequency = 880;
        _sut.End();
    }

    private double PlayNote(double arg)
    {
        return arg.SquareWave(23000, _frequency);
    }
}