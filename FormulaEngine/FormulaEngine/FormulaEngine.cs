using System.Dynamic;
using MC = MathematicalConstants;
using F = MathematicalFunctions;
using Units = UnitsOfMeasure;

public class FormulaEngine
{
	public static double? CalculateFormula(FormulaItem formulaItem, List<UnitItem> unitItems, ExpandoObject obj)
	{
		if (formulaItem is null)
		{
			Console.WriteLine("FormulaItem cannot be null");
			return null;
		}
		if (string.IsNullOrWhiteSpace(formulaItem.Formula))
		{
			Console.WriteLine("Formula must be entered.");
			return null;
		}
		
		if (unitItems is null)
		{
			Console.WriteLine("UnitItems cannot be null");
			return null;
		}
		
		if (obj is null)
		{
			Console.WriteLine("Obj cannot be null.");
			return null;
		}

			var result = Calculate(formulaItem, unitItems, obj, 0);
			return result?.Result;
	}

	// public static double? CalculateFormula(string formula, ExpandoObject obj)
	// {
	// 	if (string.IsNullOrWhiteSpace(formula))
	// 	{
	// 		Console.WriteLine("Formula must be entered.");
	// 		return null;
	// 	}
	// 	if (obj is null)
	// 	{
	// 		Console.WriteLine("Obj cannot be null.");
	// 		return null;
	// 	}

	// 	var formulaParts = formula.Split(' ');
	// 	var result = CalculateFormulaFromParts(formulaParts, obj, 0);
	// 	return result?.Result;
	// }

	private static CalculateFormulaFromPartsResult Calculate(FormulaItem formulaItem, List<UnitItem> unitItems, ExpandoObject obj, int partIndex)
	{
		try 
		{
			var parts = formulaItem.Formula.Split(' ');			
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
						// TODO: get unit
						// TODO: convert
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
					subResult = Calculate(formulaItem, unitItems, obj, currIndex + 1);

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
			Console.WriteLine(ex.Message, nameof(Calculate));
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
		var operations = new string[] {"*", "/", "+", "-"};
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

	//private static double? GetInternalVariableValue(string variableName, double displayVariableValue, List<ValueConfig> variableConfigs, List<UnitItem> unitItems)
	//{
	//	try
	//	{
	//		var variableConfig = variableConfigs.FirstOrDefault(c => c.ValueName == variableName);
	//		if (variableConfig is null || string.IsNullOrWhiteSpace(variableConfig.UnitType))
	//			return null;
			
	//		var unitType = variableConfig.UnitType;
	//		var unitTypeParts = unitType.Split('/');
	//		if (unitTypeParts.Length == 0 || unitTypeParts.Length > 2)
	//			throw new FormatException("Unit Type incorrectly formatted.");
						
	//		var unitConversionNumerator = GetInternalUnitConversionFactorFromUnitTypePart(unitItems, unitTypeParts[0]);
	//		if (unitConversionNumerator is null)
	//			return null;
						
	//		double? unitConversionDenominator = 1;
	//		if (unitTypeParts.Length == 2)
	//		{
	//			unitConversionDenominator = GetInternalUnitConversionFactorFromUnitTypePart(unitItems, unitTypeParts[1]);
	//			if (unitConversionDenominator is null)
	//				return null;
	//		}
						
	//		return displayVariableValue * unitConversionNumerator / unitConversionDenominator;			
	//	}
	//	catch (Exception ex)
	//	{
	//		Console.WriteLine(ex.Message, nameof(GetInternalVariableValue));
	//	}
	//	return null;
	//}
	
	//private static double? GetInternalUnitConversionFactorFromUnitTypePart(List<UnitItem> unitItems, string unitTypePart)
	//{
	//	try
	//	{
	//		double unitConversionFactorResult = 1;
			
	//		var unitTypeParts = unitTypePart.Split('*');
	//		double? conversionFactor = null;
	//		string selectedUnit;
	//		foreach (var unitType in unitTypeParts)
	//		{
	//			if (!Units.UnitIsSupported(unitType))
	//				continue;
					
	//			selectedUnit = unitItems.FirstOrDefault(u => u.UnitType == unitType)?.SelectedUnit;
	//			if (string.IsNullOrWhiteSpace(selectedUnit))
	//				return null;
					
	//			conversionFactor = Units.ConvertValueToInternalUnits(1, unitType, selectedUnit);
	//			if (conversionFactor is null)
	//				return null;
					
	//			unitConversionFactorResult *= conversionFactor.Value;
	//		}
			
	//		return unitConversionFactorResult;
	//	}
	//	catch (Exception ex)
	//	{
	//		Console.WriteLine(ex.Message, nameof(GetInternalUnitConversionFactorFromUnitTypePart));
	//	}
	//	return null;
	//}

	class CalculateFormulaFromPartsResult
	{
		public int LastPartIndex { get; set; }
		public double? Result { get; set; }
	}
}