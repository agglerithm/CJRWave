using BufferUtilities;

namespace CJRWave
{
    public abstract class RiffChunk
    {
        public char[] ChunkId { get; } = new char[4];             /* Chunk ID marker */
        public uint ChunkSize { get; protected set; }           /* Size of the chunk data in bytes */
        public byte[] ChunkData { get {
                var buff = GetData();
                return buff.ToByteArray();
            } }

        protected abstract BufferBuilder GetData();           /* The chunk data */
        protected void SetChunkId(string id)
        {
            var val = id.ToCharArray();
            for(int i = 0; i < 4; i++)
                ChunkId[i] = val[i];
        }
    }
}