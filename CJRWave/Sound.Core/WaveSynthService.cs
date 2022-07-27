using System.Text;
using BufferUtilities;
using Sound.Core.Models;
using Sound.Core.WaveInterop;

namespace Sound.Core;

public interface IWaveService<T>
{
    void Start();
    void End();
    bool Playing();
    double GetTime();
    WaveSynthServiceConfiguration Configuration { get; set; }
}

public class WaveSynthService : IWaveService<double>
{
    public WaveSynthService(WaveSynthServiceConfiguration config)
    {
        Configure(config);
        _maxSample = short.MaxValue;
    }

    public bool Playing()
    {
        return _ready;
    }
    private void Configure(WaveSynthServiceConfiguration config)
    {
        _headers = new WaveHeader[config.BlockCount];
        _blockFree = config.BlockCount;
        _userFunction = config.UserFunction;
        _format = new FormatRequest
        {
            WaveFormat = new WaveFormat(config.SampleRate,config.Channels),
            Callback = callback,
            Flags = config.Flags
        };
        for (int i = 0; i < config.BlockCount; i++)
        {
            _headers[i] = new WaveHeader { bufferLength = (int)config.BlockSamples * 4 };
        }
        _log = config.Log;
        _configuration = config;
    }

    private uint _blockCurrent;
    private uint _blockFree;
    private Thread _thread;
    private readonly AutoResetEvent _resetEvent = new(false);
    private WaveHeader[] _headers;
    private FormatRequest _format;
    private double _globalTime;
    private bool _ready = true;
    private Func<double, double> _userFunction;
    WaveOutWrapper _wavStruct = new();
    private Action<object>? _log;
    private WaveSynthServiceConfiguration _configuration;
    private readonly double _maxSample;

    public void Start()
    {
        _thread = new Thread(WaveTask)
        {
            IsBackground = true
        };
        _thread.Start();
    }
    public void End()
    {
        _ready = false;
        _resetEvent.Close();
        _thread.Join();
    }

    private double clipSample(double sample, double max)
    {
        if (sample > 0.0)
            return Math.Min(sample, max);
        return Math.Max(sample, -max);
    }
    private void callback(IntPtr hwaveout, Wave.WaveMessage message, IntPtr dwinstance, 
        WaveHeader wavhdr, IntPtr dwreserved)
    {
        LogValue(_blockCurrent);
        if (message != Wave.WaveMessage.WaveOutDone) return;
        if (_resetEvent.SafeWaitHandle.IsClosed)
            return;
        _blockFree++;
        _resetEvent.Set();
    }
    private void WaveTask()
    {
        _wavStruct.Open(_format);
        _globalTime = 0.0;
        double timeStep = 1.0 / _configuration.SampleRate;
        while (_ready)
        {
            if (_blockFree <= 0)
                _resetEvent.WaitOne();
            _blockFree--;
            if(_headers[_blockCurrent].flags == WaveHeaderFlags.Prepared)
                _wavStruct.UnprepareHeader(_headers[_blockCurrent]);
            var dataBuff = GetDataBuff(timeStep);
            //_headers[_blockCurrent].userData = dataBuff.PtrFromByteArray();
            _headers[_blockCurrent].dataBuffer = dataBuff.PtrFromByteArray();
            _wavStruct.PrepareHeader(_headers[_blockCurrent]);
            _wavStruct.Write(_headers[_blockCurrent]);
            _blockCurrent++;
            _blockCurrent %= _configuration.BlockCount;
        }
    }

    private byte[] GetDataBuff(double timeStep)
    {
        var bb = new BufferBuilder();
        for (var i = 0; i < _configuration.BlockSamples; i++)
        {
            var initialValue = _userFunction(_globalTime);
            var clipped = clipSample(initialValue, 1);
            var nextSample = (int)(clipped * _maxSample);
            //LogValue(nextSample);
            bb.Append(nextSample);
            _globalTime += timeStep;
        }

        return bb.ToByteArray();
    }

    private void LogValue(object obj)
    {
        _log?.Invoke(obj);
    }

    public double GetTime()
    {
        return _globalTime;
    }

    public WaveSynthServiceConfiguration Configuration
    {
        get => _configuration;
        set => Configure(value);
    }
}