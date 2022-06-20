using Sound.Core.WaveInterop;

namespace Sound.Core;

public class FormatRequest
{
    public int Channels => WaveFormat.Channels;

    public int SampleRate => WaveFormat.SampleRate;

    public int BitsPerSample => WaveFormat.BitsPerSample;
    
    public Wave.WaveInOutOpenFlags Flags { get; set; }
    
    public Wave.WaveCallback Callback { get; set; }
    public WaveFormat WaveFormat { get; set; }
}