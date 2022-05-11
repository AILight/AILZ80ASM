using AILZ80ASM.Assembler;

namespace AILZ80ASM.LineDetailItems
{
    public class LineDetailItemInvalid : LineDetailItem
    {
        private LineDetailItemInvalid(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        public static LineDetailItemInvalid Create(LineItem lineItem, AsmLoad asmLoad)
        {
            var errorMessage = "";
            if (!lineItem.IsCollectOperationString)
            {
                errorMessage = "命令にASCIIコード以外の文字が含まれています。";
            }

            throw new Exceptions.ErrorAssembleException(Error.ErrorCodeEnum.E0001, errorMessage);
        }

    }
}
