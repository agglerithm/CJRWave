using Sound.Core.WaveInterop;

namespace Sound.Core;

public class WaveSynthServiceConfiguration
{
    public WaveSynthServiceConfiguration()
    {
        BlockCount = 8;
        SampleRate = 44100;
        BlockSamples = 512;
        Flags = Wave.WaveInOutOpenFlags.CallbackEvent;
    }
    public uint BlockCount { get; set; }
    public int SampleRate { get; set; }
    public int Channels { get; set; }
    public uint BlockSamples { get; set; }
    
    public Func<double,double> UserFunction { get; set; }
    public Wave.WaveInOutOpenFlags Flags { get; set; }

    public Action<object>? Log { get; set; }

}
