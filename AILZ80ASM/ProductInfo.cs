using System;

namespace AILZ80ASM
{
    public static class ProductInfo
    {
        public static string ProductName => $"AILZ80ASM";
        public static Version ProductVersion => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        public static string ProductLongName => $"*** {ProductName} *** Z-80 Assembler, version {ProductVersion}";
        public static string Copyright => $"Copyright (C) {DateTime.Today.Year:0} by M.Ishino (AILight)";
    }
}
