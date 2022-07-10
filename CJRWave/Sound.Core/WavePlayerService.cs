using System.Diagnostics;
using BufferUtilities;
using Sound.Core.Models;
using Sound.Core.WaveInterop;

namespace Sound.Core;

public interface IWavePlayerService
{
    void Play();
    WavePlayerConfiguration Configuration { get; set; }
    Func<double, short> UserFunction { get; set; }
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
        for (var i = 0; i < BlockCount; i++)
        {
            _headers[i] = new WaveHeader { bufferLength = BlockSamples * 2 };
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
    public Func<double, short> UserFunction
    {
        get => _configuration.UserFunction;
        set => _configuration.UserFunction = value;
    }
    private int _currentByte;
    private int FileSize => _configuration.Length;
    private int _blockCurrent;

    private int _blockFree;
    
    private readonly ManualResetEventSlim _resetEvent = new(false);
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
            return;
        }

        _blockFree++;
        LogValue($"Setting:{_blockFree}");
        _resetEvent.Set();
    }
    public void Play()
    {
        _wavStruct.Open(Format);
        _globalTime = 0.0;
        var timeStep = 1.0 / SampleRate;
        while (_ready)
        {
            Debug.Assert(_blockFree >= 0, "Number of blocks should not be less than zero");
            if (_blockFree == 0)
                _resetEvent.Wait();
            _blockFree--;
            if(_headers[_blockCurrent].flags == WaveHeaderFlags.Prepared)
                _wavStruct.UnprepareHeader(new WaveWriteRequest()
                    {Flags=WaveHeaderFlags.Prepared});
            var dataBuff = GetBlockData(timeStep);

            _headers[_blockCurrent].userData = dataBuff.PtrFromByteArray();
            var request = new WaveWriteRequest()
            {
                Data = dataBuff,
                Flags = WaveHeaderFlags.Prepared
            };
            _wavStruct.PrepareHeader(request);
            _wavStruct.Write(request);
            _blockCurrent++;
            _blockCurrent %= BlockCount;
            if (_currentByte == FileSize)
                _ready = false;
        }
        _wavStruct.Close();
        _resetEvent.Reset();
    }

    private byte[] GetBlockData( double timeStep)
    {
        var bb = new BufferBuilder();
        for (var i = 0; i < BlockSamples; i++)
        {
            var nextSample = _configuration.UserFunction(_globalTime);
            bb.Append(nextSample);
            _globalTime += timeStep;
            _currentByte += 2;
        }

        return bb.ToByteArray();
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