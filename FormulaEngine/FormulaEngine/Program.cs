using System.Dynamic;

public class FormulaEngineTest
{
	private static dynamic obj;

	public static void Main(string[] args)
	{
		obj = new ExpandoObject();

		obj.P1 = 50;
		obj.P2 = 100;
		obj.P3 = -1.5;
		obj.P4 = 3;
		obj.P5 = 15;

		

		// var formula = "( VAR:P1 * VAR:P2 * ( VAR:P3 / ( VAR:P4  + VAR:P5 ) ) + 6 ) * CONST:E";
		// var formula = "VAR:P1 * VAR:P2";
		// var formula = "FUNC:POW [ ( VAR:P1 + VAR:P2 - FUNC:LOG [ ( FUNC:EXP [ ( 2 ) ] ) ] ) PARAMS:2 ]  + ( VAR:P1 + VAR:P2 ) + 6";

		// var formula = "FUNC:POW [ ( -9 ) PARAMS:6.0/2.0 ]";
		var formulaItem = GetFormulaItem();
		var unitItems = GetUnitItems();
		var result = FormulaEngine.CalculateFormula(formulaItem, unitItems, obj);
		if (result is null)
		{
			Console.WriteLine("result was null");
		}
		else
		{
			Console.WriteLine(result.ToString());
		}
	}

	private static FormulaItem GetFormulaItem()
	{
		return new FormulaItem()
		{
			Formula = "( VAR:P1 * VAR:P2 - FUNC:POW [ ( 3 ) PARAMS:2 ] )", // 50lb * 100 $/lb - 3^2 
			FormulaValueConfig = new ValueConfig()
			{
				ValueName = "Cost",
				DecimalPlaces = 2,
				Minimum = 0
			},
			VariableConfig = new List<ValueConfig>()
			{
				new ValueConfig()
				{
					ValueName = "P1",
					DecimalPlaces = 3,
					Maximum = 1000,
					Minimum = 0,
					UnitType = "WEIGHT"
				},
				new ValueConfig()
				{
					ValueName = "P2",
					DecimalPlaces = 3,
					Maximum = 1000,
					Minimum = 0,
					UnitType = "CURRENCY/WEIGHT"
				}
			}
		};
	}
	
	private static List<UnitItem> GetUnitItems()
	{
		return new List<UnitItem>()
		{
			new UnitItem() 
			{
				UnitType = "Weight",
				SelectedUnit = "KG"
			}
		};
	}
}
