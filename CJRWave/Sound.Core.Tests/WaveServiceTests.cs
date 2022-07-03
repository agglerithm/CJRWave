

using Sound.Core.Models;
using Sound.Core.WaveInterop;

namespace Sound.Core.Tests;

[TestFixture]
public class WaveServiceTests
{
    private WaveSynthService _sut;
    private double _frequency;
    private SoundSpectrum _spectrum = new SoundSpectrum(400.00);
    
    [Test]
    public  void CanRunService()
    {
        var config = new WaveServiceConfiguration<double>()
        {
            Channels = 1,
            BlockCount = 8,
            BlockSamples = 256,
            Flags = Wave.WaveInOutOpenFlags.CallbackFunction,
            UserFunction = PlayNote, 
            SampleRate = 176400,
            Log = Console.WriteLine
        };
        _sut = new WaveSynthService(config);
        _sut.Start();
        var counter = 0.0;
        var marker = counter;
                _frequency = 400.0;
                while (counter < 1.0)
                {
                    counter += .05;
                    while (marker < counter)
                    {
                        marker = _sut.GetTime();
                    }
                }

                while (counter < 2.0)
                {
                    counter += .05;
                    _spectrum.Add(_frequency * 2);
                    while (marker < counter)
                    {
                        marker = _sut.GetTime();
                    }
                }

                while (counter < 3.0)
                {
                    _spectrum.Add(_frequency * 3);
                    counter += .05;
                    while (marker < counter)
                    {
                        marker = _sut.GetTime();
                    }
                }

                while (counter < 4.0)
                {
                    counter += .05;
                    _spectrum.Add(_frequency * 4);
                    while (marker < counter)
                    {
                        marker = _sut.GetTime();
                    }
                }


                while (counter < 5.0)
                {
                    counter += .05;
                    _spectrum.Add(_frequency * 1.5);
                    while (marker < counter)
                    {
                        marker = _sut.GetTime();
                    }
                }

    
                _sut.End(); 
        
    }

    private double PlayNote(double arg)
    {
        var val = _spectrum.GetImpulse(arg, 1);
        //, _frequency * 1.5);
        return val;
    }
}