using System.Linq.Expressions;
using Sound.Core.WaveInterop;

namespace Sound.Core;

public class WavePlayerConfiguration 
{
    public WavePlayerConfiguration(WaveFormat format)
    {
        WaveFormat = format;
        BlockCount = 8;
        BlockSamples = 512;
        Flags = Wave.WaveInOutOpenFlags.CallbackEvent;
    }

    public WaveFormatEncoding Encoding
    {
        get { return WaveFormat.Encoding; }
    }
    public int BlockAlign
    {
        get { return WaveFormat.BlockAlign; }
    }
    public int BlockCount { get; set; }
    public int BlockSamples { get; set; }
    public int SampleRate
    {
        get { return WaveFormat.SampleRate; }
    }

    public WaveFormat WaveFormat { get; }
    public int Length { get; set; }

    public int Channels
    {
        get
        {
            return WaveFormat.Channels;
        }
    }
    public int BitsPerSample
    {
        get { return WaveFormat.BitsPerSample; }
    }

    public Wave.WaveCallback Callback
    {
        get;
        set;
    }
    
    public Wave.WaveInOutOpenFlags Flags { get; set; }

    public Action<object>? Log { get; set; }

    public Func<double,int> UserFunction { get; set; }
}
