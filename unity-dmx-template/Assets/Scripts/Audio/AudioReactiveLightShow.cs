using System.Collections.Generic;
using UnityEngine;
using DmxTemplate.Dmx;

namespace DmxTemplate.Audio
{
    /// <summary>
    /// AudioReactiveDriverのBass/Mid/HighをDmxFixtureのdimmer/colorへ反映するサンプル演出。
    /// Web版NEON SYNCの「AUTO」モードに相当する最小実装。実際のショーではキュー切り替えや
    /// タイムライン制御に置き換えることを想定している。
    /// </summary>
    [RequireComponent(typeof(AudioReactiveDriver))]
    public class AudioReactiveLightShow : MonoBehaviour
    {
        [Tooltip("Bass/Mid/Highで駆動するPARフィクスチャ")]
        public List<DmxParFixture> parFixtures = new List<DmxParFixture>();

        [Tooltip("Bass/Mid/Highで駆動するムービングヘッド")]
        public List<DmxMovingHeadFixture> movingHeads = new List<DmxMovingHeadFixture>();

        public Color lowColor = new Color(0f, 0.8f, 1f);
        public Color highColor = new Color(1f, 0f, 0.6f);

        private AudioReactiveDriver _driver;
        private float _sweepT;

        private void Awake() => _driver = GetComponent<AudioReactiveDriver>();

        private void Update()
        {
            float bass = _driver.Bass;
            float mid = _driver.Mid;
            float high = _driver.High;

            Color mixed = Color.Lerp(lowColor, highColor, mid);
            _sweepT += Time.deltaTime;

            foreach (var par in parFixtures)
            {
                if (par == null) continue;
                par.dimmer = Mathf.Clamp01(0.25f + bass * 0.9f);
                par.color = mixed;
            }

            foreach (var head in movingHeads)
            {
                if (head == null) continue;
                head.dimmer = Mathf.Clamp01(0.2f + high * 0.9f);
                head.color = mixed;
                head.pan = (Mathf.Sin(_sweepT * 0.5f) * 0.5f) + 0.5f;
                head.tilt = 0.5f + Mathf.Sin(_sweepT * 0.8f + 1.3f) * 0.25f;
            }
        }
    }
}
