using AILZ80ASM.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.Assembler
{
    public static class AsmException
    {
        public static T TryCatch<T>(Error.ErrorCodeEnum errorCode, string target, Func<T> func)
        {
            try
            {
                return func.Invoke();
            }
            catch (ErrorAssembleException)
            {
                throw;
            }
            catch (ErrorLineItemException)
            {
                throw;
            }
            catch (InvalidAIValueException ex)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0004, ex.Message);
            }
            catch (InvalidAIMathException ex)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0004, ex.Message);
            }
            catch (InvalidAIValueLabelOperatorException ex)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0026, ex.Message);
            }
            catch (InvalidAIValueLabelAmbiguousException ex)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0008, ex.Message);
            }
            catch (CharMapNotFoundException ex)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E2106, ex.Message);
            }
            catch (CharMapConvertException ex)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E2105, ex.Message);
            }
            catch (InvalidAIStringEscapeSequenceException ex)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0005, ex.Value);
            }
            catch (CircularReferenceException ex)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0007, ex.Message);
            }
            catch (DivideByZeroException)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0006, target);
            }
            catch (OutputAddressUsageException)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0010);
            }
            catch (UnresolvedProgramAddressException ex)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0011, ex, ex.Message);
            }
            catch (Exception)
            {
                throw new ErrorAssembleException(errorCode, $"演算対象:{target}");
            }
        }
    }
}
