using System.Runtime.InteropServices;

namespace Sound.Core.WaveInterop;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public class Gsm610WaveFormat : WaveFormat
{
    private readonly short samplesPerBlock;

    public Gsm610WaveFormat()
    {
        this.waveFormatTag = WaveFormatEncoding.Gsm610;
        this.channels = (short) 1;
        this.averageBytesPerSecond = 1625;
        this.bitsPerSample = (short) 0;
        this.blockAlign = (short) 65;
        this.sampleRate = 8000;
        this.extraSize = (short) 2;
        this.samplesPerBlock = (short) 320;
    }

    public short SamplesPerBlock => this.samplesPerBlock;

    public override void Serialize(BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write(this.samplesPerBlock);
    }
}