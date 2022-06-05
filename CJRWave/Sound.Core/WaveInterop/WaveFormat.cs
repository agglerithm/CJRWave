using System.Runtime.InteropServices;
using WaveFormatEncoding = Sound.Core.WaveInterop.WaveFormatEncoding;
using WaveFormatExtensible = Sound.Core.WaveInterop.WaveFormatExtensible;
using WaveFormatExtraData = Sound.Core.WaveInterop.WaveFormatExtraData;

namespace Sound.Core.WaveInterop;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public class WaveFormat
{
    protected WaveFormatEncoding waveFormatTag;
    protected short channels;
    protected int sampleRate;
    protected int averageBytesPerSecond;
    protected short blockAlign;
    protected short bitsPerSample;
    protected short extraSize;

    public WaveFormat()
        : this(44100, 16, 2)
    {
    }

    public WaveFormat(int sampleRate, int channels)
        : this(sampleRate, 16, channels)
    {
    }

    public int ConvertLatencyToByteSize(int milliseconds)
    {
        int byteSize = (int) ((double) this.AverageBytesPerSecond / 1000.0 * (double) milliseconds);
        if (byteSize % this.BlockAlign != 0)
            byteSize = byteSize + this.BlockAlign - byteSize % this.BlockAlign;
        return byteSize;
    }

    public static WaveFormat CreateCustomFormat(
        WaveFormatEncoding tag,
        int sampleRate,
        int channels,
        int averageBytesPerSecond,
        int blockAlign,
        int bitsPerSample)
    {
        return new WaveFormat()
        {
            waveFormatTag = tag,
            channels = (short) channels,
            sampleRate = sampleRate,
            averageBytesPerSecond = averageBytesPerSecond,
            blockAlign = (short) blockAlign,
            bitsPerSample = (short) bitsPerSample,
            extraSize = 0
        };
    }

    public static WaveFormat CreateALawFormat(int sampleRate, int channels) => WaveFormat.CreateCustomFormat(WaveFormatEncoding.ALaw, sampleRate, channels, sampleRate * channels, channels, 8);

    public static WaveFormat CreateMuLawFormat(int sampleRate, int channels) => WaveFormat.CreateCustomFormat(WaveFormatEncoding.MuLaw, sampleRate, channels, sampleRate * channels, channels, 8);

    public WaveFormat(int rate, int bits, int channels)
    {
        if (channels < 1)
            throw new ArgumentOutOfRangeException(nameof (channels), "Channels must be 1 or greater");
        this.waveFormatTag = WaveFormatEncoding.Pcm;
        this.channels = (short) channels;
        this.sampleRate = rate;
        this.bitsPerSample = (short) bits;
        this.extraSize = (short) 0;
        this.blockAlign = (short) (channels * (bits / 8));
        this.averageBytesPerSecond = this.sampleRate * (int) this.blockAlign;
    }

    public static WaveFormat CreateIeeeFloatWaveFormat(int sampleRate, int channels)
    {
        WaveFormat ieeeFloatWaveFormat = new WaveFormat()
        {
            waveFormatTag = WaveFormatEncoding.IeeeFloat,
            channels = (short) channels,
            bitsPerSample = 32,
            sampleRate = sampleRate,
            blockAlign = (short) (4 * channels)
        };
        ieeeFloatWaveFormat.averageBytesPerSecond = sampleRate * (int) ieeeFloatWaveFormat.blockAlign;
        ieeeFloatWaveFormat.extraSize = (short) 0;
        return ieeeFloatWaveFormat;
    }

    public static WaveFormat MarshalFromPtr(IntPtr pointer)
    {
        WaveFormat structure = Marshal.PtrToStructure<WaveFormat>(pointer);
        switch (structure.Encoding)
        {
            case WaveFormatEncoding.Pcm:
                structure.extraSize = (short) 0;
                break;
            case WaveFormatEncoding.Adpcm:
                structure = (WaveFormat) Marshal.PtrToStructure<AdpcmWaveFormat>(pointer);
                break;
            case WaveFormatEncoding.Gsm610:
                structure = (WaveFormat) Marshal.PtrToStructure<Gsm610WaveFormat>(pointer);
                break;
            case WaveFormatEncoding.Extensible:
                structure = (WaveFormat) Marshal.PtrToStructure<WaveFormatExtensible>(pointer);
                break;
            default:
                if (structure.ExtraSize > 0)
                {
                    structure = (WaveFormat) Marshal.PtrToStructure<WaveFormatExtraData>(pointer);
                    break;
                }
                break;
        }
        return structure;
    }

    public static IntPtr MarshalToPtr(WaveFormat format)
    {
        IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<WaveFormat>(format));
        Marshal.StructureToPtr<WaveFormat>(format, ptr, false);
        return ptr;
    }

    public static WaveFormat FromFormatChunk(BinaryReader br, int formatChunkLength)
    {
        WaveFormatExtraData waveFormatExtraData = new WaveFormatExtraData();
        waveFormatExtraData.ReadWaveFormat(br, formatChunkLength);
        waveFormatExtraData.ReadExtraData(br);
        return (WaveFormat) waveFormatExtraData;
    }

    private void ReadWaveFormat(BinaryReader br, int formatChunkLength)
    {
        if (formatChunkLength < 16)
            throw new InvalidDataException("Invalid WaveFormat Structure");
        this.waveFormatTag = (WaveFormatEncoding) br.ReadUInt16();
        this.channels = br.ReadInt16();
        this.sampleRate = br.ReadInt32();
        this.averageBytesPerSecond = br.ReadInt32();
        this.blockAlign = br.ReadInt16();
        this.bitsPerSample = br.ReadInt16();
        if (formatChunkLength <= 16)
            return;
        this.extraSize = br.ReadInt16();
        if ((int) this.extraSize == formatChunkLength - 18)
            return;
        this.extraSize = (short) (formatChunkLength - 18);
    }

    public WaveFormat(BinaryReader br)
    {
        int formatChunkLength = br.ReadInt32();
        this.ReadWaveFormat(br, formatChunkLength);
    }

    public override string ToString()
    {
        switch (this.waveFormatTag)
        {
            case WaveFormatEncoding.Pcm:
            case WaveFormatEncoding.Extensible:
                return string.Format("{0} bit PCM: {1}Hz {2} channels", (object) this.bitsPerSample, (object) this.sampleRate, (object) this.channels);
            case WaveFormatEncoding.IeeeFloat:
                return string.Format("{0} bit IEEFloat: {1}Hz {2} channels", (object) this.bitsPerSample, (object) this.sampleRate, (object) this.channels);
            default:
                return this.waveFormatTag.ToString();
        }
    }

    public override bool Equals(object obj) => obj is WaveFormat waveFormat && this.waveFormatTag == waveFormat.waveFormatTag && (int) this.channels == (int) waveFormat.channels && this.sampleRate == waveFormat.sampleRate && this.averageBytesPerSecond == waveFormat.averageBytesPerSecond && (int) this.blockAlign == (int) waveFormat.blockAlign && (int) this.bitsPerSample == (int) waveFormat.bitsPerSample;

    public override int GetHashCode() => (int) (this.waveFormatTag ^ (WaveFormatEncoding) this.channels ^ (WaveFormatEncoding) this.sampleRate ^ (WaveFormatEncoding) this.averageBytesPerSecond ^ (WaveFormatEncoding) this.blockAlign ^ (WaveFormatEncoding) this.bitsPerSample);

    public WaveFormatEncoding Encoding => this.waveFormatTag;

    public virtual void Serialize(BinaryWriter writer)
    {
        writer.Write(18 + (int) this.extraSize);
        writer.Write((short) this.Encoding);
        writer.Write((short) this.Channels);
        writer.Write(this.SampleRate);
        writer.Write(this.AverageBytesPerSecond);
        writer.Write((short) this.BlockAlign);
        writer.Write((short) this.BitsPerSample);
        writer.Write(this.extraSize);
    }

    public int Channels => (int) this.channels;

    public int SampleRate => this.sampleRate;

    public int AverageBytesPerSecond => this.averageBytesPerSecond;

    public virtual int BlockAlign => (int) this.blockAlign;

    public int BitsPerSample => (int) this.bitsPerSample;

    public int ExtraSize => (int) this.extraSize;
}