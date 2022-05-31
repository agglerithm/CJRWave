using NAudio.Wave;

namespace Sound.Core;

public class FormatRequest
{
    public int Channels  { get; set; }

    public int SampleRate { get; set; }

    public int BitsPerSample { get; set; }
    
    public WaveInterop.WaveInOutOpenFlags Flags { get; set; }

    public WaveFormat BuildWaveFormat()
    {
        return  new WaveFormat(SampleRate, Channels);
    }
    public WaveInterop.WaveCallback Callback { get; set; }
}