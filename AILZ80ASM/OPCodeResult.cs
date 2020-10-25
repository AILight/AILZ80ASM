using System;
using System.Collections.Generic;
using System.Linq;

namespace AILZ80ASM
{
    public class OPCodeResult
    {
        public enum OPCodeStatusEnum
        {
            ORG,
            DATA,
            OP,
            ERROR,
        }

        public OPCodeStatusEnum OPCodeStatus { get; private set; }
        public UInt16 Address { get; private set; }
        public string[] OPCode { get; private set; }
        public int M { get; private set; }
        public int T { get; private set; }

        public OPCodeResult(OPCodeItem opCodeItem)
            : this(opCodeItem.OPCode, opCodeItem.M, opCodeItem.T)
        {
        }

        public OPCodeResult(string[] opCode, int m, int t)
        {
            OPCodeStatus = OPCodeStatusEnum.OP;
            OPCode = opCode;
            M = m;
            T = t;
        }

        public OPCodeResult(string[] opCode)
        {
            OPCodeStatus = OPCodeStatusEnum.DATA;
            OPCode = opCode;
        }

        public OPCodeResult(UInt16 address)
        {
            OPCodeStatus = OPCodeStatusEnum.ORG;
            Address = address;
        }

        public byte[] ToBin()
        {
            return OPCode.Select(m => Convert.ToByte(m, 2)).ToArray();
        }

        internal void Assemble(Label[] labels)
        {
            throw new NotImplementedException();
        }


    }
}
