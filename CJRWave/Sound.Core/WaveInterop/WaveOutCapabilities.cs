using System.Runtime.InteropServices;

namespace Sound.Core.WaveInterop;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public struct WaveOutCapabilities
{
    private short manufacturerId;
    private short productId;
    private int driverVersion;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    private string productName;
    private SupportedWaveFormat supportedFormats;
    private short channels;
    private short reserved;
    private WaveOutSupport support;
    private Guid manufacturerGuid;
    private Guid productGuid;
    private Guid nameGuid;
    private const int MaxProductNameLength = 32;

    public int Channels => (int) this.channels;

    public bool SupportsPlaybackRateControl => (this.support & WaveOutSupport.PlaybackRate) == WaveOutSupport.PlaybackRate;

    public string ProductName => this.productName;

    public bool SupportsWaveFormat(SupportedWaveFormat waveFormat) => (this.supportedFormats & waveFormat) == waveFormat;

    public Guid NameGuid => this.nameGuid;

    public Guid ProductGuid => this.productGuid;

    public Guid ManufacturerGuid => this.manufacturerGuid;
}