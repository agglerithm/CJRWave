using CJRWave;
using NUnit.Framework;

namespace Sound.Core.Tests;

[TestFixture]
public class WavFileTests
{
    private WaveSynthService _sut;
    private WAVFile _file;
    [Test]
    public void CanPlayWavFile()
    {
        _file = new WAVFile();
        _file.Read("dance.wav");
    }
}