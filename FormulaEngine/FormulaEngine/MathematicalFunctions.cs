using System.Reflection.Metadata.Ecma335;

public class MathematicalFunctions
{
    public static double? ApplyFunction(string functionName, string functionParameters, double? inputValue)
    {
        try
        {
            if (inputValue is null)
                return null;

            switch (functionName)
            {
                case "EXP":
                    return (double)Math.Exp((double)inputValue);

                case "LOG":
                    if (inputValue < 0)
                    {
                        throw new ArgumentOutOfRangeException("A negative value was encountered by a logarithm function.");
                    }

                    if (string.IsNullOrWhiteSpace(functionParameters))
                    {
                        return (double)Math.Log((double)inputValue);
                    }
                    else
                    {
                        var logBase = int.Parse(functionParameters);
                        return (double)Math.Log((double)inputValue, logBase);
                    }

                case "POW":
                    var exponent = GetPowerExponentFromFunctionParameter(functionParameters);
                    if (exponent is null)
                        return null;

                    if (!InputValueCanBeRaisedByExponent((double)inputValue, (double)exponent))
                    {
                        throw new ArgumentOutOfRangeException($"A negative value cannot be raised to the power of {exponent}.");
                    }

                    var calculateReciprocal = exponent < 0;
                    if (calculateReciprocal)
                    {
                        return 1.0 / Math.Pow(inputValue.Value, Math.Abs(exponent.Value));
                    }
                    else
                    {
                        return (double)Math.Pow(inputValue.Value, exponent.Value);
                    }
            }
                    
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message, "MathematicalFunctions-ApplyFunction");
            throw;
        }
        return null;
    }

    private static bool InputValueCanBeRaisedByExponent(double inputValue, double exponent)
    {
        return inputValue > 0 || Math.Abs(exponent) % 1 == 0;
    }

    private static double? GetPowerExponentFromFunctionParameter(string functionParameter)
    {
        if (string.IsNullOrWhiteSpace(functionParameter))
            return null;

        if (functionParameter.Contains('/'))
        {
            var parameterParts = functionParameter.Split('/');
            var numerator = double.Parse(parameterParts[0]);
            var denominator = double.Parse(parameterParts[1]);
            return numerator / denominator;
        }
        else
        {
            return double.Parse(functionParameter);
        }
    }
}