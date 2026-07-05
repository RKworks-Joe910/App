using UnityEngine;

namespace DmxTemplate.Dmx
{
    /// <summary>
    /// DMXフィクスチャ(照明機材)の共通基底クラス。
    /// 各フィクスチャは自分の使用チャンネル数(ChannelCount)を宣言し、
    /// WriteToで自分の状態をDmxUniverseの該当チャンネルへ書き込む。
    /// </summary>
    public abstract class DmxFixture : MonoBehaviour
    {
        [Tooltip("このフィクスチャの開始チャンネル (1-512)。実機のアドレス設定と合わせること。")]
        [Range(1, 512)]
        public int startChannel = 1;

        public abstract int ChannelCount { get; }

        public abstract void WriteTo(DmxUniverse universe);

        protected int ChannelAt(int offset) => startChannel + offset;
    }
}
