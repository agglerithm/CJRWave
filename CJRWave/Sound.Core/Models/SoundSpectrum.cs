namespace Sound.Core.Models;

public class SoundSpectrum
{
    private const int MAX_FREQUENCIES = 10;
    private int _count;
    private int _lastCount;
    private IEnumerable<Tuple<double,double>> _currentFrequencies;
    private readonly Tuple<double,double>[] _frequencies = new Tuple<double,double>[MAX_FREQUENCIES];

    public SoundSpectrum(double initialFreq = 0, double initialAmp = 0)
    {
        if (initialFreq == 0) return;
        Add(initialFreq, initialAmp);
    }
    public void Add(double freq, double amp = 1)
    {
        if (freq == 0.0)
            return;
        _frequencies[_count] = new Tuple<double, double>(freq, amp);
        _count++;
    }

    public double GetImpulse(double time, double amplitude)
    {
        if (_count > _lastCount)
        {
            _currentFrequencies = _frequencies.Take(_count);
            _lastCount = _count;
        }
        return _currentFrequencies.Sum(f => time.SineWave(f.Item2,f.Item1)) * amplitude + amplitude;
    }
}