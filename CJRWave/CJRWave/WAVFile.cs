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
            var fmtTypeBuff = new byte[2];
            var numChannelsBuff = new byte[2];
            var sampleRateBuff = new byte[4];
            var avgDataRateBuff = new byte[4];
            var blockAlignBuff = new byte[2];
            var bitsPerSampleBuff = new byte[2];
            stream.Read(fmtSizeBuff, 0, 4);
            pos += 4;
            stream.Read(fmtTypeBuff, 0, 2);
            pos += 2;
            stream.Read(numChannelsBuff, 0, 2);
            pos += 2;
            stream.Read(sampleRateBuff, 0, 4);
            pos += 4;
            stream.Read(avgDataRateBuff, 0, 4);
            pos += 4;
            stream.Read(blockAlignBuff, 0, 2);
            pos += 2;
            stream.Read(bitsPerSampleBuff, 0, 2);
            pos += 2;
            Format.FormatSize = fmtSizeBuff.ByteArrayToUint();
            Format.FormatType = fmtTypeBuff.ByteArrayToUshort();
            Format.NumberOfChannels = numChannelsBuff.ByteArrayToUshort();
            Format.SampleRate = sampleRateBuff.ByteArrayToUshort();
            Format.BlockAlign = blockAlignBuff.ByteArrayToUshort();
            Format.AverageDataRate = avgDataRateBuff.ByteArrayToUint();
            Format.BitsPerSample = bitsPerSampleBuff.ByteArrayToUshort();
            var dataLabelBuff = new byte[4];
            stream.Read(dataLabelBuff, 0, 4);
            pos += 4;
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
        public ushort BlockAlign { get; set; }
        public uint AverageDataRate { get; set; }
        public uint FormatSize { get; set; }

        protected override BufferBuilder GetData()
        {
            var bb = new BufferBuilder();
            bb.Append(FormatSize);
                bb.Append(FormatType);
                bb.Append(NumberOfChannels);
                bb.Append(SampleRate);
                bb.Append(AverageDataRate);
                bb.Append(BlockAlign);
                bb.Append(BitsPerSample);
            return bb;
        }
        
        public WaveFormat GetWaveFormat()
        {
            return new WaveFormat(FormatSize,
                FormatType,
                NumberOfChannels,
                SampleRate,
                AverageDataRate,
                BlockAlign,
                BitsPerSample);
        }
    }
}