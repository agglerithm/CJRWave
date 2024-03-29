﻿using BufferUtilities;
namespace CJRWave
{
    public abstract class RIFFFile : BinaryFile
    {
        protected RiffFileChunk _chunk = new RiffFileChunk();
        protected override void ReadData(Func<FileStream> getStream)
        {
            using (var stream = getStream())
            {
                PopulateObject(stream);
            }
        }

        protected abstract void PopulateObject(FileStream stream);
    }

    public class RiffFileChunk : RiffChunk
    {
        private BufferBuilder _bb = new BufferBuilder();

        public RiffFileChunk()
        {
            SetChunkId(Constants.Riff);
            AppendData(Constants.Wave.StringToByteArray());
        }

        public void AppendData(byte[] data)
        {
            _bb.Append(data);
            ChunkSize = (uint)(_bb.Length + 8);
        }
        protected override BufferBuilder GetData()
        {
            var bb = new BufferBuilder(ChunkId.Select(c => (byte)c).ToArray());
            bb.Append(ChunkSize);
            bb.Append(ChunkData);
            return bb;
        }
    }

}