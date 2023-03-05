using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using AILZ80ASM.InstructionSet;
using AILZ80ASM.LineDetailItems.ScopeItem.ExpansionItems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AILZ80ASM.OperationItems
{
    public class OperationItemOPCode : OperationItem
    {
        private AssembleResult AssembleResult { get; set; }

        public override byte[] Bin => AsmLoad?.ISA?.ToBin(AssembleResult) ?? Array.Empty<byte>();
        public override AsmList List(AsmAddress asmAddress)
        {
            return AsmList.CreateLineItem(asmAddress, Bin, AssembleResult.InstructionItem.T == 0 ? "" : AssembleResult.InstructionItem.T.ToString(), LineItem);
        }

        public override AsmLength Length => new AsmLength(AssembleResult.InstructionItem.OPCode.Length);

        private OperationItemOPCode(InstructionSet.AssembleResult assembleResult, LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {
            AssembleResult = assembleResult;
        }

        public static OperationItemOPCode Create(LineItem lineItem, AsmLoad asmLoad)
        {
            var asssembleResult = default(AssembleResult);
            if (asmLoad.Scope.IsRegisterLabel &&
                TryReplaceRegisterLabels(lineItem.OperationString, asmLoad, out var tmpString))
            {
                // ラベルの展開を行う
                asssembleResult = asmLoad.ISA.PreAssemble(tmpString);
            }
            else 
            {
                asssembleResult = asmLoad.ISA.PreAssemble(lineItem.OperationString);
            }

            if (asssembleResult == default)
            {
                return default;
            }

            return new OperationItemOPCode(asssembleResult, lineItem, asmLoad);
        }

        private static bool TryReplaceRegisterLabels(string operationString, AsmLoad asmLoad, out string replacedString)
        {
            var result = false;
            replacedString = "";
            var splitChars = new char[] { ' ', ',', '\t' };
            var startIndex = 0;

            while (startIndex < operationString.Length)
            {
                var splitChar = "";
                var nextIndex = operationString.IndexOfAny(splitChars, startIndex);
                if (nextIndex != -1)
                {
                    splitChar = operationString.Substring(nextIndex, 1);
                }
                else
                {
                    nextIndex = operationString.Length;
                }
                var target = operationString.Substring(startIndex, nextIndex - startIndex);
                
                var label = asmLoad.FindLabelForRegister(target);
                if (label != default)
                {
                    result = true;
                    replacedString += label.ValueString;
                }
                else
                {
                    replacedString += target;
                }
                replacedString += splitChar;
                startIndex = nextIndex + 1;
            }

            return result;
        }

        public override void Assemble(AsmLoad asmLoad)
        {
            try
            {
                asmLoad.ISA.Assemble(AssembleResult, asmLoad, LineDetailExpansionItemOperation.Address);

                if (AssembleResult.InstructionItem.ErrorCode.HasValue)
                {
                    asmLoad.AddError(new ErrorLineItem(LineDetailExpansionItemOperation.LineItem, AssembleResult.InstructionItem.ErrorCode.Value));
                }

                if (AssembleResult.InnerAssembleException is AssembleOutOfRangeException exception)
                {
                    // ワーニングを積む
                    switch (exception.InstructionRegisterMode)
                    {
                        case InstructionRegister.InstructionRegisterModeEnum.Value8Bit:
                            asmLoad.AddError(new ErrorLineItem(LineDetailExpansionItemOperation.LineItem, Error.ErrorCodeEnum.W0001, exception.Value));
                            break;
                        case InstructionRegister.InstructionRegisterModeEnum.Value16Bit:
                            asmLoad.AddError(new ErrorLineItem(LineDetailExpansionItemOperation.LineItem, Error.ErrorCodeEnum.W0002, exception.Value));
                            break;
                        case InstructionRegister.InstructionRegisterModeEnum.Value8BitSigned:
                            asmLoad.AddError(new ErrorLineItem(LineDetailExpansionItemOperation.LineItem, Error.ErrorCodeEnum.W0003, exception.Value));
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                }

            }
            catch (AssembleOutOfRangeException ex)
            {
                switch (ex.InstructionRegisterMode)
                {
                    case InstructionRegister.InstructionRegisterModeEnum.RelativeAddress8Bit:
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E0003, ex.Message);

                    case InstructionRegister.InstructionRegisterModeEnum.Value0Bit:
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E0019, ex.Message);

                    case InstructionRegister.InstructionRegisterModeEnum.Value3Bit:
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E0016, ex.Message);

                    case InstructionRegister.InstructionRegisterModeEnum.Value8Bit:
                    case InstructionRegister.InstructionRegisterModeEnum.Value16Bit:
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.W0001, ex.Message);

                    case InstructionRegister.InstructionRegisterModeEnum.RestartValue:
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E0020, ex.Message);

                    default:
                        throw new NotImplementedException();
                }
            }

        }

    }
}
