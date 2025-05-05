using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Text;
using System;

class Result
{
    public static string timeConversion(string s)
    {
        string format = s.Substring(s.Length-2);
        string newHour = "";
        int hour = Convert.ToInt32(s.Substring(0,2));
        if(format == "AM")
        {
            if(hour==12) newHour = "00";
            else newHour = hour.ToString("D2");
        } else 
        {
            if(hour==12) newHour = "12";
            else newHour = (hour+12).ToString("D2");
        }
        s=s.Replace(s.Substring(0,2),newHour); 
        return s.Substring(0,s.Length-2);
    }

}

class Solution
{
    public static void Main(string[] args)
    {
        TextWriter textWriter = new StreamWriter(@System.Environment.GetEnvironmentVariable("OUTPUT_PATH"), true);

        string s = Console.ReadLine();

        string result = Result.timeConversion(s);

        textWriter.WriteLine(result);

        textWriter.Flush();
        textWriter.Close();
    }
}
