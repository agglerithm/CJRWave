using Sound.Core;

namespace CJRWave;

public interface IWaveFilePlayer
{
    void Play(string path);
    short Volume { get; set; }
}

public class WaveFilePlayer : IWaveFilePlayer
{
    private IWavePlayerService _ws;
    private static short[] _dataPoints;
    private static long _currentDataPoint;

    public WaveFilePlayer(IWavePlayerService svc)
    {
        _ws = svc;
        Volume = 5;
    }
    public short Volume { get; set; }
    public void Play(string path)
    {
        var wf = LoadFile(path);
        PlayFile(wf);
    }
    private WAVFile LoadFile(string path)
    {
        var wavFile = new WAVFile();
        wavFile.Read(path);
        _dataPoints = wavFile.Data.DataPoints;
        return wavFile;
    }

    private void PlayFile(WAVFile file)
    {
        _ws.Configuration = file.GetConfiguration();
        _ws.UserFunction = TimeAction;
        _ws.Play();
    }

    private short TimeAction(double elapsedTime)
    {
        if (_dataPoints.Length != _currentDataPoint) return (short)(_dataPoints[_currentDataPoint++] * Volume);
        return 0;
    }
}