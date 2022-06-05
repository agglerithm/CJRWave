using System.Runtime.InteropServices;

namespace Sound.Core.WaveInterop;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public class WaveFormatExtraData : WaveFormat
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
    private byte[] extraData = new byte[100];

    public byte[] ExtraData => this.extraData;

    internal WaveFormatExtraData()
    {
    }

    public WaveFormatExtraData(BinaryReader reader)
        : base(reader)
    {
        this.ReadExtraData(reader);
    }

    internal void ReadExtraData(BinaryReader reader)
    {
        if (this.extraSize <= (short) 0)
            return;
        reader.Read(this.extraData, 0, (int) this.extraSize);
    }

    public override void Serialize(BinaryWriter writer)
    {
        base.Serialize(writer);
        if (this.extraSize <= (short) 0)
            return;
        writer.Write(this.extraData, 0, (int) this.extraSize);
    }
}