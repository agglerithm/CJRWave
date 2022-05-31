using BufferUtilities;
using NAudio.Wave;
using Sound.Core.Models;

namespace Sound.Core.Tests;

public class Tests
{
    private WaveOutWrapper _sut;
    [SetUp]
    public void Setup()
    {
        _sut = new WaveOutWrapper();
    }
    
    [Test]
    public void CanOpenDevice()
    {

        Open();
        Close();
    }

    [Test]
    public void CanPrepareHeader()
    {
        Open();
        _sut.PrepareHeader(GetWriteRequest(WaveHeaderFlags.Done));
        Close();
    }
    [Test]
    public void CanWriteToDevice()
    {
        var request = GetWriteRequest(WaveHeaderFlags.Prepared);
        Open();
        _sut.PrepareHeader(request);
        _sut.Write(request);
        Close();
    }

    private void Close()
    {
        _sut.Close();
    }

    private void Open()
    {
        var format = GetFormatRequest();
        _sut.Open(format);
    }

    private static FormatRequest GetFormatRequest()
    {
        var format = new FormatRequest()
        {
            BitsPerSample = 16,
            Channels = 1,
            SampleRate = 44100,
            Flags = WaveInterop.WaveInOutOpenFlags.CallbackFunction
        };
        return format;
    }

    private static WaveWriteRequest GetWriteRequest(WaveHeaderFlags flags)
    {
        return new WaveWriteRequest()
        {
            Flags = flags,
            Data = GetRandomData()
        };
    }

    private static byte[] GetRandomData()
    {
        var bb = new BufferBuilder();
        var r = new Random();
        for (int i = 0; i < 100; i++)
        {
            var buff = new byte[400];
            r.NextBytes(buff);
            bb.Append(buff);
        }

        return bb.ToByteArray();
    }
}