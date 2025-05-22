using System;

namespace C__Day3.Misc
{
    public static class ExtentionFunctions
    {
        public static bool StringValidationCheck(this string str)
        {
            if (str.Substring(0, 1).ToLower() == "s" && str.Length == 6)
                return true;
            return false;
        }
    }
}
