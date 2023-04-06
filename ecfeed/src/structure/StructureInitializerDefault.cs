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
            structuresSetter.Activate(structures, signatureStructure);
        }

        public T Instantiate<T>(Queue<string> arguments)
        {
            return (T) Instantiate(typeof(T).Name, arguments);
        }

        public object Instantiate(string signatureStructure, Queue<string> arguments)
        {
            object element = InstantiateStructure(GetStructure(signatureStructure), arguments);

            if (arguments.Count > 0) {
                throw new SystemException("The list of parameters is too long! Parameters remaining: '" + arguments.Count + "'.");
            }

            return element;
        }

        public object[] GetTestCase(string signatureMethod, Queue<string> arguments)
        {
            var parameters = GetMethodParameters(signatureMethod);

            var testCase = new List<object>();

            foreach (var parameter in parameters) 
            {
                if (IsPrimitive(parameter)) 
                {
                    testCase.Add(InstantiatePrimitive(parameter, arguments));
                } 
                else if (IsEnum(parameter)) 
                {
                    testCase.Add(InstantiateEnum(arguments));
                } 
                else 
                {
                    testCase.Add(InstantiateStructure(GetStructure(parameter), arguments));
                }
            }

            if (arguments.Count > 0) {
                throw new SystemException("The list of parameters is too long! Parameters remaining: '" + arguments.Count + "'.");
            }

            return testCase.ToArray();
        }

        public HashSet<Structure> GetStructuresRaw()
        {
            return structures;
        }

        public HashSet<Structure> GetStructuresActive()
        {
            return structures.Where(e => e.Active).ToHashSet();
        }

        public HashSet<string> GetNamesSimpleRaw()
        {
            return structures.Select(e => e.NameSimple).ToHashSet();
        }

        public HashSet<string> GetNamesSimpleActive()
        {
            return GetStructuresActive().Select(e => e.NameSimple).ToHashSet();
        }

        public HashSet<string> GetNamesQualifiedRaw()
        {
            return structures.Select(e => e.NameQualified).ToHashSet();
        }

        public HashSet<string> GetNamesQualifiedActive()
        {
            return GetStructuresActive().Select(e => e.NameQualified).ToHashSet();
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

        private object InstantiateStructure(Structure structure, Queue<string> arguments) 
        {
            if (!structure.Active) {
                throw new SystemException("The required structure '" + structure.NameSimple + "' has not been activated!'");
            }

            var constructorParameters = new Queue<object>();

            foreach (var parameter in structure.ActiveConstructor.GetParameters()) {
                constructorParameters.Enqueue(getValue(parseConstructorParameter(parameter), arguments));
            }

            try {
                return structure.ActiveConstructor.Invoke(constructorParameters.ToArray());
            } catch (Exception) {
                throw new SystemException("The structure '" + structure.NameSimple + "' could not be instantiated!");
            }
        }

        private string parseConstructorParameter(ParameterInfo parameter) 
        {
            var name = parameter.ParameterType.Name;

            switch(name)
            {
                case "Byte": return "byte";
                case "Int16": return "short";
                case "Int32": return "int";
                case "Int64": return "long";
                case "Single": return "float";
                case "Double": return "double";
                case "Char": return "char";
            }

            return name;
        }

        private object getValue(string typeName, Queue<string> arguments) {

            if (arguments.Count <= 0) {
                throw new SystemException("The list of arguments is too short!");
            }

            switch (typeName) {
                case "byte":
                    return ParsePrimitive(arguments, typeName, e => Byte.Parse(e));
                case "short":
                    return ParsePrimitive(arguments, typeName, e => Int16.Parse(e));
                case "int":
                    return ParsePrimitive(arguments, typeName, e => Int32.Parse(e));
                case "long":
                    return ParsePrimitive(arguments, typeName, e => Int64.Parse(e));
                case "float":
                    return ParsePrimitive(arguments, typeName, e => Single.Parse(e));
                case "double":
                    return ParsePrimitive(arguments, typeName, e => Double.Parse(e));
                case "boolean":
                    return ParsePrimitive(arguments, typeName, e => Boolean.Parse(e));
                case "char":
                    return GetPrimitive(arguments)[0];
                case "String":
                    return arguments.Dequeue();
                default: {
                    return InstantiateStructure(GetStructure(typeName), arguments);
                }
            }
        }

        private delegate object Function(string value);
        private object ParsePrimitive(Queue<string> parameters, string type, Function fun) 
        {
            var value = GetPrimitive(parameters);

            try {
                return fun(value);
            } catch (Exception) {
                throw new SystemException("The value '" + value + "' cannot be parsed to '" + type + "'!");
            }
        }

        private string GetPrimitive(Queue<string> arguments) 
        {
            var value = arguments.Dequeue();

            if (value == null) {
                throw new SystemException("Primitive types cannot accept null values!");
            }

            return value;
        }
    
        private Structure GetStructure(string signature) 
        {
            foreach (var structure in structures) {
                if (structure.NameSimple == signature) {
                    return structure;
                }
            }

            throw new SystemException("The required structure '" + signature + "' could not be found in the source!");
        }

        private string[] GetMethodParameters(string signature) 
        {
            return Regex.Match(signature, @"(?<=\().+?(?=\))").Value.Split(",");
        }

        private bool IsPrimitive(string typeName) 
        {
            switch (typeName) 
            {
                case "byte":
                case "short":
                case "int":
                case "long":
                case "float":
                case "double":
                case "boolean":
                case "char":
                case "String":
                    return true;
                default:
                    return false; 
            }
        }

        private bool IsEnum(string typeName) {

            try
            {
                GetStructure(typeName);
            } 
            catch (Exception) 
            {
                return true;
            }

            return false;
        }

        private object InstantiatePrimitive(string signature, Queue<string> arguments) 
        {
            return getValue(signature, arguments);
        }

        private object InstantiateEnum(Queue<string> arguments) 
        {
            return getValue("String", arguments);
        }
    }
}