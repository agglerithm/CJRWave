using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sound.Core
{
    public class Synth
    {
        public static double Oscillate(double timeElapsed, double hertz, OscillatorTypes type, 
            double floHertz = 0, double floAmp = 0, double custom = 50)
        { 
            switch (type)
            {
                case OscillatorTypes.SineWave:
                    return timeElapsed.SineWave(floAmp,hertz);
                case OscillatorTypes.SquareWave:
                    return timeElapsed.SquareWave(floAmp, hertz);
                case OscillatorTypes.TriangleWave:
                    return timeElapsed.TriangleWave(floAmp, hertz);
                case OscillatorTypes.SawWave:
                    return timeElapsed.SawtoothWave(floAmp, hertz, custom);
                default:
                    return 0;
            } 
        }
        public static double Scale(int noteId, int scaleId)
        {
            return 8 * Math.Pow(1.0594630943592952645618252949463, noteId);
        }
        public static double Env(double time, Envelope env, double timeOn, double timeOff)
        {
            return env.Amplitude(time, timeOn, timeOff);
        }
    }
}
