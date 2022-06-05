using System.Runtime.InteropServices;

namespace Sound.Core.WaveInterop;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public class WaveFormatExtensible : WaveFormat
{
    private short wValidBitsPerSample;
    private int dwChannelMask;
    private Guid subFormat;

    private WaveFormatExtensible()
    {
    }

    public WaveFormatExtensible(int rate, int bits, int channels)
        : base(rate, bits, channels)
    {
        this.waveFormatTag = WaveFormatEncoding.Extensible;
        this.extraSize = (short) 22;
        this.wValidBitsPerSample = (short) bits;
        for (int index = 0; index < channels; ++index)
            this.dwChannelMask |= 1 << index;
        if (bits == 32)
            this.subFormat = AudioMediaSubtypes.MEDIASUBTYPE_IEEE_FLOAT;
        else
            this.subFormat = AudioMediaSubtypes.MEDIASUBTYPE_PCM;
    }

    public WaveFormat ToStandardWaveFormat()
    {
        if (this.subFormat == AudioMediaSubtypes.MEDIASUBTYPE_IEEE_FLOAT && this.bitsPerSample == (short) 32)
            return WaveFormat.CreateIeeeFloatWaveFormat(this.sampleRate, (int) this.channels);
        return this.subFormat == AudioMediaSubtypes.MEDIASUBTYPE_PCM ? new WaveFormat(this.sampleRate, (int) this.bitsPerSample, (int) this.channels) : (WaveFormat) this;
    }

    public Guid SubFormat => this.subFormat;

    public override void Serialize(BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write(this.wValidBitsPerSample);
        writer.Write(this.dwChannelMask);
        byte[] byteArray = this.subFormat.ToByteArray();
        writer.Write(byteArray, 0, byteArray.Length);
    }

    public override string ToString() => "WAVE_FORMAT_EXTENSIBLE " + AudioMediaSubtypes.GetAudioSubtypeName(this.subFormat) + " " + string.Format("{0}Hz {1} channels {2} bit", (object) this.SampleRate, (object) this.Channels, (object) this.BitsPerSample);
}

 public class AudioMediaSubtypes
  {
    public static readonly Guid MEDIASUBTYPE_PCM = new Guid("00000001-0000-0010-8000-00AA00389B71");
    public static readonly Guid MEDIASUBTYPE_PCMAudioObsolete = new Guid("e436eb8a-524f-11ce-9f53-0020af0ba770");
    public static readonly Guid MEDIASUBTYPE_MPEG1Packet = new Guid("e436eb80-524f-11ce-9f53-0020af0ba770");
    public static readonly Guid MEDIASUBTYPE_MPEG1Payload = new Guid("e436eb81-524f-11ce-9f53-0020af0ba770");
    public static readonly Guid MEDIASUBTYPE_MPEG2_AUDIO = new Guid("e06d802b-db46-11cf-b4d1-00805f6cbbea");
    public static readonly Guid MEDIASUBTYPE_DVD_LPCM_AUDIO = new Guid("e06d8032-db46-11cf-b4d1-00805f6cbbea");
    public static readonly Guid MEDIASUBTYPE_DRM_Audio = new Guid("00000009-0000-0010-8000-00aa00389b71");
    public static readonly Guid MEDIASUBTYPE_IEEE_FLOAT = new Guid("00000003-0000-0010-8000-00aa00389b71");
    public static readonly Guid MEDIASUBTYPE_DOLBY_AC3 = new Guid("e06d802c-db46-11cf-b4d1-00805f6cbbea");
    public static readonly Guid MEDIASUBTYPE_DOLBY_AC3_SPDIF = new Guid("00000092-0000-0010-8000-00aa00389b71");
    public static readonly Guid MEDIASUBTYPE_RAW_SPORT = new Guid("00000240-0000-0010-8000-00aa00389b71");
    public static readonly Guid MEDIASUBTYPE_SPDIF_TAG_241h = new Guid("00000241-0000-0010-8000-00aa00389b71");
    public static readonly Guid MEDIASUBTYPE_I420 = new Guid("30323449-0000-0010-8000-00AA00389B71");
    public static readonly Guid MEDIASUBTYPE_IYUV = new Guid("56555949-0000-0010-8000-00AA00389B71");
    public static readonly Guid MEDIASUBTYPE_RGB1 = new Guid("e436eb78-524f-11ce-9f53-0020af0ba770");
    public static readonly Guid MEDIASUBTYPE_RGB24 = new Guid("e436eb7d-524f-11ce-9f53-0020af0ba770");
    public static readonly Guid MEDIASUBTYPE_RGB32 = new Guid("e436eb7e-524f-11ce-9f53-0020af0ba770");
    public static readonly Guid MEDIASUBTYPE_RGB4 = new Guid("e436eb79-524f-11ce-9f53-0020af0ba770");
    public static readonly Guid MEDIASUBTYPE_RGB555 = new Guid("e436eb7c-524f-11ce-9f53-0020af0ba770");
    public static readonly Guid MEDIASUBTYPE_RGB565 = new Guid("e436eb7b-524f-11ce-9f53-0020af0ba770");
    public static readonly Guid MEDIASUBTYPE_RGB8 = new Guid("e436eb7a-524f-11ce-9f53-0020af0ba770");
    public static readonly Guid MEDIASUBTYPE_UYVY = new Guid("59565955-0000-0010-8000-00AA00389B71");
    public static readonly Guid MEDIASUBTYPE_VIDEOIMAGE = new Guid("1d4a45f2-e5f6-4b44-8388-f0ae5c0e0c37");
    public static readonly Guid MEDIASUBTYPE_YUY2 = new Guid("32595559-0000-0010-8000-00AA00389B71");
    public static readonly Guid MEDIASUBTYPE_YV12 = new Guid("31313259-0000-0010-8000-00AA00389B71");
    public static readonly Guid MEDIASUBTYPE_YVU9 = new Guid("39555659-0000-0010-8000-00AA00389B71");
    public static readonly Guid MEDIASUBTYPE_YVYU = new Guid("55595659-0000-0010-8000-00AA00389B71");
    public static readonly Guid WMFORMAT_MPEG2Video = new Guid("e06d80e3-db46-11cf-b4d1-00805f6cbbea");
    public static readonly Guid WMFORMAT_Script = new Guid("5C8510F2-DEBE-4ca7-BBA5-F07A104F8DFF");
    public static readonly Guid WMFORMAT_VideoInfo = new Guid("05589f80-c356-11ce-bf01-00aa0055595a");
    public static readonly Guid WMFORMAT_WaveFormatEx = new Guid("05589f81-c356-11ce-bf01-00aa0055595a");
    public static readonly Guid WMFORMAT_WebStream = new Guid("da1e6b13-8359-4050-b398-388e965bf00c");
    public static readonly Guid WMMEDIASUBTYPE_ACELPnet = new Guid("00000130-0000-0010-8000-00AA00389B71");
    public static readonly Guid WMMEDIASUBTYPE_Base = new Guid("00000000-0000-0010-8000-00AA00389B71");
    public static readonly Guid WMMEDIASUBTYPE_DRM = new Guid("00000009-0000-0010-8000-00AA00389B71");
    public static readonly Guid WMMEDIASUBTYPE_MP3 = new Guid("00000055-0000-0010-8000-00AA00389B71");
    public static readonly Guid WMMEDIASUBTYPE_MP43 = new Guid("3334504D-0000-0010-8000-00AA00389B71");
    public static readonly Guid WMMEDIASUBTYPE_MP4S = new Guid("5334504D-0000-0010-8000-00AA00389B71");
    public static readonly Guid WMMEDIASUBTYPE_M4S2 = new Guid("3253344D-0000-0010-8000-00AA00389B71");
    public static readonly Guid WMMEDIASUBTYPE_P422 = new Guid("32323450-0000-0010-8000-00AA00389B71");
    public static readonly Guid WMMEDIASUBTYPE_MPEG2_VIDEO = new Guid("e06d8026-db46-11cf-b4d1-00805f6cbbea");
    public static readonly Guid WMMEDIASUBTYPE_MSS1 = new Guid("3153534D-0000-0010-8000-00AA00389B71");
    public static readonly Guid WMMEDIASUBTYPE_MSS2 = new Guid("3253534D-0000-0010-8000-00AA00389B71");
    public static readonly Guid WMMEDIASUBTYPE_PCM = new Guid("00000001-0000-0010-8000-00AA00389B71");
    public static readonly Guid WMMEDIASUBTYPE_WebStream = new Guid("776257d4-c627-41cb-8f81-7ac7ff1c40cc");
    public static readonly Guid WMMEDIASUBTYPE_WMAudio_Lossless = new Guid("00000163-0000-0010-8000-00AA00389B71");
    public static readonly Guid WMMEDIASUBTYPE_WMAudioV2 = new Guid("00000161-0000-0010-8000-00AA00389B71");
    public static readonly Guid WMMEDIASUBTYPE_WMAudioV7 = new Guid("00000161-0000-0010-8000-00AA00389B71");
    public static readonly Guid WMMEDIASUBTYPE_WMAudioV8 = new Guid("00000161-0000-0010-8000-00AA00389B71");
    public static readonly Guid WMMEDIASUBTYPE_WMAudioV9 = new Guid("00000162-0000-0010-8000-00AA00389B71");
    public static readonly Guid WMMEDIASUBTYPE_WMSP1 = new Guid("0000000A-0000-0010-8000-00AA00389B71");
    public static readonly Guid WMMEDIASUBTYPE_WMV1 = new Guid("31564D57-0000-0010-8000-00AA00389B71");
    public static readonly Guid WMMEDIASUBTYPE_WMV2 = new Guid("32564D57-0000-0010-8000-00AA00389B71");
    public static readonly Guid WMMEDIASUBTYPE_WMV3 = new Guid("33564D57-0000-0010-8000-00AA00389B71");
    public static readonly Guid WMMEDIASUBTYPE_WMVA = new Guid("41564D57-0000-0010-8000-00AA00389B71");
    public static readonly Guid WMMEDIASUBTYPE_WMVP = new Guid("50564D57-0000-0010-8000-00AA00389B71");
    public static readonly Guid WMMEDIASUBTYPE_WVP2 = new Guid("32505657-0000-0010-8000-00AA00389B71");
    public static readonly Guid WMMEDIATYPE_Audio = new Guid("73647561-0000-0010-8000-00AA00389B71");
    public static readonly Guid WMMEDIATYPE_FileTransfer = new Guid("D9E47579-930E-4427-ADFC-AD80F290E470");
    public static readonly Guid WMMEDIATYPE_Image = new Guid("34A50FD8-8AA5-4386-81FE-A0EFE0488E31");
    public static readonly Guid WMMEDIATYPE_Script = new Guid("73636d64-0000-0010-8000-00AA00389B71");
    public static readonly Guid WMMEDIATYPE_Text = new Guid("9BBA1EA7-5AB2-4829-BA57-0940209BCF3E");
    public static readonly Guid WMMEDIATYPE_Video = new Guid("73646976-0000-0010-8000-00AA00389B71");
    public static readonly Guid WMSCRIPTTYPE_TwoStrings = new Guid("82f38a70-c29f-11d1-97ad-00a0c95ea850");
    public static readonly Guid MEDIASUBTYPE_WAVE = new Guid("e436eb8b-524f-11ce-9f53-0020af0ba770");
    public static readonly Guid MEDIASUBTYPE_AU = new Guid("e436eb8c-524f-11ce-9f53-0020af0ba770");
    public static readonly Guid MEDIASUBTYPE_AIFF = new Guid("e436eb8d-524f-11ce-9f53-0020af0ba770");
    public static readonly Guid[] AudioSubTypes = new Guid[13]
    {
      AudioMediaSubtypes.MEDIASUBTYPE_PCM,
      AudioMediaSubtypes.MEDIASUBTYPE_PCMAudioObsolete,
      AudioMediaSubtypes.MEDIASUBTYPE_MPEG1Packet,
      AudioMediaSubtypes.MEDIASUBTYPE_MPEG1Payload,
      AudioMediaSubtypes.MEDIASUBTYPE_MPEG2_AUDIO,
      AudioMediaSubtypes.MEDIASUBTYPE_DVD_LPCM_AUDIO,
      AudioMediaSubtypes.MEDIASUBTYPE_DRM_Audio,
      AudioMediaSubtypes.MEDIASUBTYPE_IEEE_FLOAT,
      AudioMediaSubtypes.MEDIASUBTYPE_DOLBY_AC3,
      AudioMediaSubtypes.MEDIASUBTYPE_DOLBY_AC3_SPDIF,
      AudioMediaSubtypes.MEDIASUBTYPE_RAW_SPORT,
      AudioMediaSubtypes.MEDIASUBTYPE_SPDIF_TAG_241h,
      AudioMediaSubtypes.WMMEDIASUBTYPE_MP3
    };
    public static readonly string[] AudioSubTypeNames = new string[13]
    {
      "PCM",
      "PCM Obsolete",
      "MPEG1Packet",
      "MPEG1Payload",
      "MPEG2_AUDIO",
      "DVD_LPCM_AUDIO",
      "DRM_Audio",
      "IEEE_FLOAT",
      "DOLBY_AC3",
      "DOLBY_AC3_SPDIF",
      "RAW_SPORT",
      "SPDIF_TAG_241h",
      "MP3"
    };

    public static string GetAudioSubtypeName(Guid subType)
    {
      for (int index = 0; index < AudioMediaSubtypes.AudioSubTypes.Length; ++index)
      {
        if (subType == AudioMediaSubtypes.AudioSubTypes[index])
          return AudioMediaSubtypes.AudioSubTypeNames[index];
      }
      return subType.ToString();
    }
  }