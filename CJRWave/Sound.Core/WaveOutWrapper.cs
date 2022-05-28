using System.Runtime.InteropServices;
using Windows.Win32.Media.Audio;
using BufferUtilities;
using NAudio;
using NAudio.Wave;

namespace Sound.Core;

public class WaveOutWrapper
{
    private IntPtr _deviceId;
    private WaveFormat _lpFormat;
    private IntPtr _dwInstance;
    private WaveInterop.WaveInOutOpenFlags _flags;
    private IntPtr _currentWaveOutHandle;
    private WaveHeader _lpWaveOutHdr;
    private  List<WaveOutCapabilities> _devices = new List<WaveOutCapabilities>();

    public WaveOutWrapper()
    {
        _deviceId = new IntPtr();
        _dwInstance = new IntPtr();
        var deviceCount = WaveInterop.waveOutGetNumDevs();
        for (int i = 0; i < deviceCount; i++)
        {
            var wavCap = default(WaveOutCapabilities);
            WaveInterop.waveOutGetDevCaps(_deviceId, out wavCap, typeof(WaveOutCapabilities).SizeOf());
        }
    }
    public void Open()
    {
        WaveInterop.waveOutOpen(out var _currentWaveOutHandle, _deviceId, _lpFormat, _callback, _dwInstance, _flags);
    }

    public MmResult Close()
    {
        return WaveInterop.waveOutClose(_currentWaveOutHandle);
    }

    public void Write(WaveWriteRequest req)
    {
        var hdr = new WaveHeader()
        {
            bufferLength = req.Data.Length,
            flags = req.GetFlags()
        };
        var result = WaveInterop.waveOutWrite(_currentWaveOutHandle.DereferenceStruct<HWAVEOUT>(), hdr, 0);
        if (result != MmResult.NoError)
            throw new MmException(result);
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

public class MmException:Exception
{
    public MmException(MmResult result)
    {
        MultiMediaResult = result;
    }

    public MmResult MultiMediaResult { get; }
}