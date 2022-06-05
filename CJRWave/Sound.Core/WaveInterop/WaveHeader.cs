using System.Runtime.InteropServices;

namespace Sound.Core.WaveInterop;

[StructLayout(LayoutKind.Sequential)]
public sealed class WaveHeader
{
    public IntPtr dataBuffer;
    public int bufferLength;
    public int bytesRecorded;
    public IntPtr userData;
    public WaveHeaderFlags flags;
    public int loops;
    public IntPtr next;
    public IntPtr reserved;
}