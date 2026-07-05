using UnityEngine;

namespace DmxTemplate.Audio
{
    /// <summary>
    /// AudioSourceのスペクトラムを解析し、Bass/Mid/Highのレベル(0-1程度)を算出する。
    /// 既存のWeb版(NEON SYNC)と同様に、この値をDmxFixtureのdimmer/colorへ
    /// 割り当てることで音楽連動の照明演出ができる。
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioReactiveDriver : MonoBehaviour
    {
        private const int SampleSize = 512;

        [Range(0f, 1f)] public float smoothing = 0.6f;
        [Tooltip("スペクトラム値の増幅率。機材や音源に応じて調整する。")]
        public float gain = 50f;

        public float Bass { get; private set; }
        public float Mid { get; private set; }
        public float High { get; private set; }

        private AudioSource _source;
        private readonly float[] _spectrum = new float[SampleSize];

        private void Awake() => _source = GetComponent<AudioSource>();

        private void Update()
        {
            _source.GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);

            float bass = Average(0, SampleSize / 20);
            float mid = Average(SampleSize / 20, SampleSize / 4);
            float high = Average(SampleSize / 4, SampleSize * 3 / 5);

            Bass = Mathf.Lerp(bass, Bass, smoothing);
            Mid = Mathf.Lerp(mid, Mid, smoothing);
            High = Mathf.Lerp(high, High, smoothing);
        }

        private float Average(int start, int end)
        {
            float sum = 0f;
            for (int i = start; i < end; i++) sum += _spectrum[i];
            return Mathf.Clamp01(sum / (end - start) * gain);
        }
    }
}
