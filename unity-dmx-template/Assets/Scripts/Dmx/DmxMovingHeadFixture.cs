using UnityEngine;

namespace DmxTemplate.Dmx
{
    /// <summary>
    /// ムービングヘッド(可動照明)のDMXフィクスチャ。
    /// Pan(16bit)/Tilt(16bit)/Dimmer(8bit)/Color(8bit、簡易)の計6chを使用。
    /// 実機のチャンネルマップに合わせてChannelAtのオフセットを調整すること。
    /// </summary>
    [RequireComponent(typeof(Light))]
    public class DmxMovingHeadFixture : DmxFixture
    {
        [Header("可動範囲 (実機仕様に合わせる)")]
        public float panRangeDegrees = 270f;
        public float tiltRangeDegrees = 130f;

        [Header("Unityライト側のプレビュー設定")]
        public float maxIntensity = 10f;

        [Header("現在値 (Inspectorで直接操作、またはスクリプトから設定)")]
        [Range(0f, 1f)] public float pan = 0.5f;
        [Range(0f, 1f)] public float tilt = 0.5f;
        [Range(0f, 1f)] public float dimmer = 1f;
        public Color color = Color.white;

        private Light _light;
        private Quaternion _restRotation;

        // Pan(Hi,Lo) + Tilt(Hi,Lo) + Dimmer + Color(簡易1ch)
        public override int ChannelCount => 6;

        private void Awake()
        {
            _light = GetComponent<Light>();
            _restRotation = transform.localRotation;
        }

        public override void WriteTo(DmxUniverse universe)
        {
            ushort panValue = (ushort)Mathf.RoundToInt(Mathf.Clamp01(pan) * 65535f);
            ushort tiltValue = (ushort)Mathf.RoundToInt(Mathf.Clamp01(tilt) * 65535f);

            universe[ChannelAt(0)] = (byte)(panValue >> 8);
            universe[ChannelAt(1)] = (byte)(panValue & 0xFF);
            universe[ChannelAt(2)] = (byte)(tiltValue >> 8);
            universe[ChannelAt(3)] = (byte)(tiltValue & 0xFF);
            universe[ChannelAt(4)] = (byte)Mathf.RoundToInt(Mathf.Clamp01(dimmer) * 255f);
            universe[ChannelAt(5)] = (byte)Mathf.RoundToInt(color.grayscale * 255f);

            float panAngle = (pan - 0.5f) * panRangeDegrees;
            float tiltAngle = (tilt - 0.5f) * tiltRangeDegrees;
            transform.localRotation = _restRotation * Quaternion.Euler(tiltAngle, panAngle, 0f);

            if (_light != null)
            {
                _light.color = color;
                _light.intensity = dimmer * maxIntensity;
            }
        }
    }
}
