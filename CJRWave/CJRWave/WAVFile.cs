using System.Drawing;
using System.Xml;
using BufferUtilities;
using Sound.Core;
using Sound.Core.WaveInterop;

namespace CJRWave
{
    public class WAVFile : RIFFFile
    {
        public WAVHeader Header { get; set; }
        public WavDataChunk Data { get; }
        public WavFormatChunk Format { get; }

        public WAVFile()
        {
            Header = new WAVHeader();
            Data = new WavDataChunk();
            Format = new WavFormatChunk();
        }
        protected override void PopulateObject(FileStream stream)
        {
            var riffBuff = new byte[4];
            var sizeBuff = new byte[4];
            var waveBuff = new byte[4];
            var fmtBuff = new byte[4];
            var pos = 0;
            stream.Read(riffBuff, 0, 4);
            pos += 4;
            if (riffBuff.ByteArrayToString() != Constants.Riff)
                throw new InvalidFileFormatException();
            stream.Read(sizeBuff, 0, 4);
            pos += 4;
            stream.Read(waveBuff, 0, 4);
            pos += 4;
            stream.Read(fmtBuff, 0, 4);
            pos += 4;
            if(waveBuff.ByteArrayToString() != Constants.Wave)
                throw new InvalidFileFormatException();
            if(fmtBuff.ByteArrayToString() != Constants.Format)
                throw new InvalidFileFormatException();
            this.Size = sizeBuff.ByteArrayToInt();
            var fmtSizeBuff = new byte[4];
            var fmtDataBuff = new byte[16];
            stream.Read(fmtSizeBuff, 0, 4);
            pos += 4;
            stream.Read(fmtDataBuff, 0, 16);
            pos += 16;
            Format.DataBuffer = fmtDataBuff;
            var dataLabelBuff = new byte[4];
            stream.Read(dataLabelBuff, 0, 4);
            if (dataLabelBuff.ByteArrayToString() != "data")
                throw new InvalidFileFormatException();
            var dataLength = (int)(stream.Length - pos);
            var data = new byte[dataLength];
            stream.Read(data, 0, dataLength);
            Data.DataPoints = data.ByteArrayToSpan<short>().ToArray();
        }

        public int Size { get; set; }

        public WavePlayerConfiguration GetConfiguration()
        {
            var val = new WavePlayerConfiguration(Format.GetWaveFormat())
            {
                Flags = Wave.WaveInOutOpenFlags.CallbackFunction,
                Length = Size
            };
            return val;
        }
    }

    public class WavDataChunk : RiffChunk
    {
        public BufferBuilder Data { get; }
        public short[] DataPoints { get; internal set; }

        public WavDataChunk()
        {
            SetChunkId(Constants.Data);
            Data = new BufferBuilder();
        }
        protected override BufferBuilder GetData()
        {
            return Data;
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
            var bb = new BufferBuilder();
            bb.Append(ChunkId);
            GetDataBuffer(bb);
            return bb;
        }

        private void GetDataBuffer(BufferBuilder bb)
        {
            if (DataBuffer != null)
                bb.Append(DataBuffer);
            else
            {
                bb.Append(FormatType);
                bb.Append(NumberOfChannels);
                bb.Append(SampleRate);
                bb.Append(BlockAlign);
                bb.Append(AverageDataRate);
                bb.Append(BitsPerSample);
            }
        }

        public byte[] DataBuffer { get; set; }

        public WaveFormat GetWaveFormat()
        {
            var bb = GetData();
            var reader = bb.GetReader();
            return new WaveFormat(reader);
        }
    }
}