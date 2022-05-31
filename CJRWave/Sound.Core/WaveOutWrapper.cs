using System.Runtime.InteropServices;
using BufferUtilities;
using NAudio;
using NAudio.Wave;
using Sound.Core.Models;

namespace Sound.Core;

public struct WaveOutWrapper
{
    private IntPtr _currentWaveOutHandle;

    public WaveOutWrapper()
    {
        _currentWaveOutHandle = MarshalExtensions.Allocate(MarshalExtensions.SizeOf<int>());
    }
    public void Open(FormatRequest format)
    {
        var deviceId = (IntPtr)0;
        var dwInstance = this.PtrFromStruct();
        Validate(WaveInterop.waveOutOpen(
            out  _currentWaveOutHandle, 
            deviceId, format.BuildWaveFormat(), format.Callback, dwInstance, format.Flags));
    }

    public void PrepareHeader(WaveWriteRequest req)
    {
        var result = WaveInterop.waveOutPrepareHeader(_currentWaveOutHandle, 
            req.GetHeader(), MarshalExtensions.SizeOf<WaveHeader>());
        Validate(result);
    }
    public void UnprepareHeader(WaveWriteRequest req)
    {
        var result = WaveInterop.waveOutUnprepareHeader(_currentWaveOutHandle, 
            req.GetHeader(), MarshalExtensions.SizeOf<WaveHeader>());
        Validate(result);
    }
    public void Close()
    {
        var result = WaveInterop.waveOutClose(_currentWaveOutHandle);
        Validate(result);
    }

    public void Write(WaveWriteRequest req)
    {
        var result = WaveInterop.waveOutWrite(
            _currentWaveOutHandle, 
            req.GetHeader(), MarshalExtensions.SizeOf<WaveHeader>());
        Validate(result);
    }

    private static void Validate(MmResult result)
    {
        if (result != MmResult.NoError)
            throw new MmException(result);
    }

}

public class MmException:Exception
{
    public MmException(MmResult result):base(result.ToString())
    {
        MultiMediaResult = result;
    }

    public MmResult MultiMediaResult { get; }
}