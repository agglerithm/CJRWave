namespace Sound.Core.Models;

public class SoundSpectrum
{
    private const int MAX_FREQUENCIES = 10;
    private int _count;
    private int _lastCount;
    private IEnumerable<double> _currentFrequencies;
    private double[] _frequencies = new double[MAX_FREQUENCIES];

    public SoundSpectrum(double initial = 0)
    {
        Add(initial);
    }
    public void Add(double freq)
    {
        if (freq <= 0.0)
            return;
        _frequencies[_count] = freq;
        _count++;
    }

    public double GetImpulse(double time, double amplitude)
    {
        if (_count > _lastCount)
        {
            _currentFrequencies = _frequencies.Take(_count);
            _lastCount = _count;
        }
        return _currentFrequencies.Sum(f => time.GetSine(f)) * amplitude;
    }
}