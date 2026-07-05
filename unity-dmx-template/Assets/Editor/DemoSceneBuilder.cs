#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using DmxTemplate.Dmx;

namespace DmxTemplate.EditorTools
{
    /// <summary>
    /// メニューから、PARライト2台+ムービングヘッド1台のサンプルDMXリグを
    /// ワンクリックでシーンに生成するエディタ拡張。
    /// </summary>
    public static class DemoSceneBuilder
    {
        [MenuItem("DMX Template/Build Demo Rig in Scene")]
        public static void BuildDemoRig()
        {
            var rigObject = new GameObject("DMX Lighting Rig");
            var sender = rigObject.AddComponent<ArtNetSender>();
            sender.targetIp = "255.255.255.255";
            sender.universe = 0;

            var rig = rigObject.AddComponent<DmxLightingRig>();
            rig.autoCollectFromChildren = false;

            var par1 = CreatePar("PAR 1 (Left)", new Vector3(-3f, 4f, 0f), rigObject.transform);
            var par2 = CreatePar("PAR 2 (Right)", new Vector3(3f, 4f, 0f), rigObject.transform);
            var movingHead = CreateMovingHead("Moving Head 1", new Vector3(0f, 5f, -2f), rigObject.transform);

            rig.fixtures.Clear();
            rig.fixtures.Add(par1.GetComponent<DmxParFixture>());
            rig.fixtures.Add(par2.GetComponent<DmxParFixture>());
            rig.fixtures.Add(movingHead.GetComponent<DmxMovingHeadFixture>());
            rig.AutoAssignChannels();

            Selection.activeGameObject = rigObject;
            Debug.Log("[DemoSceneBuilder] DMXデモリグをシーンに作成しました。ArtNetSenderのTarget IPを実機/受信ソフトに合わせて設定してください。");
        }

        private static GameObject CreatePar(string name, Vector3 position, Transform parent)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);
            go.transform.position = position;
            var light = go.AddComponent<Light>();
            light.type = LightType.Point;
            light.range = 12f;
            go.AddComponent<DmxParFixture>();
            return go;
        }

        private static GameObject CreateMovingHead(string name, Vector3 position, Transform parent)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);
            go.transform.position = position;
            go.transform.rotation = Quaternion.Euler(60f, 0f, 0f);
            var light = go.AddComponent<Light>();
            light.type = LightType.Spot;
            light.range = 20f;
            light.spotAngle = 30f;
            go.AddComponent<DmxMovingHeadFixture>();
            return go;
        }
    }
}
#endif
