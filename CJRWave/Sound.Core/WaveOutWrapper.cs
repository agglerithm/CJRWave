using BufferUtilities;
using Sound.Core.WaveInterop;

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
        Validate(Wave.waveOutOpen(
            out  _currentWaveOutHandle, 
            deviceId, format.WaveFormat, format.Callback, dwInstance, format.Flags));
    }

    public void PrepareHeader(WaveHeader header)
    {        
        MmResult result;
        try
        {
            result = Wave.waveOutPrepareHeader(_currentWaveOutHandle, 
                header, MarshalExtensions.SizeOf<WaveHeader>());
        }
        catch (Exception ex)
        {
            throw new WaveOutException(ex);
        }
        Validate(result);
    }
    public void UnprepareHeader(WaveHeader header)
    {        
        MmResult result;
        try
        {
            result = Wave.waveOutUnprepareHeader(_currentWaveOutHandle, 
                header, MarshalExtensions.SizeOf<WaveHeader>());
        }
        catch (Exception ex)
        {
            throw new WaveOutException(ex);
        }
        Validate(result);
    }
    public void Close()
    {
        MmResult result;
        try
        {
            result = Wave.waveOutClose(_currentWaveOutHandle);
        }
        catch (Exception ex)
        {
            throw new WaveOutException(ex);
        }
        Validate(result);
    }

    public void Write(WaveHeader req)
    {
        MmResult result;
        try
        {
            result = Wave.waveOutWrite(
                _currentWaveOutHandle, 
                req, MarshalExtensions.SizeOf<WaveHeader>());
        }
        catch (Exception ex)
        {
            throw new WaveOutException(ex);
        }
        Validate(result);
    }

    private static void Validate(MmResult result)
    {
        if (result != MmResult.NoError)
            throw new MmException(result);
    }

}

public class WaveOutException : Exception
{
    public WaveOutException(Exception innerException):base(innerException.ToString())
    { 
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