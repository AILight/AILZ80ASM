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
#### v0.9.9.6(開発中): 2022/03/07
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

#### v0.9.9.6(開発中): 2022/03/07
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

#### v0.9.9.6(開発中): 2022/03/07
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
