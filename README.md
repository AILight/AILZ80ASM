# Z80 Assembler 'AILZ80ASM'
[![Build Status](https://ailight.visualstudio.com/AILight%20Games/_apis/build/status/AILight.AILZ80ASM_Test?branchName=main)](https://ailight.visualstudio.com/AILight%20Games/_build/latest?definitionId=8&branchName=main)

AILZ80ASMは、C#で書かれた.NET 6の環境で動作するZ80アセンブラです。

##### 読み方
- 「あいるぜっとはちまるあせむ」

##### 免責事項
- ご利用は自己責任でお願いします。利用により損害等が発生したとき、作者は責任をおいかねます。

##### 連絡方法
- [不具合: GitHub Issues](https://github.com/AILight/AILZ80ASM/issues)
- [質問、提案: GitHub Discussions](https://github.com/AILight/AILZ80ASM/discussions)
- [上記内容+お気軽に: Twitter @AILight](https://twitter.com/AILight)

## 入手方法 
- 本ソースコードを、Visual Studio 2022でビルドする。
- 実行形式をダウンロードして利用する。 [ダウンロード](https://github.com/AILight/AILZ80ASM/releases)
	- 実行形式: win-x64版、osx-x64版、linux-x64版

※ 実行形式は、「[自己完結型の実行可能ファイル](https://docs.microsoft.com/ja-jp/dotnet/core/deploying/#publish-self-contained)」になっています。.NETの環境を用意する必要はありません。

## サポートツール
- [Visual Studio Code 拡張機能(色付け機能)](https://marketplace.visualstudio.com/items?itemName=AILight.ailight-z80-assembler)

## パフォーマンス
- アセンブル時間
	- 処理時間: 1.608 sec
	- 出力結果: 47,735 bytes
	- 処理行数: 約20,000行
- [ベンチマーク・プロジェクト](https://github.com/AILight/AILZ80ASM/tree/main/AILZ80ASM.Benchmark)

## 使い方
AILZ80ASM [<オプション>] <オプション指定文字列:ファイル名等> [ <オプション指定文字列:ファイル名等>]
```
> AILZ80ASM sample.z80
```
コンソールアプリですので、コンソールより起動してください。

##### 入力ファイル形式
- UTF-8、SHIFT_JIS
- ファイルの形式は自動的に判断されます。
- 誤認識する場合には、コマンドラインオプション(--input-encode)をお使いください。

## コマンドラインオプション
| コマンドライン | 説明
----|----
| -i, --input <files>            | アセンブリ対象のファイルをスペース区切りで指定します。 (オプション名の省略が可能)
| -ie, --input-encode <mode>     | 入力ファイルのエンコードを選択します。 [auto, utf-8, shift_jis] デフォルト値:auto
| -o, --output <file>            | 出力ファイルを指定します。
| -om, --output-mode <mode>      | 出力ファイルのモードを選択します。 [bin, hex, t88, cmt, sym, equ, lst, err, tag] デフォルト値:bin
| -oe, --output-encode <mode>    | 出力ファイルのエンコードを選択します。 [auto, utf-8, shift_jis] デフォルト値:auto
| -lm, --list-mode <mode>        | リストの出力形式を選択します。 [simple, middle, full] デフォルト値:full
| -lob, --list-omit-binary       | リストの出力でバイナリーインクルードを省略出力をします。
| -ep, --entry-point <address>   | エントリーポイントを指定します。
| -ts, --tab-size <size>         | TABのサイズを指定します。 デフォルト値:4
| -dw, --disable-warning <codes> | Warning、Informationをオフにするコードをスペース区切りで指定します。
| -ul, --unused-label            | 未使用ラベルを確認します。
| -cd, --change-dir <directory>  | アセンブル実行時のカレントディレクトリを変更します。終了時に元に戻ります。
| -gap, --gap-default <gapByte>  | アセンブラのギャップのデフォルト値を指定します。 デフォルト値:$FF
| -df, --list-file               | アセンブル出力結果のDIFFを取ります。アセンブル結果は出力されません。
| -df, --diff-file               | アセンブル出力結果のDIFFを取ります。アセンブル結果は出力されません。
| -nsa, --no-super-asm           | スーパーアセンブルモードを無効にします。
| -f, --force                    | 出力ファイルを上書きします。
| -v, --version                  | バージョンを表示します。
| -?, -h, --help <help>          | ヘルプを表示します。各オプションの詳細ヘルプを表示します。例： -h --input-mode
| -??, --readme                  | Readme.mdを表示します。

#### コマンドライン例
```
■ sample.z80をアセンブル、出力はBIN形式
> AILZ80ASM sample.z80

■ sample.z80をアセンブル、出力はBIN形式、上書き確認プロンプトを表示しないで上書きをする
> AILZ80ASM sample.z80 -f

■ sample.z80をアセンブル、出力はBIN形式、ログファイルを出力
> AILZ80ASM sample.z80 -bin -err

■ sample.z80をアセンブル、出力はBIN形式、LST形式、SYM形式、ログファイルを出力
> AILZ80ASM sample.z80 -bin -lst -sym -err

■ sample.z80をアセンブル、出力はCMT形式
> AILZ80ASM sample.z80 -cmt
> AILZ80ASM sample.z80 -om cmt

■ sample.z80をアセンブル、出力はBIN形式、CMT形式、リストの出力（形式：シンプル、タブサイズ8）
> AILZ80ASM sample.z80 -bin -cmt -lst -lm simple -ts 8

■ sample.z80をアセンブル、出力はBIN形式、ファイル名は、output.bin
> AILZ80ASM sample.z80 -bin output.bin
> AILZ80ASM -i sample.z80 -o output.bin -om bin

■ sample.z80をアセンブル、出力はBIN形式、指定（W0001,W9001,W9002）のワーニング表示をOFFにする
> AILZ80ASM sample.z80 -bin output.bin -dw W0001 W9001 W9002

■ -omオプションのヘルプを表示
> AILZ80ASM -h -om
```

## コマンドライン 戻り値
- 0: アセンブルが正常に終了
- 1: アセンブルにエラーがあり、処理途中で終了、--diff-file オプションで出力ファイルの不一致あり
- 2: コマンドライン引数に間違いがあり、終了
- 3: 内部エラーが発生、アセンブルの強制終了

## コマンドライン デフォルト値の設定
EXEと同じフォルダに以下の形式で「AILZ80ASM.json」を保存
- [AILZ80ASM.json](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM/Samples/Profiles/AILZ80ASM.json)
```
{
  "default-options": [
    "-err"
  ],
  "disable-warnings": [
    "W0001",
    "W9001",
    "W9002"
  ]
}
```

## スーパーアセンブルモードについて
特定のエラーがアセンブル時に発生した場合、再アセンブルを行ってエラーを解決するためのモードです。このモードを適用する前に、アセンブル解析の結果を確認し、再アセンブルによってエラーが解決できる可能性がある場合に限り、このモードが実行されます。このモードは、以下のエラーに対して有効です。

- E0010: 出力アドレスに影響する場所では$$は使えません。
- E0010: 参照したラベルのプログラムアドレスが確定できませんでした。

※ スーパーアセンブルモードを無効にする場合には、コマンドラインオプションに '-nsa' を付けてください

## エントリーポイントについて
CMT形式の読み込みアドレスに利用されるエントリーポイントは、以下の優先順位で利用されます。
 1. コマンドラインオプション (-ep) で指定した値
 1. END で指定した値
 1. アセンブル結果が出力される最初のプログラムアドレス

## 新バージョン導入の手順
 1. 新バージョンを入手
 1. インストール済みのAILZ80ASMでアセンブルを行う
 1. インストール済みのAILZ80ASMを退避
 1. 新バージョンでAILZ80ASMを置き換える
 1. 新バージョンでコマンドラインオプションに「-df」を付け、差分確認アセンブルを行う
 1. アセンブル結果が一致しているかが表示される
 1. アセンブル結果が一致していたら置き換え可能、一致していなければ置き換え不可なので退避したプログラムを元に戻す

# ソースコード書式

## ニーモニック
- ザイログ・ニーモニックの表記
- 大文字、小文字の区別はしない 
- [本アセンブラで利用できるニーモニック一覧](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestRD_ALL/Test.Z80)

## 未定義命令 (Undocumented Instructions) 
- [IXH, IXL, IYH, IYLに対応](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestUD_LD/Test.Z80)
- [SLL](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestUD_SLL/Test.Z80)
- [IN F,(C)](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestUD_IN_And_OUT/Test.Z80#L1), [IN (C)](https://github.com/AILight/AILZ80ASM/blob/d38775a0854778fe82b36bce6fbf7a6fdf5e0c78/AILZ80ASM.Test/Test/TestUD_IN_And_OUT/Test.Z80#L3)
- [OUT (C), 0](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestUD_IN_And_OUT/Test.Z80#L2)
- [ビット操作](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/ReleaseDecision/InstructionSet.z80#L786)

## ラベル
ラベルは、ネームスペース、標準ラベル、ローカルラベルで指定します。ネームスペースを指定しない場合には、 'NAME_SPACE_DEFAULT' が割り当てられています。必要に応じて変更をしてください。ラベルは指定した行以降は、下位のラベルを利用するときには上位のラベル名を省略することが出来ます。テンポラリーラベルは、.@に数字で宣言します。

#### ラベルに使用できる文字と出来ない文字列
- ラベルに使用できる文字
  - 英文字: a-zA-Z
  - 数字: 0-9
  - 記号: -
- ラベルに使用できない文字列
  - 先頭が数字の文字列
  - 数値に変換できる文字列
  - 予約文字列（命令名、レジスタ名、フラグ名）
- テンポラリーラベルに使用できる文字
  - 数字: 0-9

#### 大文字と小文字の区別
- 大文字と小文字は区別しません

#### ラベルの宣言
- [<ネームスペース名>]
  - デフォルト： `NAME_SPACE_DEFAULT`
```
[NAME_SPACE_DEFAULT]		; ネームスペース指定
```
- <ラベル名[:]>
  - 標準ラベル
```
LABLE:			; ラベル指定
```
- <[.]ローカルラベル名>
  - ローカルラベル
```
LABLE:			; ラベル指定
.A			; ローカルラベル
.B			; ローカルラベル
```
- <[.]@テンポラリラベル番号>
  - テンポラリー
```
LABLE:			; ラベル指定
.A			; ローカルラベル
.@1			; テンポラリーラベル
.B			; ローカルラベル
.@1			; テンポラリーラベル
```

#### ラベルの指定
- <ネームスペース>.<ラベル名>.<ローカルラベル名>
- <ラベル名>.<ローカルラベル名>
- <ローカルラベル名>

#### テンポラリーラベルの指定
ローカルラベルが存在する場合
- <ネームスペース>.<ラベル名>.<ローカルラベル名>.<テンポラリーラベル名>
- <ラベル名>.<ローカルラベル名>.<テンポラリーラベル名>
- <ローカルラベル名>.<テンポラリーラベル名>
- <テンポラリーラベル名>
ローカルラベルが存在しない場合
- <ネームスペース>.<ラベル名>..<テンポラリーラベル名>
- <ラベル名>..<テンポラリーラベル名>
- <テンポラリーラベル名>

#### ラベルの機能
- .@H をラベルに追加すると、上位1バイトを取得 （.@HIGH でも可能）
- .@L をラベルに追加すると、下位1バイトを取得 （.@LOW  でも可能）
- .@T をラベルに追加すると、ラベルに指定した名前を文字列として取得 （.@TEXT でも可能）
- .@E をラベルに追加すると、ラベルが存在すると#TRUE、存在しないと#FALSE （.@EXISTS でも可能）
- [@H@Lサンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestLB_EQU/Test.Z80#L18)
- [@Tサンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestPP_MacroEx/Test.Z80#L10)
- [@Eサンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestPP_Include/Test.INC#L1)

#### ラベル指定サンプル
```
	org $8000
	ld a, 0
addr:
	ld a, (addr)
	ld hl, addr
.test
	ld a, (addr.test)
	ld hl, addr.test
	ld hl, (addr.test)
```

#### テンポラリーラベルの指定サンプル
```
	org $8000
	ld a, 0
addr:
	ld a, (addr)
.@1						; (1) addr..@1
	ld hl, addr
.test
	ld a,(.@1)			; 参照先 (2) 【注意:(1)は参照されない】
.@1						; (2) addr.text.@1
	ld hl, addr.test
.@2						; (3) addr.text.@2
	ld hl, (addr.test)
	; 参照
	ld hl, (addr..@1) 	; 参照先 (1)
	ld hl, (.text.@1) 	; 参照先 (2)
	ld hl, (.@1) 		; 参照先 (2)
	ld hl, (.@2) 		; 参照先 (3)
	; 特殊アクセス
EQT:
.@1	equ	%00001100	; (4) EQT..@1
.@2	equ	%00001101	; (5) EQT..@2
.@3	equ	%00001110	; (6) EQT..@3
	;【マジック実装】本来ならEQT.AL.@1 を参照するが、存在しないとき EQT..@1を参照する
.AL equ	(.@1 | .@2 | .@3) ; 参照先 (4) | (5) | (6)
```

## 即値
- 2進数：先頭に0b or %、もしくは末尾にbを付けます。また _ を含める事が出来ます。
- 8進数：先頭に0o、もしくは末尾にoを付けます。
- 10進数：数値だけを指定します。
- 16進数：先頭に0x or $、もしくは末尾にHを付けます。
- 文字列:先頭と末尾に **'** を付けます。値が要求される場所では、最大2バイトの数値として扱えます。
- 文字列:先頭と末尾に **"** を付けます。値が要求される場所では、最大2バイトの数値として扱えます。
- BOOL型:真:#TRUE、偽:#FALSE

## 文字と文字列について
- 文字列を扱うときには、 **'** で囲んでください。
- 文字列を扱うときには、 **"** で囲んでください。
- 文字列は、SJISとして扱います。
- SJIS以外を扱うには
	- 文字列に@<CHARMAP名>:"あいうえお" と記述するとCHARMAPの情報で変換を行います。
	- アセンブラに内蔵されているCHARMAPは、シフトJIS、JIS第1水準、JIS第1水準・第2水準です。
	- CHARMAPを使うと、独自の変換テーブルを使う事が可能です。
```
	DB "テスト"        ; SJISで変換、アセンブラのデフォルト値
	DB @JIS12:"テスト" ; JIS第一・二水準で変換
	DB @SJIS:"テスト"  ; SJISで変換
```

### エスケープシーケンス
| エスケープ シーケンス | 表現 | Unicode エンコーディング
----|----|----
| \\' | 単一引用符「'」 | 0x0027 |
| \\" | 単一引用符「"」 | 0x0022 |
| \\\\ | 円記号「\\」 | 0x005C |
| \0 | Null | 0x0000 |
| \a | ベル (警告) | 0x0007 |
| \b | バックスペース | 0x0008 |
| \f | フォーム フィード | 0x000C |
| \n | 改行 | 0x000A |
| \r | キャリッジ リターン | 0x000D |
| \t | 水平タブ | 0x0009 |
| \v | 垂直タブ | 0x000B |

### 内蔵CHARMAP一覧
| 名前 | デフォルト | 詳細
----|----|----
| @SJIS | * | シフトJIS
| @JIS1 |  | JIS第1水準
| @JIS12 |  | JIS第1水準・第2水準
	

## ロケーションカウンタ
- $  は、現在のプログラム・ロケーションカウンタを参照することができます
- $$ は、現在のアウトプット・ロケーションカウンタを参照することができます

`詳細は、ORG命令の章をご覧ください`

## 式
値を指定するところに、式を使用することができます。使用できる演算子と優先順位は、下の表のとおりです。
| 優先順位 | 演算子 | コメント |
----|----|----
| 1 | ( ) | 括弧 |
| 2 | low high exists text | 下位8ビット 上位8ビット ラベル存在 ラベル値文字列 |
| 2 | backward forward near far | 後方ラベル 前方ラベル 近いラベル 遠いラベル |
| 3 | + - ! ~ | 正記号 負記号 論理NOT ビット反転 |
| 4 | * / % | 乗算 除算 剰余 |
| 5 | + - | 加算 減算 |
| 6 | << >> | 左シフト 右シフト |
| 7 | < > <= >= | 算術比較 |
| 8 | == != | 算術比較 |
| 9 | & | ビットAND |
| 10 | ^ | ビットXOR |
| 11 | \| | ビットOR |
| 12 | && | 論理AND |
| 13 | \|\| | 論理OR |
| 14 | : | 三項演算子 |
| 15 | ? | 三項演算子 |
※大文字と小文字は区別しません

#### low 演算子
下位8ビットを取得します
```
	ld a, low $1234 ; a = $34
```

#### high 演算子
上位8ビットを取得します
```
	ld a, high $1234 ; a = $12
```

#### exists 演算子
ラベルが存在する時には、 #TRUEを返します。ラベルが存在しない時には、#FALSE を返します。
```
LB1	equ $10

	#IF exists LB1
		;展開されます
	#ENDIF
	
	#IF exists LB2
		;展開されません
	#ENDIF

```

#### text 演算子
ラベルに設定された文字列を取得します。
```
	LDEX a, 1
	LDEX b, 1

LDEX MACRO arg1, arg2
	#IF text arg1 == "a"
		ld a, arg2
	#ELSE
		ld b, arg2
	#ENDIF

	ENDM
```

#### backward 演算子
後方のラベルを指定します。
```
LB:
.Sub1
.@1				; ここが参照されています
.Sub2
	JP backward .@1
.Sub3
.@1
```

#### forward 演算子
前方のラベルを指定します。
```
LB:
.Sub1
.@1
.Sub2
	JP forward .@1
.Sub3
.@1				; ここが参照されています
```

#### near 演算子
プログラムアドレスが近いラベルを参照します。距離が同じ場合には後方のラベルが優先されます。

#### far 演算子
プログラムアドレスが遠いラベルを参照します。距離が同じ場合には後方のラベルが優先されます。

## アドレス指定について
式に()を利用することが出来ますが、命令には()が含まれるものがあります。式の全体を()で囲むとアドレス指定になります。
```
	ld a,($1234 + 5)	;アドレス指定
	ld a,($1234) + 5	;値として指定されます。16bit値が指定されているので、ワーニングが表示されます。
```
	
## コメント
- ; 以降はコメントとして処理されます

## アドレス・制御命令
#### ORG <式>, [<式2>], [<式3>]
本アセンブラの特徴として、アセンブル時のアドレスとアウトプット時のアドレスを別で管理しています。通常モードで利用する場合には、第一引数だけで利用してください。アウトプットアドレスは自動的に制御されます。第二引数を利用した時には、アウトプットアドレスを制御できます。その時にはROM出力モードになり、ラベルに使われるアドレスと出力のアドレスを別々に制御することが出来ます。プログラム・ロケーションカウンターは、アセンブル内で重複可能になります。アウトプット時のアドレスは、アウトプット・ロケーションカウンターと呼び、バイナリデータの出力位置を指定します。出力用のアドレスになりますので、重複することは出来ません。
	
- <式>
  - プログラム・ロケーションカウンターの値を変更します。
  - 出力されるラベルの計算に影響を与えます。プログラム中で何度でも変更可能です。ROM出力モードでは、アドレスの重複も可能です。
- <式2>
  - アウトプット・ロケーションカウンターの値を変更します。こちらを指定するとROM出力モードになります。
  - 出力されるバイナリーに影響を与えます。プログラム中で何度でも変更可能です。アドレスの重複は不可能です。
  - バイナリーの出力を制御したいときに使用します
- <式3>
  - 先アドレスを指定した場合には、<ギャップ値>で埋めます。<式3>を設定するとその値で埋めます。
- メモリのアライメントを合わせるには、ALIGNをお勧めします。
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestCS_ORG/Test.Z80)

```
	ORG $1000
LB1000:
	LD A, (LB1000)

	ORG $1010
LB1010:
	LD A, (LB1010)

	ORG $2000, $0020
LB2000:
	LD A, (LB2000)
```
出力結果
```
0000 3A 00 10 00 00 00 00 00 00 00 00 00 00 00 00 00
0010 3A 10 10 00 00 00 00 00 00 00 00 00 00 00 00 00
0020 3A 00 20 00 00 00 00 00 00 00 00 00 00 00 00 00
```

#### ALIGN <式>, [<式2>]
- ロケーションカウンタの値を、<式>で設定したアライメント境界まで移動します。
- <式>に設定できる値は、2のべき乗である必要があります。
- 移動により空いた領域には、<ギャップ値> または <式2> の値で埋められます。
- ALIGN以降のプログラム等の出力情報が無い場合には、出力結果は切り詰められます。
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestCS_ALIGN/Test.Z80)

#### DS <式>, [<式2>]
- ロケーションカウンタの値を、<式>で設定した値を加算した場所に移動します。
- <式>のバイト数、<ギャップ値>で埋めます。<式2>を設定するとその値で埋めます
- DS以降のプログラム等の出力情報が無い場合には、出力結果は切り詰められます。
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestLB_DSDBSDWS/Test.Z80#L8)

## 制御命令
#### <ラベル> EQU <式>
- 指定したラベルに、<式> の値を持たせます。
	- 即値、式、$、$$、（文字、文字列、#TRUE or #FALSE version:1.0.0以降）
- ローカルラベルで利用することも可能です。
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestLB_EQU/Test.Z80)

```
PORT_A  equ $CC
.WRITE  equ $01
.READ   equ $02

	LD A, PORT_A.WRITE
	OUT (PORT_A), A

	LD A, PORT_A.READ
	OUT (PORT_A), A
```

#### CHARMAP <CHARMAP名>, <ファイル名>
アセンブラ内で利用する、文字列の変換テーブルをロードします。
- <CHARMAP名>は、先頭に@を付ける形で命名します。例:@SJIS
- <ファイル名>は、変換テーブルをロードします。
- ファイル形式
	1. Json形式(UTF-8 with BOM)
	1. [ファイル形式のサンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM/CharMaps/SJIS.json)
- [使い方のサンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestLB_CharMap/Test.Z80)
	
#### INCLUDE <ファイル名>, [<ファイルタイプ>], [<開始位置>], [<長さ>]
ファイル名の内容を読み取り、その場所に展開します
- <ファイル名>は、ロードしたいファイル名を指定します。
- <ファイルタイプ>は、TEXT と BINARY が選択できます。省略するとTEXTになります。また短縮形 T, B が使えます。
- <開始位置>は、ファイルの読み出し開始位置が指定できます。（ファイルタイプがBINARYの時に有効）
- <長さ>は、ファイルの読み込み長さが指定できます。（ファイルタイプがBINARYの時に有効）
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestPP_Include/Test.Z80)
```
include "Test.inc"			; テキストファイルとして展開されます
include "Test.inc", B			; バイナリーファイルとして展開されます
include "Test.inc", B, $1000		; バイナリーファイルとして展開されます。読み込み位置は、$1000からになります。
include "Test.inc", B, $1000, 200	; バイナリーファイルとして展開されます。読み込み位置は、$1000からになります。読み込み長さは200バイトです。
include "Test.inc", B, , 200		; バイナリーファイルとして展開されます。読み込み位置は、$0000からになります。読み込み長さは200バイトです。
```

#### DB <式>, [<式>]
- <式>の1バイト値を設定します
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestLB_DBDW/Test.Z80#L8)

#### DB [<変数名>=<ループ開始値>..<ループ終了値>:<式>]
- ループの条件で、式の内容を展開します
- ネストも可能
- 例：DB [Y=0..2:[X=0..4:Y*8+X]]
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestLB_DBDW/Test.Z80#L290)

#### DW <式>, [<式>]
- <式>の2バイト値を設定します
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestLB_DBDW/Test.Z80#L296)

#### DW [<変数名>=<ループ開始値>..<ループ終了値>:<式>]
- ループの条件で、式の内容を展開します
- ネストも可能
- 例：DW [Y=24..0:$8000 + Y * $140]
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestLB_DBDW/Test.Z80#L292)

#### DBFIL <式>, [<式2>]
- <式>のバイト数、0で埋めます。<式2>を設定するとその値で埋めます
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestLB_DSDBSDWS/Test.Z80#L9)

#### DWFIL <式>, [<式2>]
- <式>の２バイト数、0で埋めます。<式2>を設定するとその値で埋めます
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestLB_DSDBSDWS/Test.Z80#L10)

## マクロ
#### <マクロ名> MACRO [<引数1>, <引数2>]　～ ENDM
- MACROからENDMまでがマクロとして定義されます
- 引数に付けた名前がマクロ内で利用できます
- マクロ名に()を含める事が出来ます。ただし先頭に付ける事は出来ません
- [サンプル1](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestPP_MacroCompatible/Test.Z80)
- [サンプル2](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestPP_MacroEx/Test.Z80)
- [サンプル3](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestPP_MacroRegister/Test.Z80)

```
ARG1	equ 2
.Three  equ 3
	
	ALLLD
	TestArg ARG1, ARG1.Three
	INITLD B	; レジスター名を指定

ALLLD MACRO
	ld a,1
	ld b,2
	ld c,3
	ld d,4
	ld e,5
	ld h,6
	ld l,7
	ENDM

TestArg MACRO a1, a2
	ld a, a1
	ld b, a2
	ENDM

INITLD MACRO REG
	LD A, REG
	LD REG, 0
	ENDM
```

#### REPT <式1> [LAST <式2>]　～ ENDM
- 式1に設定した値の回数分をREPTの中に記述してある命令を展開します。
- 式2には、最終の展開時に削除したい命令数を負の値で設定します。ラベルだけの行や空行は削除対象外です。
- ネストに対応しています
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestPP_RepeatCompatible/Test.Z80)

```
	REPT 3
        xor     a
	ENDM

	REPT 8 LAST -1
        ld      (hl), a
        set     5, h
        ld      (hl), a
        add     hl, de
	ENDM
```
※ LAST -1で消える行にラベルが設定されている場合にはエラーになります。2行に分ける等の工夫をしてください。
```
	REPT 8 LAST -1
        ld      (hl), a
        set     5, h
        or		a
		jr		z, .@1
        add     hl, de
.@1		add     hl, de
	ENDM
```

#### <名前> ENUM ～ <ラベル名> : <長さ> = <値> ～ ENDM
- ENUMは、名前とラベル名で組み合わされたEQUが定義されます。
- <ラベル名>は、<名前>.<ラベル名> 形式でアクセスが可能です。
- <長さ>は、次の要素の値を決める時の加算値になります。（デフォルトは1）
- <値>は、はその要素に設定される値を表します。（デフォルトは、前の要素の<値>+<長さ>）

```
Color ENUM
    RED = 5          ; 5
    GREEN            ; 6
    BLUE :2          ; 7, サイズ2バイト
    YELLOW           ; 9
    ORANGE :2 = 12   ; 12, サイズ2バイト
    CYAN             ; 14
    PURPLE = RED-1   ; 4
ENDM

INIT:
    LD  A, Color.RED	; 5
    LD  B, Color.GREEN	; 6
    LD  C, Color.BLUE	; 7
    LD  D, Color.YELLOW	; 9
    LD  E, Color.ORANGE	; 12
    LD  H, Color.CYAN	; 14
    LD  L, Color.PURPLE	; 4
```

## FUNCTION <名前>([<引数1>, <引数2>]) => <式>
式をまとめる事が出来ます
- 複数行にまたがる事は出来ません。
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestPP_Function/Test.Z80)

```
	LD A, ABS(-1)	; LD A, 1
	LD B, ABS(1)	; LD B, 1

Function ABS(value) => value < 0 ? value * -1 : value
```

## END [<式1>]
アセンブルの実行を中断します。これ以降のソースコードはアセンブルされません。アセンブル結果は出力されます。
-  式1に設定した値は、エントリーポイントに使われます。利用個所: CMT出力

## コード・チェック
#### CHECK ALIGN <式1>[, <式2>]　～　ENDC
CHECKからENDCで囲まれた範囲のアセンブル結果がアライメント境界を超えるとアセンブルエラー(E6002)になります。
- 式2に設定した値は、アライメント境界のオフセット値になります。2バイトデータの先頭だけアライメント境界内に入っている事を保証したいときに、2と設定します。
```
	org 0x100
                                
	CHECK ALIGN 256
	DB 0, 1, 2, 3 ,4
	ENDC
                            
	org 0x1FF
                               
    CHECK ALIGN 256 ; **** E6002 ****
	DB 0, 1, 2, 3 ,4
    ENDC
```

## プリプロセッサ
#### 条件付きアセンブル
条件付きアセンブルを制御します
- #IF: 条件付きアセンブルを開始します。コードは、指定された条件がTRUEの時にアセンブル対象になります。
- #ELIF: 前の条件付きアセンブルを終了して、指定された条件がTRUEの時にアセンブル対象になります。
- #ELSE: 前の条件付きアセンブルを終了して、前条件がFALSEの時にアセンブル対象になります。
- #ENDIF: 条件付きアセンブルを終了します。
- #ERROR: 無条件にエラーを発生させます。
- #TRUE: 真(bool型)
- #FALSE: 偽(bool型)
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestPP_Conditional/Test.Z80)
```
#if mode == 1
	ld a,1
	#print "mode == 1の処理がされました"
#elif mode == 2
	ld d,4
#else
	#error "modeの値が範囲外です。"
#endif
```

###### #PRINT <引数1> [<引数2>]
アセンブルの情報をインフォメーション[I0001]として表示します。

- 引数1: 出力の文字列を設定します。{#}を指定すると、引数2以降の値を含めて出力出来ます。
- 引数2: 引数1で指定したフォーマットに表示する値を設定します。

```
TEST1	EQU 1
TEST2	EQU 2

	#PRINT "TEST1:{0}, TEST2:{1}", TEST1, TEST2
```

#### PRAGMA (プラグマ)
###### PRAGMA ONCE
ソースコードをアセンブルするときに、記述があるファイルを1回だけアセンブルすることを指定します
```
#pragma once

LABEL	equ 00FFH
```

ラベルの再定義エラーを回避する方法 (#pragma onceの利用を推奨)
```
#if LABEL.@EXISTS
LABEL	equ 00FFH
#endif
```
```
#if exists LABEL
LABEL	equ 00FFH
#endif
```

###### #LIST <引数1>
LST形式のファイルの出力を停止します。

- 引数1: bool型をしています。TRUE: 出力あり, FALSE: 出力なし

```
on  equ #TRUE
off equ #FALSE

    ld  a, 0

    #LIST off

    ld  b, 1

    #LIST on

    ld  c, 2
    
    ret
```

## 表記の揺れ対応
- (IX) → (IX+0)
- (IY) → (IY+0)
- SUB A, → SUB
- EX HL,DE → EX DE,HL
- .local: → .local

## エラー
- レベル分けされており、E:Error,W:Warning,I:Information があります。
- Errorに該当する行がある場合には、ソースコードは最後まで評価されますが、アセンブル結果は出力されません。
- [エラーコード一覧](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM/Assembler/Error.cs#L142)

## 仕様の裏話
- AILZ80ASMでのENDMは、「END Macro」ではなく「End of Multi-Purpose Block」と言い張っていますので、色々な命令で使われています。

## 謝辞
- 内藤時浩様（サンプルコード）[プログラミング指南 - Code Knowledge](https://codeknowledge.livedoor.blog/)
- 山本昌志様（Z80命令セット）[Yamamoto's Laboratory](http://www.yamamo10.jp/yamamoto/index.html)
- 神楽坂朋様（Z80命令表）[神楽坂製作所](http://tomocyan.net/~kagurazaka/html/index2.html)
- Thomas Scherrer様（Z80 Undocumented Instructions） [Thomas Scherrer Z80-Family HomePage](http://www.z80.info/index.htm)

## Z80資料
- [Z80 CPU User Manual - Zilog](http://www.zilog.com/docs/z80/um0080.pdf)
