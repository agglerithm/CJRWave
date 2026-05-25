namespace Sound.Core
{
    public struct Note
    {
        public int Id { get; set; }
        public double OnTime { get; set; }
        public double OffTime { get; set; }
        public Instrument? Channel { get; set; }

    }
}