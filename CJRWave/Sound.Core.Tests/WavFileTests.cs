using CJRWave;
using NUnit.Framework;
using static System.Net.WebRequestMethods;

namespace Sound.Core.Tests;

[TestFixture]
public class WavFileTests
{
    private WaveSynthService _sut;
    private WAVFile _file;
    [Test]
    public void CanPlayWavFile()
    {
        new WaveFilePlayer(new WavePlayerService(s => Console.WriteLine(s))).Play("Chaconne.wav");
    }
}