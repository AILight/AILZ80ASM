﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineDetailExpansionItemBinaryFile : LineDetailExpansionItem
    {
        public FileInfo FileInfo { get; private set; }
        public string FileStart { get; private set; }
        public string FileLength { get; private set; }

        private byte[] _Bin;
        public override byte[] Bin
        {
            get 
            {
                return _Bin;
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
            var fileStart = string.IsNullOrEmpty(FileStart) ? 0 : (int)AIMath.ConvertToUInt32(FileStart, asmLoad);
            fileSize -= fileStart;

            if (fileSize < 0)
            {
                throw new ErrorMessageException(Error.ErrorCodeEnum.E2006);
            }    

            var readLength = string.IsNullOrEmpty(FileLength) ? int.MaxValue : (int)AIMath.ConvertToUInt32(FileLength, asmLoad);
            if (readLength > fileSize)
            {
                readLength = fileSize;
            }

            _Bin = new byte[readLength];

            using var fileStream = FileInfo.OpenRead();
            fileStream.Seek(fileStart, SeekOrigin.Begin);
            fileStream.Read(_Bin, 0, readLength);

            asmAddress = new AsmAddress(asmAddress, new AsmLength(_Bin.Length));

        }
    }
}