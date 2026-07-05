using System;

namespace DmxTemplate.Dmx
{
    /// <summary>
    /// DMX512の1ユニバース分（512チャンネル）の値を保持するバッファ。
    /// チャンネル番号は1-512（DMXの慣習に合わせて1始まり）。
    /// </summary>
    [Serializable]
    public class DmxUniverse
    {
        public const int ChannelCount = 512;

        private readonly byte[] _channels = new byte[ChannelCount];

        public byte this[int channel]
        {
            get => _channels[Validate(channel) - 1];
            set => _channels[Validate(channel) - 1] = value;
        }

        public byte[] RawData => _channels;

        public void Clear() => Array.Clear(_channels, 0, _channels.Length);

        private static int Validate(int channel)
        {
            if (channel < 1 || channel > ChannelCount)
                throw new ArgumentOutOfRangeException(nameof(channel), $"DMXチャンネルは1〜{ChannelCount}の範囲で指定してください。");
            return channel;
        }
    }
}
