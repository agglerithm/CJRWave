using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sound.Core
{
    public class Envelope
    {
        public double AttackTime { get; set; }
        public double DecayTime { get; set; }
        public double SustainTime{ get; set; }
        public double ReleaseTime { get; set; }
        public double StartAmplitude { get; set; }
        public double Amplitude(double timeElapsed, double timeOn, double timeOff)
        {
            double amplitude = 0;
            double releaseAmplitude = 0;

            if (timeOn > timeOff)
            {
                double lifetime = timeElapsed - timeOn;

                if (lifetime <= AttackTime)
                    amplitude = lifetime / AttackTime * StartAmplitude;

                if (lifetime > AttackTime && lifetime <= AttackTime + DecayTime)
                    amplitude = ((lifetime - AttackTime) / DecayTime * (SustainTime - StartAmplitude)) + StartAmplitude;
                if (lifetime > (AttackTime + DecayTime))
                    amplitude = SustainTime;
            }
            else //Note off
            {
                double lifetime = timeElapsed - timeOff;
                if (lifetime <= AttackTime)
                    releaseAmplitude = lifetime / AttackTime * StartAmplitude;

                if (lifetime > AttackTime && lifetime <= AttackTime + DecayTime)
                    releaseAmplitude = (lifetime - AttackTime) / DecayTime * (SustainTime - StartAmplitude) + StartAmplitude;

                if (lifetime > (AttackTime + DecayTime))
                    releaseAmplitude = SustainTime;

                amplitude = ((timeElapsed - timeOff) / ReleaseTime) * (0 - releaseAmplitude) + releaseAmplitude;
            }
            if(amplitude <= 0.01)
                amplitude = 0;

            return amplitude;
        }

        public static double Env(double time, Envelope env, double timeOn, double timeOff)
        {
            return env.Amplitude(time, timeOn, timeOff);
        }
    }
}
