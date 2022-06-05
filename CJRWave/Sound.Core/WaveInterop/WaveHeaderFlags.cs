namespace Sound.Core.WaveInterop;

[Flags]
public enum WaveHeaderFlags
{
    BeginLoop = 4,
    Done = 1,
    EndLoop = 8,
    InQueue = 16, // 0x00000010
    Prepared = 2,
}