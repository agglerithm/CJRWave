namespace Sound.Core.WaveInterop;

[Flags]
internal enum WaveOutSupport
{
    Pitch = 1,
    PlaybackRate = 2,
    Volume = 4,
    LRVolume = 8,
    Sync = 16, // 0x00000010
    SampleAccurate = 32, // 0x00000020
}