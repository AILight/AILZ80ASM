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
            throw new Exceptions.ErrorAssembleException(Error.ErrorCodeEnum.E0001);
        }

    }
}
