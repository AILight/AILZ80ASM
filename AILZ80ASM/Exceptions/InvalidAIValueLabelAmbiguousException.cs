using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.Exceptions
{
    public class InvalidAIValueLabelAmbiguousException : Exception
    {
        public enum LabelType
        {
            Normal,
            Anonymous,
        }

        public LabelType LabelTypeEnum { get; private set; }

        public InvalidAIValueLabelAmbiguousException(LabelType labelType, string message)
            : base(message)
        {
            LabelTypeEnum = labelType;
        }
    }
}
