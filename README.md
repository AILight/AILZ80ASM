# Z80 Assembler 'AILZ80ASM' （まだベータです）
[![Build Status](https://ailight.visualstudio.com/AILight%20Games/_apis/build/status/AILight.AILZ80ASM_Test?branchName=main)](https://ailight.visualstudio.com/AILight%20Games/_build/latest?definitionId=8&branchName=main)

AILZ80ASMは、C#で書かれた.NET Coreの環境で動作するZ80アセンブラです。

##### 免責事項
- ご利用は自己責任でお願いします。利用により損害等が発生したとき、作者は責任をおいかねます。

## 入手方法 
- 本ソースコードを、Visual Studio 2019でビルドする。
- 実行形式をダウンロードして利用する。 [ダウンロード](https://github.com/AILight/AILZ80ASM/releases)　（準備中）

## 使い方
AILZ80ASM [<オプション>] <オプション指定文字列:ファイル名等> [ <オプション指定文字列:ファイル名等>]
```
> AILZ80ASM -i sample.z80 -o sample.z80
```
##### ファイル形式
- UTF-8

## コマンドラインオプション
- -i, --input <input>      アセンブリ対象のファイルをスペース区切りで指定します。
- -o, --output <output>    出力ファイルを指定します。
- -s, --symbol <symbol>    シンボルファイルを指定します。
- -l, --list <list>        リストファイルを指定します。
- -v, --version            バージョンを表示します。
- -?, -h, --help           バージョンを表示します。

# ソースコード書式

## ニーモニック
- ザイログ・ニーモニックの表記
- 大文字、小文字の区別はしない 
- [本アセンブラで利用できるニーモニック一覧](https://github.com/AILight/AILZ80ASM/blob/master/AILZ80ASM.Test/Test/TestRD_ALL/Test.Z80)

## 未定義命令 (Undocumented Instructions) 
- [IXH, IXL, IYH, IYLに対応](https://github.com/AILight/AILZ80ASM/blob/master/AILZ80ASM.Test/Test/TestUD_LD/Test.Z80)
- [SLL](https://github.com/AILight/AILZ80ASM/blob/master/AILZ80ASM.Test/Test/TestUD_SLL/Test.Z80)
- [IN F,(C)](https://github.com/AILight/AILZ80ASM/blob/d38775a0854778fe82b36bce6fbf7a6fdf5e0c78/AILZ80ASM.Test/Test/TestUD_IN_And_OUT/Test.Z80#L1), [IN (C)](https://github.com/AILight/AILZ80ASM/blob/d38775a0854778fe82b36bce6fbf7a6fdf5e0c78/AILZ80ASM.Test/Test/TestUD_IN_And_OUT/Test.Z80#L3)
- [OUT (C), 0](https://github.com/AILight/AILZ80ASM/blob/d38775a0854778fe82b36bce6fbf7a6fdf5e0c78/AILZ80ASM.Test/Test/TestUD_IN_And_OUT/Test.Z80#L2)
- [ビット操作](https://github.com/AILight/AILZ80ASM/blob/e445250854525bc124ff6156ee328d085be31028/AILZ80ASM.Test/ReleaseDecision/InstructionSet.z80#L786)

## ラベル
ラベルは、ネームスペース、標準ラベル、ローカルラベルで指定します。ネームスペースを指定しない場合には、 'main' が割り当てられています。必要に応じて変更をしてください。
ラベルは指定した行以降は、下位のラベルを利用するときには上位のラベル名を省略することが出来ます。

`マクロ内から外部のラベルを参照するときには、ネームスペース付きで呼び出す必要がありますので、注意が必要です。`
	
#### ラベルの宣言
- <ネームスペース名[::]> 
  - デフォルト： `main`
- <ラベル名[:]>
  - 標準ラベル
- <[.]ローカルラベル名>
  - ローカルラベル

#### ラベルの指定
- <ネームスペース>:<ラベル名>.<ローカルラベル名>
- <ラベル名>.<ローカルラベル名>
- <ローカルラベル名>

#### ラベルの機能
- .@Hをラベルに追加すると、上位1バイトを取得
- .@Lをラベルに追加すると、下位1バイトを取得
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/master/AILZ80ASM.Test/Test/TestLB_EQU_Test/Test.Z80)

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
- 2進数：末尾に% or bを付けます。また _ を含める事が出来ます。
- 10進数：何もつけません
- 16進数：先頭に$もしくは末尾にHを付けます

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
本アセンブラの特徴として、アセンブル時のアドレスとアウトプット時のアドレスを別で管理しています。アセンブル時のアドレスは、プログラム・ロケーションカウンターと呼び、アセンブル内でいつでも変更可能です。1つのソースコードの中でアドレスが重複しても問題ありません。アウトプット時のアドレスは、アウトプット・ロケーションカウンターと呼び、バイナリデータの出力位置を指定します。出力用のアドレスになりますので、重複することは出来ませんし、アドレスを戻すことも出来ません。
	
- <式>
  - プログラム・ロケーションカウンターの値を変更します。
  - 出力されるラベルの計算に影響を与えます。プログラム中で何度でも変更可能です。アドレスの重複も可能です。
- <式2>
  - アウトプット・ロケーションカウンターの値を変更します。デフォルトは0です。
  - 出力されるバイナリーに影響を与えます。プログラム中で何度でも変更可能です。アドレスの重複は不可能です。
  - バイナリーの出力を制御したいときに使用します（ROM化等）
- <式3>
  - 先アドレスを指定した場合には、0で埋めます。<式3>を設定するとその値で埋めます。
- メモリのアライメントを合わせるには、ALIGNをお勧めします。
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/master/AILZ80ASM.Test/Test/TestCS_ORG/Test.Z80)

```
	ORG $1000
LB1000:
	LD A, (LB1000)

	ORG $2000
LB2000:
	LD A, (LB2000)

	ORG $3000, $0010
LB3000:
	LD A, (LB3000)
```
出力結果
```
0000 3A 00 10 3A 00 20 00 00 00 00 00 00 00 00 00 00
0010 3A 00 30
```

#### <ラベル> EQU <式>
- 指定したラベルに、<式> の値を持たせます。
- ローカルラベルで利用することも可能です。
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/master/AILZ80ASM.Test/Test/TestLB_EQU_Test/Test.Z80)

```
PORT_A  equ $CC
.WRITE  equ $01
.READ   equ $02

	LD A, PORT_A.WRITE
	OUT (PORT_A), A

	LD A, PORT_B.READ
	OUT (PORT_A), A
```

#### ALIGN <式>, [<式2>]
- ロケーションカウンタの値を、<式>で設定したアライメント境界まで移動します。
- <式>に設定できる値は、2のべき乗である必要があります。
- 移動により空いた領域には、0 または <式2> の値で埋められます。
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/master/AILZ80ASM.Test/Test/TestCS_ALIGN/Test.Z80)

#### INCLUDE [<ファイル名>], [<ファイルタイプ>], [<開始位置>], [<長さ>]
- ファイル名の内容を、その場所に展開します
- ファイルタイプ：TEXT と BINARY が選択できます。省略するとTEXTになります。また短縮形 T, B が使えます。
- 開始位置:ファイルの読み出し開始位置が指定できます。（ファイルタイプがBINARYの時に有効）
- 長さ:ファイルの読み込み長さが指定できます。（ファイルタイプがBINARYの時に有効）
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/master/AILZ80ASM.Test/Test/TestPP_Include/Test.Z80)
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
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/31c6fb2d12272e558538f96fd38e05c27bd7943e/AILZ80ASM.Test/Test/TestLB_DSDBSDWS_Test/Test.Z80#L9)

#### DWS <式>, [<式2>]
- <式>の２バイト数、0で埋めます。<式2>を設定するとその値で埋めます
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/31c6fb2d12272e558538f96fd38e05c27bd7943e/AILZ80ASM.Test/Test/TestLB_DSDBSDWS_Test/Test.Z80#L11)

## マクロ
#### MACRO <マクロ名> [<引数1>, <引数2>]　～ END MACRO
- MACROからEND MACROまでがマクロとして定義されます
- 引数に付けた名前がマクロ内で利用できます
- マクロの中から外の要素を参照するには、ネームスペースを含めた名前にする必要があります。[サンプル](https://github.com/AILight/AILZ80ASM/blob/034672f506f5253b74824598faf35fdbc5000c99/AILZ80ASM.Test/Test/TestPP_Macro/Test.Z80#L55)
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/master/AILZ80ASM.Test/Test/TestPP_Macro/Test.Z80)
```
ARG1	equ 2
.Three  equ 3
	
	ALLLD
	TestArg ARG1, ARG1.Three

MACRO ALLLD
	ld a,1
	ld b,2
	ld c,3
	ld d,4
	ld e,5
	ld h,6
	ld l,7
END MACRO

MACRO TestArg a1, a2
	ld a, a1
	ld b, a2
END MACRO
```

#### REPEAT <式1> [LAST <式2>]　～ END REPEAT
- 式1に設定した値の回数分をREPEATの中に記述してある命令を展開します
- 式2には、最終の展開時に削除したい命令数を負の値で設定します
- ネストに対応しています
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/master/AILZ80ASM.Test/Test/TestPP_Repeat/Test.Z80)

```
REPEAT 3
        xor     a
END REPEAT

REPEAT 8 LAST -1
        ld      (hl), a
        set     5, h
        ld      (hl), a
        add     hl, de
END REPEAT
```

## 条件付きアセンブル
5つの命令を使用して、条件付きアセンブルを制御します
- #IF: 条件付きアセンブルを開始します。コードは、指定された条件がTRUEの時にアセンブル対象になります。
- #ELIF: 前の条件付きアセンブルを終了して、指定された条件がTRUEの時にアセンブル対象になります。
- #ELSE: 前の条件付きアセンブルを終了して、前条件がFALSEの時にアセンブル対象になります。
- #ENDIF: 条件付きアセンブルを終了します。
- #ERROR: 無条件にエラーを発生させます。
- [サンプル](https://github.com/AILight/AILZ80ASM/blob/master/AILZ80ASM.Test/Test/TestPP_Conditional/Test.Z80)

```
#if mode == 1
	ld a,1
#elif mode == 2
	ld d,4
#else
	#error "modeの値が範囲外です。"
#endif
```
