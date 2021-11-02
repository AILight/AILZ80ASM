using System;
using System.IO;
using System.Reflection;

namespace AILZ80ASM
{
    public static class ProductInfo
    {
        public static string ProductName => $"AILZ80ASM";
        public static Version ProductVersion => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        public static string ProductLongName => $"*** {ProductName} *** Z-80 Assembler, version {ProductVersion}";
        public static string Copyright => $"Copyright (C) {File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location).Year} by M.Ishino (AILight)";
    }
}
