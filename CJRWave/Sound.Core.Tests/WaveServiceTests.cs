

using Sound.Core.WaveInterop;

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
            Flags = Wave.WaveInOutOpenFlags.CallbackFunction,
            UserFunction = PlayNote
        };
        _sut = new WaveService(config);
        _sut.Start();
        var counter = 0.0;
            while(counter < 1.0)
            {
                counter += .05;
                while (_sut.GetTime() < counter)
                {
                    _frequency = 440;
                }
            }
            _sut.End(); 
        
    }
    
    private double PlayNote(double arg)
    {
        var val = arg.Chord(1, _frequency, 
            _frequency * 1.5,
            _frequency * 2, 
            _frequency * 3, 
            _frequency * 4, 
            _frequency * 5);
        return val;
    }
}