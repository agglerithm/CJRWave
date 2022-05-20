using BufferUtilities;

namespace CJRWave
{
    public class WAVFile : RIFFFile
    {
        public WavDataChunk Data { get; }
        public WavFormatChunk Format { get; }

    }

    public class WavDataChunk : RiffChunk
    {
        public WavDataChunk()
        {
            SetChunkId(Constants.Data);
        }
        protected override BufferBuilder GetData()
        {
            throw new NotImplementedException();
        }
    }

    public class WavFormatChunk : RiffChunk
    {
        public WavFormatChunk()
        {
            SetChunkId(Constants.Format);
        }
        public uint FormatType { get; set; }
        public ushort NumberOfChannels { get; set; }
        public ushort SampleRate { get; set; } 
        public ushort BitsPerSample { get; set; }
        public uint BlockAlign { get; set; }
        public ushort AverageDataRate { get; set; }
        protected override BufferBuilder GetData()
        {
            throw new NotImplementedException();
        }
    }
}