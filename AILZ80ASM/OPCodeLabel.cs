namespace AILZ80ASM
{
    public class OPCodeLabel
    {
        public enum ValueTypeEnum
        {
            IndexOffset,
            Value8,
            e8,
            Value16
        }

        public ValueTypeEnum ValueType { get; private set; }
        public string ValueString { get; private set; }

        public OPCodeLabel(ValueTypeEnum valueType, string valueString)
        {
            ValueType = valueType;
            ValueString = valueString;
        }
    }
}
