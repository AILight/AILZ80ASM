﻿using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System;
using System.IO;

namespace AILZ80ASM.LineDetailItems.ScopeItem.ExpansionItems
{
    public class LineDetailExpansionItemBinaryFile : LineDetailExpansionItem
    {
        public FileInfo FileInfo { get; private set; }
        public string FileStart { get; private set; }
        public string FileLength { get; private set; }

        private byte[] _Bin;

        public override AsmResult[] BinResults
        {
            get
            {
                return new[] { new AsmResult() { Address = this.Address, Data = _Bin, LineItem = this.LineItem } };
            }
        }

        public LineDetailExpansionItemBinaryFile(LineItem lineItem, FileInfo fileInfo, string start, string length)
            : base(lineItem)
        {
            FileInfo = fileInfo;
            FileStart = start;
            FileLength = length;
        }

        public override void PreAssemble(ref AsmAddress asmAddress, AsmLoad asmLoad)
        {
            base.PreAssemble(ref asmAddress, asmLoad);

            var fileSize = (int)FileInfo.Length;
            var fileStart = string.IsNullOrEmpty(FileStart) ? 0 : AIMath.Calculation(FileStart, asmLoad, asmAddress).ConvertTo<int>();
            fileSize -= fileStart;

            if (fileStart < 0)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E2004);
            }

            if (fileSize < 0)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E2006);
            }

            var readLength = string.IsNullOrEmpty(FileLength) ? int.MaxValue : AIMath.Calculation(FileLength, asmLoad, asmAddress).ConvertTo<int>();
            if (readLength < 0)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E2005);
            }
            if (readLength > fileSize)
            {
                readLength = fileSize;
            }

            _Bin = new byte[readLength];

            using var fileStream = FileInfo.OpenRead();
            try
            {
                fileStream.Seek(fileStart, SeekOrigin.Begin);
            }
            catch
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E2004);
            }
            fileStream.Read(_Bin, 0, readLength);

            Length = new AsmLength(_Bin.Length);
            asmAddress = new AsmAddress(asmAddress, Length);

        }

        public override AsmList List
        {
            get 
            {
                return AsmList.CreateLineItemBinaryFile(Address, this?.LineItem?.ErrorLineItem?.ErrorCode, this?.LineItem?.ErrorLineItem?.ErrorMessage, _Bin);
            }
        }
    }
}
