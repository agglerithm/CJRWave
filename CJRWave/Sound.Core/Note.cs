namespace Sound.Core
{
    public struct Note
    {
        public int Id { get; set; }
        public double OnTime { get; set; }
        public double OffTime { get; set; }
        public Instrument? Channel { get; set; }
        public static double Scale(int noteId, int scaleId)
        {
            return 8 * Math.Pow(1.0594630943592952645618252949463, noteId);
        }
    }
}