using NAudio.Wave;

namespace Sound.Core;

public class WaveService
{
    public WaveService(WaveServiceConfiguration config)
    {
        _headers = new WaveHeader[config.BlockCount];
        _blocks = new ushort[config.BlockCount * config.BlockSamples];
        _channels = (uint)config.Channels;
        _sampleRate = (uint)config.SampleRate;
        _blockCount = (uint)config.BlockCount;
        _blockSamples = (uint)config.BlockSamples;
        _blockFree = _blockCount;
        _format = new FormatRequest
        {
            Channels = config.Channels, 
            SampleRate = config.SampleRate,
            Callback = callback,
            Flags = config.Flags
        };
    }
    private uint _sampleRate;
    private uint _channels;
    private readonly uint _blockCount;
    private uint _blockSamples;
    private uint _blockCurrent;
    private uint _blockFree;
    private Thread _thread;
    private readonly Mutex _mutex = new();
    private WaveHeader[] _headers;
    private ushort[] _blocks;
    private readonly FormatRequest _format;
    WaveOutWrapper _wavStruct = new();
    
    public void Start()
    {
        _wavStruct.Open(_format);
        _thread = new Thread(new ThreadStart(WaveTask))
        {
            IsBackground = true
        };
        _thread.Start();
    }
    public void End()
    {
        _thread.Join();
    }
    private void callback(IntPtr hwaveout, WaveInterop.WaveMessage message, IntPtr dwinstance, WaveHeader wavhdr, IntPtr dwreserved)
    {		
        if (message != WaveInterop.WaveMessage.WaveOutDone) return;

        _blockFree++;
        _mutex.ReleaseMutex();
    }
    private void WaveTask()
    {
        bool ready = true;
        while (ready)
        {
            if (_blockFree == 0)
                _mutex.WaitOne();
            _blockFree--;
        }


    }
}

public class WaveServiceConfiguration
{
    public WaveServiceConfiguration()
    {
        BlockCount = 8;
        SampleRate = 44100;
        BlockSamples = 512;
        Channels = 2;
    }
    public int BlockCount { get; set; }
    public int SampleRate { get; set; }
    public int Channels { get; set; }
    public int BlockSamples { get; set; }
    public WaveInterop.WaveInOutOpenFlags Flags { get; set; }
}