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
    public static void miniMaxSum(List<int> arr)
    {
        arr.Sort();
        long sum = arr.Select(i => (long)i).Sum();
        long min=0,max=0;
        if(arr.Count()>=2){
            min = sum-arr[arr.Count()-1];
            max = sum-arr[0];
        }
        if(arr.Count()==1)
        {
            min = arr[0];
            max = arr[0];
        }
        Console.WriteLine($"{min} {max}");
    }
}

class Solution
{
    public static void Main(string[] args)
    {

        List<int> arr = Console.ReadLine().TrimEnd().Split(' ').ToList().Select(arrTemp => Convert.ToInt32(arrTemp)).ToList();

        Result.miniMaxSum(arr);
    }
}
