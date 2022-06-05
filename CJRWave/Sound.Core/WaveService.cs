using BufferUtilities;
using Sound.Core.Models;
using Sound.Core.WaveInterop;

namespace Sound.Core;

public class WaveService
{
    public WaveService(WaveServiceConfiguration config)
    {
        _headers = new WaveHeader[config.BlockCount];
        _blocks = new BufferBuilder();
        _channels = (uint)config.Channels;
        _sampleRate = (uint)config.SampleRate;
        _blockCount = (uint)config.BlockCount;
        _blockSamples = (uint)config.BlockSamples;
        _blockFree = _blockCount;
        _userFunction = config.UserFunction;
        _format = new FormatRequest
        {
            Channels = config.Channels, 
            SampleRate = config.SampleRate,
            Callback = callback,
            Flags = config.Flags
        };
        for (int i = 0; i < _blockCount; i++)
        {
            _headers[i] = new WaveHeader { bufferLength = (int)_blockSamples * 4 };
        }

        _log = config.Log;
    }
    private uint _sampleRate;
    private uint _channels;
    private readonly uint _blockCount;
    private uint _blockSamples;
    private uint _blockCurrent;
    private uint _blockFree;
    private Thread _thread;
    private AutoResetEvent _resetEvent = new(false);
    private WaveHeader[] _headers;
    private BufferBuilder _blocks;
    private readonly FormatRequest _format;
    private double _globalTime;
    private bool _ready = true;
    private Func<double, double> _userFunction;
    WaveOutWrapper _wavStruct = new();
    private readonly Action<object> _log;

    public void Start()
    {
        _thread = new Thread(new ThreadStart(WaveTask))
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
    private void callback(IntPtr hwaveout, Wave.WaveMessage message, IntPtr dwinstance, WaveHeader wavhdr, IntPtr dwreserved)
    {
        if (message != Wave.WaveMessage.WaveOutDone) return;

        _blockFree++;
        _resetEvent.Set();
    }
    private void WaveTask()
    {
        _wavStruct.Open(_format);
        _globalTime = 0.0;
        double timeStep = 1.0 / _sampleRate;
        short maxSample = short.MaxValue;
            while (_ready)
            {
                var bb = new BufferBuilder();
                if (_blockFree == 0)
                    _resetEvent.WaitOne();
                _blockFree--;
                if(_headers[_blockCurrent].flags == WaveHeaderFlags.Prepared)
                    _wavStruct.UnprepareHeader(new WaveWriteRequest()
                        {Flags=WaveHeaderFlags.Prepared});
                int nextSample;
                for (var i = 0; i < _blockSamples; i++)
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
                _blockCurrent %= _blockCount;
            }
    }

    private void LogValue(object obj)
    {
        if (_log == null)
            return;
        _log(obj);
    }

    public double GetTime()
    {
        return _globalTime;
    }
}