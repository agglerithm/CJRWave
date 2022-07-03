
namespace Sound.Core
{
    public static class SoundExtensions
    {
        private const double PI = 3.14159;
        public static double GetSine(this double timeElapsed, double frequency)
        {
            return Math.Sin(frequency.AngularVelocity() * timeElapsed);
        }
        public static double SineWave(this double timeElapsed, double amplitude, double frequency)
        {
            return timeElapsed.GetSine(frequency) * amplitude;
        }

        public static double SquareWave(this double timeElapsed, double amplitude, double frequency)
        {
            var val =  timeElapsed.GetSine(frequency);
            if (val < 0)
                return -amplitude;
            return amplitude;
        }
        public static double TriangleWave(this double timeElapsed, double amplitude, double frequency)
        {
            return Math.Asin(timeElapsed.SineWave(frequency,amplitude)) * 2.0 / PI;
        }
        public static double Chord(this double timeElapsed, double amplitude, params double[] frequencies)
        {
            var sum = frequencies.Where(f => f != 0).Sum(f => timeElapsed.GetSine(f));
            return amplitude * sum;
        }
        public static double Aliens(this double timeElapsed, double amplitude, params double[] frequencies)
        {
            var sum = frequencies.Where(f => f != 0).Sum(f => timeElapsed.GetSine(f)) * amplitude;
            return sum;
        }
        private static double AngularVelocity(this double frequency)
        {
            return 2.0 * PI * frequency;
        }
    }
}