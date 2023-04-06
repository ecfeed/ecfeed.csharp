using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace EcFeed
{
    internal class StructuresSetterDefault : StructuresSetter
    {
        private StructureSetter structureSetter;

        private StructuresSetterDefault(StructureSetter structureSetter)
        {
            this.structureSetter = structureSetter;
        }

        public static StructuresSetter Get(StructureSetter structureSetter)
        {
            return new StructuresSetterDefault(structureSetter);
        }

        public List<Structure> Parse(Assembly assembly, params string[] sources)
        {
            var structures = new List<Structure>();

            var namespaces = assembly.GetTypes().Select(e => e.Namespace).Where(e => e != null).ToHashSet();
            
            foreach (var source in sources)
            {
                if (namespaces.Contains(source))
                {
                    AddStructures(assembly, source, structures);
                }
                else
                {
                    throw new SystemException($"The namespace {source} does not exist or does not contain any structures!");
                }
            }

            return structures;
        }

        private void AddStructures(Assembly assembly, string source, List<Structure> structures) 
        {
            var types = assembly.GetTypes().Where(e => e.IsClass).Where(e => e.Namespace == source).ToList();

            foreach (var type in types)
            {
                structures.Add(structureSetter.Parse(type));
            }
        }

        public void Activate(HashSet<Structure> structures, params string[] signatures) 
        {
            foreach (var signature in signatures)
            {
                Activate(structures, signature);
            }
        }

        public void Activate(HashSet<Structure> structures, string signature) 
        {
            var activated = false;

            var signatureParsed = GetSignatureName(signature);

            foreach (var structure in structures) {
                if (structure.NameSimple.Equals(signatureParsed)) {
                    structureSetter.Activate(structure, signature);
                    activated = true;
                }
            }

            if (!activated) {
                throw new SystemException("The structure '" + signature + "' could not be activated! It might not exist in the source.");
            }
        }

        private String GetSignatureName(String signature) 
        {
            var signatureParsed = signature.Replace(" ", "");

            if (signatureParsed.Contains("(")) {
                signatureParsed = signatureParsed.Substring(0, signatureParsed.LastIndexOf("("));
            }

            if (signatureParsed.Contains(".")) {
                signatureParsed = signatureParsed.Substring(signatureParsed.LastIndexOf(".") + 1);
            }

            return signatureParsed;
        }
    }
}