# Z80 Assembler 'AILZ80ASM' ベータ版
[![Build Status](https://ailight.visualstudio.com/AILight%20Games/_apis/build/status/AILight.AILZ80ASM_Test?branchName=main)](https://ailight.visualstudio.com/AILight%20Games/_build/latest?definitionId=8&branchName=main)

AILZ80ASMは、C#で書かれた.NET 6の環境で動作するZ80アセンブラです。

##### 読み方
- 「あいるぜっとはちまるあせむ」

##### 免責事項
- 現在、ベータ版です。不具合が残っている可能性がありますので、ご注意ください。
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

## パフォーマンス
- アセンブル時間
	- 処理時間: 3.67 sec
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
- -i, --input <files>        アセンブリ対象のファイルをスペース区切りで指定します。(オプション名の省略が可能）
- -ie, --input-encode <mode> 入力ファイルのエンコードを選択します。 [auto, utf-8, shift_jis] デフォルト:auto
- -o, --output <file>        出力ファイルを指定します。
- -om, --output-mode <mode>  出力ファイルのモードを選択します。 [bin, t88, cmt, sym, lst, equ, err] デフォルト:bin
- -oe, --output-encode <mode>出力ファイルのエンコードを選択します。 [auto, utf-8, shift_jis] デフォルト:auto
- -lm, --list-mode <mode>    リストの出力形式を選択します。 [simple, middle, full] デフォルト:full
- -ts, --tab-size <size>     TABのサイズを指定します。 デフォルト:4
- -dw,                       Warning、Informationをオフにするコードをスペース区切りで指定します。
- --disable-warning <codes>
- -ul, --unused-label        未使用ラベルを確認します。
- -cd,                       アセンブル実行時のカレントディレクトリを変更します。終了時に元に戻ります。
- --change-dir <directory>
- -df, --diff-file           アセンブル出力結果のDIFFを取ります。アセンブル結果は出力されません。
- -v, --version              バージョンを表示します。
- -?, -h, --help <help>      ヘルプを表示します。各オプションの詳細ヘルプを表示します。例： -h --output-mode
- -??, --readme              Readme.mdを表示します。

```
■ sample.z80をアセンブル、出力はBIN形式
> AILZ80ASM sample.z80

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
## 新バージョン導入の手順
 1. 新バージョンを入手
 1. インストール済みのAILZ80ASMでアセンブルを行う
 1. インストール済みのAILZ80ASMを退避
 1. 新バージョンでAILZ80ASMを置き換える
 1. 新バージョンでコマンドラインオプションに「-df」を付け、差分確認アセンブルを行う
 1. アセンブル結果が一致しているが表示される
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
ラベルは、ネームスペース、標準ラベル、ローカルラベルで指定します。ネームスペースを指定しない場合には、 'NS_Main' が割り当てられています。必要に応じて変更をしてください。
ラベルは指定した行以降は、下位のラベルを利用するときには上位のラベル名を省略することが出来ます。

#### ラベルの宣言
- [<ネームスペース名>]
  - デフォルト： `NS_Main`
```
[NS_Main]		; ネームスペース指定
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

#### ラベルの指定
- <ネームスペース>.<ラベル名>.<ローカルラベル名>
- <ラベル名>.<ローカルラベル名>
- <ローカルラベル名>

#### ラベルの機能
- .@H をラベルに追加すると、上位1バイトを取得 （.@HIGH でも可能）
- .@L をラベルに追加すると、下位1バイトを取得 （.@LOW  でも可能）
- .@T をラベルに追加すると、ラベルに指定した名前を文字列として取得 （.@TEXT でも可能）
- .@E をラベルに追加すると、ラベルが存在すると#TRUE、存在しないと#FALSE （.@EXISTS でも可能）
- [@H@Lサンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestLB_EQU_Test/Test.Z80)
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
	ld a,(addr.test)
	ld hl, addr.test
	ld hl, (addr.test)
```

## 即値
- 2進数：先頭に%、もしくは末尾にbを付けます。また _ を含める事が出来ます。
- 8進数：末尾にoを付けます。
- 10進数：数値だけを指定します。
- 16進数：先頭に$ or 0x もしくは末尾にHを付けます。
- 1文字:先頭と末尾に **'** を付けます。半角を使うと1バイトの数値として扱えます。
- 文字列:先頭と末尾に **"** を付けます。2バイトの数値として扱えます。

## 文字と文字列について
- １文字を扱うときには、 **'** で囲んでください。
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
| 2 | + - ! ~ | 正記号 負記号 論理NOT ビット反転 |
| 3 | * / % | 乗算 除算 剰余 |
| 4 | + - | 加算 減算 |
| 5 | << >> | 左シフト 右シフト |
| 6 | < > <= >= | 算術比較 |
| 7 | == != | 算術比較 |
| 8 | & | ビットAND |
| 9 | ^ | ビットXOR |
| 10 | \| | ビットOR |
| 11 | && | 論理AND |
| 12 | \|\| | 論理OR |
| 13 | : | 三項演算子 |
| 14 | ? | 三項演算子 |

## アドレス指定について
式に()を利用することが出来ますが、命令には()が含まれるものがあります。式の全体を()で囲むとアドレス指定をなります。
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
  - 先アドレスを指定した場合には、0で埋めます。<式3>を設定するとその値で埋めます。
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
- 移動により空いた領域には、0 または <式2> の値で埋められます。
- ALIGN以降のプログラム等の出力情報が無い場合には、出力結果は切り詰められます。
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestCS_ALIGN/Test.Z80)

#### DS <式>, [<式2>]
- ロケーションカウンタの値を、<式>で設定した値を加算した場所に移動します。
- <式>のバイト数、0で埋めます。<式2>を設定するとその値で埋めます
- DS以降のプログラム等の出力情報が無い場合には、出力結果は切り詰められます。
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestLB_DSDBSDWS_Test/Test.Z80#L9)

## 制御命令
#### <ラベル> EQU <式>
- 指定したラベルに、<式> の値を持たせます。
	- 即値、式、$、$$、（文字、文字列、#TRUE or #FALSE version:1.0.0以降）
- ローカルラベルで利用することも可能です。
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestLB_EQU_Test/Test.Z80)

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
- [使い方のサンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestLB_CharMap_Test/Test.Z80)
	
#### INCLUDE <ファイル名>, [<ファイルタイプ>], [<開始位置>], [<長さ>], [<文字変換:実装予定>]
ファイル名の内容を読み取り、その場所に展開します
- <ファイル名>は、ロードしたいファイル名を指定します。
- <ファイルタイプ>は、TEXT と BINARY が選択できます。省略するとTEXTになります。また短縮形 T, B が使えます。
- <開始位置>は、ファイルの読み出し開始位置が指定できます。（ファイルタイプがBINARYの時に有効）
- <長さ>は、ファイルの読み込み長さが指定できます。（ファイルタイプがBINARYの時に有効）
- <文字変換:実装予定>は、CHARMAP名を指定します。指定したCHARMAPのルールに従ってバイナリー展開されます。このオプションを使うときには、ファイルはUTF-8で保存してください。（ファイルタイプがBINARYの時に有効））
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
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestLB_DBDW_Test/Test.Z80#L4)

#### DB [<変数名>=<ループ開始値>..<ループ終了値>:<式>]
- ループの条件で、式の内容を展開します
- ネストも可能
- 例：DB [Y=0..2:[X=0..4:Y*8+X]]
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestLB_DBDW_Test/Test.Z80#L292)

#### DW <式>, [<式>]
- <式>の2バイト値を設定します
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestLB_DBDW_Test/Test.Z80#L5)

#### DW [<変数名>=<ループ開始値>..<ループ終了値>:<式>]
- ループの条件で、式の内容を展開します
- ネストも可能
- 例：DW [Y=24..0:$8000 + Y * $140]
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestLB_DBDW_Test/Test.Z80#L292)

#### DBF <式>, [<式2>]
- <式>のバイト数、0で埋めます。<式2>を設定するとその値で埋めます
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestLB_DSDBSDWS_Test/Test.Z80#L9)

#### DWF <式>, [<式2>]
- <式>の２バイト数、0で埋めます。<式2>を設定するとその値で埋めます
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestLB_DSDBSDWS_Test/Test.Z80#L11)

## マクロ
#### <マクロ名> MACRO [<引数1>, <引数2>]　～ ENDM
- MACROからENDMまでがマクロとして定義されます
- 引数に付けた名前がマクロ内で利用できます
- マクロ名に()を含める事が出来ます。ただし先頭に付ける事は出来ません
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestPP_MacroCompatible/Test.Z80)
```
ARG1	equ 2
.Three  equ 3
	
	ALLLD
	TestArg ARG1, ARG1.Three

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
```

#### REPT <式1> [LAST <式2>]　～ ENDM
- 式1に設定した値の回数分をREPTの中に記述してある命令を展開します
- 式2には、最終の展開時に削除したい命令数を負の値で設定します
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

## FUNCTION <名前>([<引数1>, <引数2>]) => <式>
式をまとめる事が出来ます
- 複数行にまたがる事は出来ません。
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestPP_Function/Test.Z80)

```
	LD A, ABS(-1)	; LD A, 1
	LD B, ABS(1)	; LD B, 1

Function ABS(value) => value < 0 ? value * -1 : value
```

## END
アセンブルの実行を中断します。これ以降のソースコードはアセンブルされません。アセンブル結果は出力されます。

## プリプロセッサ
#### 条件付きアセンブル
条件付きアセンブルを制御します
- #IF: 条件付きアセンブルを開始します。コードは、指定された条件がTRUEの時にアセンブル対象になります。
- #ELIF: 前の条件付きアセンブルを終了して、指定された条件がTRUEの時にアセンブル対象になります。
- #ELSE: 前の条件付きアセンブルを終了して、前条件がFALSEの時にアセンブル対象になります。
- #ENDIF: 条件付きアセンブルを終了します。
- #PRINT: アセンブル画面に情報を表示します。(将来実装予定)
- #ERROR: 無条件にエラーを発生させます。
- #TRUE: 真(bool型)
- #FALSE: 偽(bool型)
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestPP_Conditional/Test.Z80)
```
#if mode == 1
	ld a,1
#elif mode == 2
	ld d,4
#else
	#error "modeの値が範囲外です。"
#endif
```

ラベルの再定義エラーを回避する方法 (#pragma onceの仕様を推奨)
```
#if LABEL.@EXISTS
LABEL	equ 00FFH
#endif
```

#### PRAGMA (プラグマ)
###### PRAGMA ONCE
ソースコードをアセンブルするときに、記述があるファイルを1回だけアセンブルすることを指定します
```
#pragma once

LABEL	equ 00FFH
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
 - [エラーコード一覧](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM/Error.cs)

## 謝辞
- 内藤時浩様（サンプルコード）[プログラミング指南 - Code Knowledge](https://codeknowledge.livedoor.blog/)
- 山本昌志様（Z80命令セット）[Yamamoto's Laboratory](http://www.yamamo10.jp/yamamoto/index.html)
- 神楽坂朋様（Z80命令表）[神楽坂製作所](http://tomocyan.net/~kagurazaka/html/index2.html)
- Thomas Scherrer様（Z80 Undocumented Instructions） [Thomas Scherrer Z80-Family HomePage](http://www.z80.info/index.htm)

## Z80資料
- [Z80 CPU User Manual - Zilog](http://www.zilog.com/docs/z80/um0080.pdf)
