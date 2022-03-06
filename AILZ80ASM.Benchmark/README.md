# Z80 Assembler 'AILZ80ASM' Benchmark

## ベンチマークプロジェクトとは？
- パフォーマンスチューニングを行うときに使うプロジェクト
- 機能を追加する事で速度が低下していないかを確認する為に利用

## 実行方法
- Releaseビルド
- コンソールより「AILZ80ASM.Benchmark.exe」を実行

## ベンチマーク結果
#### v0.9.9.4:2022/03/06
```
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1526 (21H1/May2021Update)
Intel Core i7-8650U CPU 1.90GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.200
  [Host]     : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT
  DefaultJob : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT


|     Method |    Mean |   Error |  StdDev |
|----------- |--------:|--------:|--------:|
| Benchmark1 | 10.08 s | 0.114 s | 0.095 s |
```

## 利用ライブラリ
- [BenchmarkDotNet](https://benchmarkdotnet.org/)
