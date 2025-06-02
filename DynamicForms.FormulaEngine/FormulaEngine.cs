using System.Dynamic;
using System;
using System.Collections.Generic;
using MC = DynamicForms.ForumulaEngine.MathematicalConstants;
using F = DynamicForms.ForumulaEngine.MathematicalFunctions;
using System.Linq;

namespace DynamicForms.ForumulaEngine
{
    public class FormulaEngine
    {
        public static double? CalculateFormula(string formula, ExpandoObject obj)
        {
            if (string.IsNullOrWhiteSpace(formula))
            {
                throw new ArgumentNullException("Formula must be entered.");
            }
            if (obj is null)
            {
                throw new ArgumentNullException("Obj cannot be null.");
            }

            var formulaParts = formula.Split(' ');
            var result = CalculateFormulaFromParts(formulaParts, obj, 0);
            return result?.Result;
        }

        private static CalculateFormulaFromPartsResult CalculateFormulaFromParts(string[] parts, ExpandoObject obj, int partIndex)
        {
            try
            {
                var objDict = obj as IDictionary<string, object>;

                CalculateFormulaFromPartsResult subResult = null;

                double number;
                string variableName;
                double? variableValue;
                string constantName;
                double? constantValue;
                double? result = null;
                string operation = "";
                string functionName = "";
                string functionParams = "";
                double? functionResult = null;
                bool evaluatingFunctionInputExpression = false;

                string part;
                int currIndex;

                for (currIndex = partIndex; currIndex < parts.Length; currIndex++)
                {
                    part = parts[currIndex];

                    if (string.IsNullOrWhiteSpace(part))
                        continue;

                    if (FormulaPartIsVariable(part))
                    {
                        variableValue = null;

                        variableName = ExtractFormulaPartValue(part);
                        if (string.IsNullOrWhiteSpace(variableName))
                            return null;

                        if (objDict.ContainsKey(variableName) && objDict[variableName] != null)
                        {
                            variableValue = Convert.ToDouble(objDict[variableName]);
                        }

                        result = GetResultFromValue(result, variableValue, operation);
                        if (result is null)
                            return null;
                    }
                    else if (FormulaPartIsConstant(part))
                    {
                        constantName = ExtractFormulaPartValue(part);

                        constantValue = MC.GetConstant(constantName);

                        result = GetResultFromValue(result, constantValue, operation);
                        if (result is null)
                            return null;
                    }
                    else if (FormulaPartIsFunction(part))
                    {
                        functionName = ExtractFormulaPartValue(part);
                    }
                    else if (FormulaPartIsFunctionParameters(part))
                    {
                        functionParams = ExtractFormulaPartValue(part);
                    }
                    else if (FormulaPartIsBasicOperation(part))
                    {
                        operation = part;
                    }
                    else if (part == "[")
                    {
                        evaluatingFunctionInputExpression = true;
                        continue;
                    }
                    else if (part == "]")
                    {
                        functionResult = F.ApplyFunction(functionName, functionParams, subResult?.Result);
                        if (functionResult is null)
                            return null;

                        result = GetResultFromValue(result, functionResult, operation);
                        if (result is null)
                            return null;

                        evaluatingFunctionInputExpression = false;
                    }
                    else if (part == "(")
                    {
                        subResult = CalculateFormulaFromParts(parts, obj, currIndex + 1);

                        if (!evaluatingFunctionInputExpression)
                        {
                            result = GetResultFromValue(result, subResult?.Result, operation);
                            if (result is null)
                                return null;
                        }

                        currIndex = subResult.LastPartIndex;
                    }
                    else if (part == ")")
                    {
                        return new CalculateFormulaFromPartsResult()
                        {
                            LastPartIndex = currIndex,
                            Result = result
                        };
                    }
                    else
                    {
                        if (!double.TryParse(part, out number))
                        {
                            Console.WriteLine("Incorrectly formatted number encountered.");
                            return null;
                        }

                        result = GetResultFromValue(result, number, operation);
                        if (result is null)
                            return null;
                    }
                }

                return new CalculateFormulaFromPartsResult()
                {
                    LastPartIndex = currIndex,
                    Result = result
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, nameof(CalculateFormulaFromParts));
            }
            return null;
        }

        private static bool FormulaPartIsVariable(string part)
        {
            return part.StartsWith("VAR:");
        }

        private static bool FormulaPartIsConstant(string part)
        {
            return part.StartsWith("CONST:");
        }

        private static bool FormulaPartIsBasicOperation(string formulaPart)
        {
            var operations = new string[] { "*", "/", "+", "-" };
            return operations.Contains(formulaPart);
        }

        private static bool FormulaPartIsFunction(string formulaPart)
        {
            return formulaPart.StartsWith("FUNC:");
        }

        private static bool FormulaPartIsFunctionParameters(string formulaPart)
        {
            return formulaPart.StartsWith("PARAMS:");
        }

        private static string ExtractFormulaPartValue(string formulaPart)
        {
            var startIndex = formulaPart.IndexOf(':') + 1;
            if (formulaPart.Length > startIndex)
            {
                return formulaPart.Substring(startIndex);
            }
            return null;
        }

        private static double? GetResultFromValue(double? result, double? value, string operation)
        {
            if (value is null)
                return null;

            if (result is null)
            {
                result = value;
            }
            else
            {
                result = ApplyOperation(result.Value, value.Value, operation);
                if (result is null)
                    return null;
            }
            return result;
        }

        private static double? ApplyOperation(double result, double value, string operation)
        {
            switch (operation)
            {
                case "*":
                    return result * value;

                case "/":
                    if (value == 0)
                    {
                        Console.WriteLine("Divide by 0 encountered.");
                        return null;
                    }
                    return result / value;

                case "+":
                    return result + value;

                case "-":
                    return result - value;

                default:
                    Console.WriteLine("Malformed formula - invalid or missing operation.");
                    return null;
            }
        }

        class CalculateFormulaFromPartsResult
        {
            public int LastPartIndex { get; set; }
            public double? Result { get; set; }
        }
    }
}