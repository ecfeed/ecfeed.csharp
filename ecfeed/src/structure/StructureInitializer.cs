using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;


namespace EcFeed
{
    public interface StructureInitializer
    {
        void Source(Assembly assembly, params string[] source);

        void Activate(params string[] signatureStructure);

        T Instantiate<T>(T type, Queue<string> arguments);

        object Instantiate(string signatureStructure, Queue<string> arguments);

        object[] GetTestCase(string signatureMethod, Queue<string> arguments);

        HashSet<Structure> GetStructuresRaw();

        HashSet<Structure> GetStructuresActive();

        HashSet<string> GetNamesSimpleRaw();

        HashSet<string> GetNamesSimpleActive();

        HashSet<string> GetNamesQualifiedRaw();

        HashSet<string> GetNamesQualifiedActive();
    }
}