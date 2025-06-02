public class ValueConfig
{
    public string ValueName { get; set; } // variable name or function name
    public string UnitType { get; set; }
    public string InternalUnit { get; set; }
    public int? DecimalPlaces { get; set; }
    public double? Minimum { get; set; }
    public double? Maximum { get; set; }
}