using BufferUtilities;
using Sound.Core.Models;
using Sound.Core.WaveInterop;

namespace Sound.Core;

public interface IWavePlayerService
{
    void Play();
    WavePlayerConfiguration Configuration { get; set; }
}

public class WavePlayerService : IWavePlayerService
{
    private void Configure(WavePlayerConfiguration config)
    {
        _configuration = config;
        _headers = new WaveHeader[BlockCount];
        _blockFree = BlockCount;
        _configuration.Callback = Callback;
        _configuration.Log = Log;
        for (int i = 0; i < BlockCount; i++)
        {
            _headers[i] = new WaveHeader { bufferLength = (int)BlockSamples * 4 };
        }
    }
    private int SampleRate => _configuration.SampleRate;

    private int BlockCount {
        get => _configuration.BlockCount;
        set => _configuration.BlockCount = value;
    }
    private int BlockSamples {
        get => _configuration.BlockSamples;
        set => _configuration.BlockSamples = value;
    }

    private int _currentByte;
    private int FileSize
    {
        get { return _configuration.Length; }
        set { _configuration.Length = value; }
    }
    private int _blockCurrent;

    private int _blockFree;
    
    private AutoResetEvent _resetEvent = new(false);
    private WaveHeader[] _headers;
    private FormatRequest Format
    {
        get
        {
            if (_format != null) return _format;
            return _format = new FormatRequest
            {
                WaveFormat = _configuration.WaveFormat,
                Callback = _configuration.Callback,
                Flags = _configuration.Flags
            };
        }
    }

    private double _globalTime;
    private bool _ready = true;
    WaveOutWrapper _wavStruct = new();
    private Action<object>? Log
    {
        get;
        set;
    }
    private WavePlayerConfiguration _configuration;
    private FormatRequest? _format;

    public WavePlayerService(Action<object>? log = null)
    {
        Log = log;
    }
    

    private void Callback(IntPtr hwaveout, Wave.WaveMessage message, IntPtr dwinstance, WaveHeader wavhdr, IntPtr dwreserved)
    {
        if (message != Wave.WaveMessage.WaveOutDone)
        {
            LogValue(message);
            return;
        }

        _blockFree++;
        _resetEvent.Set();
    }
    public void Play()
    {
        _wavStruct.Open(Format);
        _globalTime = 0.0;
        double timeStep = 1.0 / SampleRate;
        while (_ready)
        {
            var bb = new BufferBuilder();
            if (_blockFree == 0)
                _resetEvent.WaitOne();
            _blockFree--;
            if(_headers[_blockCurrent].flags == WaveHeaderFlags.Prepared)
                _wavStruct.UnprepareHeader(new WaveWriteRequest()
                    {Flags=WaveHeaderFlags.Prepared});
            for (var i = 0; i < BlockSamples; i++)
            {
                int nextSample = _configuration.UserFunction(_globalTime);
                LogValue(nextSample);
                bb.Append(nextSample);
                _globalTime += timeStep;
                _currentByte += 2;
            }

            _headers[_blockCurrent].userData = bb.ToByteArray().PtrFromByteArray();
            var request = new WaveWriteRequest()
            {
                Data = bb.ToByteArray(),
                Flags = WaveHeaderFlags.Prepared
            };
            _wavStruct.PrepareHeader(request);
            _wavStruct.Write(request);
            _blockCurrent++;
            _blockCurrent %= BlockCount;
            LogValue(_currentByte);
            if (_currentByte == FileSize)
                _ready = false;
        }
        _wavStruct.Close();
    }

    public WavePlayerConfiguration Configuration
    {
        get => _configuration;
        set => Configure(value);
    }
    private void LogValue(object obj)
    {
        Log?.Invoke(obj);
    }

}