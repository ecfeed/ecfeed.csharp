using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EcFeed
{
    internal class StructureSetterDefault : StructureSetter
    {
        private StructureSetterDefault()
        {}

        public static StructureSetter Get()
        {
            return new StructureSetterDefault();
        }

        public Structure Parse(Type source)
        {
            var structure = new Structure();

            structure.Source = source;
            structure.NameQualified = GetNameQualified(source);
            structure.NameSimple = GetNameSimple(source);
            structure.Constructors = GetConstructors(source);

            return structure;
        }

        private string GetNameQualified(Type source)
        {
            return  source.FullName;
        }

        private string GetNameSimple(Type source)
        {
            return source.Name;
        }

        private Dictionary<string, ConstructorInfo> GetConstructors(Type source)
        {
            var constructors = new Dictionary<string, ConstructorInfo>();

            foreach (var constructor in source.GetConstructors())
            {
                constructors[GetNameSimple(source) + ParseConstructor(constructor)] = constructor;
            }

            return constructors;
        }

        private string ParseConstructor(ConstructorInfo constructor) 
        {
            var parametersParsed = new List<string>();
            
            foreach (var parameter in constructor.GetParameters())
            {
                parametersParsed.Add(parseConstructorParameter(parameter));
            }

            return "(" + String.Join(",", parametersParsed) + ")";
        }

        private string parseConstructorParameter(ParameterInfo parameter) 
        {

            return GetNameSimple(parameter.GetType());
        }

        public void Activate(Structure structure, string signature)
        {
            var signatureParsed = getSignatureDefinition(signature);

            foreach (var constructor in structure.Constructors) {
                if (constructor.Key.Equals(signatureParsed)) {
                    if (structure.ActiveConstructor != null && structure.ActiveConstructor != constructor.Value) {
                        throw new SystemException("The redefinition of constructors is not supported. Affected structure: '" + structure.NameSimple + "'.");
                    }

                    structure.ActiveConstructor = constructor.Value;
                    break;
                }
            }

            if (structure.ActiveConstructor == null) {
                throw new SystemException("The constructor for the structure '" + structure.NameSimple + "' is not defined in the source!");
            }

            structure.Active = true;
        }

        private String getSignatureDefinition(String signature) 
        {
            var argumentPairs = Regex.Match(signature, @"(?<=\().+?(?=\))").Value.Split(",");
            var argumentTypes = new List<string>();

            for (var i = 0 ; i < argumentPairs.Length ; i++) {
                var argumentParsed = argumentPairs[i].Trim().Split(" ");
                argumentTypes.Add(argumentParsed[0]);
            }

            var signatureParsed = signature;

            if (signatureParsed.Contains(".")) {
                signatureParsed = signatureParsed.Substring(signature.LastIndexOf(".") + 1);
            }

            signatureParsed = signatureParsed.Substring(0, signatureParsed.LastIndexOf("("));
            signatureParsed = signatureParsed + "(" + String.Join(",", argumentTypes) + ")";

            return signatureParsed;
        }
    }
}