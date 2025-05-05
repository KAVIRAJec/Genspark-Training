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
    public static void plusMinus(List<int> arr)
    {
        int pos = 0, neg = 0, zero = 0;
        foreach(int i in arr)
        {
            if(i>0)pos++;
            else if(i<0)neg++;
            else zero++;
        }
        Console.WriteLine("{0:0.000000}",(float)pos/arr.Count);
        Console.WriteLine("{0:0.000000}",(float)neg/arr.Count);
        Console.WriteLine("{0:0.000000}",(float)zero/arr.Count);
    }

}

class Solution
{
    public static void Main(string[] args)
    {
        int n = Convert.ToInt32(Console.ReadLine().Trim());

        List<int> arr = Console.ReadLine().TrimEnd().Split(' ').ToList().Select(arrTemp => Convert.ToInt32(arrTemp)).ToList();

        Result.plusMinus(arr);
    }
}
