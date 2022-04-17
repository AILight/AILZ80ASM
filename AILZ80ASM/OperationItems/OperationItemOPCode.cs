using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using AILZ80ASM.InstructionSet;
using AILZ80ASM.LineDetailItems.ScopeItem.ExpansionItems;
using System;
using System.Linq;

namespace AILZ80ASM.OperationItems
{
    public class OperationItemOPCode : OperationItem
    {
        private AsmLoad AsmLoad {get; set; }
        private AssembleResult AssembleResult { get; set; }

        public override byte[] Bin => AsmLoad?.ISA?.ToBin(AssembleResult) ?? Array.Empty<byte>();
        public override AsmList List(AsmAddress asmAddress)
        {
            return AsmList.CreateLineItem(asmAddress, Bin, AssembleResult.InstructionItem.T == 0 ? "" : AssembleResult.InstructionItem.T.ToString(), LineDetailExpansionItemOperation.LineItem);
        }

        public override AsmLength Length => new AsmLength(AssembleResult.InstructionItem.OPCode.Length);

        private OperationItemOPCode(InstructionSet.AssembleResult assembleResult, LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmAddress address, AsmLoad asmLoad)
        {
            AssembleResult = assembleResult;
            LineDetailExpansionItemOperation = lineDetailExpansionItemOperation;
            AsmLoad = asmLoad;
        }

        public new static bool CanCreate(string operation, AsmLoad asmLoad)
        {
            if (string.IsNullOrEmpty(operation))
            {
                //空文字もOperationItemOPCodeとして処理をする
                return true;
            }

            if (asmLoad.ISA.PreAssemble(operation) != default)
            {
                return true;
            }

            return false;
        }

        public new static OperationItem Create(LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmAddress address, AsmLoad asmLoad)
        {
            var returnValue = default(OperationItemOPCode);
            var code = lineDetailExpansionItemOperation.LineItem.OperationString;
            if (!string.IsNullOrEmpty(code))
            {
                var result = asmLoad.ISA.PreAssemble(code);
                if (result != default)
                {
                    returnValue = new OperationItemOPCode(result, lineDetailExpansionItemOperation, address, asmLoad);
                }
            }

            return returnValue;
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
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E0017, ex.Message);

                    default:
                        throw new NotImplementedException();
                }
            }

        }

    }
}
