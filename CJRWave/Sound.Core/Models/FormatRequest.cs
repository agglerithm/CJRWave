using Sound.Core.WaveInterop;

namespace Sound.Core;

public class FormatRequest
{
    public int Channels  { get; set; }

    public int SampleRate { get; set; }

    public int BitsPerSample { get; set; }
    
    public Wave.WaveInOutOpenFlags Flags { get; set; }

    public WaveFormat BuildWaveFormat()
    {
        return  new WaveFormat(SampleRate, Channels);
    }
    public Wave.WaveCallback Callback { get; set; }
}