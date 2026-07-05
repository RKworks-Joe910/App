using System.Collections.Generic;
using UnityEngine;

namespace DmxTemplate.Dmx
{
    /// <summary>
    /// 複数のDmxFixtureを束ね、毎フレーム1つのDmxUniverseにまとめてArtNetSenderへ渡す。
    /// </summary>
    [RequireComponent(typeof(ArtNetSender))]
    public class DmxLightingRig : MonoBehaviour
    {
        [Tooltip("このリグが管理するフィクスチャ一覧。")]
        public List<DmxFixture> fixtures = new List<DmxFixture>();

        [Tooltip("trueの場合、Awake時に子オブジェクトからDmxFixtureを自動収集する。")]
        public bool autoCollectFromChildren = true;

        private readonly DmxUniverse _universe = new DmxUniverse();
        private ArtNetSender _sender;

        private void Awake()
        {
            _sender = GetComponent<ArtNetSender>();
            if (autoCollectFromChildren)
                fixtures.AddRange(GetComponentsInChildren<DmxFixture>());
        }

        private void Update()
        {
            _universe.Clear();
            foreach (var fixture in fixtures)
            {
                if (fixture == null || !fixture.isActiveAndEnabled) continue;
                fixture.WriteTo(_universe);
            }
            _sender.Send(_universe);
        }

        /// <summary>
        /// 登録順にチャンネルを詰めて自動採番する。実機のアドレス設定と一致させたい場合は
        /// 各フィクスチャのstartChannelを手動設定し、このメソッドは使わないこと。
        /// </summary>
        [ContextMenu("チャンネルを自動割り当て")]
        public void AutoAssignChannels()
        {
            int next = 1;
            foreach (var fixture in fixtures)
            {
                if (fixture == null) continue;
                fixture.startChannel = next;
                next += fixture.ChannelCount;
            }
        }
    }
}
