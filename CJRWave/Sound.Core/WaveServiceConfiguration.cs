using BufferUtilities;
using Sound.Core.WaveInterop;

namespace Sound.Core;

public class WaveServiceConfiguration
{
    public WaveServiceConfiguration()
    {
        BlockCount = 8;
        SampleRate = 44100;
        BlockSamples = 512;
        Channels = 2;
    }
    public int BlockCount { get; set; }
    public int SampleRate { get; set; }
    public int Channels { get; set; }
    public int BlockSamples { get; set; }
    
    public Func<double,double> UserFunction { get; set; }
    public Wave.WaveInOutOpenFlags Flags { get; set; }

    public Action<object> Log { get; set; }

}