using AILZ80ASM.AILight;
using AILZ80ASM.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM.Assembler
{
    public class Function
    {
        public string GlobalLabelName { get; private set; }
        public string Name { get; private set; }
        public string FullName => $"{GlobalLabelName}.{Name}";

        public string[] Args { get; private set; }
        public string Formula { get; private set; }

        private int CallCounter { get; set; }
        private const int LIMIT_MAX_RECURSIVE_CALL = 50;

        public Function(string functionName, string[] args, string formula, AsmLoad asmLoad)
        {
            this.GlobalLabelName = asmLoad.Scope.GlobalLabelName;
            this.Name = functionName;

            Args = args;

            Formula = formula;
            CallCounter = 0;
        }

        /// <summary>
        /// Functionを計算する
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="asmLoad"></param>
        /// <param name="asmAddress"></param>
        /// <returns></returns>
        public AIValue Calculation(string[] arguments, AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            if (Args.Length != arguments.Length)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E4004);
            }

            try
            {
                CallCounter++;
                if (CallCounter > LIMIT_MAX_RECURSIVE_CALL)
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E4003);
                }

                var guid = $"{Guid.NewGuid():N}";
                return asmLoad.CreateLocalScope($"function_{guid}", $"label_{guid}", localAsmLoad =>
                {
                    foreach (var index in Enumerable.Range(0, arguments.Length))
                    {
                        var label = new LabelFunctionArg(Args[index], arguments[index], localAsmLoad, asmLoad);
                        if (label.Invalidate)
                        {
                            throw new ErrorAssembleException(Error.ErrorCodeEnum.E4005);
                        }
                        localAsmLoad.AddLabel(label);
                    }

                    return AIMath.Calculation(Formula, localAsmLoad);

                });
            }
            catch
            {
                throw;
            }
            finally
            {
                CallCounter--;
            }
        }

        /// <summary>
        /// ロングファンクション名を生成する
        /// </summary>
        /// <param name="labelName"></param>
        /// <param name="asmLoad"></param>
        /// <returns></returns>
        public static string GetFunctionFullName(string functionName, AsmLoad asmLoad)
        {
            if (functionName.Contains("."))
            {
                return functionName;
            }

            return $"{asmLoad.Scope.GlobalLabelName}.{functionName}";
        }
    }
}
