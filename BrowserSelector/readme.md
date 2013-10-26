#BrowserSelector
関連付けに設定して正規表現で起動するブラウザを設定するソフトです。

2chやニコニコ動画等の関連付けのUrlを専用のブラウザで開きたいと思う方におすすめです。

Windows Vista移行に対応(UAC関係のコードを抜けばXPでも動くはず)

##インストール方法
BrowserSelector.exe --install

アンインストール方法はありません。
元々使っていたブラウザをデフォルト設定にしましょう。

##使い方
BrowserSelector.jsonをexeファイルと同じディレクトリに作成する。
BrowserSelector.sample.jsonを参考にjsonファイルを書く

正規表現エンジンは.NET4のデフォルトです
他にもオプションがあるが、そのうち書きます。

##TODO
高速化するのであればサービス化をサポートしてみたい。