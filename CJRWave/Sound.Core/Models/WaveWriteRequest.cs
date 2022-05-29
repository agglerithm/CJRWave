using BufferUtilities;
using NAudio.Wave;

namespace Sound.Core.Models;

public class WaveWriteRequest
{
    public byte[] Data { get; set; }
    public bool Done { get; set; }
    public bool Prepared { get; set; }
    public bool BeginLoop { get; set; }
    public bool EndLoop { get; set; }

    public WaveHeaderFlags Flags { get; set; }

    public WaveHeader GetHeader()
    {
        return new WaveHeader()
        {
            bufferLength = Data.Length,
            flags = Flags,
            dataBuffer = Data.PtrFromByteArray()
            
        };
    }

}