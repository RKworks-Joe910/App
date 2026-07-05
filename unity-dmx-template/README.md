# Unity DMXライティングテンプレート

Unity上のシーンライトと、Art-Net経由の実機DMX照明(PAR/ムービングヘッドなど)を
連動させるための最小構成テンプレートです。`Assets/` の中身をそのまま既存の
Unityプロジェクトへドロップイン、または本フォルダをプロジェクトルートとして
新規プロジェクトを開いても使えます。

## 前提

- Unity 2022.3 LTS以降を推奨(Built-in / URP / HDRP いずれのレンダーパイプラインでも動作)
- 出力プロトコルはArt-Net(UDP, ポート6454)。sACNやUSB DMXインターフェースは
  未実装ですが、`ArtNetSender` と同じインタフェースで差し替え可能な設計にしてあります。

## セットアップ手順

1. Unityプロジェクトを開き、このフォルダの `Assets/Scripts` と `Assets/Editor` を
   プロジェクトの `Assets` 配下にコピーする。
2. メニューの **DMX Template > Build Demo Rig in Scene** を実行する。
   - `DMX Lighting Rig` オブジェクトが生成され、PARライト2台とムービングヘッド1台の
     サンプルフィクスチャが子オブジェクトとして配置される。
3. `DMX Lighting Rig` を選択し、`Art Net Sender` コンポーネントの `Target Ip` を
   実機のArt-Netノード/DMXコンバータのIPアドレスに変更する。同一サブネットであれば
   `255.255.255.255`(ブロードキャスト)のままでも動作確認できる。
4. 実機がまだ手元にない場合は、PCに無料のArt-Net受信/モニタツール
   (例: QLC+のDMXモニタ機能など)を入れて同一LAN上で受信すると、パケットが
   届いているかを確認できる。
5. Playすると `DmxLightingRig` が毎フレーム全フィクスチャの状態を1つのDMXユニバースに
   まとめ、`ArtNetSender` がArt-Net(ArtDMX)パケットとして送信する。各フィクスチャの
   `dimmer` / `color` / `pan` / `tilt` をInspector上で動かせばリアルタイムに反映される。

## 構成

```
Assets/
  Scripts/
    Dmx/
      DmxUniverse.cs          DMX512の1ユニバース分のバイト配列を保持
      ArtNetSender.cs         DmxUniverseをArt-Net(ArtDMX)パケットとしてUDP送信
      DmxFixture.cs           フィクスチャの共通基底クラス(開始チャンネル/書き込み処理)
      DmxParFixture.cs        RGB PARライト(Dimmer+RGB、またはRGBのみ)
      DmxMovingHeadFixture.cs ムービングヘッド(Pan/Tilt/Dimmer/Color)
      DmxLightingRig.cs       複数フィクスチャを束ねてArtNetSenderへ渡す
    Audio/
      AudioReactiveDriver.cs     AudioSourceからBass/Mid/Highレベルを算出
      AudioReactiveLightShow.cs  Bass/Mid/Highをフィクスチャのdimmer/colorへ反映するサンプル
  Editor/
    DemoSceneBuilder.cs   デモ用リグをシーンに生成するエディタメニュー
```

## 実機に合わせたカスタマイズ

- **チャンネルマップ**: `DmxParFixture` / `DmxMovingHeadFixture` は汎用的な
  チャンネル構成の一例です。実機のマニュアル記載のパーソナリティ/モードに
  合わせて `DmxFixture` を継承した専用クラスを作るか、既存クラスの
  `WriteTo` 内のオフセットを調整してください。
- **チャンネル番号の割り当て**: `DmxLightingRig.AutoAssignChannels()` は
  登録順に連番でチャンネルを割り振ります。実機側でDIPスイッチ等により
  開始アドレスが固定されている場合は、各フィクスチャの `Start Channel` を
  Inspectorで直接指定し、自動割り当ては使わないでください。
- **音楽連動**: `AudioReactiveDriver` をAudioSource付きオブジェクトにアタッチし、
  `AudioReactiveLightShow` で対象フィクスチャを指定すると、既存のWeb版
  「NEON SYNC」と同様にBass/Mid/Highに応じて照明が反応します。実際の演出では
  `AudioReactiveLightShow` をキュー切り替えやタイムライン制御に置き換える
  ことを想定しています。
- **プロトコルの変更**: sACN(E1.31)やUSB DMX(ENTTEC等)を使いたい場合は、
  `ArtNetSender.Send(DmxUniverse)` と同じシグネチャの送信クラスを実装し、
  `DmxLightingRig` から呼び出すクラスを差し替えてください。USB経由の場合は
  別途ネイティブプラグイン/SDKの導入が必要です。

## 注意点

- Art-Netのブロードキャスト送信(`255.255.255.255`)は同一サブネット内でのみ
  有効です。別のVLAN/サブネットにある機材へ送る場合は、対象IPを直接指定する
  (ユニキャスト)必要があります。
- 送信にはUDPポート6454を使用します。ファイアウォールで送信がブロックされて
  いないか確認してください。
- 本テンプレートにはUnityプロジェクト本体(ProjectSettings/Packagesなど)は
  含まれていません。既存プロジェクトへのスクリプト追加、または新規プロジェクト
  作成後にこのフォルダの中身を配置してご利用ください。
