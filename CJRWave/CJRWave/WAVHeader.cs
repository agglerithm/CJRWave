namespace CJRWave
{
    public class WAVHeader
    {
        public uint FileSize { get; set; }
        public ushort FormatType { get; set; }
        public int FormatDataLength { get; set; }
        public ushort NumberOfChannels { get; set; } 
        public uint SampleRate { get; set; }
        public ushort BitsPerSample { get; set; }
        public uint DataSize { get; set; }
    }
}