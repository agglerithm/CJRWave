using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace Sound.Core
{
    public abstract class Instrument
    {
        public double Volume { get; set; }
        public Envelope? Envelope { get; set; }
        public double MaxLifeTime { get; set; }
        public string? Name { get; set; }
        public abstract double Sound(double time, Note n, bool noteFinished);
    }

    public class Bell : Instrument
    {
        public Bell() { 
            Envelope = new Envelope
            {
                AttackTime = 0.01,
                DecayTime = 1.0,
                SustainTime = 0.0,
                ReleaseTime = 1.0,
                StartAmplitude = 1.0
            };
            MaxLifeTime = 3.0;
            Volume = 1.0;
            Name = "Bell";
        }

        public override double Sound(double time, Note n, bool noteFinished)
        {
            double amplitude = Envelope.Env(time, Envelope, n.OnTime, n.OffTime);
            if (amplitude <= 0.0) noteFinished = true;
            double sound = 
        }
    }
}
