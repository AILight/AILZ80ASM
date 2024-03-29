﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.Tools
{
    public static class CreateCharMapJson_SJIS
    {
        private static readonly int[][] SHIFT_JIS_ValidValues = new[]
        {
            new [] { 0x8140, 0x817e},
            new [] { 0x8180, 0x81ac},
            new [] { 0x81b8, 0x81bf},
            new [] { 0x81c8, 0x81ce},
            new [] { 0x81da, 0x81e8},
            new [] { 0x81f0, 0x81f7},
            new [] { 0x81fc, 0x81fc},
            new [] { 0x824f, 0x8258},
            new [] { 0x8260, 0x8279},
            new [] { 0x8281, 0x829a},
            new [] { 0x829f, 0x82f1},
            new [] { 0x8340, 0x837e},
            new [] { 0x8380, 0x8396},
            new [] { 0x839f, 0x83b6},
            new [] { 0x83bf, 0x83d6},
            new [] { 0x8440, 0x8460},
            new [] { 0x8470, 0x847e},
            new [] { 0x8480, 0x8491},
            new [] { 0x849f, 0x84be},
            new [] { 0x8740, 0x875d},
            new [] { 0x875f, 0x8775},
            new [] { 0x877e, 0x877e},
            new [] { 0x8780, 0x878f},
            new [] { 0x8793, 0x8794},
            new [] { 0x8798, 0x8799},
            new [] { 0x889f, 0x88fc},
            new [] { 0x8940, 0x897e},
            new [] { 0x8980, 0x89fc},
            new [] { 0x8a40, 0x8a7e},
            new [] { 0x8a80, 0x8afc},
            new [] { 0x8b40, 0x8b7e},
            new [] { 0x8b80, 0x8bfc},
            new [] { 0x8c40, 0x8c7e},
            new [] { 0x8c80, 0x8cfc},
            new [] { 0x8d40, 0x8d7e},
            new [] { 0x8d80, 0x8dfc},
            new [] { 0x8e40, 0x8e7e},
            new [] { 0x8e80, 0x8efc},
            new [] { 0x8f40, 0x8f7e},
            new [] { 0x8f80, 0x8ffc},
            new [] { 0x9040, 0x907e},
            new [] { 0x9080, 0x90fc},
            new [] { 0x9140, 0x917e},
            new [] { 0x9180, 0x91fc},
            new [] { 0x9240, 0x927e},
            new [] { 0x9280, 0x92fc},
            new [] { 0x9340, 0x937e},
            new [] { 0x9380, 0x93fc},
            new [] { 0x9440, 0x947e},
            new [] { 0x9480, 0x94fc},
            new [] { 0x9540, 0x957e},
            new [] { 0x9580, 0x95fc},
            new [] { 0x9640, 0x967e},
            new [] { 0x9680, 0x96fc},
            new [] { 0x9740, 0x977e},
            new [] { 0x9780, 0x97fc},
            new [] { 0x9840, 0x9872},
            new [] { 0x989f, 0x98fc},
            new [] { 0x9940, 0x997e},
            new [] { 0x9980, 0x99fc},
            new [] { 0x9a40, 0x9a7e},
            new [] { 0x9a80, 0x9afc},
            new [] { 0x9b40, 0x9b7e},
            new [] { 0x9b80, 0x9bfc},
            new [] { 0x9c40, 0x9c7e},
            new [] { 0x9c80, 0x9cfc},
            new [] { 0x9d40, 0x9d7e},
            new [] { 0x9d80, 0x9dfc},
            new [] { 0x9e40, 0x9e7e},
            new [] { 0x9e80, 0x9efc},
            new [] { 0x9f40, 0x9f7e},
            new [] { 0x9f80, 0x9ffc},
            new [] { 0xe040, 0xe07e},
            new [] { 0xe080, 0xe0fc},
            new [] { 0xe140, 0xe17e},
            new [] { 0xe180, 0xe1fc},
            new [] { 0xe240, 0xe27e},
            new [] { 0xe280, 0xe2fc},
            new [] { 0xe340, 0xe37e},
            new [] { 0xe380, 0xe3fc},
            new [] { 0xe440, 0xe47e},
            new [] { 0xe480, 0xe4fc},
            new [] { 0xe540, 0xe57e},
            new [] { 0xe580, 0xe5fc},
            new [] { 0xe640, 0xe67e},
            new [] { 0xe680, 0xe6fc},
            new [] { 0xe740, 0xe77e},
            new [] { 0xe780, 0xe7fc},
            new [] { 0xe840, 0xe87e},
            new [] { 0xe880, 0xe8fc},
            new [] { 0xe940, 0xe97e},
            new [] { 0xe980, 0xe9fc},
            new [] { 0xea40, 0xea7e},
        };

        public static void SJIS()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            var encoding = System.Text.Encoding.GetEncoding("shift_jis");
            var result = new Dictionary<string, byte[]>();

            for (var index = 0x20; index <= 0x7e; index++)
            {
                var bytes = new byte[] { (byte)index };
                var text = encoding.GetString(bytes);
                result.Add(text, bytes);
            }

            for (var index = 0xa1; index <= 0xdf; index++)
            {
                var bytes = new byte[] { (byte)index };
                var text = encoding.GetString(bytes); ;
                result.Add(text, bytes);
            }

            foreach (var item in SHIFT_JIS_ValidValues)
            {
                for (var index = item[0]; index <= item[1]; index++)
                {
                    var bytes = new byte[] { (byte)(index >> 8), (byte)index };
                    var text = encoding.GetString(bytes); ;
                    result.Add(text, bytes);
                }
            }

            foreach (var item in result)
            {
                if (item.Value.Length == 1)
                {
                    Console.WriteLine($"\"{item.Key}\": [ {item.Value[0]} ],");
                }
                else
                {
                    Console.WriteLine($"\"{item.Key}\": [ {item.Value[0]}, {item.Value[1]} ],");
                }
            }

        }
    }
}
