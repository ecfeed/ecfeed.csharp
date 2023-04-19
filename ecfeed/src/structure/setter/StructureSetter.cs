using System;

namespace EcFeed
{
    public interface StructureSetter
    {
        Structure Parse(Type source);
        void Activate(Structure structure, string signature);
    }
}