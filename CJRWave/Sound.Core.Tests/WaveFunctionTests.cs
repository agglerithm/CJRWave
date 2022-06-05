using NUnit.Framework;

namespace Sound.Core.Tests;

[TestFixture]
public class WaveFunctionTests
{
    [Test]
    public void CanGetSineWave()
    {
        double j = 0;
        double timespan = .0001;
        int max = int.MaxValue;
        for (double i = 0; i < 1000; i++)
        {
            var smallTimeSpanWave = j.SineWave(.5, 440);
            j += timespan;
            Console.WriteLine($"{smallTimeSpanWave}:{smallTimeSpanWave * max}");
        }
    }
    
    [Test]
    public void CanGetSquareWave()
    {
        double j = 0;
        double timespan = .0001;
        int max = int.MaxValue;
        for (double i = 0; i < 1000; i++)
        {
            var smallTimeSpanWave = j.SquareWave(.5, 440);
            j += timespan;
            Console.WriteLine($"{smallTimeSpanWave}:{smallTimeSpanWave * max}");
        }
    }
}