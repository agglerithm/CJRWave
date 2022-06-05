using BufferUtilities;
using Sound.Core.WaveInterop;

namespace Sound.Core.Models;

public class WaveWriteRequest
{
    public byte[] Data { get; set; }

    public WaveHeaderFlags Flags { get; set; }

    public WaveHeader GetHeader()
    {
        if (Data == null)
            throw new NullReferenceException("Data must be initialized first");
        return new WaveHeader()
        {
            bufferLength = Data.Length,
            flags = Flags,
            dataBuffer = Data.PtrFromByteArray()
            
        };
    }

}