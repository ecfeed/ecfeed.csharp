using System;
using System.Reflection;
using System.Collections.Generic;

namespace EcFeed
{
    public class Structure
    {
        internal Type Source { get; set; } = null;

        internal bool Active { get; set; } = false;
        internal ConstructorInfo ActiveConstructor { get; set; } = null;

        internal string NameSimple { get; set; } = "";
        internal string NameQualified { get; set; } = "";

        internal Dictionary<string, ConstructorInfo> Constructors { get; set; } = new Dictionary<string, ConstructorInfo>();

        public override string ToString() 
        {
            return NameSimple;
        }

        public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }
            
            if (o == null) 
            {
                return false;
            }

            if (o.GetType() != this.GetType())
            {
                return false;
            }

            Structure structure = o as Structure;

            return NameSimple.Equals(structure.NameSimple);
        }

        public override int GetHashCode()
        {
            return NameSimple.GetHashCode();
        }
    }
}