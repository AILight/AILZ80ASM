﻿using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class AIValueTest
    {
        [TestMethod]
        public void IsNumberTest()
        {
            Assert.IsTrue(AIValue.IsNumber("01"));
            Assert.IsTrue(AIValue.IsNumber("123"));
            Assert.IsTrue(AIValue.IsNumber("123D"));
            Assert.IsTrue(AIValue.IsNumber("FFFH"));
            Assert.IsTrue(AIValue.IsNumber("0FFFH"));
            Assert.IsTrue(AIValue.IsNumber("0b0000_1111"));
            Assert.IsTrue(AIValue.IsNumber("%0000_1111"));
            Assert.IsTrue(AIValue.IsNumber("0000_1111b"));
            Assert.IsTrue(AIValue.IsNumber("$FFFF"));
            Assert.IsTrue(AIValue.IsNumber("0xFFFF"));
            Assert.IsTrue(AIValue.IsNumber("777o"));
            Assert.IsTrue(AIValue.IsNumber("0o777"));

            Assert.IsFalse(AIValue.IsNumber("O123"));
            Assert.IsFalse(AIValue.IsNumber("FFF"));
            Assert.IsFalse(AIValue.IsNumber("b0000_1111"));
            Assert.IsFalse(AIValue.IsNumber("0000_1111%"));
            Assert.IsFalse(AIValue.IsNumber("0000_1111"));
            Assert.IsFalse(AIValue.IsNumber("0FFF"));
            Assert.IsFalse(AIValue.IsNumber("0xXFFF"));
            Assert.IsFalse(AIValue.IsNumber("0o888"));
            Assert.IsFalse(AIValue.IsNumber("o888"));
            Assert.IsFalse(AIValue.IsNumber("888o"));
        }

        [TestMethod]
        public void IsFunction()
        {
            {
                var target = "ABC() + 10";
                Assert.IsTrue(AIValue.TryParseFunction(ref target, out var resultFunction));
                Assert.AreEqual(resultFunction, "ABC()");
            }

            {
                var target = "ABC(ABC(ABC(ABC()))) + 10";
                Assert.IsTrue(AIValue.TryParseFunction(ref target, out var resultFunction));
                Assert.AreEqual(resultFunction, "ABC(ABC(ABC(ABC())))");
            }

            {
                var target = "ABC(ABC(ABC(ABC(\")(\")))) + 10";
                Assert.IsTrue(AIValue.TryParseFunction(ref target, out var resultFunction));
                Assert.AreEqual(resultFunction, "ABC(ABC(ABC(ABC(\")(\"))))");
            }
        }

        [TestMethod]
        public void ConstractorTest()
        {
            var valueBool = new AIValue(true);
            var valueInt32 = new AIValue(2);
            var valueString = new AIValue("2", AIValue.ValueTypeEnum.String);
            var valueChar_1 = new AIValue('2');
            var valueChar_2 = new AIValue("2", AIValue.ValueTypeEnum.Char);

            Assert.AreEqual(valueBool.ConvertTo<bool>(), true);
            Assert.AreEqual(valueInt32.ConvertTo<int>(), 2);
            Assert.AreEqual(valueString.ConvertTo<string>(), "2");
            Assert.AreEqual(valueChar_1.ConvertTo<char>(), '2');
            Assert.AreEqual(valueChar_2.ConvertTo<char>(), '2');
        }

        [TestMethod]
        public void ConvertToTest()
        {
            {
                var aiValue = new AIValue(2);
                Assert.AreEqual(aiValue.ConvertTo<int>(), 2);
            }
            {
                var aiValue = new AIValue("2");
                Assert.ThrowsException<ErrorAssembleException>(() =>
                {
                    aiValue.ConvertTo<int>();
                });
                Assert.ThrowsException<ErrorAssembleException>(() =>
                {
                    aiValue.ConvertTo<UInt16>();
                });
                Assert.ThrowsException<ErrorAssembleException>(() =>
                {
                    aiValue.ConvertTo<UInt32>();
                });
            }
            {
                var minValue = new AIValue(Int16.MinValue);
                var maxValue = new AIValue(Int16.MaxValue);
                var minOverValue = new AIValue((UInt16.MaxValue + 2) * -1);
                var maxOverValue = new AIValue( UInt16.MaxValue + 1);

                Assert.AreEqual(minValue.ConvertTo<UInt16>(), 0x8000);
                Assert.AreEqual(maxValue.ConvertTo<UInt16>(), 0x7FFF);

                Assert.ThrowsException<ErrorAssembleException>(() =>
                {
                    minOverValue.ConvertTo<UInt16>();
                });
                Assert.ThrowsException<ErrorAssembleException>(() =>
                {
                    maxOverValue.ConvertTo<UInt16>();
                });
            }

            {
                var minValue = new AIValue(sbyte.MinValue);
                var maxValue = new AIValue(sbyte.MaxValue);
                var minOverValue = new AIValue((byte.MaxValue + 2) * -1);
                var maxOverValue = new AIValue(byte.MaxValue + 1);

                Assert.AreEqual(minValue.ConvertTo<byte>(), 0x80);
                Assert.AreEqual(maxValue.ConvertTo<byte>(), 0x7F);

                Assert.ThrowsException<ErrorAssembleException>(() =>
                {
                    minOverValue.ConvertTo<byte>();
                });
                Assert.ThrowsException<ErrorAssembleException>(() =>
                {
                    maxOverValue.ConvertTo<byte>();
                });
            }
        }

        [TestMethod]
        public void SetValueTest()
        {
            // 16進数
            {
                var aiValueA = new AIValue("$FFFF");
                var aiValueB = new AIValue("FFFFH");
                var aiValueC = new AIValue("0xFFFF");

                aiValueA.SetValue(default(AsmLoad), default(AsmAddress));
                aiValueB.SetValue(default(AsmLoad), default(AsmAddress));
                aiValueC.SetValue(default(AsmLoad), default(AsmAddress));

                Assert.AreEqual(aiValueA.ConvertTo<UInt16>(), 0xFFFF);
                Assert.AreEqual(aiValueB.ConvertTo<UInt16>(), 0xFFFF);
                Assert.AreEqual(aiValueC.ConvertTo<UInt16>(), 0xFFFF);

                var aiValueD = new AIValue("0xFFFFFFFFFF");
                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    aiValueD.SetValue(default(AsmLoad), default(AsmAddress));
                });
            }

            // 8進数
            {
                var aiValueA = new AIValue("777o");
                var aiValueB = new AIValue("111o");
                var aiValueC = new AIValue("0o555");

                aiValueA.SetValue(default(AsmLoad), default(AsmAddress));
                aiValueB.SetValue(default(AsmLoad), default(AsmAddress));
                aiValueC.SetValue(default(AsmLoad), default(AsmAddress));

                Assert.AreEqual(aiValueA.ConvertTo<UInt16>(), 511);
                Assert.AreEqual(aiValueB.ConvertTo<UInt16>(), 73);
                Assert.AreEqual(aiValueC.ConvertTo<UInt16>(), 365);

                var aiValueD = new AIValue("777777777777777o");
                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    aiValueD.SetValue(default(AsmLoad), default(AsmAddress));
                });
            }

            // 2進数
            {
                var aiValueA = new AIValue("1111_1111B");
                var aiValueB = new AIValue("%1111_1111");
                var aiValueC = new AIValue("0b1111_1111");

                aiValueA.SetValue(default(AsmLoad), default(AsmAddress));
                aiValueB.SetValue(default(AsmLoad), default(AsmAddress));
                aiValueC.SetValue(default(AsmLoad), default(AsmAddress));

                Assert.AreEqual(aiValueA.ConvertTo<UInt16>(), 0xFF);
                Assert.AreEqual(aiValueB.ConvertTo<UInt16>(), 0xFF);
                Assert.AreEqual(aiValueC.ConvertTo<UInt16>(), 0xFF);

                var aiValueD = new AIValue("1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111B");
                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    aiValueD.SetValue(default(AsmLoad), default(AsmAddress));
                });
            }

            // 10進数
            {
                var aiValueA = new AIValue("65535");
                var aiValueB = new AIValue("12345");
                var aiValueC = new AIValue("1234D");

                aiValueA.SetValue(default(AsmLoad), default(AsmAddress));
                aiValueB.SetValue(default(AsmLoad), default(AsmAddress));
                aiValueC.SetValue(default(AsmLoad), default(AsmAddress));

                Assert.AreEqual(aiValueA.ConvertTo<UInt16>(), 65535);
                Assert.AreEqual(aiValueB.ConvertTo<UInt16>(), 12345);
                Assert.AreEqual(aiValueC.ConvertTo<UInt16>(), 1234);

                var aiValueD = new AIValue("65535655356553565535");
                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    aiValueD.SetValue(default(AsmLoad), default(AsmAddress));
                });
            }
        }

        [TestMethod]
        public void CalculationTest()
        {
            var aiValue0 = new AIValue(0);
            var aiValue1 = new AIValue(1);
            var aiValue2 = new AIValue(2);
            var aiValue3 = new AIValue(3);
            var aiValueABC = new AIValue("ABC", AIValue.ValueTypeEnum.String);
            var aiValueDEF = new AIValue("DEF", AIValue.ValueTypeEnum.String);
            var aiValueTrue = new AIValue(true);
            var aiValueFalse = new AIValue(false);

            var aiValueNegation = new AIValue("!", AIValue.ValueTypeEnum.Operation);
            var aiValueBitwiseComplement = new AIValue("~", AIValue.ValueTypeEnum.Operation);
            var aiValuePlus = new AIValue("+", AIValue.ValueTypeEnum.Operation);
            var aiValueMinus = new AIValue("-", AIValue.ValueTypeEnum.Operation);
            var aiValueMultiplication = new AIValue("*", AIValue.ValueTypeEnum.Operation);
            var aiValueDivision = new AIValue("/", AIValue.ValueTypeEnum.Operation);
            var aiValueRemainder = new AIValue("%", AIValue.ValueTypeEnum.Operation);
            var aiValueLeftShift = new AIValue("<<", AIValue.ValueTypeEnum.Operation);
            var aiValueRightShift = new AIValue(">>", AIValue.ValueTypeEnum.Operation);
            var aiValueLess = new AIValue("<", AIValue.ValueTypeEnum.Operation);
            var aiValueGreater = new AIValue(">", AIValue.ValueTypeEnum.Operation);
            var aiValueLessEqual = new AIValue("<=", AIValue.ValueTypeEnum.Operation);
            var aiValueGreaterEqual = new AIValue(">=", AIValue.ValueTypeEnum.Operation);
            var aiValueEqual = new AIValue("==", AIValue.ValueTypeEnum.Operation);
            var aiValueNotEqual = new AIValue("!=", AIValue.ValueTypeEnum.Operation);
            var aiValueAnd = new AIValue("&", AIValue.ValueTypeEnum.Operation);
            var aiValueXor = new AIValue("^", AIValue.ValueTypeEnum.Operation);
            var aiValueOr = new AIValue("|", AIValue.ValueTypeEnum.Operation);
            var aiValueConditionalOr = new AIValue("||", AIValue.ValueTypeEnum.Operation);
            var aiValueConditionalAnd = new AIValue("&&", AIValue.ValueTypeEnum.Operation);

            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
            var asmAddress = new AsmAddress();

            // 引数1個
            {
                Assert.ThrowsException<InvalidOperationException>(() =>
                {
                    AIValue.Calculation(aiValue0, aiValue1, asmLoad, asmAddress);
                });

                Assert.ThrowsException<InvalidOperationException>(() =>
                {
                    AIValue.Calculation(aiValuePlus, aiValue1, asmLoad, asmAddress);
                });
            }

            // 引数2個
            {
                Assert.ThrowsException<InvalidOperationException>(() =>
                {
                    AIValue.Calculation(aiValue0, aiValue1, aiValue2, asmLoad, asmAddress);
                });

                Assert.ThrowsException<InvalidOperationException>(() =>
                {
                    AIValue.Calculation(aiValueNegation, aiValue1, aiValue2, asmLoad, asmAddress);
                });
            }

            // 引数3個
            {
                Assert.ThrowsException<InvalidOperationException>(() =>
                {
                    AIValue.Calculation(aiValue0, aiValue1, aiValue2, aiValue3, asmLoad, asmAddress);
                });

                Assert.ThrowsException<InvalidOperationException>(() =>
                {
                    AIValue.Calculation(aiValueNegation, aiValue1, aiValue2, aiValue3, asmLoad, asmAddress);
                });
            }

            // 反転テスト
            {
                var result0 = AIValue.Calculation(aiValueNegation, aiValue1, asmLoad, asmAddress);
                Assert.AreEqual(~1, result0.ConvertTo<int>());
                
                var result1 = AIValue.Calculation(aiValueNegation, aiValueTrue, asmLoad, asmAddress);
                Assert.AreEqual(false, result1.ConvertTo<bool>());

                var result2 = AIValue.Calculation(aiValueNegation, aiValueFalse, asmLoad, asmAddress);
                Assert.AreEqual(true, result2.ConvertTo<bool>());
                
                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueNegation, aiValueABC, asmLoad, asmAddress);
                });
            }

            // ビット反転
            {
                var result0 = AIValue.Calculation(aiValueBitwiseComplement, aiValue1, asmLoad, asmAddress);
                Assert.AreEqual(~1, result0.ConvertTo<int>());

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueBitwiseComplement, aiValueTrue, asmLoad, asmAddress);
                });

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueBitwiseComplement, aiValueABC, asmLoad, asmAddress);
                });
            }

            // プラス
            {
                var result0 = AIValue.Calculation(aiValuePlus, aiValue1, aiValue2, asmLoad, asmAddress);
                Assert.AreEqual(3, result0.ConvertTo<int>());

                var result1 = AIValue.Calculation(aiValuePlus, aiValueABC, aiValueDEF, asmLoad, asmAddress);
                Assert.AreEqual("ABCDEF", result1.ConvertTo<string>());

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValuePlus, aiValue1, aiValueABC, asmLoad, asmAddress);
                });
            }

            // マイナス
            {
                var result0 = AIValue.Calculation(aiValueMinus, aiValue1, aiValue2, asmLoad, asmAddress);
                Assert.AreEqual(-1, result0.ConvertTo<int>());

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueMinus, aiValueABC, aiValueDEF, asmLoad, asmAddress);
                });

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueMinus, aiValue1, aiValueABC, asmLoad, asmAddress);
                });
            }

            // 掛け算
            {
                var result0 = AIValue.Calculation(aiValueMultiplication, aiValue1, aiValue2, asmLoad, asmAddress);
                Assert.AreEqual(2, result0.ConvertTo<int>());

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueMultiplication, aiValueABC, aiValueDEF, asmLoad, asmAddress);
                });

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueMultiplication, aiValue1, aiValueABC, asmLoad, asmAddress);
                });
            }

            // 割り算
            {
                var result0 = AIValue.Calculation(aiValueDivision, aiValue3, aiValue2, asmLoad, asmAddress);
                Assert.AreEqual(1, result0.ConvertTo<int>());

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueDivision, aiValueABC, aiValueDEF, asmLoad, asmAddress);
                });

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueDivision, aiValue1, aiValueABC, asmLoad, asmAddress);
                });
            }

            // 余り
            {
                var result0 = AIValue.Calculation(aiValueRemainder, aiValue1, aiValue2, asmLoad, asmAddress);
                Assert.AreEqual(1, result0.ConvertTo<int>());

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueRemainder, aiValueABC, aiValueDEF, asmLoad, asmAddress);
                });

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueRemainder, aiValue1, aiValueABC, asmLoad, asmAddress);
                });
            }


            // 左シフト
            {
                var result0 = AIValue.Calculation(aiValueLeftShift, aiValue1, aiValue2, asmLoad, asmAddress);
                Assert.AreEqual(4, result0.ConvertTo<int>());

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueLeftShift, aiValueABC, aiValueDEF, asmLoad, asmAddress);
                });

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueLeftShift, aiValue1, aiValueABC, asmLoad, asmAddress);
                });
            }

            // 右シフト
            {
                var result0 = AIValue.Calculation(aiValueRightShift, aiValue3, aiValue1, asmLoad, asmAddress);
                Assert.AreEqual(1, result0.ConvertTo<int>());

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueRightShift, aiValueABC, aiValueDEF, asmLoad, asmAddress);
                });

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueRightShift, aiValue1, aiValueABC, asmLoad, asmAddress);
                });
            }

            // より小さい
            {
                var result0 = AIValue.Calculation(aiValueLess, aiValue1, aiValue1, asmLoad, asmAddress);
                Assert.AreEqual(false, result0.ConvertTo<bool>());

                var result1 = AIValue.Calculation(aiValueLess, aiValue1, aiValue2, asmLoad, asmAddress);
                Assert.AreEqual(true, result1.ConvertTo<bool>());

                var result2 = AIValue.Calculation(aiValueLess, aiValue2, aiValue1, asmLoad, asmAddress);
                Assert.AreEqual(false, result2.ConvertTo<bool>());

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueLess, aiValueABC, aiValueDEF, asmLoad, asmAddress);
                });

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueLess, aiValue1, aiValueABC, asmLoad, asmAddress);
                });
            }

            // より大きい
            {
                var result0 = AIValue.Calculation(aiValueGreater, aiValue1, aiValue1, asmLoad, asmAddress);
                Assert.AreEqual(false, result0.ConvertTo<bool>());

                var result1 = AIValue.Calculation(aiValueGreater, aiValue1, aiValue2, asmLoad, asmAddress);
                Assert.AreEqual(false, result1.ConvertTo<bool>());

                var result2 = AIValue.Calculation(aiValueGreater, aiValue2, aiValue1, asmLoad, asmAddress);
                Assert.AreEqual(true, result2.ConvertTo<bool>());

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueGreater, aiValueABC, aiValueDEF, asmLoad, asmAddress);
                });

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueGreater, aiValue1, aiValueABC, asmLoad, asmAddress);
                });
            }

            // 以下
            {
                var result0 = AIValue.Calculation(aiValueLessEqual, aiValue1, aiValue1, asmLoad, asmAddress);
                Assert.AreEqual(true, result0.ConvertTo<bool>());

                var result1 = AIValue.Calculation(aiValueLessEqual, aiValue1, aiValue2, asmLoad, asmAddress);
                Assert.AreEqual(true, result1.ConvertTo<bool>());

                var result2 = AIValue.Calculation(aiValueLessEqual, aiValue2, aiValue1, asmLoad, asmAddress);
                Assert.AreEqual(false, result2.ConvertTo<bool>());

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueLessEqual, aiValueABC, aiValueDEF, asmLoad, asmAddress);
                });

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueLessEqual, aiValue1, aiValueABC, asmLoad, asmAddress);
                });
            }

            // 以上
            {
                var result0 = AIValue.Calculation(aiValueGreaterEqual, aiValue1, aiValue1, asmLoad, asmAddress);
                Assert.AreEqual(true, result0.ConvertTo<bool>());

                var result1 = AIValue.Calculation(aiValueGreaterEqual, aiValue1, aiValue2, asmLoad, asmAddress);
                Assert.AreEqual(false, result1.ConvertTo<bool>());

                var result2 = AIValue.Calculation(aiValueGreaterEqual, aiValue2, aiValue1, asmLoad, asmAddress);
                Assert.AreEqual(true, result2.ConvertTo<bool>());

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueGreaterEqual, aiValueABC, aiValueDEF, asmLoad, asmAddress);
                });

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueGreaterEqual, aiValue1, aiValueABC, asmLoad, asmAddress);
                });
            }


            // イコール
            {
                var result0 = AIValue.Calculation(aiValueEqual, aiValue1, aiValue1, asmLoad, asmAddress);
                Assert.AreEqual(true, result0.ConvertTo<bool>());

                var result1 = AIValue.Calculation(aiValueEqual, aiValue1, aiValue2, asmLoad, asmAddress);
                Assert.AreEqual(false, result1.ConvertTo<bool>());

                var result2 = AIValue.Calculation(aiValueEqual, aiValue2, aiValue1, asmLoad, asmAddress);
                Assert.AreEqual(false, result2.ConvertTo<bool>());

                var result3 = AIValue.Calculation(aiValueEqual, aiValueABC, aiValueABC, asmLoad, asmAddress);
                Assert.AreEqual(true, result3.ConvertTo<bool>());

                var result4 = AIValue.Calculation(aiValueEqual, aiValueABC, aiValueDEF, asmLoad, asmAddress);
                Assert.AreEqual(false, result4.ConvertTo<bool>());

                var result5 = AIValue.Calculation(aiValueEqual, aiValueTrue, aiValueTrue, asmLoad, asmAddress);
                Assert.AreEqual(true, result5.ConvertTo<bool>());

                var result6 = AIValue.Calculation(aiValueEqual, aiValueTrue, aiValueFalse, asmLoad, asmAddress);
                Assert.AreEqual(false, result6.ConvertTo<bool>());

                var result7 = AIValue.Calculation(aiValueEqual, aiValueFalse, aiValueFalse, asmLoad, asmAddress);
                Assert.AreEqual(true, result7.ConvertTo<bool>());

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueEqual, aiValue1, aiValueABC, asmLoad, asmAddress);
                });
            }

            // Notイコール
            {
                var result0 = AIValue.Calculation(aiValueNotEqual, aiValue1, aiValue1, asmLoad, asmAddress);
                Assert.AreEqual(false, result0.ConvertTo<bool>());

                var result1 = AIValue.Calculation(aiValueNotEqual, aiValue1, aiValue2, asmLoad, asmAddress);
                Assert.AreEqual(true, result1.ConvertTo<bool>());

                var result2 = AIValue.Calculation(aiValueNotEqual, aiValue2, aiValue1, asmLoad, asmAddress);
                Assert.AreEqual(true, result2.ConvertTo<bool>());

                var result3 = AIValue.Calculation(aiValueNotEqual, aiValueABC, aiValueABC, asmLoad, asmAddress);
                Assert.AreEqual(false, result3.ConvertTo<bool>());

                var result4 = AIValue.Calculation(aiValueNotEqual, aiValueABC, aiValueDEF, asmLoad, asmAddress);
                Assert.AreEqual(true, result4.ConvertTo<bool>());

                var result5 = AIValue.Calculation(aiValueNotEqual, aiValueTrue, aiValueTrue, asmLoad, asmAddress);
                Assert.AreEqual(false, result5.ConvertTo<bool>());

                var result6 = AIValue.Calculation(aiValueNotEqual, aiValueTrue, aiValueFalse, asmLoad, asmAddress);
                Assert.AreEqual(true, result6.ConvertTo<bool>());

                var result7 = AIValue.Calculation(aiValueNotEqual, aiValueFalse, aiValueFalse, asmLoad, asmAddress);
                Assert.AreEqual(false, result7.ConvertTo<bool>());

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueNotEqual, aiValue1, aiValueABC, asmLoad, asmAddress);
                });
            }

            // And
            {
                var result0 = AIValue.Calculation(aiValueAnd, aiValue2, aiValue3, asmLoad, asmAddress);
                Assert.AreEqual(2, result0.ConvertTo<int>());

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueAnd, aiValueABC, aiValueDEF, asmLoad, asmAddress);
                });

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueAnd, aiValue1, aiValueABC, asmLoad, asmAddress);
                });
            }

            // Xor
            {
                var result0 = AIValue.Calculation(aiValueXor, aiValue2, aiValue3, asmLoad, asmAddress);
                Assert.AreEqual(1, result0.ConvertTo<int>());

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueXor, aiValueABC, aiValueDEF, asmLoad, asmAddress);
                });

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueXor, aiValue1, aiValueABC, asmLoad, asmAddress);
                });
            }

            // Or
            {
                var result0 = AIValue.Calculation(aiValueOr, aiValue1, aiValue2, asmLoad, asmAddress);
                Assert.AreEqual(3, result0.ConvertTo<int>());

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueXor, aiValueABC, aiValueDEF, asmLoad, asmAddress);
                });

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueXor, aiValue1, aiValueABC, asmLoad, asmAddress);
                });
            }

            // 条件接続のOr
            {
                var result0 = AIValue.Calculation(aiValueConditionalOr, aiValueTrue, aiValueTrue, asmLoad, asmAddress);
                Assert.AreEqual(true, result0.ConvertTo<bool>());

                var result1 = AIValue.Calculation(aiValueConditionalOr, aiValueTrue, aiValueFalse, asmLoad, asmAddress);
                Assert.AreEqual(true, result1.ConvertTo<bool>());

                var result2 = AIValue.Calculation(aiValueConditionalOr, aiValueFalse, aiValueTrue, asmLoad, asmAddress);
                Assert.AreEqual(true, result2.ConvertTo<bool>());

                var result3 = AIValue.Calculation(aiValueConditionalOr, aiValueFalse, aiValueFalse, asmLoad, asmAddress);
                Assert.AreEqual(false, result3.ConvertTo<bool>());

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueConditionalOr, aiValueABC, aiValueDEF, asmLoad, asmAddress);
                });

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueConditionalOr, aiValue1, aiValueABC, asmLoad, asmAddress);
                });
            }

            // 条件接続のAnd
            {
                var result0 = AIValue.Calculation(aiValueConditionalAnd, aiValueTrue, aiValueTrue, asmLoad, asmAddress);
                Assert.AreEqual(true, result0.ConvertTo<bool>());

                var result1 = AIValue.Calculation(aiValueConditionalAnd, aiValueTrue, aiValueFalse, asmLoad, asmAddress);
                Assert.AreEqual(false, result1.ConvertTo<bool>());

                var result2 = AIValue.Calculation(aiValueConditionalAnd, aiValueFalse, aiValueTrue, asmLoad, asmAddress);
                Assert.AreEqual(false, result2.ConvertTo<bool>());

                var result3 = AIValue.Calculation(aiValueConditionalAnd, aiValueFalse, aiValueFalse, asmLoad, asmAddress);
                Assert.AreEqual(false, result3.ConvertTo<bool>());

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueConditionalAnd, aiValueABC, aiValueDEF, asmLoad, asmAddress);
                });

                Assert.ThrowsException<InvalidAIValueException>(() =>
                {
                    AIValue.Calculation(aiValueConditionalAnd, aiValue1, aiValueABC, asmLoad, asmAddress);
                });
            }
        }
    }
}
