using System;
using System.Linq;

namespace EcFeed
{
    public static class Dependencies
    {
        public static Type[] UserType { get; }

        static Dependencies()
        {
            UserType =  AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(t => t.GetTypes())
                       .Where(
                           t => t.IsEnum && 
                           !t.FullName.StartsWith("System.") && 
                           !t.FullName.StartsWith("NUnit.") &&
                           !t.FullName.StartsWith("Interop+") &&
                           !t.FullName.StartsWith("Microsoft.") &&
                           !t.FullName.StartsWith("Newtonsoft.") &&
                           !t.FullName.StartsWith("MS.") &&
                           !t.FullName.StartsWith("Mono.") &&
                           !t.FullName.StartsWith("Internal.")
                        )
                        .ToArray();
        }

    }   

}