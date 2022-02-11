# Z80 Assembler 'AILZ80ASM' ベータ版
[![Build Status](https://ailight.visualstudio.com/AILight%20Games/_apis/build/status/AILight.AILZ80ASM_Test?branchName=main)](https://ailight.visualstudio.com/AILight%20Games/_build/latest?definitionId=8&branchName=main)

AILZ80ASMは、C#で書かれた.NET 6の環境で動作するZ80アセンブラです。

##### 読み方
- 「あいるぜっとはちまるあせむ」

##### 免責事項
- 現在、ベータ版です。不具合が残っている可能性がありますので、ご注意ください。
- ご利用は自己責任でお願いします。利用により損害等が発生したとき、作者は責任をおいかねます。

## 入手方法 
- 本ソースコードを、Visual Studio 2019でビルドする。
- 実行形式をダウンロードして利用する。 [ダウンロード](https://github.com/AILight/AILZ80ASM/releases)

※ 実行形式は、「[自己完結型の実行可能ファイル](https://docs.microsoft.com/ja-jp/dotnet/core/deploying/#publish-self-contained)」になっています。.NETの環境を用意する必要はありません。

## 使い方
AILZ80ASM [<オプション>] <オプション指定文字列:ファイル名等> [ <オプション指定文字列:ファイル名等>]
```
> AILZ80ASM sample.z80
```
##### 入力ファイル形式
- UTF-8、SHIFT_JIS
- ファイルの形式は自動的に判断されます。
- 誤認識する場合には、コマンドラインオプション(--encode)をお使いください。

## コマンドラインオプション
- -i, --input <files>          アセンブリ対象のファイルをスペース区切りで指定します。
- -en, --encode <mode>         ファイルのエンコードを選択します。（UTF-8、SHIFT_JIS）
- -o, --output <file>          出力ファイルを指定します。
- -om, --output-mode <outMode> 出力ファイルのモードを選択します。（デフォルト：BIN、T88、CMT）
- -s, --symbol <symbol>        シンボルファイルを指定します。
- -l, --list <list>            リストファイルを指定します。
- -lm,--list-mode <mode>       リストの出力形式を指定します。（デフォルト：full、simple、middle）
- -e, --error <file>           アセンブル結果を出力します。
- -t, --trim                   DSで確保したメモリが、出力データの最後にある場合にトリムされます。
- -dw,                         Warning、Informationをオフにするコードをスペース区切りで指定します。
- --disable-warning <codes>
- -v, --version                バージョンを表示します。
- -?, -h, --help <help>        ヘルプを表示します。各オプションの詳細ヘルプを表示します。例： -h --input-mode

```
■ sample.z80をアセンブル、出力はBIN形式
> AILZ80ASM sample.z80

■ sample.z80をアセンブル、出力はBIN形式、ログファイルを出力
> AILZ80ASM sample.z80 -e

■ sample.z80をアセンブル、出力はCMT形式
> AILZ80ASM sample.z80 -cmt
> AILZ80ASM sample.z80 -om cmt

■ sample.z80をアセンブル、出力はBIN形式、CMT形式、リストの出力（形式：シンプル）、DSをTrim
> AILZ80ASM sample.z80 -bin -cmt -l -t -lm simple

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
- 1: アセンブルにエラーがあり、処理途中で終了
- 2: コマンドライン引数に間違いがあり、終了
- 3: 内部エラーが発生、アセンブルの強制終了

## コマンドライン デフォルト値の設定
EXEと同じフォルダに以下の形式で「AILZ80ASM.json」を保存
- [AILZ80ASM.json](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM/Samples/Profiles/AILZ80ASM.json)
```
{
  "default-options": [
    "-e",
    "-t"
  ],
  "disable-warnings": [
    "W0001",
    "W9001",
    "W9002"
  ]
}
```

# ソースコード書式

## ニーモニック
- ザイログ・ニーモニックの表記
- 大文字、小文字の区別はしない 
- [本アセンブラで利用できるニーモニック一覧](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestRD_ALL/Test.Z80)

## 未定義命令 (Undocumented Instructions) 
- [IXH, IXL, IYH, IYLに対応](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestUD_LD/Test.Z80)
- [SLL](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestUD_SLL/Test.Z80)
- [IN F,(C)](https://github.com/AILight/AILZ80ASM/blob/d38775a0854778fe82b36bce6fbf7a6fdf5e0c78/AILZ80ASM.Test/Test/TestUD_IN_And_OUT/Test.Z80#L1), [IN (C)](https://github.com/AILight/AILZ80ASM/blob/d38775a0854778fe82b36bce6fbf7a6fdf5e0c78/AILZ80ASM.Test/Test/TestUD_IN_And_OUT/Test.Z80#L3)
- [OUT (C), 0](https://github.com/AILight/AILZ80ASM/blob/d38775a0854778fe82b36bce6fbf7a6fdf5e0c78/AILZ80ASM.Test/Test/TestUD_IN_And_OUT/Test.Z80#L2)
- [ビット操作](https://github.com/AILight/AILZ80ASM/blob/e445250854525bc124ff6156ee328d085be31028/AILZ80ASM.Test/ReleaseDecision/InstructionSet.z80#L786)

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
- [@H@Lサンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestLB_EQU_Test/Test.Z80)
- [@Tサンプル](https://github.com/AILight/AILZ80ASM/blob/74dd29bfbceda9c986ed50fe651b0c0d89127637/AILZ80ASM.Test/Test/TestPP_MacroEx/Test.Z80#L10)

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
- 2進数、10進数、16進数
- 2進数：先頭に%、もしくは末尾にbを付けます。また _ を含める事が出来ます。
- 10進数：何もつけません
- 16進数：先頭に$ or 0x もしくは末尾にHを付けます

## 文字と文字列について
- １文字を扱うときには、 **'** で囲んでください。
- 文字列を扱うときには、 **"** で囲んでください。

## 文字と文字列について（現在仕様検討中）
- エスケープシーケンスは未対応、今後対応予定
- 文字コードの指定
	- デフォルト値(SHIFT_JIS)
	- 即値指定：DB @SJIS"あいうえお"
	- 全体で指定する場合：CHARMAP @SJIS
- 定義ファイル
	- 検索順
		1. 作業ディレクトリ
		1. EXEディレクトリ
		1. EXE内包ファイル (SJIS)
	- ファイル形式
		1. Json形式（対応表）

	
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

## 制御命令
#### ORG <式>, [<式2>], [<式3>]
本アセンブラの特徴として、アセンブル時のアドレスとアウトプット時のアドレスを別で管理しています。通常モードで利用する場合には、第一引数だけで利用してください。アウトプットアドレスは自動的に制御されます。第二引数を利用した時には、アウトプットアドレスを制御できます。その時にはROM出力モードになり、ラベルに使われるアドレスと出力のアドレスを別々に制御することが出来ます。プログラム・ロケーションカウンターは、アセンブル内でいつでも変更可能になります。1つのソースコードの中でアドレスが重複しても問題ありません。アウトプット時のアドレスは、アウトプット・ロケーションカウンターと呼び、バイナリデータの出力位置を指定します。出力用のアドレスになりますので、重複することは出来ませんし、アドレスを戻すことも出来ません。
	
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

#### <ラベル> EQU <式>
- 指定したラベルに、<式> の値を持たせます。
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

#### ALIGN <式>, [<式2>]
- ロケーションカウンタの値を、<式>で設定したアライメント境界まで移動します。
- <式>に設定できる値は、2のべき乗である必要があります。
- 移動により空いた領域には、0 または <式2> の値で埋められます。
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/main/AILZ80ASM.Test/Test/TestCS_ALIGN/Test.Z80)

#### INCLUDE [<ファイル名>], [<ファイルタイプ>], [<開始位置>], [<長さ>]
- ファイル名の内容を、その場所に展開します
- ファイルタイプ：TEXT と BINARY が選択できます。省略するとTEXTになります。また短縮形 T, B が使えます。
- 開始位置:ファイルの読み出し開始位置が指定できます。（ファイルタイプがBINARYの時に有効）
- 長さ:ファイルの読み込み長さが指定できます。（ファイルタイプがBINARYの時に有効）
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
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/31c6fb2d12272e558538f96fd38e05c27bd7943e/AILZ80ASM.Test/Test/TestLB_DBDW_Test/Test.Z80#L4)

#### DB [<変数名>=<ループ開始値>..<ループ終了値>:<式>]
- ループの条件で、式の内容を展開します
- ネストも可能
- 例：DB [Y=0..2:[X=0..4:Y*8+X]]
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/31c6fb2d12272e558538f96fd38e05c27bd7943e/AILZ80ASM.Test/Test/TestLB_DBDW_Test/Test.Z80#L292)

#### DW <式>, [<式>]
- <式>の2バイト値を設定します
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/31c6fb2d12272e558538f96fd38e05c27bd7943e/AILZ80ASM.Test/Test/TestLB_DBDW_Test/Test.Z80#L5)

#### DW [<変数名>=<ループ開始値>..<ループ終了値>:<式>]
- ループの条件で、式の内容を展開します
- ネストも可能
- 例：DW [Y=24..0:$8000 + Y * $140]
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/31c6fb2d12272e558538f96fd38e05c27bd7943e/AILZ80ASM.Test/Test/TestLB_DBDW_Test/Test.Z80#L292)

#### DS <式>, [<式2>] 及び DBS <式>, [<式2>]
- <式>のバイト数、0で埋めます。<式2>を設定するとその値で埋めます
- コマンドラインオプションで、--trim を指定するとトリム出力されます（出力の最後のエリアがDSで確保されている場合、且つ式2を指定していない場合）
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/31c6fb2d12272e558538f96fd38e05c27bd7943e/AILZ80ASM.Test/Test/TestLB_DSDBSDWS_Test/Test.Z80#L9)

#### DWS <式>, [<式2>]
- <式>の２バイト数、0で埋めます。<式2>を設定するとその値で埋めます
- コマンドラインオプションで、--trim を指定するとトリム出力されます（出力の最後のエリアがDWSで確保されている場合、且つ式2を指定していない場合）
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/31c6fb2d12272e558538f96fd38e05c27bd7943e/AILZ80ASM.Test/Test/TestLB_DSDBSDWS_Test/Test.Z80#L11)

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

## 条件付きアセンブル
5つの命令を使用して、条件付きアセンブルを制御します
- #IF: 条件付きアセンブルを開始します。コードは、指定された条件がTRUEの時にアセンブル対象になります。
- #ELIF: 前の条件付きアセンブルを終了して、指定された条件がTRUEの時にアセンブル対象になります。
- #ELSE: 前の条件付きアセンブルを終了して、前条件がFALSEの時にアセンブル対象になります。
- #ENDIF: 条件付きアセンブルを終了します。
- #ERROR: 無条件にエラーを発生させます。
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
未対応、今後対応予定です。
				
## 表記の揺れ対応
- (IX) → (IX+0)
- (IY) → (IY+0)
- SUB A, → SUB
- EX HL,DE → EX DE,HL

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
