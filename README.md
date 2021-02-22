# FixFBXHeight
シーン中のメッシュ(FBX)の底面をY = 0に揃えた状態でメッシュを保存しなおすためのエディタ拡張です。<br>
This is an Unity editor tool for adjusting height of FBX mesh to satisfy minimum Y coordinate in the mesh gets 0.<br><br>
![](https://github.com/ButchKoh/FixFBXHeight/blob/main/Screenshot.png)

# Package Dependencies / パッケージ依存
FBX Exporter ver.3.2.1<br>
Editor Coroutines ver.1.0.1<br>

# How to Use / 使い方
Assets内のどこかに「Editor」フォルダを作って、その中にcsファイルを放り投げてください。
ツールバーからウィンドウが開きます。シーン中の3DモデルをオブジェクトフィールドにD&Dして[Start]を押すと処理が始まります。
処理が終わると、元のメッシュと変換後のメッシュがAssetsフォルダ以下に出力されているはずです。
処理を強制的に中断する場合は[quit]を押してください。<br>
Create an "Editor" folder somewhere in Assets and toss the .cs file into it.
The window will open from the toolbar->Tools. 
Select the 3D model in the scene on the object field and press [Start] to start the process.
If [Local to Absolute] is clicked, all the vertices will be converted from local transtorm to world coordinate in advance.
Both original mesh and converted mesh will be output in Assets folder.
If you want to abort while the process, press [quit].

# Notice / メモ
.metaファイルの生成タイミングの関係で警告文が出るかもしれません。原因が判ったら修正します(このままでも使えるには使えます)。<br>
It may put a warning message due to the .meta file generation.
