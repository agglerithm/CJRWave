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
}
