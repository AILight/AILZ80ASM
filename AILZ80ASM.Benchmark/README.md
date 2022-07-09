# Z80 Assembler 'AILZ80ASM' Benchmark

## ベンチマーク・プロジェクトとは？
- AILZ80ASMのパフォーマンスを監視するためのプロジェクト
  - 新機能追加による速度低下を監視
- パフォーマンス・チューニング作業後の効果を可視化

## 実行方法
- Releaseビルド
- コンソールより「AILZ80ASM.Benchmark.exe」を実行

## 確認方法
- Meanの数値を確認
  - [Benchmark1](https://github.com/AILight/AILZ80ASM/tree/main/AILZ80ASM.Benchmark/TestSource/Benchmark1) をアセンブルした処理時間

## ベンチマーク結果
#### v0.9.9.24: 2022/07/09
- 演算処理のパフォーマンスチューニング
```
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1766 (21H1/May2021Update)
Intel Core i7-8650U CPU 1.90GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.203
  [Host]     : .NET 6.0.6 (6.0.622.26707), X64 RyuJIT
  DefaultJob : .NET 6.0.6 (6.0.622.26707), X64 RyuJIT


|     Method |    Mean |    Error |   StdDev |
|----------- |--------:|---------:|---------:|
| Benchmark1 | 1.608 s | 0.0236 s | 0.0210 s |
```

#### v0.9.9.24: 2022/07/03
- 演算処理を強化
```
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1766 (21H1/May2021Update)
Intel Core i7-8650U CPU 1.90GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.203
  [Host]     : .NET 6.0.6 (6.0.622.26707), X64 RyuJIT
  DefaultJob : .NET 6.0.6 (6.0.622.26707), X64 RyuJIT


|     Method |    Mean |    Error |   StdDev |
|----------- |--------:|---------:|---------:|
| Benchmark1 | 4.205 s | 0.0519 s | 0.0434 s |
```

#### v0.9.9.22: 2022/06/04
- リリース候補版
```
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1706 (21H1/May2021Update)
Intel Core i7-8650U CPU 1.90GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.203
  [Host]     : .NET 6.0.5 (6.0.522.21309), X64 RyuJIT
  DefaultJob : .NET 6.0.5 (6.0.522.21309), X64 RyuJIT


|     Method |    Mean |    Error |   StdDev |
|----------- |--------:|---------:|---------:|
| Benchmark1 | 1.537 s | 0.0221 s | 0.0195 s |
```

#### v0.9.9.19: 2022/05/22
- 命令解析部の効率化
```
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1706 (21H1/May2021Update)
Intel Core i7-8650U CPU 1.90GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.203
  [Host]     : .NET 6.0.5 (6.0.522.21309), X64 RyuJIT
  DefaultJob : .NET 6.0.5 (6.0.522.21309), X64 RyuJIT


|     Method |    Mean |    Error |   StdDev |
|----------- |--------:|---------:|---------:|
| Benchmark1 | 1.583 s | 0.0313 s | 0.0277 s |
```


#### v0.9.9.16: 2022/05/17
- エラー処理の見直し
```
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1706 (21H1/May2021Update)
Intel Core i7-8650U CPU 1.90GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.203
  [Host]     : .NET 6.0.5 (6.0.522.21309), X64 RyuJIT
  DefaultJob : .NET 6.0.5 (6.0.522.21309), X64 RyuJIT


|     Method |    Mean |    Error |   StdDev |
|----------- |--------:|---------:|---------:|
| Benchmark1 | 3.009 s | 0.0575 s | 0.0538 s |
```

#### v0.9.9.13: 2022/05/02
- 演算処理の強化
```
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1645 (21H1/May2021Update)
Intel Core i7-8650U CPU 1.90GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.202
  [Host]     : .NET 6.0.4 (6.0.422.16404), X64 RyuJIT
  DefaultJob : .NET 6.0.4 (6.0.422.16404), X64 RyuJIT


|     Method |    Mean |    Error |   StdDev |
|----------- |--------:|---------:|---------:|
| Benchmark1 | 3.676 s | 0.0464 s | 0.0388 s |
```

#### v0.9.9.10(開発中): 2022/04/10
- ORGの機能を強化した
```
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1586 (21H1/May2021Update)
Intel Core i7-8650U CPU 1.90GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.200
  [Host]     : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT
  DefaultJob : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT


|     Method |    Mean |    Error |   StdDev |
|----------- |--------:|---------:|---------:|
| Benchmark1 | 4.404 s | 0.0817 s | 0.0724 s |
```

#### v0.9.9.7: 2022/03/07
- 各種命令の判断順番の見直しを行った
```
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1526 (21H1/May2021Update)
Intel Core i7-8650U CPU 1.90GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.200
  [Host]     : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT
  DefaultJob : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT


|     Method |    Mean |    Error |   StdDev |
|----------- |--------:|---------:|---------:|
| Benchmark1 | 3.879 s | 0.0718 s | 0.1720 s |
```

#### v0.9.9.7(開発中): 2022/03/07
- PreAssembleのZ80命令解析部の一次分類分け処理：先頭一文字の判断から命令文字列に変更
```
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1526 (21H1/May2021Update)
Intel Core i7-8650U CPU 1.90GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.200
  [Host]     : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT
  DefaultJob : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT


|     Method |    Mean |    Error |   StdDev |
|----------- |--------:|---------:|---------:|
| Benchmark1 | 5.889 s | 0.0286 s | 0.0223 s |
```

#### v0.9.9.7(開発中): 2022/03/07
- PreAssembleの判断処理をキャッシュ対応
```
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1526 (21H1/May2021Update)
Intel Core i7-8650U CPU 1.90GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.200
  [Host]     : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT
  DefaultJob : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT


|     Method |    Mean |    Error |   StdDev |
|----------- |--------:|---------:|---------:|
| Benchmark1 | 5.995 s | 0.0694 s | 0.0579 s |
```

#### v0.9.9.4: 2022/03/06
```
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1526 (21H1/May2021Update)
Intel Core i7-8650U CPU 1.90GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.200
  [Host]     : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT
  DefaultJob : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT


|     Method |    Mean |   Error |  StdDev |
|----------- |--------:|--------:|--------:|
| Benchmark1 | 10.33 s | 0.197 s | 0.194 s |
```

## 項目の説明
```
Mean   : Arithmetic mean of all measurements
Error  : Half of 99.9% confidence interval
StdDev : Standard deviation of all measurements
1 s    : 1 Second (1 sec)
```

## 利用ライブラリ
- [BenchmarkDotNet](https://benchmarkdotnet.org/)
