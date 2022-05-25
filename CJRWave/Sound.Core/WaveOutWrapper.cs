using System.Runtime.InteropServices;
using Windows.Win32.Media.Audio;
using NAudio;
using NAudio.Wave;

namespace Sound.Core;

public class WaveOutWrapper
{
    private IntPtr _deviceId;
    private WaveFormat _lpFormat;
    private IntPtr _dwInstance;
    private WaveInterop.WaveInOutOpenFlags _flags;
    private HWAVEOUT? _currentWaveOutHandle;
    private WaveHeader _lpWaveOutHdr;


    public void Open()
    {
        WaveInterop.waveOutOpen(out var waveOutPtr, _deviceId, _lpFormat, _callback, _dwInstance, _flags);
        _currentWaveOutHandle = (HWAVEOUT?)Marshal.PtrToStructure(waveOutPtr, typeof(HWAVEOUT));
    }

    public MmResult Close()
    {
        IntPtr waveOutPtr = default;
        Marshal.StructureToPtr(_currentWaveOutHandle, waveOutPtr, false);
        return WaveInterop.waveOutClose(waveOutPtr);
    }

    public MmResult Write(WaveWriteRequest req)
    {
        var hdr = new WaveHeader()
        {
            bufferLength = req.Data.Length,
            flags = req.GetFlags()
        };
        return WaveInterop.waveOutWrite(_currentWaveOutHandle.Value, hdr, 0);
    }

    private void _callback(IntPtr hwaveout, WaveInterop.WaveMessage message, IntPtr dwinstance, WaveHeader wavhdr, IntPtr dwreserved)
    {
        throw new NotImplementedException();
    }
}

public class WaveWriteRequest
{
    public byte[] Data { get; set; }
    public bool Done { get; set; }
    public bool Prepared { get; set; }
    public bool BeginLoop { get; set; }
    public bool EndLoop { get; set; }

    public WaveHeaderFlags GetFlags()
    {
        throw new NotImplementedException();
    }
}