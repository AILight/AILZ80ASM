namespace AILZ80ASM
{
    public class LineDetailItemInvalid : LineDetailItem
    {
        private LineDetailItemInvalid(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        public static LineDetailItemInvalid Create(LineItem lineItem, AsmLoad asmLoad)
        {
            throw new ErrorAssembleException(Error.ErrorCodeEnum.E0001);
        }

    }
}
