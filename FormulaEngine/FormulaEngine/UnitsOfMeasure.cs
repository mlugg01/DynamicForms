public class UnitsOfMeasure
{
	private static string[] _supportedUnitTypes = 
	{
		"ANGLE",
		"DISTANCE",
		"LENGTH",
		"TEMPERATURE",
		"WEIGHT"
	};
	
	public static bool UnitIsSupported(string unitType) => _supportedUnitTypes.Contains(unitType);
	
	public static double? ConvertValueToInternalUnits(double? value, string unitType, string displayUnit)
	{
		switch (unitType)
		{
			case "ANGLE":
				return GetAngleInInternalUnits(value, displayUnit);
				
			case "DISTANCE":
				return GetDistanceInInternalUnits(value, displayUnit);
				
			case "LENGTH":
				return GetLengthInInternalUnits(value, displayUnit);
				
			case "TEMPERATURE":
				return GetTemperatureInInternalUnits(value, displayUnit);
				
			case "WEIGHT":
				return GetWeightInInternalUnits(value, displayUnit);
				
			default:
				return value;
		}
	}
	
	
	#region Angles

	private const double DEGREES_TO_RADIANS_MULTIPLIER = Math.PI / 180;
	private const double RADIANS_TO_DEGREES_MULTIPLIER = 180 / Math.PI;

	public static double? GetAngleInInternalUnits(double? angle, string displayUnit)
	{
		// Internal units for angle are radians (RAD).

		if (angle is null || displayUnit == "RAD")
			return angle;

		switch (displayUnit)
		{
			case "DEG":
				return angle * DEGREES_TO_RADIANS_MULTIPLIER;

			default:
				return null;
		}
	}

	public static double? GetAngleInDisplayUnits(double? angle, string displayUnit)
	{
		// Internal units for angle are radians (RAD).

		if (angle is null || displayUnit == "RAD")
			return angle;

		switch (displayUnit)
		{
			case "DEG":
				return angle * RADIANS_TO_DEGREES_MULTIPLIER;

			default:
				return null;
		}
	}

	#endregion

	#region Distances

	private const double MILES_TO_KILOMETERS_MULTIPLIER = 1.609344f;
	private const double KILOMETERS_TO_MILES_MULTIPLIER = 0.6213711922f;

	public static double? GetDistanceInInternalUnits(double? distance, string displayUnit)
	{
		if (distance is null || displayUnit == "KM")
			return distance;

		switch (displayUnit)
		{
			case "MI":
				return distance * MILES_TO_KILOMETERS_MULTIPLIER;

			default:
				return null;
		}
	}

	public static double? GetDistanceInDisplayUnits(double? distance, string displayUnit)
	{
		if (distance is null || displayUnit == "KM")
			return distance;

		switch (displayUnit)
		{
			case "MI":
				return distance * KILOMETERS_TO_MILES_MULTIPLIER;

			default:
				return null;
		}
	}

	#endregion

	#region Lengths
	private const double FEET_TO_METERS_MULTIPLIER = 0.3048000000012192000000048768f;
	private const double METERS_TO_FEET_MULTIPLIER = 3.280839895f;

	private const double INCHES_TO_METERS_MULTIPLIER = 0.02540000000003708400000005414264f;
	private const double METERS_TO_INCHES_MULTIPLER = 39.3700787401f;

	private const double CENTIMETERS_TO_METERS_MULTIPLIER = 0.01f;
	private const double METERS_TO_CENTIMETERS_MULTIPLIER = 100f;

	private const double MILLIMETERS_TO_METERS_MULTIPLIER = 0.001f;
	private const double METERS_TO_MILLIMETERS_MULTIPLIER = 1000f;

	public static double? GetLengthInInternalUnits(double? length, string displayUnit)
	{
		// Internal units for length are meters (M).

		if (length is null || displayUnit == "M")
			return length;

		switch (displayUnit)
		{
			case "F":
				return length * FEET_TO_METERS_MULTIPLIER;
				
			case "IN":
				return length * INCHES_TO_METERS_MULTIPLIER;

			case "CM":
				return length * CENTIMETERS_TO_METERS_MULTIPLIER;

			case "MM":
				return length * MILLIMETERS_TO_METERS_MULTIPLIER;

			default:
				return null;
		}
	}

	public static double? GetLengthInDisplayUnits(double? length, string displayUnit)
	{
		// Internal units for length are meters (M).

		if (length is null || displayUnit == "M")
			return length;

		switch (displayUnit)
		{
			case "F":
				return length * METERS_TO_FEET_MULTIPLIER;

			case "IN":
				return length * METERS_TO_INCHES_MULTIPLER;

			case "CM":
				return length * METERS_TO_CENTIMETERS_MULTIPLIER;

			case "MM":
				return length * METERS_TO_MILLIMETERS_MULTIPLIER;

			default:
				return null;
		}
	}

	#endregion

	#region  Temperatures

	private const double CELSIUS_TO_KELVIN_ADDITIVE = 273.15f;
	private const double KELVIN_TO_CELSIUS_ADDITIVE = -273.15f;

	private const double FAHRENHEIT_TO_KELVIN_MULTIPLIER = 5.0f/9;
	private const double FAHRENHEIT_TO_KELVIN_ADDITIVE = 459.67f;

	private const double KELVIN_TO_FAHRENHEIT_MULTIPLER = 9.0f/5;
	private const double KELVIN_TO_FAHRENHEIT_ADDITIVE = -459.67f;

	public static double? GetTemperatureInInternalUnits(double? temperature, string displayUnit)
	{
		// Internal units for temperature are Kelvin (K).

		if (temperature is null || displayUnit == "K")
			return temperature;

		switch (displayUnit)
		{
			case "C":
				return temperature + CELSIUS_TO_KELVIN_ADDITIVE;

			case "F":
				return (temperature + FAHRENHEIT_TO_KELVIN_ADDITIVE) * FAHRENHEIT_TO_KELVIN_MULTIPLIER;

			default:
				return null;
		}
	}

	public static double? GetTemperatureInDisplayUnits(double? temperature, string displayUnit)
	{
		// Internal units for temperature are Kelvin (K).

		if (temperature is null || displayUnit == "K")
			return temperature;

		switch (displayUnit)
		{
			case "C":
				return temperature + KELVIN_TO_CELSIUS_ADDITIVE;

			case "F":
				return (temperature * KELVIN_TO_FAHRENHEIT_MULTIPLER) + KELVIN_TO_FAHRENHEIT_ADDITIVE;

			default:
				return null;
		}
	}

	#endregion

	#region  Weights

	private const double KILOGRAMS_TO_NEWTONS_MULTIPLIER = 9.8066500286389f;
	private const double NEWTONS_TO_KILOGRAMS_MULTIPLIER = 0.101971621f;

	private const double POUNDS_TO_NEWTONS_MULTIPLIER = 4.4482216282509f;
	private const double NEWTONS_TO_POUNDS_MULTIPLIER = 0.22480894244319f;

	public static double? GetWeightInInternalUnits(double? weight, string displayUnit)
	{
		// Internal units for weight is Newtons (N)

		if (weight is null || displayUnit == "N")
			return weight;

		switch (displayUnit)
		{
			case "KG":
				return weight * KILOGRAMS_TO_NEWTONS_MULTIPLIER;

			case "LB":
				return weight * POUNDS_TO_NEWTONS_MULTIPLIER;

			default:
				return null;
		}
	}

	public static double? GetWeightInDisplayUnits(double? weight, string displayUnit)
	{
		// Internal units for weight is Newtons (N)

		if (weight is null || displayUnit == "N")
			return weight;

		switch (displayUnit)
		{
			case "KG":
				return weight * NEWTONS_TO_KILOGRAMS_MULTIPLIER;

			case "LB":
				return weight * NEWTONS_TO_POUNDS_MULTIPLIER;

			default:
				return null;
		}
	}

	#endregion
}