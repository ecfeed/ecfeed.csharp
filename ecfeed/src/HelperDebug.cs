// #define VERBOSE

using System;
using System.Diagnostics;

namespace EcFeed
{
    internal static class HelperDebug
    {
        [Conditional("VERBOSE")]
        public static void PrintTrace(string header, string trace)
        {
            Console.WriteLine($"{ DateTime.Now.ToString()} - { header }\n{ trace }\n");
        }
        
    }
}