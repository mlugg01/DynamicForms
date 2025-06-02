
public class MathematicalConstants
{
    public static double? GetConstant(string constantName)
    {
        switch (constantName)
        {
            case "E":
                return Math.E;

            case "PI":
                return Math.PI;

            case "TAU":
                return Math.Tau;

            default:
                return null;
        }
    }
}

