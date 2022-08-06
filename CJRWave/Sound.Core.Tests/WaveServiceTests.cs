

using Sound.Core.Models;
using Sound.Core.WaveInterop;

namespace Sound.Core.Tests;

[TestFixture]
public class WaveServiceTests
{
    private WaveSynthService _sut;
    private double _frequency;
    private readonly SoundSpectrum _spectrum = new(400.00);
    
    [Test]
    public  void CanRunServiceWithSpectrum()
    {
        var config = new WaveSynthServiceConfiguration()
        {
            Channels = 1,
            BlockCount = 1,
            BlockSamples = 512,
            Flags = Wave.WaveInOutOpenFlags.CallbackFunction,
            UserFunction = PlayNote, 
            SampleRate = 44100,
            Log = Console.WriteLine
        };
        _sut = new WaveSynthService(config);
        _sut.Start();
        var counter = 0.0;
        _frequency = 400.0;
        _spectrum.Add(_frequency * 2,2);
        _spectrum.Add(_frequency * 3,.7);
        _spectrum.Add(_frequency * 4,1.5);
        while (counter < 1.0)
        {
            counter = _sut.GetTime();
        }
        while (counter < 2.0)
        {
            counter = _sut.GetTime();
        }
        while (counter < 3.0)
        {
            counter = _sut.GetTime(); 
        }
        while (counter < 4.0)
        {
            counter = _sut.GetTime();
        }
        _spectrum.Add(_frequency * 1.5,3);
        while (counter < 5.0)
        {
            counter = _sut.GetTime();
        }
        _sut.End(); 
    }

    private double PlayNote(double arg)
    {
        var val = _spectrum.GetImpulse(arg, 1);
        //, _frequency * 1.5);
        return val;
    }
    
    [Test]
    public  void CanRunServiceWithSineWave()
    {
        var baseFreq = 400.00;
        var config = new WaveSynthServiceConfiguration()
        {
            Channels = 1,
            BlockCount = 1,
            BlockSamples = 512,
            Flags = Wave.WaveInOutOpenFlags.CallbackFunction,
            UserFunction = PlaySineWave, 
            SampleRate = 44100,
            Log = Console.WriteLine
        };
        _sut = new WaveSynthService(config);
        _sut.Start();
        var counter = 0.0;
        _frequency = baseFreq;
        while (counter < 1.0)
        {
            counter = _sut.GetTime();
        }
        _sut.End(); 
    }

    private double PlaySineWave(double arg)
    {
        return arg.SineWave(1,_frequency);
    }
}