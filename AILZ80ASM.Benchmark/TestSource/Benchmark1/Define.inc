        PG1VRM		            EQU $8200		; ページ１のテキストデータ
        GRAM                    EQU $0000       ; 
        GRAM1                   EQU $8000       ; VRAM1
        GRAM1L                  EQU $8000		; グラフィックプレーン
        GRAM1H                  EQU $A000		; グラフィックプレーン
        GRAM2                   EQU $0000       ; VRAM2
        GRAM2L                  EQU $0000		; グラフィックプレーン
        GRAM2H                  EQU $2000		; グラフィックプレーン
        GRAM3                   EQU $4000       ; VRAM3
        GRAM3L		            EQU $4000		; グラフィックプレーン
        GRAM3H		            EQU $6000		; グラフィックプレーン

        GRAM_SIZE	            EQU $4000		; グラフィックプレーン
        GRAM_COUNT              EQU 3           ; 3枚
        GRAM_WIDTH              EQU $28         ; 1ライン 40(28H)バイト
        GRAM_PLANE              EQU $2000       ; 1プレーンサイズ

        ADR_PROGRAM             EQU $C000       ; プログラムエリア
        SIZ_BANK                EQU $2000       ; バンクサイズ

        ; PORT 95-98
        IOP_DEV_A               EQU $95         ; ポートA
        IOP_DEV_B               EQU $96         ; ポートB
        IOP_DEV_C               EQU $97         ; ポートC
        IOP_DEV_MODE            EQU $98         ; モードセット
        IOD_DEV_MODE_ARI_STOP   EQU 1000_0010b  ; 停止
        IOD_DEV_MODE_ARI_RETURN EQU 1000_0011b  ; 復帰
        
        ; PORT HDD、CRT、KEY
        IOP_HDDCRTKEY          EQU $BB         ; ハード設定
        IOD_HDDCRTKEY_HDD_ON   EQU 0000_1000b  ; HDD_ON
        IOD_HDDCRTKEY_HDD_OFF  EQU 0000_0000b  ; HDD_OFF
        IOD_HDDCRTKEY_CRT_0000 EQU 0000_0100b  ; ページ0000
        IOD_HDDCRTKEY_CRT_4000 EQU 0000_0110b  ; ページ4000
        IOD_HDDCRTKEY_CRT_8000 EQU 0000_0000b  ; ページ8000
        IOD_HDDCRTKEY_CRT_C000 EQU 0000_0010b  ; ページC000
        IOD_HDDCRTKEY_KEY_ON   EQU 0000_0000b  ; KEY_ON
        IOD_HDDCRTKEY_KEY_OFF  EQU 0000_0001b  ; KEY_OFF

        ; つよつよカード
        IOP_CARD_ADR_0000_1FFF  EQU $80
        IOP_CARD_ADR_2000_3FFF  EQU $81
        IOP_CARD_ADR_4000_5FFF  EQU $82
        IOP_CARD_ADR_6000_7FFF  EQU $83
        IOP_CARD_ADR_8000_9FFF  EQU $84
        IOP_CARD_ADR_A000_BFFF  EQU $85
        IOP_CARD_ADR_C000_DFFF  EQU $86
        IOP_CARD_ADR_E000_FFFF  EQU $87
        IOD_CARD_ROM            EQU 0000_0000b ; ROM
        IOD_CARD_RAM            EQU 0100_0000b ; RAM
        IOD_CARD_ROM_SET_0      EQU 0000_0000b ; ROM SET #0
        IOD_CARD_ROM_SET_1      EQU 0001_0000b ; ROM SET #1
        IOD_CARD_ROM_SET_2      EQU 0010_0000b ; ROM SET #2
        IOD_CARD_ROM_SET_3      EQU 0011_0000b ; ROM SET #2
        IOD_CARD_ROM_BANK_0     EQU 0000_0000b ; ROM BANK #0
        IOD_CARD_ROM_BANK_1     EQU 0000_0001b ; ROM BANK #1
        IOD_CARD_ROM_BANK_2     EQU 0000_0010b ; ROM BANK #2
        IOD_CARD_ROM_BANK_3     EQU 0000_0011b ; ROM BANK #3
        IOD_CARD_ROM_BANK_4     EQU 0000_0100b ; ROM BANK #4
        IOD_CARD_ROM_BANK_5     EQU 0000_0101b ; ROM BANK #5
        IOD_CARD_ROM_BANK_6     EQU 0000_0110b ; ROM BANK #6
        IOD_CARD_ROM_BANK_7     EQU 0000_0111b ; ROM BANK #7
        IOD_CARD_ROM_BANK_8     EQU 0000_1000b ; ROM BANK #8
        IOD_CARD_ROM_BANK_9     EQU 0000_1001b ; ROM BANK #9
        IOD_CARD_ROM_BANK_A     EQU 0000_1010b ; ROM BANK #A
        IOD_CARD_ROM_BANK_B     EQU 0000_1011b ; ROM BANK #B
        IOD_CARD_ROM_BANK_C     EQU 0000_1100b ; ROM BANK #C
        IOD_CARD_ROM_BANK_D     EQU 0000_1101b ; ROM BANK #D
        IOD_CARD_ROM_BANK_E     EQU 0000_1110b ; ROM BANK #E
        IOD_CARD_ROM_BANK_F     EQU 0000_1111b ; ROM BANK #F

        ; PORT メモリ
        IOP_RMEM_SELECT_L       EQU $F0         ; 下位：0000H～3FFFH 上位：4000H～7FFFH
        IOP_RMEM_SELECT_H       EQU $F1         ; 下位：8000H～BFFFH 上位：C000H～FFFFH
        IOP_WMEM_SELECT         EQU $F2         ; メモリブロック書き込み設定

        IOD_RMEM_IMEM_IMEM      EQU $DD         ; 
        IOD_RMEM_ROM34_IMEM     EQU $7D         ; 
        IOD_RMEM_IMEM_ROM34     EQU $D7         ; 
        IOD_RMEM_ROM34_ROM34    EQU $77         ; 

        IOP_WMEM_IMEM	        EQU $55		    ; 
        IOP_WMEM_OMEM	        EQU $AA		    ; 
        IOP_WMEM_IMEM_ROM       EQU $A5	        ;
