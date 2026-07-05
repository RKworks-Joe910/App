using System;
using System.Net.Sockets;
using UnityEngine;

namespace DmxTemplate.Dmx
{
    /// <summary>
    /// DmxUniverseの内容をArt-Net(ArtDMX)パケットとしてUDP送信する。
    /// Art-Netノードや各種DMXコンバータ(Art-Net受信対応品)への出力を想定。
    /// </summary>
    public class ArtNetSender : MonoBehaviour
    {
        [Header("Art-Net送信先")]
        [Tooltip("Art-NetノードのIPアドレス。同一サブネット全体に送る場合は 255.255.255.255 のままでOK。")]
        public string targetIp = "255.255.255.255";
        public int targetPort = 6454;

        [Tooltip("Art-Netユニバース番号 (0-32767)")]
        public int universe = 0;

        [Tooltip("1秒あたりの送信回数の上限。0にすると毎フレーム送信する。")]
        public float sendRateHz = 30f;

        private UdpClient _udpClient;
        private byte _sequence = 1;
        private float _sendInterval;
        private float _timeSinceLastSend;

        private static readonly byte[] ArtNetId =
            { (byte)'A', (byte)'r', (byte)'t', (byte)'-', (byte)'N', (byte)'e', (byte)'t', 0x00 };

        private void Awake()
        {
            _udpClient = new UdpClient { EnableBroadcast = true };
            _sendInterval = sendRateHz > 0f ? 1f / sendRateHz : 0f;
        }

        private void OnDestroy()
        {
            _udpClient?.Close();
        }

        private void LateUpdate()
        {
            _timeSinceLastSend += Time.deltaTime;
        }

        /// <summary>
        /// ユニバースの内容を即座にArt-Netパケットとして送信する。
        /// sendRateHzで指定した間隔に達していない場合は送信をスキップする。
        /// </summary>
        public void Send(DmxUniverse dmxUniverse)
        {
            if (_sendInterval > 0f && _timeSinceLastSend < _sendInterval) return;
            _timeSinceLastSend = 0f;

            byte[] packet = BuildPacket(dmxUniverse.RawData);
            try
            {
                _udpClient.Send(packet, packet.Length, targetIp, targetPort);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[ArtNetSender] 送信に失敗しました: {e.Message}");
            }

            _sequence = _sequence == 255 ? (byte)1 : (byte)(_sequence + 1);
        }

        private byte[] BuildPacket(byte[] dmxData)
        {
            // Art-Netの仕様上データ長は偶数である必要がある。
            int dataLength = dmxData.Length % 2 == 0 ? dmxData.Length : dmxData.Length + 1;
            byte[] packet = new byte[18 + dataLength];

            Buffer.BlockCopy(ArtNetId, 0, packet, 0, ArtNetId.Length);
            packet[8] = 0x00;  // OpCode下位バイト (OpDMX = 0x5000, リトルエンディアン格納)
            packet[9] = 0x50;  // OpCode上位バイト
            packet[10] = 0;    // ProtVerHi
            packet[11] = 14;   // ProtVerLo
            packet[12] = _sequence;
            packet[13] = 0;    // Physical (未使用)
            packet[14] = (byte)(universe & 0xFF);        // SubUni
            packet[15] = (byte)((universe >> 8) & 0xFF);  // Net
            packet[16] = (byte)((dataLength >> 8) & 0xFF); // LengthHi
            packet[17] = (byte)(dataLength & 0xFF);        // LengthLo

            Buffer.BlockCopy(dmxData, 0, packet, 18, dmxData.Length);

            return packet;
        }
    }
}
