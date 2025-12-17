using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AILZ80ASM.InstructionSet
{
    public class InstructionSet
    {
        public char NumberReplaseChar { get; set; }
        public char[] SplitChars { get; set; }
        public HashSet<string> RegisterAndFlagNamesSet { get; set; }
        public InstructionRegister[] InstructionRegisters { get; set; }
        public InstructionItem[] InstructionItems { get; set; }
        public HashSet<string> InstructionNamesSet { get; set; }
        public Dictionary<string, InstructionItem[]> InstructionDic { get; set; } = new();

        public void MakeDataSet()
        {
            var instructionList = new List<string>();
            var instructionDic = new Dictionary<string, List<InstructionItem>>();

            foreach (var item in InstructionItems)
            {
                var instructionNames = item.MakeDataSet(SplitChars, InstructionRegisters);
                instructionList.AddRange(instructionNames);
                foreach (var key in instructionNames.Select(m => m.ToLower()).Distinct())
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
                InstructionDic.Add(item.Key, item.Value.ToArray());
            }

            InstructionNamesSet = new HashSet<string>(instructionList.Distinct(), StringComparer.OrdinalIgnoreCase);
        }
    }
}
