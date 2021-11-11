using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AILZ80ASM.Instructions
{
    public class InstructionSet
    {
        public char[] SplitChars { get; set; }
        public string[] Brackets { get; set; }
        public string[] RegisterNames { get; set; }
        public InstructionRegister[] InstructionRegisters { get; set; }
        public InstructionItem[] InstructionItems { get; set; }
        public string[] InstructionNames { get; set; }
        public Dictionary<char, InstructionItem[]> InstructionDic { get; set; } = new();

        public void MakeDataSet()
        {
            var instructionList = new List<string>();
            var instructionDic = new Dictionary<string, List<InstructionItem>>();

            foreach (var item in InstructionItems)
            {
                var instructionNames = item.MakeDataSet(SplitChars, Brackets, InstructionRegisters);
                instructionList.AddRange(instructionNames);
                foreach (var key in instructionNames.Select(m => m[0].ToString().ToLower()).Distinct())
                {
                    if (!instructionDic.ContainsKey(key))
                    {
                        instructionDic.Add(key, new List<InstructionItem>());
                    }
                    instructionDic[key].Add(item);
                }
            }
            foreach (var item in instructionDic)
            {
                var instructionItems = item.Value.ToArray();
                InstructionDic.Add(item.Key.ToLower()[0], instructionItems);
                InstructionDic.Add(item.Key.ToUpper()[0], instructionItems);
            }

            InstructionNames = instructionList.Distinct().ToArray();
        }
    }
}
