using UnityEngine;

namespace DmxTemplate.Dmx
{
    /// <summary>
    /// 汎用RGB PARライトのDMXフィクスチャ。
    /// Dimmer+RGB(4ch)、またはRGBのみ(3ch)のシンプルなモードに対応。
    /// 実機のパーソナリティモードに合わせてChannelModeを選ぶこと。
    /// </summary>
    [RequireComponent(typeof(Light))]
    public class DmxParFixture : DmxFixture
    {
        public enum ChannelMode { DimmerRgb, RgbOnly }

        [Header("チャンネル構成")]
        public ChannelMode channelMode = ChannelMode.DimmerRgb;

        [Header("Unityライト側のプレビュー設定")]
        [Tooltip("dimmer=1のときのUnity Light.intensity")]
        public float maxIntensity = 8f;

        [Header("現在値 (Inspectorで直接操作、またはスクリプトから設定)")]
        [Range(0f, 1f)] public float dimmer = 1f;
        public Color color = Color.white;

        private Light _light;

        public override int ChannelCount => channelMode == ChannelMode.DimmerRgb ? 4 : 3;

        private void Awake() => _light = GetComponent<Light>();

        public override void WriteTo(DmxUniverse universe)
        {
            int offset = 0;
            byte dimmerByte = ToByte(dimmer);

            if (channelMode == ChannelMode.DimmerRgb)
                universe[ChannelAt(offset++)] = dimmerByte;

            universe[ChannelAt(offset++)] = ToByte(color.r);
            universe[ChannelAt(offset++)] = ToByte(color.g);
            universe[ChannelAt(offset)]   = ToByte(color.b);

            if (_light != null)
            {
                _light.color = color;
                _light.intensity = dimmer * maxIntensity;
            }
        }

        private static byte ToByte(float v) => (byte)Mathf.RoundToInt(Mathf.Clamp01(v) * 255f);
    }
}
