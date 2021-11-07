using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.Instructions
{
    public class ISA
    {
        public enum EndiannessEnum
        {
            LittleEndian,
            BigEndian,
        }

        protected InstructionSet InstructionSet;
        //protected string[] OPCode { get; set; }
        public EndiannessEnum Endianness { get; protected set; } = EndiannessEnum.LittleEndian;

        public string Instruction { get; set; }

        static ISA()
        {

        }

        public ISA()
        {
        }

        public virtual bool PreAssemble()
        {
            /*
            foreach (var item in InstructionItems)
            {

            }
            */
            return false;
        }

    }
}
