using System.Runtime.InteropServices;

namespace Sound.Core.WaveInterop;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public class AdpcmWaveFormat : WaveFormat
{
    private short samplesPerBlock;
    private short numCoeff;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
    private short[] coefficients;

    private AdpcmWaveFormat()
        : this(8000, 1)
    {
    }

    public int SamplesPerBlock => (int) this.samplesPerBlock;

    public int NumCoefficients => (int) this.numCoeff;

    public short[] Coefficients => this.coefficients;

    public AdpcmWaveFormat(int sampleRate, int channels)
        : base(sampleRate, 0, channels)
    {
        this.waveFormatTag = WaveFormatEncoding.Adpcm;
        this.extraSize = (short) 32;
        switch (this.sampleRate)
        {
            case 8000:
            case 11025:
                this.blockAlign = (short) 256;
                break;
            case 22050:
                this.blockAlign = (short) 512;
                break;
            default:
                this.blockAlign = (short) 1024;
                break;
        }
        this.bitsPerSample = (short) 4;
        this.samplesPerBlock = (short) (((int) this.blockAlign - 7 * channels) * 8 / ((int) this.bitsPerSample * channels) + 2);
        this.averageBytesPerSecond = this.SampleRate * (int) this.blockAlign / (int) this.samplesPerBlock;
        this.numCoeff = (short) 7;
        this.coefficients = new short[14]
        {
            (short) 256,
            (short) 0,
            (short) 512,
            (short) -256,
            (short) 0,
            (short) 0,
            (short) 192,
            (short) 64,
            (short) 240,
            (short) 0,
            (short) 460,
            (short) -208,
            (short) 392,
            (short) -232
        };
    }

    public override void Serialize(BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write(this.samplesPerBlock);
        writer.Write(this.numCoeff);
        foreach (short coefficient in this.coefficients)
            writer.Write(coefficient);
    }

    public override string ToString() => string.Format("Microsoft ADPCM {0} Hz {1} channels {2} bits per sample {3} samples per block", (object) this.SampleRate, (object) this.channels, (object) this.bitsPerSample, (object) this.samplesPerBlock);
}