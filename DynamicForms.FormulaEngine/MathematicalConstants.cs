using System;

namespace DynamicForms.ForumulaEngine
{
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


                default:
                    return null;
            }
        }
    }
}