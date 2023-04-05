using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;


namespace EcFeed
{
    internal class StructureInitializerDefault : StructureInitializer
    {
        private StructuresSetter structuresSetter = StructuresSetterDefault.Get(StructureSetterDefault.Get());
        private HashSet<Structure> structures = new HashSet<Structure>();

        private StructureInitializerDefault()
        {}

        public static StructureInitializer Get()
        {
            return new StructureInitializerDefault();
        }

        public void Source(Assembly assembly, params string[] source)
        {
            var structures = structuresSetter.Parse(assembly, source);

            foreach (var structure in structures)
            {
                AddStructure(structure);
            }
        }

        public void Activate(params string[] signatureStructure)
        {

        }

        public T Instantiate<T>(T type, Queue<string> arguments)
        {
            return default(T);
        }

        public object Instantiate(string signatureStructure, Queue<string> arguments)
        {
            return null;
        }

        public object[] GetTestCase(string signatureMethod, Queue<string> arguments)
        {
            return null;
        }

        public HashSet<Structure> GetStructuresRaw()
        {
            return structures;
        }

        public HashSet<Structure> GetStructuresActive()
        {
            return null;
        }

        public HashSet<string> GetNamesSimpleRaw()
        {
            var results = new HashSet<string>();

            foreach (var structure in structures)
            {
                results.Add(structure.NameSimple);
            }

            return results;
        }

        public HashSet<string> GetNamesSimpleActive()
        {
            return null;
        }

        public HashSet<string> GetNamesQualifiedRaw()
        {
            var results = new HashSet<string>();

            foreach (var structure in structures)
            {
                results.Add(structure.NameQualified);
            }

            return results;
        }

        public HashSet<string> GetNamesQualifiedActive()
        {
            return null;
        }

        private void AddStructure(Structure source) 
        {
            var success = structures.Add(source);

            if (!success) {

                foreach (var structure in structures) {
                    if (structure.Equals(source)) {
                        throw new SystemException("The structure '" + source.NameQualified + "' could not be added! " +
                                "There is at least one additional structure with the same name, i.e. '" + structure.NameQualified + "'.");
                    }
                }

                throw new SystemException("The structure '" + source.NameQualified + "' could not be added!");
            }
        }
    }
}