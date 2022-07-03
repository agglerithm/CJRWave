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
    WaveServiceConfiguration<T> Configuration { get; set; }
}

public class WaveSynthService : IWaveService<double>
{
    public WaveSynthService(WaveServiceConfiguration<double> config)
    {
        Configure(config);
    }

    public bool Playing()
    {
        return _ready;
    }
    private void Configure(WaveServiceConfiguration<double> config)
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
    private AutoResetEvent _resetEvent = new(false);
    private static Mutex _mutex = new();
    private WaveHeader[] _headers;
    private FormatRequest _format;
    private double _globalTime;
    private bool _ready = true;
    private Func<double, double> _userFunction;
    WaveOutWrapper _wavStruct = new();
    private Action<object>? _log;
    private WaveServiceConfiguration<double> _configuration;

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
        if (message != Wave.WaveMessage.WaveOutDone) return;

        _blockFree++;
        _resetEvent.Set();
    }
    private void WaveTask()
    {
        _wavStruct.Open(_format);
        _globalTime = 0.0;
        double timeStep = 1.0 / _configuration.SampleRate;
        short maxSample = short.MaxValue;
        while (_ready)
        {
            var bb = new BufferBuilder();
            if (_blockFree <= 0)
                _resetEvent.WaitOne();
            _blockFree--;
            if(_headers[_blockCurrent].flags == WaveHeaderFlags.Prepared)
                _wavStruct.UnprepareHeader(new WaveWriteRequest()
                    {Flags=WaveHeaderFlags.Prepared});
            int nextSample;
            for (var i = 0; i < _configuration.BlockSamples; i++)
            {
                var initialValue = _userFunction(_globalTime);
                var clipped = clipSample(initialValue, 1);
                nextSample = (int)(clipped * (float)maxSample);
                LogValue(nextSample);
                bb.Append(nextSample);
                _globalTime += timeStep;
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
            _blockCurrent %= _configuration.BlockCount;
        }
    }

    private void LogValue(object obj)
    {
        _log?.Invoke(obj);
    }

    public double GetTime()
    {
        return _globalTime;
    }

    public WaveServiceConfiguration<double> Configuration
    {
        get { return _configuration; } 
        set{Configure(value);}
    }
}