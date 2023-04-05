using System;

namespace EcFeed
{
    internal interface StructureSetter
    {
        Structure Parse(Type source);
        void Activate(Structure structure, string signature);
    }
}