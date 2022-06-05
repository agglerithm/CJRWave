using System.Runtime.InteropServices;

namespace Sound.Core.WaveInterop;

public class Wave
{
  [DllImport("winmm.dll")]
  public static extern MmResult waveOutPrepareHeader(
    IntPtr hWaveOut,
    WaveHeader lpWaveOutHdr,
    int uSize);

  [DllImport("winmm.dll")]
  public static extern MmResult waveOutUnprepareHeader(
    IntPtr hWaveOut,
    WaveHeader lpWaveOutHdr,
    int uSize);

  [DllImport("winmm.dll")]
  public static extern MmResult waveOutWrite(
    IntPtr hWaveOut,
    WaveHeader lpWaveOutHdr,
    int uSize);

  [DllImport("winmm.dll")]
  public static extern MmResult waveOutOpen(
    out IntPtr hWaveOut,
    IntPtr uDeviceID,
    WaveFormat lpFormat,
    Wave.WaveCallback dwCallback,
    IntPtr dwInstance,
    Wave.WaveInOutOpenFlags dwFlags);

  [DllImport("winmm.dll", EntryPoint = "waveOutOpen")]
  public static extern MmResult waveOutOpenWindow(
    out IntPtr hWaveOut,
    IntPtr uDeviceID,
    WaveFormat lpFormat,
    IntPtr callbackWindowHandle,
    IntPtr dwInstance,
    Wave.WaveInOutOpenFlags dwFlags);

  [DllImport("winmm.dll")]
  public static extern MmResult waveOutReset(IntPtr hWaveOut);

  [DllImport("winmm.dll")]
  public static extern MmResult waveOutClose(IntPtr hWaveOut);

  [DllImport("winmm.dll")]
  public static extern MmResult waveOutPause(IntPtr hWaveOut);

  [DllImport("winmm.dll")]
  public static extern MmResult waveOutRestart(IntPtr hWaveOut);

  [DllImport("winmm.dll")]
  public static extern MmResult waveOutGetPosition(
    IntPtr hWaveOut,
    ref MmTime mmTime,
    int uSize);

  [DllImport("winmm.dll")]
  public static extern MmResult waveOutSetVolume(IntPtr hWaveOut, int dwVolume);

  [DllImport("winmm.dll")]
  public static extern MmResult waveOutGetVolume(IntPtr hWaveOut, out int dwVolume);

  [DllImport("winmm.dll", CharSet = CharSet.Auto)]
  public static extern MmResult waveOutGetDevCaps(
    IntPtr deviceID,
    out WaveOutCapabilities waveOutCaps,
    int waveOutCapsSize);

  [DllImport("winmm.dll")]
  public static extern int waveInGetNumDevs();
    
  public enum WaveMessage
  {
    WaveOutOpen = 955, // 0x000003BB
    WaveOutClose = 956, // 0x000003BC
    WaveOutDone = 957, // 0x000003BD
    WaveInOpen = 958, // 0x000003BE
    WaveInClose = 959, // 0x000003BF
    WaveInData = 960, // 0x000003C0
  }
    
  public delegate void WaveCallback(
    IntPtr hWaveOut,
    Wave.WaveMessage message,
    IntPtr dwInstance,
    WaveHeader wavhdr,
    IntPtr dwReserved);
    
  [Flags]
  public enum WaveInOutOpenFlags
  {
    CallbackNull = 0,
    CallbackFunction = 196608, // 0x00030000
    CallbackEvent = 327680, // 0x00050000
    CallbackWindow = 65536, // 0x00010000
    CallbackThread = 131072, // 0x00020000
  }
}