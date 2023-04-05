using System;
using System.Collections.Generic;
using System.Reflection;

namespace EcFeed
{
    internal interface StructuresSetter
    {
        List<Structure> Parse(Assembly assembly, params string[] source);
        void Activate(HashSet<Structure> structure, params string[] signatures);
    }
}